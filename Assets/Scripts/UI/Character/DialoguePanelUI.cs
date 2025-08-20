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
    private DialoguePayload currentPayload;

    public void ShowDialogue(DialoguePayload payload) { 
        speaker.text = payload.Speaker;
        dialogueText.text = payload.Text;
        currentPayload = payload;
    }

    public void OnAdvanceClicked()
    {
        string nextNodeId = currentPayload?.NextNodeId;
        OnNextClicked?.Invoke(nextNodeId);
    }
}
