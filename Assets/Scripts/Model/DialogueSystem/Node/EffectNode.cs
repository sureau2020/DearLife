using System.Collections.Generic;

public class EffectNode : BaseNode
{
    public Dictionary<EffectType,int> Effects = new();

    public EffectNode(string id, string nextNodeId, Dictionary<EffectType, int> effects) 
        : base(id, nextNodeId)
    {
        Effects = effects;
    }

    public override NodeExecResult Execute(Dictionary<string, int> parameters)
    {
        foreach (var effect in Effects)
        {
            GameManager.Instance.StateManager.ApplyEffect(effect.Key, effect.Value);
        }

        // 执行完效果，立刻走下一节点
        return new NodeExecResult
        {
            Type = NodeExecResultType.Advance,
            Payload = NextNodeId // 可以为空，Runner 按顺序走
        };
    }
}
