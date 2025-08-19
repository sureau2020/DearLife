using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] DialogueRunner runner;
    [SerializeField] private DialoguePanelUI dailyDialoguePanel;
    [SerializeField] private DialoguePanelUI characterDialoguePanel;

    void Awake()
    {
        runner.OnShowDialogue += HandleShowDialogue;
        runner.OnShowChoices += HandleShowChoices;
        runner.OnDialogueEnd += HandleEnd;
    }

    private void HandleShowDialogue(DialoguePayload payload, DialogueType type) { 
        
    }

    private void HandleShowChoices(List<ChoiceOption> choices) { 
    
    }

    private void HandleEnd() { 
    
    }

}
