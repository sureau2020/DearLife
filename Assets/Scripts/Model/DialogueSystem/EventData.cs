

using System.Collections.Generic;


public enum DialogueType { Daily, Item, Special, Clothes }

public class EventData 
{
    public string EventId;
    public DialogueType Type; 
    public List<string> Tags; // �¼���Ӧɶ�Ը������Ը�������ܴ������¼� 
    public string StartNodeId;  // ��ڽڵ�
    public Dictionary<string, BaseNode> Nodes;


    // ĿǰӲ���룬д����tag����
    public EventData(string eventId, DialogueType type, string startNodeId, Dictionary<string, BaseNode> nodes)
    {
        EventId = eventId;
        Type = type;
        StartNodeId = startNodeId;
        Nodes = nodes;
        Tags = new List<string> { "calm" }; 
    }
}
