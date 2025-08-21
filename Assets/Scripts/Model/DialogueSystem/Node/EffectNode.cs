using System.Collections.Generic;

public class EffectNode : BaseNode
{
    public Dictionary<string,int> Effects = new();

    public EffectNode(string id, string nextNodeId, Dictionary<string, int> effects) 
        : base(id, nextNodeId)
    {
        Effects = effects;
    }

    // Ŀǰֻ��+Ч����û��=Ч��
    public override NodeExecResult Execute()
    {
        foreach (var effect in Effects)
        {
            GameManager.Instance.SetValueOfState(effect.Key, effect.Value);
        }

        // ִ����Ч������������һ�ڵ�
        return new NodeExecResult
        {
            Type = NodeExecResultType.Advance,
            Payload = NextNodeId 
        };
    }
}
