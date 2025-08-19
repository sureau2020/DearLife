using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] DialogueRunner runner;
    [SerializeField] private DialoguePanelUI dailyDialoguePanel;
    [SerializeField] private DialoguePanelUI characterDialoguePanel;
    [SerializeField] private ChoicePanelUI choicePanelUI;

    void Awake()
    {
        runner.OnShowDialogue += HandleShowDialogue;
        runner.OnShowChoices += HandleShowChoices;
        runner.OnDialogueEnd += HandleEnd;
        runner.StartDialogue += HandleStart;
    }

    void OnDestroy()
    {
        runner.OnShowDialogue -= HandleShowDialogue;
        runner.OnShowChoices -= HandleShowChoices;
        runner.OnDialogueEnd -= HandleEnd;
        runner.StartDialogue -= HandleStart;
    }

    private void HandleStart(DialogueType type) {
        switch(type) {
            case DialogueType.Daily:
                dailyDialoguePanel.gameObject.SetActive(true);
                break;
            case DialogueType.Item:
                characterDialoguePanel.gameObject.SetActive(true);
                break;
            default:
                dailyDialoguePanel.gameObject.SetActive(true);
                break;
        }
    }

    private void HandleShowDialogue(DialoguePayload payload, DialogueType type) { 
        switch(type) {
            case DialogueType.Daily:
                dailyDialoguePanel.ShowDialogue(payload);
                break;
            case DialogueType.Item:
                characterDialoguePanel.ShowDialogue(payload);
                break;
            default:
                dailyDialoguePanel.ShowDialogue(payload);
                break;
        }
    }

    private void HandleShowChoices(List<ChoiceOption> choices) { 
    
    }

    private void HandleEnd() { 
    
    }

    
}
