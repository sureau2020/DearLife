
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class AIChat : MonoBehaviour
{
    private string systemPrompt = @"你是视觉小说角色对话生成器。
任务：1. 每句独立
2. 不要编号
3. 不要解释
4. 输出为JSON数组
必须使用简体中文。
必须是第一人称对话。
每句话不超过20字。
禁换行,禁解释，禁角色名，禁额外引号（JSON字符串引号必须保留），禁任何 Markdown 代码块标签或额外的文字说明
输出几句角色的动作和回答。
语气必须符合角色性格和当前关系，根据角色健康状态调整语气。";

    private string userPrompt = @"角色：{characterName}
性格：
{personnalities}
人物补充设定：
{characterInfo}
当前情境：
{characterName}收到“{itemInfo}”。
赠送者称呼：
{playerName}
当前关系：
{relation}
健康状态：
{healthStatus}
生成一句此时说的话。";

    private int maxLine = 5; // 预设的最大对话，和 topics 数组长度一致

    private readonly HashSet<UnityWebRequest> activeRequests = new HashSet<UnityWebRequest>();
    private int requestGeneration;

    private string[] topics = new string[]
    {
        "话题是收到的东西，如果是食物描述味道",
        "话题是关心给予者，用{playerName}称呼给予者",
        "话题是自己的心情",
        "话题是由这个东西联想到的一件过去的事",
        "话题是由这个东西联想到的一件生活小事",
        "话题是由这个东西联想到的一个想法"
    };

    public void GenerateAIDialogue(ItemData item, CharacterData character, Action<OperationResult<EventData>> callback)
    {
        string prompt = CombinePrompt(item, character);
        StartCoroutine(GenerateDialogue(prompt,
            onSuccess: (eventData) =>
            {
                callback?.Invoke(OperationResult<EventData>.Complete(eventData));
            },
            onFail: (errorMsg) =>
            {
                callback?.Invoke(OperationResult<EventData>.Fail(errorMsg));
            }));
    }

    private string CombinePrompt(ItemData item, CharacterData character)
    {
        string result = userPrompt + GenerateRamdomPrompt();
        string characterName = character.Name;
        string personnalities = character.GetPersonalityDescription();
        string playerName = character.PlayerAppellation;
        string relation = character.Relationship;
        string characterInfo = GameManager.Instance.StateManager.Settings.Prompt ?? "无";
        string itemInfo = $"{item.Name}，{item.Description}";
        string healthStatus = character.HealthState.ToString();

        return result.Replace("{characterName}", characterName)
            .Replace("{personnalities}", personnalities)
            .Replace("{playerName}", playerName)
            .Replace("{relation}", relation)
            .Replace("{characterInfo}", characterInfo)
            .Replace("{itemInfo}", itemInfo)
            .Replace("{healthStatus}", healthStatus);
    }

    private string GenerateRamdomPrompt()
    {
        System.Random rand = new System.Random();
        int linesNum = rand.Next(1, maxLine + 1);
        int topicsNum = rand.Next(topics.Length);
        return $"\n生成{linesNum}句此时说的话,\n{topics[topicsNum]}. 输出为JSON数组";
    }

    private void OnDisable()
    {
        requestGeneration++;
        foreach (var request in activeRequests)
        {
            request.Abort();
            request.Dispose();
        }
        activeRequests.Clear();
        StopAllCoroutines();
    }

    public IEnumerator GenerateDialogue(
        string prompt,
        Action<EventData> onSuccess,
        Action<string> onFail,
        float timeout = 30f)
    {
        if (!isActiveAndEnabled) yield break;

        GameSettings settings = GameManager.Instance?.StateManager?.Settings;
        if (settings == null)
        {
            onFail?.Invoke("GameManager 数据未初始化");
            yield break;
        }
        if (!AIDialogueProviders.TryGet(settings.API, out var provider))
        {
            onFail?.Invoke("不支持的 AI 接口，请检查供应商设置");
            yield break;
        }
        if (string.IsNullOrWhiteSpace(settings.Key) || string.IsNullOrWhiteSpace(settings.Model))
        {
            onFail?.Invoke("请先填写 AI 密钥和模型");
            yield break;
        }
        if (string.IsNullOrWhiteSpace(prompt) || timeout <= 0 || float.IsNaN(timeout) || float.IsInfinity(timeout))
        {
            onFail?.Invoke("AI 提示词或超时设置无效");
            yield break;
        }

        int generation = requestGeneration;
        UnityWebRequest request = null;
        UnityWebRequestAsyncOperation operation = null;
        try
        {
            request = provider.CreateRequest(settings.Key.Trim(), settings.Model.Trim(), systemPrompt, prompt);
            request.timeout = Mathf.Max(1, Mathf.CeilToInt(timeout));
            activeRequests.Add(request);
            operation = request.SendWebRequest();
        }
        catch (Exception)
        {
            if (request != null)
            {
                activeRequests.Remove(request);
                request.Dispose();
            }
            onFail?.Invoke(provider.Name + " 请求启动失败，请检查设置与网络");
        }
        if (operation == null) yield break;

        try
        {
            float startedAt = Time.realtimeSinceStartup;
            while (true)
            {
                if (generation != requestGeneration || !isActiveAndEnabled) yield break;
                if (operation.isDone) break;
                if (Time.realtimeSinceStartup - startedAt >= timeout)
                {
                    request.Abort();
                    onFail?.Invoke("AI请求超时");
                    yield break;
                }
                yield return null;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                onFail?.Invoke(GetRequestError(provider.Name, request));
                yield break;
            }

            EventData eventData = null;
            string error = null;
            try
            {
                string rawText = provider.ReadResponseText(request.downloadHandler.text);
                List<string> lines = ParseDialogueLines(rawText);
                if (GameManager.Instance?.StateManager?.Character == null)
                    throw new InvalidOperationException("GameManager 数据未初始化");
                eventData = CreateMultiLineDialogueEvent(lines);
            }
            catch (Exception ex)
            {
                error = "AI对话解析失败: " + ex.Message;
            }

            // Callbacks run outside parsing so consumer exceptions cannot trigger a second callback.
            if (error != null) onFail?.Invoke(error);
            else onSuccess?.Invoke(eventData);
        }
        finally
        {
            if (activeRequests.Remove(request)) request.Dispose();
        }
    }

    private static List<string> ParseDialogueLines(string rawText)
    {
        rawText = rawText.Trim();
        if (rawText.StartsWith("```"))
        {
            int newline = rawText.IndexOf('\n');
            if (newline < 0 || !rawText.EndsWith("```"))
                throw new InvalidOperationException("AI 返回的 JSON 代码块不完整");
            rawText = rawText.Substring(newline + 1, rawText.Length - newline - 4).Trim();
        }

        var array = JArray.Parse(rawText);
        if (array.Count == 0) throw new InvalidOperationException("对话数组为空");
        var lines = new List<string>();
        foreach (var line in array)
        {
            if (line.Type != JTokenType.String || string.IsNullOrWhiteSpace((string)line))
                throw new InvalidOperationException("对话数组必须只包含非空字符串");
            lines.Add(((string)line).Trim());
        }
        return lines;
    }

    private static string GetRequestError(string providerName, UnityWebRequest request)
    {
        string message;
        switch (request.responseCode)
        {
            case 400:
            case 422: message = "请求参数无效，请检查模型设置"; break;
            case 401:
            case 403: message = "认证失败，请检查对应供应商的密钥和权限"; break;
            case 402: message = "账户余额不足"; break;
            case 404: message = "模型不存在，请检查模型名称"; break;
            case 429: message = "请求过于频繁，请稍后重试"; break;
            case 500:
            case 503: message = "服务暂不可用，请稍后重试"; break;
            default: message = "请求失败或超时，请检查网络后重试"; break;
        }
        return $"{providerName}（HTTP {request.responseCode}）：{message}";
    }

    private EventData CreateMultiLineDialogueEvent(List<string> lines)
    {
        string eventId = "ai_event_" + Guid.NewGuid().ToString("N");
        string speakerName = GameManager.Instance.StateManager.Character.Name;
        Dictionary<string, BaseNode> nodes = new Dictionary<string, BaseNode>();

        for (int i = 0; i < lines.Count; i++)
        {
            string nodeId = "node" + i;
            string nextNodeId = (i < lines.Count - 1) ? "node" + (i + 1) : null;

            DialogueNode node = new DialogueNode(
                nodeId,
                nextNodeId, // 连向下一个节点
                speakerName,
                lines[i].Trim()
            );

            nodes.Add(nodeId, node);
        }

        // 入口点固定为 node0
        return new EventData(
            eventId,
            DialogueType.Item,
            "node0",
            nodes
        );
    }

    private EventData CreateItemDialogueEvent(string text)
    {
        string nodeId = "node0";
        string eventId = "ai_event_" + Guid.NewGuid().ToString("N");
        string speakerName = GameManager.Instance.StateManager.Character.Name;

        DialogueNode node = new DialogueNode(
            nodeId,
            null,
            speakerName,
            text
        );

        Dictionary<string, BaseNode> nodes = new Dictionary<string, BaseNode>
        {
            { nodeId, node }
        };

        return new EventData(
            eventId,
            DialogueType.Item,
            nodeId,
            nodes
        );
    }
}