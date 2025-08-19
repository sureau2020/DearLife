using System.Collections.Generic;

public class NavigateNode : BaseNode
{
    public string TargetEventId; // Ҫ��ת���¼� ID

    public override NodeExecResult Execute(Dictionary<string, int> parameters)
    {
        // ����֪ͨ Runner ���¼�
        return new NodeExecResult
        {
            Type = NodeExecResultType.NavigateEvent,
            Payload = TargetEventId
        };
    }
}
