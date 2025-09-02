// ������ʾ��Ϸ�е���Ʒ���ݣ���������Ʒ�ײ�model�Ľ�����
using Newtonsoft.Json;
using System.Collections.Generic;

public class ItemData
{
    public string Id { get; set; }
    public string Name { get; set; } 
    public string Description { get; set; }

    public string ImagePath { get; set; }
    public string Author { get; set; }
    public int Price { get; set; }

    public ItemType Type { get; set; }

    public Dictionary<EffectType, int> Effect { get; set; }

    public List<string> Events { get; set; }

    public List<string> FilteredEventIds { get; set; }


    // ɸѡ���з��Ͻ�ɫ�Ը���¼�
    public void FilterEventsByCharacterTraits()
    {
        FilteredEventIds = new List<string>();
        foreach (var eventId in Events)
        {
            if (EventDataBase.IsEventMatchCharacterByEventId(eventId))
            {
                FilteredEventIds.Add(eventId);
            }
        }
    }

    [JsonConstructor]
    public ItemData()
    {
        Effect = new Dictionary<EffectType, int>();
        Events = new List<string>();
        FilteredEventIds = new List<string>();
    }
}
