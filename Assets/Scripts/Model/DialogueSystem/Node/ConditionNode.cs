using System.Collections.Generic;

public class ConditionNode : BaseNode
{
    public Dictionary<string,Condition> Conditions;
    public string TrueNodeId;
    public string FalseNodeId;

    public ConditionNode(string id, Dictionary<string, Condition> conditions, string trueNodeId, string falseNodeId)
        : base(id, trueNodeId)
    {
        Conditions = conditions;
        TrueNodeId = trueNodeId;
        FalseNodeId = falseNodeId;
    }

    // 这个节点会检查所有条件，如果全部满足则走 TrueNodeId，否则走 FalseNodeId
    public override NodeExecResult Execute()
    {
        foreach (var condition in Conditions)
        {
            int stateValue = GameManager.Instance.GetValueOfState(condition.Key);
            if (stateValue == int.MinValue)
            {
                return ReturnResult(false);
            }
            ConditionType conditionType = condition.Value.Type;
            bool result = false;
            switch (conditionType)
            {
                case ConditionType.AtLeast:
                    result = stateValue >= condition.Value.Value;
                    break;
                case ConditionType.AtMost:
                    result = stateValue <= condition.Value.Value;
                    break;
                case ConditionType.Equal:
                    result = stateValue == condition.Value.Value;
                    break;
            }
            if (!result)
            {
                return ReturnResult(false);
            }
        }
        return ReturnResult(true);
    }

    private NodeExecResult ReturnResult(bool allConditionsMet)
    {
        return new NodeExecResult
        {
            Type = NodeExecResultType.Advance,
            Payload = allConditionsMet ? TrueNodeId : FalseNodeId
        };
    }

}

public class Condition { 
    public ConditionType Type;
    public int Value;
}

public enum ConditionType
{
    AtLeast,
    AtMost,
    Equal
}