using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDataBase : MonoBehaviour
{

    private static Dictionary<string, EventData> eventMap = new Dictionary<string, EventData>();
    public static EventData GetEvent(string eventId)
    {
        if (eventMap.TryGetValue(eventId, out var ev))
            return ev;
        Debug.LogWarning($"EventDatabase: Event {eventId} not found!");
        return null;
    }
}
