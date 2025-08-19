using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNode : BaseNode
{
    [SerializeField] private string dialogueText;
    [SerializeField] private string speakerName;
    [SerializeField] private string nextNodeId;

    public override NodeExecResult Execute(Dictionary<string, int> parameters)
    {
        
        return new NodeExecResult
        {
            Type = NodeExecResultType.ShowDialogue,
            Payload = new DialoguePayload
            {
                Speaker = speakerName,
                Text = dialogueText,
                NextNodeId = nextNodeId
            }
        };
    }
}

public class DialoguePayload
{
    public string Speaker { get; set; }
    public string Text { get; set; }
    public string NextNodeId { get; set; }
}
