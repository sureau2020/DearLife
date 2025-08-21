
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
                { "node_001", new DialogueNode("node_001", "node_002", "{characterName}", "�����������ѽ��") },
                { "node_002", new DialogueNode("node_002", null,       "Bob",   "�ǰ���{characterName}��ȥ���߰ɡ�") }
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
            DialogueType.Item,
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
