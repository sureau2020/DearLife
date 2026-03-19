
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
任务：生成一句角色台词。
必须使用简体中文。
必须是第一人称对话。
不超过200个汉字。
禁换行/解释/角色名
输出几句角色的动作和回答。
语气必须符合角色性格。";

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
        string characterName = character.Name;
        string personnalities = character.GetPersonalityDescription();
        string playerName = character.PlayerAppellation;
        string relation = character.Relationship;
        string characterInfo = GameManager.Instance.StateManager.Settings.Prompt ?? "无";
        string itemInfo = $"{item.Name}，{item.Description}";
        string healthStatus = character.HealthState.ToString();

        return userPrompt.Replace("{characterName}", characterName)
            .Replace("{personnalities}", personnalities)
            .Replace("{playerName}", playerName)
            .Replace("{relation}", relation)
            .Replace("{characterInfo}", characterInfo)
            .Replace("{itemInfo}", itemInfo)
            .Replace("{healthStatus}", healthStatus);
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
                maxOutputTokens = 300, // 稍微多给一点余量
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

                // 逐级检查，哪级报错哪级会停
                if (res == null) throw new Exception("JSON 转换结果为 null");
                if (res.candidates == null || res.candidates.Length == 0) throw new Exception("candidates 为空");
                if (res.candidates[0].content == null) throw new Exception("content 为空");
                if (res.candidates[0].content.parts == null || res.candidates[0].content.parts.Count == 0) throw new Exception("parts 为空");

                string line = res.candidates[0].content.parts[0].text;
                if (string.IsNullOrEmpty(line)) throw new Exception("text 字段内容为空");

                // 这一步最危险，建议加个 null 检查
                if (GameManager.Instance?.StateManager?.Character == null)
                {
                    onFail?.Invoke("解析成功但 GameManager 数据未初始化");
                    yield break;
                }

                EventData eventData = CreateItemDialogueEvent(line);
                onSuccess?.Invoke(eventData);
            }
            catch (Exception ex)
            {
                onFail?.Invoke($"解析链条中断: {ex.Message}");
            }
        }
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