// ������ʾ��Ϸ�е���Ʒ���ݣ���������Ʒ�ײ�model�Ľ�����
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


    // �����õĹ��캯��, effect,event��д����
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
        //������001���¼�
        Events = new List<string> { "event_001","event_002" };
        FilterEventsByCharacterTraits();
    }



    // ɸѡ���з��Ͻ�ɫ�Ը���¼�
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
