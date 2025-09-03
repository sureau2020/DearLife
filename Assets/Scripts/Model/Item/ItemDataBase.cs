using System.Collections.Generic;
using System.Linq;

public class ItemDataBase 
{
    private static readonly Dictionary<string, ItemData> allItems = new();// ��Ϸ�����пɻ����Ʒ��ͼ����keyΪ��ƷID


    public static void AddItem(ItemData item)
    {
        allItems[item.Id] = item;
        item.FilterEventsByCharacterTraits();
    }

    // ͨ��ID��ȡ��Ʒ��û�ҵ�����null
    public static ItemData GetItemById(string id)
    {
        if (allItems.TryGetValue(id, out ItemData item))
        {
            return item;
        }
        return null; 
    }

    // ��ȡ�������͵�������Ʒ
    public static List<ItemData> GetItemsByType(ItemType type)
    {
        List<ItemData> result = new List<ItemData>();
        foreach (ItemData item in allItems.Values)
        {
            if (item.Type == type)
                result.Add(item);
        }
        return result;
    }

    // ��ȡ������Ʒ
    public static List<ItemData> GetAllItems()
    {
        return new List<ItemData>(allItems.Values);
    }
}
