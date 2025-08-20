
using System.Collections.Generic;
using UnityEngine;

public class EventDataBase 
{

    private static Dictionary<string, EventData> eventMap = new Dictionary<string, EventData>();

    static EventDataBase()
    {
        AddEvent(new EventData(
           "event_001",
           DialogueType.Item,
           "node_001",
           new Dictionary<string, BaseNode>
           {
                { "node_001", new DialogueNode("node_001", "node_002", "Alice", "�����������ѽ��") },
                { "node_002", new DialogueNode("node_002", null,       "Bob",   "�ǰ�����ȥ���߰ɡ�") }
           }
       ));

        AddEvent(new EventData(
            "event_002",
            DialogueType.Item,
            "node_101",
            new Dictionary<string, BaseNode>
            {
                { "node_101", new DialogueNode("node_101", "node_102", "��Ա", "��ӭ���٣������ʲô��") },
                { "node_102", new DialogueNode("node_102", null,       "���", "����һ��ƻ����") }
            }
        ));

        AddEvent(new EventData(
            "event_003",
            DialogueType.Item,
            "node_201",
            new Dictionary<string, BaseNode>
            {
                { "node_201", new DialogueNode("node_201", "node_202", "������", "���������ˡ����ҵ���ܾ��ˡ�") },
                { "node_202", new DialogueNode("node_202", null,       "���",   "�㵽����˭��") }
            }
        ));

        AddEvent(new EventData(
            "event_004",
            DialogueType.Item,
            "node_301",
            new Dictionary<string, BaseNode>
            {
                { "node_301", new DialogueNode("node_301", "node_302", "����", "�����·�ͦ�ʺ���ģ�") },
                { "node_302", new DialogueNode("node_302", null,       "���", "�����лл��") }
            }
        ));

        AddEvent(new EventData(
            "event_005",
            DialogueType.Item,
            "node_401",
            new Dictionary<string, BaseNode>
            {
                { "node_401", new DialogueNode("node_401", "node_402", "Alice", "�緹��Ե�ʲô��") },
                { "node_402", new DialogueNode("node_402", null,       "Bob",   "�������������ͺá�") }
            }
        ));
    }

    public static void AddEvent(EventData eventData)
    {
        eventMap[eventData.EventId] = eventData;
    }



    public static EventData GetEvent(string eventId)
    {
        if (eventMap.TryGetValue(eventId, out var ev))
            return ev;
        Debug.LogWarning($"EventDatabase: Event {eventId} not found!");
        return null;
    }
}
