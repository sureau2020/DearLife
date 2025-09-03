using System.Collections.Generic;
using System.Linq;

public class ItemDataBase 
{
    private static readonly Dictionary<string, ItemData> allItems = new();// 游戏中所有可获得物品的图鉴，key为物品ID


    public static void AddItem(ItemData item)
    {
        allItems[item.Id] = item;
        item.FilterEventsByCharacterTraits();
    }

    // 通过ID获取物品，没找到返回null
    public static ItemData GetItemById(string id)
    {
        if (allItems.TryGetValue(id, out ItemData item))
        {
            return item;
        }
        return null; 
    }

    // 获取给定类型的所有物品
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

    // 获取所有物品
    public static List<ItemData> GetAllItems()
    {
        return new List<ItemData>(allItems.Values);
    }
}
