using System.Collections.Generic;
using System.Linq;
using static UnityEditor.Progress;

public class ItemDataBase 
{
    private static readonly Dictionary<string, ItemData> allItems = new();// ��Ϸ�����пɻ����Ʒ��ͼ����keyΪ��ƷID

    // ��̬���캯������ʼ����������
    static ItemDataBase()
    {
        AddItem(new ItemData("food001", "ƻ��", "�ָ�������ƻ��", 5, ItemType.Food));
        AddItem(new ItemData("food002", "���", "�򵥵����", 8, ItemType.Food));
        AddItem(new ItemData("food003", "ţ��", "����ţ��", 6, ItemType.Food));
    }

    public static void AddItem(ItemData item)
    {
        allItems[item.Id] = item;
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

}
