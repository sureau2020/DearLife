using System.Collections.Generic;

public class ConditionNode : BaseNode
{
    public string ParamName;
    public int CompareValue;
    public string TrueNodeId;
    public string FalseNodeId;

    public ConditionNode(string id, string paramName, int compareValue, string trueNodeId, string falseNodeId)
        : base(id, trueNodeId)
    {
        ParamName = paramName;
        CompareValue = compareValue;
        TrueNodeId = trueNodeId;
        FalseNodeId = falseNodeId;
    }

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