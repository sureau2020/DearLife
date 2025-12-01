using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class WardrobeSlot
{
    public string State;   // "Locked", "Empty", "Own", "AddButton"
    public bool IsBuiltIn; // 是否为内置衣服
    public string Id;      // BuiltIn 的衣服 ID
    public int Price = 100; // 解锁价格

    public WardrobeSlot(string state, bool isBuiltIn, string id, int price)
    {
        State = state;
        IsBuiltIn = isBuiltIn;
        Id = id;
        Price = price;
    }

    
}

[System.Serializable]  
public class WardrobeData
{
    public static Dictionary<string, WardrobeSlot> Slots = new Dictionary<string, WardrobeSlot>();


    public static void AddCloth(WardrobeSlot slot)
    {
        Slots[slot.Id] = slot;
    }


    public static List<WardrobeSlot> GetAllClothes()
    {
        //排除id为1、2、3的内置衣服
        //var excludeIds = new HashSet<string> { "1", "2", "3" };
        //var result = new List<WardrobeSlot>();
        
        //foreach (var slot in Slots.Values)
        //{
        //    if (!excludeIds.Contains(slot.Id))
        //    {
        //        result.Add(slot);
        //    }
        //}
        
        return new List<WardrobeSlot>(Slots.Values);
    }
}
