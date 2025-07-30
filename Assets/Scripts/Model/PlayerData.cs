using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 这个类表示玩家（使用这个软件的用户）数据，处理与玩家底层model的交互
public class PlayerData 
{
    public const int MaxBagCapacity = 30; // 背包最大容量30项不同物品
    public int Money { get; private set; } = 0;//金钱

    public List<ItemEntryData> Items { get; private set; } = new List<ItemEntryData>();//背包里有啥物品



    // 购买给定数量的物品，没钱返回，背包满了且没有相同物品返回。(返回失败OperationResult)
    // 成功则扣钱并添加物品到背包，没有相同物品则新建一项，有的话数量加1，都插入list首位，返回成功OperationResult
    public OperationResult BuyItem(int cost, ItemData item, int quantity) {
        if (!IsMoneyEnough(cost * quantity)) { 
            return OperationResult.Fail("金币不足。");
        }
        ItemEntryData isInBag = GetTheItemInBag(item.Id);
        if (!IsBagHasSpace() && isInBag == null)
        {
            return OperationResult.Fail("背包已满。");
        }
        Money -= cost; 
        if( isInBag == null)
        {
            // 背包里没有这个物品，添加新条目
            ItemEntryData newItem = new(item.Id, quantity);
            Items.Insert(0, newItem);
        }
        else
        {
            isInBag.Count+= quantity;
            Items.Remove(isInBag);
            Items.Insert(0, isInBag);
        }
        return OperationResult.Complete();
    }


    // 使用背包里的物品，扣除数量，返回操作结果
    public OperationResult UseItem(string itemId, int quantity, CharacterData character)
    {
        ItemEntryData entry = GetTheItemInBag(itemId);
        if (entry == null || entry.Count < quantity)
        {
            return OperationResult.Fail("背包里物品数量不足。");
        }
        entry.Count -= quantity;
        if (entry.Count <= 0)
        {
            Items.Remove(entry);
        }
        return character.ApplyItemEffect(ItemDataBase.GetItemById(itemId), quantity);
    }


    // 检查背包里是否有相同物品,有的话返回该物品条目，没有则返回null
    public ItemEntryData GetTheItemInBag(string itemId)
    {
        foreach (var entry in Items)
        {
            if (entry.ItemID == itemId)
            {
                return entry;
            }
        }
        return null;
    }



    //获取背包里特定物品的数量，没有返回0
    public int GetItemCount(string itemId)
    {
        ItemEntryData entry = GetTheItemInBag(itemId);
        if (entry != null)
        {
            return entry.Count;
        }
        return 0;
    }

    // 背包还有空位不
    public bool IsBagHasSpace()
    {
        return Items.Count <= MaxBagCapacity;
    }


    public bool IsMoneyEnough(int cost)
    {
        return Money >= cost;
    }




}
