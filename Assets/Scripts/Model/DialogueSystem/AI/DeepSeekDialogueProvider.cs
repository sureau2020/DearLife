using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

public sealed class DeepSeekDialogueProvider : IAIDialogueProvider
{
    public string Name => "DeepSeek";
    public IReadOnlyList<string> Models { get; } = Array.AsReadOnly(new[]
    {
        "deepseek-flash", "deepseek-v4-pro"
    });

    public UnityWebRequest CreateRequest(string apiKey, string model, string systemPrompt, string userPrompt)
    {
        string json = JsonConvert.SerializeObject(new
        {
            model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            stream = false,
            thinking = new { type = "disabled" },
            max_tokens = 2000,
            temperature = 0.7f
        });

        var request = new UnityWebRequest("https://api.deepseek.com/chat/completions", "POST");
        try
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
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
        var choices = response["choices"] as JArray;
        if (choices == null || choices.Count == 0)
            throw new InvalidOperationException("DeepSeek 未返回有效对话");
        var choice = choices[0];
        if ((string)choice?["finish_reason"] != "stop")
            throw new InvalidOperationException("DeepSeek 未正常完成回答，请重试");

        // Only the final answer is dialogue; reasoning_content is deliberately ignored.
        var content = choice?["message"]?["content"];
        if (content?.Type != JTokenType.String || string.IsNullOrWhiteSpace((string)content))
            throw new InvalidOperationException("DeepSeek 未返回有效对话");

        return (string)content;
    }
}
