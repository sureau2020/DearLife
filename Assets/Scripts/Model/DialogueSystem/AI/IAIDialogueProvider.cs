using System.Collections.Generic;
using UnityEngine.Networking;

public interface IAIDialogueProvider
{
    string Name { get; }
    IReadOnlyList<string> Models { get; }

    // The caller owns, sends and disposes the returned request.
    UnityWebRequest CreateRequest(string apiKey, string model, string systemPrompt, string userPrompt);
    string ReadResponseText(string responseJson);
}
