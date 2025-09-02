// 这个类表示游戏中的商品数据，处理与商品底层model的交互。
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


    // 筛选所有符合角色性格的事件
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
