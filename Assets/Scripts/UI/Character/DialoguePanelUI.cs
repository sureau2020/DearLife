using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialoguePanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speaker;
    [SerializeField] private TextMeshProUGUI dialogueText;


    public void ShowDialogue(DialoguePayload payload) { 
        speaker.text = payload.Speaker;
        dialogueText.text = payload.Text;
    }



}
