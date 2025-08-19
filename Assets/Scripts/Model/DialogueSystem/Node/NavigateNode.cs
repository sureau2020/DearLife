using System.Collections.Generic;

public class NavigateNode : BaseNode
{
    public string TargetEventId; // 要跳转的事件 ID

    public override NodeExecResult Execute(Dictionary<string, int> parameters)
    {
        // 立刻通知 Runner 跳事件
        return new NodeExecResult
        {
            Type = NodeExecResultType.NavigateEvent,
            Payload = TargetEventId
        };
    }
}
