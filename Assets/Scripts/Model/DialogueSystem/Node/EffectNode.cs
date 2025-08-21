using System.Collections.Generic;

public class EffectNode : BaseNode
{
    public Dictionary<string,int> Effects = new();

    public EffectNode(string id, string nextNodeId, Dictionary<string, int> effects) 
        : base(id, nextNodeId)
    {
        Effects = effects;
    }

    // 目前只有+效果，没有=效果
    public override NodeExecResult Execute()
    {
        foreach (var effect in Effects)
        {
            GameManager.Instance.SetValueOfState(effect.Key, effect.Value);
        }

        // 执行完效果，立刻走下一节点
        return new NodeExecResult
        {
            Type = NodeExecResultType.Advance,
            Payload = NextNodeId 
        };
    }
}
