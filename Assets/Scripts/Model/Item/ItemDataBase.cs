using System.Collections.Generic;
using System.Linq;
using static UnityEditor.Progress;

public class ItemDataBase 
{
    private static readonly Dictionary<string, ItemData> allItems = new();// 游戏中所有可获得物品的图鉴，key为物品ID

    // 静态构造函数，初始化测试数据
    static ItemDataBase()
    {
        AddItem(new ItemData("food001", "苹果", "恢复体力的苹果", 5, ItemType.Food));
        AddItem(new ItemData("food002", "面包", "简单的面包", 8, ItemType.Food));
        AddItem(new ItemData("food003", "牛奶", "新鲜牛奶", 6, ItemType.Food));
    }

    public static void AddItem(ItemData item)
    {
        allItems[item.Id] = item;
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

}
