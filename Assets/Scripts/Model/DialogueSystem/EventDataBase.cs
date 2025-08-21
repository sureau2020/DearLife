using System.Collections.Generic;

public class EventDataBase 
{
    private static Dictionary<DialogueType, HashSet<string>> eventMapByType = new Dictionary<DialogueType, HashSet<string>>();
    private static Dictionary<string, EventData> eventMap = new Dictionary<string, EventData>();
    private static List<string> candidateDailyEventIds = new List<string>();


    // ĿǰӲ���룬д�����¼����ԣ�������̶���һ��ɸѡ
    static EventDataBase()
    {
        AddEvent(new EventData(
           "event_001",
           DialogueType.Item,
           "node_001",
           new Dictionary<string, BaseNode>
           {
                { "node_001", new DialogueNode("node_001", "node_002", "{characterName}", "�����������ѽ��") },
                { "node_002", new DialogueNode("node_002", null,       "Bob",   "�ǰ���{characterName}��ȥ���߰ɡ�") }
           }
       ));

        AddEvent(new EventData(
            "event_002",
            DialogueType.Daily,
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

                // ��ҽӻ���Ȼ�����ѡ��
                { "node_302", new DialogueNode("node_302", "node_Choice", "{characterName}", "�����лл��Ҫ��Ҫ���ھʹ��ϣ�") },

                // ѡ��ڵ㣬û�жԻ���ֻ��ѡ��
                { "node_Choice", new ChoiceNode(
                    "node_Choice",
                    "node_303", // Ĭ�ϵġ�һ��ѡ�id����������Ĭ�ϵ�nextNodeId
                    new List<ChoiceOption>
                    {
                        new ChoiceOption("��ȻҪ��", "node_303"),
                        new ChoiceOption("�������˰ɡ���", "node_304")
                    }
                )},

                { "node_303", new DialogueNode("node_303", null, "����", "̫���ˣ��㴩����ĺܺ��ʡ�") },
                { "node_304", new DialogueNode("node_304", null, "����", "�ðɣ��´��л����ٴ���") }
            }
        ));



        AddEvent(new EventData(
            "event_005",
            DialogueType.Daily,
            "node_401",
            new Dictionary<string, BaseNode>
            {
                { "node_401", new DialogueNode("node_401", "node_402", "{characterName}", "�緹��Ե�ʲô��") },
                { "node_402", new DialogueNode("node_402", null,       "Bob",   "�������{characterName}�����ͺá�") }
            }
        ));

        AddEvent(new EventData(
            "event_007",
            DialogueType.Item,
            "node_501",
            new Dictionary<string, BaseNode>
            {
                { "node_501", new DialogueNode("node_501", "node_choice", "����", "����ȥ�����棿") },

                { "node_choice", new ChoiceNode(
                    "node_choice",
                    "node_502", // Ĭ��ѡ��� nextNodeId����������
                    new List<ChoiceOption>
                    {
                        // ������ǰ�¼��ķ�֧
                        new ChoiceOption("ȥ��԰�ɣ�", "node_502"),
                        // ��������¼����� NavigateNode ʵ��
                        new ChoiceOption("ȥ�̳���䣡", "node_nav")
                    }
                )},

                { "node_502", new DialogueNode("node_502", null, "���", "�ð�������ȥ��԰��") },

                // ��ת�ڵ㣬ֱ�Ӵ�����һ���¼�
                { "node_nav", new NavigateNode("node_nav", null, "event_003") }
            }
        ));

        // �¼�ʾ�������� ConditionNode
        AddEvent(new EventData(
            "event_008",
            DialogueType.Item,
            "node_start",
            new Dictionary<string, BaseNode>
            {
        // ��ʼ�ڵ㣺�������
        { "node_start", new ConditionNode(
            "node_start",
            new Dictionary<string, Condition>
            {
                { "SSS", new Condition { Type = ConditionType.AtMost, Value = 120 } },   // San >= 50
                { "A", new Condition { Type = ConditionType.Equal, Value = 100 } }  // Money >= 50
            },
            "node_true",  // ��������
            "node_false"  // ����������
        )},

        // ���������֧
        { "node_true", new DialogueNode("node_true", "node_choice", "���", "̫���ˣ���״̬�ܺã����ǿ��Գ����ˣ�") },

        // �����������֧
        { "node_false", new DialogueNode("node_false", "node_choice", "���", "�������Ҫ��Ϣ�򲹳�һЩ���ߡ�") },

        // ѡ��ڵ�
        { "node_choice", new ChoiceNode(
            "node_choice",
            "node_end",
            new List<ChoiceOption>
            {
                new ChoiceOption("ȥ��԰�ɣ�", "node_park"),
                new ChoiceOption("ȥ�̳���䣡", "node_nav")
            }
        )},

        { "node_park", new DialogueNode("node_park", null, "����", "�ð�������ȥ��԰�棡") },

        // NavigateNode ��ת�������¼�
        { "node_nav", new NavigateNode("node_nav", null, "event_003") },

        { "node_end", new DialogueNode("node_end", null, "ϵͳ", "�¼�����") }
            }
        ));

        // �¼�ʾ�������� EffectNode
        AddEvent(new EventData(
            "event_009",
            DialogueType.Item,
            "node_start",
            new Dictionary<string, BaseNode>
            {
        // ��ʼ�ڵ㣺��ͨ�Ի�
        { "node_start", new DialogueNode(
            "node_start",
            "node_effect",
            "����",
            "�٣������״̬��ô����"
        )},

        // Ч���ڵ㣺���� San �� Money
        { "node_effect", new EffectNode(
            "node_effect",
            "node_after_effect",
            new Dictionary<string,int>
            {
                { "Love", 50 },      
                { "Money", 50 },
            }
        )},

        // Ч����Ի�
        { "node_after_effect", new DialogueNode(
            "node_after_effect",
            "node_choice",
            "���",
            "�ۣ��Ҹо��ö��ˣ�"
        )},

        // ѡ��ڵ�
        { "node_choice", new ChoiceNode(
            "node_choice",
            "node_end",
            new List<ChoiceOption>
            {
                new ChoiceOption("ȥɢ���ɣ�", "node_walk"),
                new ChoiceOption("ȥ������", "node_shop")
            }
        )},

        { "node_walk", new DialogueNode("node_walk", null, "����", "�ð�������ȥɢ����") },

        { "node_shop", new DialogueNode("node_shop", null, "����", "�ã�����ȥ�̵��䣡") },

        { "node_end", new DialogueNode("node_end", null, "ϵͳ", "�¼�����") }
            }
        ));


    }


    // ����¼������ݿ⣬ͬʱ����𴢴�id
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


    // tag���з���true
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
