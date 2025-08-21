using System.Collections.Generic;

public class EventDataBase 
{
    private static Dictionary<DialogueType, HashSet<string>> eventMapByType = new Dictionary<DialogueType, HashSet<string>>();
    private static Dictionary<string, EventData> eventMap = new Dictionary<string, EventData>();
    private static List<string> candidateDailyEventIds = new List<string>();


    // 目前硬编码，写死了事件测试，启动后固定有一次筛选
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
            DialogueType.Daily,
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
            DialogueType.Daily,
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

        // 事件示例：测试 EffectNode
        AddEvent(new EventData(
            "event_009",
            DialogueType.Item,
            "node_start",
            new Dictionary<string, BaseNode>
            {
        // 开始节点：普通对话
        { "node_start", new DialogueNode(
            "node_start",
            "node_effect",
            "好友",
            "嘿，你今天状态怎么样？"
        )},

        // 效果节点：增加 San 和 Money
        { "node_effect", new EffectNode(
            "node_effect",
            "node_after_effect",
            new Dictionary<string,int>
            {
                { "Love", 50 },      
                { "Money", 50 },
            }
        )},

        // 效果后对话
        { "node_after_effect", new DialogueNode(
            "node_after_effect",
            "node_choice",
            "玩家",
            "哇，我感觉好多了！"
        )},

        // 选择节点
        { "node_choice", new ChoiceNode(
            "node_choice",
            "node_end",
            new List<ChoiceOption>
            {
                new ChoiceOption("去散步吧！", "node_walk"),
                new ChoiceOption("去买东西！", "node_shop")
            }
        )},

        { "node_walk", new DialogueNode("node_walk", null, "好友", "好啊，我们去散步！") },

        { "node_shop", new DialogueNode("node_shop", null, "好友", "好，我们去商店逛逛！") },

        { "node_end", new DialogueNode("node_end", null, "系统", "事件结束") }
            }
        ));


    }


    // 添加事件到数据库，同时按类别储存id
    public static void AddEvent(EventData eventData)
    {
        if (eventMap.ContainsKey(eventData.EventId))
        {
            return;
        }
        eventMap.Add(eventData.EventId, eventData);
        RegisterEventByType(eventData);
        FilterDailyEventsMatchCharacter(eventData);
    }

    private static void RegisterEventByType(EventData eventData)
    {
        if (!eventMapByType.ContainsKey(eventData.Type))
        {
            eventMapByType[eventData.Type] = new HashSet<string>();
        }
        eventMapByType[eventData.Type].Add(eventData.EventId);
    }

    private static void FilterDailyEventsMatchCharacter(EventData eventData) {
        if (IsEventMatchCharacter(eventData)) { 
            candidateDailyEventIds.Add(eventData.EventId);
        }
    }


    public static EventData GetEvent(string eventId)
    {
        if (eventMap.TryGetValue(eventId, out var ev))
            return ev;
        return null;
    }

    public static List<string> GetCandidateDailyEventIds() { 
        return candidateDailyEventIds;
    }

    //public static EventData GetItemEvent(string eventId) { 
    //    return GetEvent(eventId, DialogueType.Item);
    //}

    //private static EventData GetEvent(string eventId, DialogueType type)
    //{
    //    if (eventMapByType.TryGetValue(type, out var events) && events.TryGetValue(eventId, out var eventData))
    //    {
    //        return eventData;
    //    }
    //    return null;
    //}


    public static bool IsEventMatchCharacterByEventId(string eventId) { 
        EventData eventData = GetEvent(eventId);
        if (eventData == null) return false;
        return IsEventMatchCharacter(eventData);
    }


    // tag都有返回true
    public static bool IsEventMatchCharacter(EventData eventData)
    {
        HashSet<string> personalities = GameManager.Instance.StateManager.Character.PersonalityTags;
        foreach (var tag in eventData.Tags)
        {
            if (!personalities.Contains(tag))
            {
                return false; 
            }
        }
        return true; 
    }

}
