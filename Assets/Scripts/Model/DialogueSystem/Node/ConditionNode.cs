using System.Collections.Generic;

public class ConditionNode : BaseNode
{
    public int CompareValue;
    public string TrueNodeId;
    public string FalseNodeId;

    public ConditionNode(string id, int compareValue, string trueNodeId, string falseNodeId)
        : base(id, trueNodeId)
    {
        CompareValue = compareValue;
        TrueNodeId = trueNodeId;
        FalseNodeId = falseNodeId;
    }

    public override NodeExecResult Execute()
    {
        return null;
        //bool result = value >= CompareValue;

        //return new NodeExecResult
        //{
        //    Type = NodeExecResultType.Advance,
        //    Payload = result ? TrueNodeId : FalseNodeId
        //};
    }
}