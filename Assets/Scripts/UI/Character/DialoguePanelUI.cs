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

    public void ShowDialogue(DialoguePayload payload) {
        // ���payload��Speaker����{playerName}�����滻Ϊ�������
        if (payload.Speaker.Contains("{characterName}"))
        {
            speaker.text = payload.Speaker.Replace("{characterName}", GameManager.Instance.StateManager.Character.Name);
        }
        else { 
            speaker.text = payload.Speaker;
        }
        if (payload.Text.Contains("{characterName}"))
        {
            dialogueText.text = payload.Text.Replace("{characterName}", GameManager.Instance.StateManager.Character.Name);
        }
        else
        {
            dialogueText.text = payload.Text;
        }
        nextNodeId = payload.NextNodeId;

    }

    public void OnAdvanceClicked()
    {
        OnNextClicked?.Invoke(nextNodeId);
    }
}
