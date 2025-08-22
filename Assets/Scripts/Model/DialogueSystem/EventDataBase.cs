using System.Collections.Generic;

public class EventDataBase 
{
    private static Dictionary<DialogueType, HashSet<string>> eventMapByType = new Dictionary<DialogueType, HashSet<string>>();
    private static Dictionary<string, EventData> eventMap = new Dictionary<string, EventData>();
    private static List<string> candidateDailyEventIds = new List<string>();



    // 添加事件到数据库，同时按类别储存id
    public static void AddEvent(EventData eventData)
    {
        if (eventMap.ContainsKey(eventData.EventId))
        {
            return;
        }
        eventMap.Add(eventData.EventId, eventData);
        RegisterEventByType(eventData);
    }


    // 注册事件到对应的类型映射,如果是每日事件，还会筛选符合角色性格的事件
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

    // 获取所有事件ID
    public static List<string> GetAllEventIds()
    {
        return new List<string>(eventMap.Keys);
    }
}
