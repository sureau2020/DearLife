

using System.Collections.Generic;


public enum DialogueType { Daily, Item, Special, Clothes }

public class EventData 
{
    public string EventId;
    public DialogueType Type; 
    public List<string> Tags; // 事件对应啥性格，所有性格都满足才能触发此事件 
    public string StartNodeId;  // 入口节点
    public Dictionary<string, BaseNode> Nodes;


    // 目前硬编码，写死了tag测试
    public EventData(string eventId, DialogueType type, string startNodeId, Dictionary<string, BaseNode> nodes)
    {
        EventId = eventId;
        Type = type;
        StartNodeId = startNodeId;
        Nodes = nodes;
        Tags = new List<string> { "calm" }; 
    }
}
