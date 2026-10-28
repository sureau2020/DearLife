using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

public sealed class GeminiDialogueProvider : IAIDialogueProvider
{
    public string Name => "Gemini";
    public IReadOnlyList<string> Models { get; } = Array.AsReadOnly(new[]
    {
        "gemini-3.1-flash-lite-preview", "gemini-3.1-pro-preview", "gemini-3-flash-preview"
    });

    public UnityWebRequest CreateRequest(string apiKey, string model, string systemPrompt, string userPrompt)
    {
        string json = JsonConvert.SerializeObject(new
        {
            system_instruction = new { parts = new[] { new { text = systemPrompt } } },
            contents = new[] { new { role = "user", parts = new[] { new { text = userPrompt } } } },
            generationConfig = new { maxOutputTokens = 2000, temperature = 0.7f }
        });

        string url = "https://generativelanguage.googleapis.com/v1beta/models/" +
            Uri.EscapeDataString(model) + ":generateContent";
        var request = new UnityWebRequest(url, "POST");
        try
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-goog-api-key", apiKey);
            return request;
        }
        catch
        {
            request.Dispose();
            throw;
        }
    }

    public string ReadResponseText(string responseJson)
    {
        var response = JObject.Parse(responseJson);
        var candidates = response["candidates"] as JArray;
        if (candidates == null || candidates.Count == 0)
            throw new InvalidOperationException("Gemini 未返回有效对话");
        var candidate = candidates[0];
        if ((string)candidate?["finishReason"] != "STOP")
            throw new InvalidOperationException("Gemini 未正常完成回答，请重试");

        var text = new StringBuilder();
        var parts = candidate?["content"]?["parts"] as JArray;
        if (parts != null)
        {
            foreach (var part in parts)
            {
                if ((bool?)part["thought"] == true) continue;
                if (part["text"]?.Type == JTokenType.String) text.Append((string)part["text"]);
            }
        }
        if (string.IsNullOrWhiteSpace(text.ToString()))
            throw new InvalidOperationException("Gemini 未返回有效对话");

        return text.ToString();
    }
}
