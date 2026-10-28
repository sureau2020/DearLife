using System;
using System.Collections.Generic;

public static class AIDialogueProviders
{
    // Add new provider implementations here; AIChat does not depend on their protocols.
    private static readonly Dictionary<string, IAIDialogueProvider> providers =
        new Dictionary<string, IAIDialogueProvider>(StringComparer.OrdinalIgnoreCase)
        {
            { "Gemini", new GeminiDialogueProvider() },
            { "DeepSeek", new DeepSeekDialogueProvider() }
        };

    public static bool TryGet(string name, out IAIDialogueProvider provider)
    {
        provider = null;
        return !string.IsNullOrWhiteSpace(name) && providers.TryGetValue(name.Trim(), out provider);
    }
}
