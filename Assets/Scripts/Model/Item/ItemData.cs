// 这个类表示游戏中的商品数据，处理与商品底层model的交互。
using System.Collections.Generic;

public class ItemData
{
    public string Id { get; set; }
    public string Name { get; private set; } 
    public string Description { get; private set; }

    public string Author { get; private set; }
    public int Price { get; private set; }

    public ItemType Type { get; private set; }

    public Dictionary<EffectType, int> Effect { get; private set; }

    public List<string> Events { get; private set; }

    public List<string> FilteredEventIds { get; private set; }


    // 测试用的构造函数, effect,event是写死的
    public ItemData(string id, string name, string description, int price, ItemType type)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Type = type;
        Effect = new Dictionary<EffectType, int>
        {
            { EffectType.Full, 20 }
        };
        //包含“001”事件
        Events = new List<string> { "event_001","event_002" };
        FilterEventsByCharacterTraits();
    }



    // 筛选所有符合角色性格的事件
    private void FilterEventsByCharacterTraits()
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
}
