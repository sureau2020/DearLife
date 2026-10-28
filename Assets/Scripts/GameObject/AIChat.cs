
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json; 

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
禁换行,禁解释，禁角色名，禁引号，禁任何 Markdown 代码块标签或额外的文字说明
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

    #region Gemini 数据结构
    private class GeminiRequest
    {
        public SystemInstruction system_instruction;
        public List<Content> contents;
        public GenerationConfig generationConfig;
    }

    private class SystemInstruction { public List<Part> parts; }
    private class GenerationConfig { public int maxOutputTokens; public float temperature; }

    // 响应解析类
    [Serializable]
    public class GeminiResponse
    {
        [JsonProperty("candidates")]
        public Candidate[] candidates;
    }

    [Serializable]
    public class Candidate
    {
        [JsonProperty("content")]
        public Content content;
        [JsonProperty("finishReason")]
        public string finishReason;
    }

    [Serializable]
    public class Content
    {
        [JsonProperty("role")]
        public string role;
        [JsonProperty("parts")]
        public List<Part> parts; // 注意这里是 List
    }

    [Serializable]
    public class Part
    {
        [JsonProperty("text")]
        public string text;

        // Gemini 3 新增的思考内容映射（可选，不写也不影响解析 text）
        [JsonProperty("thought")]
        public string thought;
    }
    #endregion

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
        int linesNum = rand.Next(maxLine);
        int topicsNum = rand.Next(topics.Length);
        return $"\n生成{linesNum}句此时说的话,\n{topics[topicsNum]}. 输出为JSON数组";
    }

    public IEnumerator GenerateDialogue(
        string prompt,
        Action<EventData> onSuccess,
        Action<string> onFail,
        float timeout = 10f)
    {
        GameSettings settings = GameManager.Instance.StateManager.Settings;
        string apiKey = settings.Key;
        string model = settings.Model;

        // 关键点 1: Gemini 的 API Key 直接拼接在 URL 后面
        string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
        
        // 关键点 2: 使用 Newtonsoft 构建标准的 Gemini 请求对象
        var requestObj = new GeminiRequest
        {
            system_instruction = new SystemInstruction
            {
                parts = new List<Part> { new Part { text = systemPrompt } }
            },
            contents = new List<Content>
            {
                new Content
                {
                    role = "user",
                    parts = new List<Part> { new Part { text = prompt } }
                }
            },
            generationConfig = new GenerationConfig
            {
                maxOutputTokens = 2000, // 稍微多给一点余量
                temperature = 0.7f
            }
        };
        string jsonBody = JsonConvert.SerializeObject(requestObj);

        using (UnityWebRequest req = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            float timer = 0f;
            var operation = req.SendWebRequest();

            while (!operation.isDone)
            {
                if (timer > timeout)
                {
                    onFail?.Invoke("AI请求超时");
                    yield break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            if (req.result != UnityWebRequest.Result.Success)
            {
                // 如果报错，尝试解析报错信息，方便调试
                onFail?.Invoke($"Gemini Error: {req.error}\nDetail: {req.downloadHandler.text}");
                yield break;
            }
            try
            {
                var res = JsonConvert.DeserializeObject<GeminiResponse>(req.downloadHandler.text);

                // 基础检查
                if (res?.candidates == null || res.candidates.Length == 0) throw new Exception("AI未返回有效内容");

                string rawText = res.candidates[0].content.parts[0].text.Trim();

                // --- 核心改动：清理 Markdown 标签并解析数组 ---
                if (rawText.StartsWith("```json"))
                {
                    rawText = rawText.Replace("```json", "").Replace("```", "").Trim();
                }

                // 使用 Newtonsoft 解析为字符串列表
                List<string> lines = JsonConvert.DeserializeObject<List<string>>(rawText);

                if (lines == null || lines.Count == 0) throw new Exception("解析后的对话数组为空");

                // 检查 GameManager 状态
                if (GameManager.Instance?.StateManager?.Character == null)
                {
                    onFail?.Invoke("GameManager 数据未初始化");
                    yield break;
                }

                // --- 核心改动：调用支持多节点的生成方法 ---
                EventData eventData = CreateMultiLineDialogueEvent(lines);
                onSuccess?.Invoke(eventData);
            }
            catch (Exception ex)
            {
                onFail?.Invoke($"解析 JSON 数组失败: {ex.Message}\n原始文本: {req.downloadHandler.text}");
            }
        }
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