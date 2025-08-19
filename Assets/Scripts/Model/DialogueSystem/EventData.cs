

using System.Collections.Generic;


public enum DialogueType { Daily, Item, Special, Clothes }

public class EventData 
{
    public string EventId;
    public DialogueType Type; 
    public string StartNodeId;  // 入口节点
    public Dictionary<string, BaseNode> Nodes;


}
