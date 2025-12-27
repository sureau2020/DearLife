using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 这个类表示玩家（使用这个软件的用户）数据，处理与玩家底层model的交互
public class PlayerData 
{
    public const int MaxBagCapacity = 8; // 背包最大容量8项不同物品

    public const int MaxMoney = 99999999; 

    public int Money { get; private set; } = 0;//金钱

    public event Action<int> OnMoneyChanged;


    // list是倒序存的，最新的物品在前面
    public List<ItemEntryData> Items { get; private set; } = new List<ItemEntryData>();//背包里有啥物品

    public PlayerData(int money)
    {
        Money = money;

    }



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
        SpendMoney(cost * quantity);
        if ( isInBag == null)
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

    // 购买衣服，没钱返回失败OperationResult，成功扣钱返回成功OperationResult
    //TODO: 衣服购买后要保存
    public OperationResult BuyCloth(int cost, WardrobeSlot cloth)
    {
        if (!IsMoneyEnough(cost))
        {
            return OperationResult.Fail("金币不足。");
        }
        if (cloth.State == "AddButton")
        {
            cloth.State = "Empty";
        }
        else {
            cloth.State = "Own";
        }
        SpendMoney(cost);
        return OperationResult.Complete();
    }


    //完成任务后玩家收到金币
    public OperationResult EarnMoney(int salary)
    {
        long tmp = (long)Money + salary;
        if (tmp > MaxMoney)
        {
            Money = MaxMoney;
            OnMoneyChanged?.Invoke(Money);
            return OperationResult.Fail("金币已达上限，无法继续获得金币。");
        }
        if (salary < 0 || Money < 0 || tmp < 0)
        {
            Money = 0;
            OnMoneyChanged?.Invoke(Money);
            return OperationResult.Fail("非法的金币数，已归零。");
        }
        Money += salary;
        OnMoneyChanged?.Invoke(Money);
        _ = GameManager.Instance.StateManager.SaveStateAsync();
        return OperationResult.Complete();
    }


    // REQUIRE:调用时已经验证过钱够了
    // 花钱,通知更新状态
    public void SpendMoney(int cost)
    {
        Money -= cost;
        _ = GameManager.Instance.StateManager.SaveStateAsync();
        OnMoneyChanged?.Invoke(Money);
    }

    // 钱归零,通知更新状态
    public void ResetMoney()
    {
        Money = 0;
        _ = GameManager.Instance.StateManager.SaveStateAsync();
        OnMoneyChanged?.Invoke(Money);
    }


    // 拿去玩家背包里的物品，扣除数量，返回操作结果
    public OperationResult UseItem(string itemId, int quantity)
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
        return OperationResult.Complete();
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
        return Items.Count < MaxBagCapacity;
    }


    public bool IsMoneyEnough(int cost)
    {
        return Money >= cost;
    }

    
}
