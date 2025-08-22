using System.Collections.Generic;

public class EventDataBase 
{
    private static Dictionary<DialogueType, HashSet<string>> eventMapByType = new Dictionary<DialogueType, HashSet<string>>();
    private static Dictionary<string, EventData> eventMap = new Dictionary<string, EventData>();
    private static List<string> candidateDailyEventIds = new List<string>();



    // ����¼������ݿ⣬ͬʱ����𴢴�id
    public static void AddEvent(EventData eventData)
    {
        if (eventMap.ContainsKey(eventData.EventId))
        {
            return;
        }
        eventMap.Add(eventData.EventId, eventData);
        RegisterEventByType(eventData);
    }


    // ע���¼�����Ӧ������ӳ��,�����ÿ���¼�������ɸѡ���Ͻ�ɫ�Ը���¼�
    private static void RegisterEventByType(EventData eventData)
    {
        if (!eventMapByType.ContainsKey(eventData.Type))
        {
            eventMapByType[eventData.Type] = new HashSet<string>();
        }
        eventMapByType[eventData.Type].Add(eventData.EventId);
        if (eventData.Type == DialogueType.Daily)
        {
            FilterDailyEventsMatchCharacter(eventData);
        }
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

    // ��ȡ�����¼�ID
    public static List<string> GetAllEventIds()
    {
        return new List<string>(eventMap.Keys);
    }
}
