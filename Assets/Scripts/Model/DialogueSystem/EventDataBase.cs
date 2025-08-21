
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
                { "node_001", new DialogueNode("node_001", "node_002", "{characterName}", "今天天气真好呀！") },
                { "node_002", new DialogueNode("node_002", null,       "Bob",   "是啊，{characterName}出去走走吧。") }
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

                // 玩家接话，然后进入选择
                { "node_302", new DialogueNode("node_302", "node_Choice", "{characterName}", "真的吗？谢谢！要不要现在就穿上？") },

                // 选择节点，没有对话，只有选项
                { "node_Choice", new ChoiceNode(
                    "node_Choice",
                    "node_303", // 默认的“一个选项”id，就随便给个默认的nextNodeId
                    new List<ChoiceOption>
                    {
                        new ChoiceOption("当然要！", "node_303"),
                        new ChoiceOption("还是算了吧……", "node_304")
                    }
                )},

                { "node_303", new DialogueNode("node_303", null, "好友", "太好了！你穿上真的很合适。") },
                { "node_304", new DialogueNode("node_304", null, "好友", "好吧，下次有机会再穿！") }
            }
        ));



        AddEvent(new EventData(
            "event_005",
            DialogueType.Item,
            "node_401",
            new Dictionary<string, BaseNode>
            {
                { "node_401", new DialogueNode("node_401", "node_402", "{characterName}", "午饭想吃点什么？") },
                { "node_402", new DialogueNode("node_402", null,       "Bob",   "随便啦，{characterName}决定就好。") }
            }
        ));

        AddEvent(new EventData(
            "event_007",
            DialogueType.Item,
            "node_501",
            new Dictionary<string, BaseNode>
            {
                { "node_501", new DialogueNode("node_501", "node_choice", "好友", "我们去哪里玩？") },

                { "node_choice", new ChoiceNode(
                    "node_choice",
                    "node_502", // 默认选项的 nextNodeId，可以随便给
                    new List<ChoiceOption>
                    {
                        // 继续当前事件的分支
                        new ChoiceOption("去公园吧！", "node_502"),
                        // 跳到别的事件，用 NavigateNode 实现
                        new ChoiceOption("去商场逛逛！", "node_nav")
                    }
                )},

                { "node_502", new DialogueNode("node_502", null, "玩家", "好啊，我们去公园！") },

                // 跳转节点，直接触发另一个事件
                { "node_nav", new NavigateNode("node_nav", null, "event_003") }
            }
        ));

        // 事件示例：测试 ConditionNode
        AddEvent(new EventData(
            "event_008",
            DialogueType.Item,
            "node_start",
            new Dictionary<string, BaseNode>
            {
        // 开始节点：检查条件
        { "node_start", new ConditionNode(
            "node_start",
            new Dictionary<string, Condition>
            {
                { "SSS", new Condition { Type = ConditionType.AtMost, Value = 120 } },   // San >= 50
                { "A", new Condition { Type = ConditionType.Equal, Value = 100 } }  // Money >= 50
            },
            "node_true",  // 条件满足
            "node_false"  // 条件不满足
        )},

        // 条件满足分支
        { "node_true", new DialogueNode("node_true", "node_choice", "玩家", "太棒了，你状态很好，我们可以出发了！") },

        // 条件不满足分支
        { "node_false", new DialogueNode("node_false", "node_choice", "玩家", "你可能需要休息或补充一些道具。") },

        // 选择节点
        { "node_choice", new ChoiceNode(
            "node_choice",
            "node_end",
            new List<ChoiceOption>
            {
                new ChoiceOption("去公园吧！", "node_park"),
                new ChoiceOption("去商场逛逛！", "node_nav")
            }
        )},

        { "node_park", new DialogueNode("node_park", null, "好友", "好啊，我们去公园玩！") },

        // NavigateNode 跳转到其他事件
        { "node_nav", new NavigateNode("node_nav", null, "event_003") },

        { "node_end", new DialogueNode("node_end", null, "系统", "事件结束") }
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
