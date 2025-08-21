
using System.Collections.Generic;
using UnityEngine;

public class DialogueNode : BaseNode
{
    public string dialogueText;
    public string speakerName;

    public DialogueNode(string id, string nextNodeId, string speakerName, string dialogueText) 
        : base(id, nextNodeId)
    {
        this.speakerName = speakerName;
        this.dialogueText = dialogueText;
    }

    public override NodeExecResult Execute()
    {
        
        return new NodeExecResult
        {
            Type = NodeExecResultType.ShowDialogue,
            Payload = new DialoguePayload
            {
                Speaker = speakerName,
                Text = dialogueText,
                NextNodeId = NextNodeId,
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
