using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class DialoguePanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speaker;
    [SerializeField] private TextMeshProUGUI dialogueText;

    public event Action<string> OnNextClicked;
    private string nextNodeId;

    public void ShowDialogue(DialoguePayload payload)
    {
        CharacterData character = GameManager.Instance.StateManager.Character;

        var replacements = new Dictionary<string, string>
        {
            { "{characterName}", character.Name },
            { "{appellation}", character.PlayerAppellation },
            { "{characterPronoun}", character.Pronoun }
        };

        string speakerStr = payload.Speaker;
        string textStr = payload.Text;
        foreach (var kv in replacements)
        {
            if (speakerStr.Contains(kv.Key))
                speakerStr = speakerStr.Replace(kv.Key, kv.Value);
            if (textStr.Contains(kv.Key))
                textStr = textStr.Replace(kv.Key, kv.Value);
        }

        speaker.text = speakerStr;
        dialogueText.text = textStr;
        nextNodeId = payload.NextNodeId;
    }

    public void OnAdvanceClicked()
    {
        OnNextClicked?.Invoke(nextNodeId);
    }
}
