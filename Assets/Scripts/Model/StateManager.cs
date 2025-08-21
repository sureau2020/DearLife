

using System;
using System.Collections.Generic;
using System.Diagnostics;

public class StateManager 
{
    public PlayerData Player { get; }
    public CharacterData Character { get; }
    public GameSettings Settings { get; }
    public Dictionary<string, int> CustomStates { get; }

    private const int DelayTime = 10;

    private int cleanDecayCounter = 0;
    private int sanDecayCounter = 0;


    public StateManager(PlayerData player, CharacterData character, GameSettings settings, Dictionary<string,int> customStates)
    {
        Player = player;
        Character = character;
        Settings = settings;
        CustomStates = customStates;
    }


    // 没有就返回minValue
    public int GetCustomState(string key)
    {
        if (CustomStates.TryGetValue(key, out int value))
        {
            return value;
        }
        return int.MinValue; 
    }

    public void SetCustomState(string key, int value)
    {
        if (CustomStates.ContainsKey(key))
        {
            CustomStates[key] += value;
        }
        else
        {
            CustomStates.Add(key, value);
        }
    }


    // 每分钟调用一次，衰减角色状态
    public void DecayStates()
    {
        Character.ChangeFull(-1);

        cleanDecayCounter++;
        if (cleanDecayCounter >= 2)
        {
            Character.ChangeClean(-1);
            cleanDecayCounter = 0;
        }

        sanDecayCounter++;
        if (sanDecayCounter >= 4)
        {
            Character.ChangeSan(-1);
            sanDecayCounter = 0;
        }
    }


    // 购买物品，扣除金钱，添加物品到背包，返回操作结果
    public OperationResult BuyItem(int singlePrice, ItemData item, int quantity)
    {
        return Player.BuyItem(singlePrice, item, quantity);
    }


    // 从玩家背包里拿去物品，扣除数量，应用效果，返回操作结果
    public OperationResult UseItem(string itemId, int quantity)
    {
        OperationResult isTakeItem = Player.UseItem(itemId, quantity);
        if (!isTakeItem.Success)
        {
            return isTakeItem;
        }
        return Character.ApplyItemEffect(itemId, quantity);
    }

    public OperationResult ApplyEffect(EffectType type, int quantity)
    {
        return Character.ApplyEffect(type, quantity);
    }

    // 任务标记为完成，让玩家获得报酬
    public OperationResult CompleteMission(MissionData mission)
    {
        if (mission == null)
        {
            return OperationResult.Fail("任务不存在，尝试重启。");
        }
        OperationResult isOKSetting = Settings.Validate();
        if (!isOKSetting.Success)
        {
            return isOKSetting;
        }
        OperationResult isNormallyComplete = mission.CompleteMission();
        if (!isNormallyComplete.Success)
        {
            return isNormallyComplete;
        }
        int salary = Calculators.RandomSalary(mission.Duration,mission.Difficulty,Settings.MaxSalaryFactor,Settings.HourlyWage,Settings.DifficultyBonus);
        return Player.EarnMoney(salary);
    }



    // 检查玩家是否有足够的金钱，有的话最近几个任务的ddl推迟
    public OperationResult PushRecentMissionsDDL(MissionData mission)
    {
        if (!Player.IsMoneyEnough(Settings.DelayCost))
        {
            return OperationResult.Fail("金币不足，无法延迟任务。");
        }
        OperationResult isDelayAllowed = TaskManagerModel.Instance.pushDDL(mission, DelayTime);
        if (!isDelayAllowed.Success)
        {
            return isDelayAllowed;
        }
        Player.SpendMoney(Settings.DelayCost);
        return OperationResult.Complete();
    }

    public List<ItemEntryData> GetAllItemsInBackPack()
    {
        return Player.Items;
    }
}
