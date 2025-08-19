using System.Collections.Generic;

public class ConditionNode : BaseNode
{
    public string ParamName;
    public int CompareValue;
    public string TrueNodeId;
    public string FalseNodeId;

    public override NodeExecResult Execute(Dictionary<string, int> parameters)
    {
        parameters.TryGetValue(ParamName, out int value);
        bool result = value >= CompareValue;

        return new NodeExecResult
        {
            Type = NodeExecResultType.Advance,
            Payload = result ? TrueNodeId : FalseNodeId
        };
    }
}