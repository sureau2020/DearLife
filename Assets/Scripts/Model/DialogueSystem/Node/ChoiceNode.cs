using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceNode : BaseNode
{
    public List<ChoiceOption> Options;

    public override NodeExecResult Execute(Dictionary<string, int> parameters)
    {
        return new NodeExecResult
        {
            Type = NodeExecResultType.ShowChoices,
            Payload = Options
        };
    }
}

public class ChoiceOption
{
    public string Text { get; private set; }
    public string NextNodeId { get; private set; }
}
