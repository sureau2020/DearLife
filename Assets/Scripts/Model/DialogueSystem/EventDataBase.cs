
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
                { "node_001", new DialogueNode("node_001", "node_002", "Alice", "今天天气真好呀！") },
                { "node_002", new DialogueNode("node_002", null,       "Bob",   "是啊，出去走走吧。") }
           }
       ));

        AddEvent(new EventData(
            "event_002",
            DialogueType.Item,
            "node_101",
            new Dictionary<string, BaseNode>
            {
                { "node_101", new DialogueNode("node_101", "node_102", "店员", "欢迎光临！想买点什么吗？") },
                { "node_102", new DialogueNode("node_102", null,       "玩家", "给我一个苹果。") }
            }
        ));

        AddEvent(new EventData(
            "event_003",
            DialogueType.Item,
            "node_201",
            new Dictionary<string, BaseNode>
            {
                { "node_201", new DialogueNode("node_201", "node_202", "神秘人", "你终于来了……我等你很久了。") },
                { "node_202", new DialogueNode("node_202", null,       "玩家",   "你到底是谁？") }
            }
        ));

        AddEvent(new EventData(
            "event_004",
            DialogueType.Item,
            "node_301",
            new Dictionary<string, BaseNode>
            {
                { "node_301", new DialogueNode("node_301", "node_302", "好友", "这套衣服挺适合你的！") },
                { "node_302", new DialogueNode("node_302", null,       "玩家", "真的吗？谢谢！") }
            }
        ));

        AddEvent(new EventData(
            "event_005",
            DialogueType.Item,
            "node_401",
            new Dictionary<string, BaseNode>
            {
                { "node_401", new DialogueNode("node_401", "node_402", "Alice", "午饭想吃点什么？") },
                { "node_402", new DialogueNode("node_402", null,       "Bob",   "随便啦，你决定就好。") }
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
