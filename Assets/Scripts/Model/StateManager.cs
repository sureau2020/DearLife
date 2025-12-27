using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StateManager 
{
    public PlayerData Player { get; }
    public CharacterData Character { get; }
    public GameSettings Settings { get; }
    public Dictionary<string, int> CustomStates { get; }
    

    public DateTime SaveTime { get; set; } = DateTime.Now;

    private const int DelayTime = 10;

    private int cleanDecayCounter = 0;
    private int sanDecayCounter = 0;


    public StateManager(PlayerData player, CharacterData character, GameSettings settings, Dictionary<string,int> customStates, DateTime savetime)
    {
        Player = player;
        Character = character;
        Settings = settings;
        CustomStates = customStates;
        SaveTime = savetime;
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


    //衰减角色状态
    public void DecayStates() {
        if (Character.HealthState == HealthState.Dead){
                return;
        }
        if (Character.HealthState == HealthState.Crazy)
        {
            Character.ChangeFull(-3);
            Character.ChangeLove(-2);
        }
        else {
            Character.ChangeFull(-1);
        }
        cleanDecayCounter++;
        if (cleanDecayCounter >= 2)
        {
            Character.ChangeClean(-1);
            cleanDecayCounter = 0;
        }
        sanDecayCounter++;
        if (sanDecayCounter >= 4)
        {
            if (Character.HealthState == HealthState.Dirty)
            {
                Character.ChangeSan(-3);
            }
            else
                Character.ChangeSan(-1);
            sanDecayCounter = 0;
        }
    }

    public void DecayStatesWithoutNotify()
    {
        if (Character.HealthState == HealthState.Dead)
        {
            return;
        }
        if (Character.HealthState == HealthState.Crazy)
        {
            Character.ChangeFull(-3,false);
            Character.ChangeLove(-2, false);
        }
        else
        {
            Character.ChangeFull(-1, false);
        }
        cleanDecayCounter++;
        if (cleanDecayCounter >= 2)
        {
            Character.ChangeClean(-1, false);
            cleanDecayCounter = 0;
        }
        sanDecayCounter++;
        if (sanDecayCounter >= 4)
        {
            if (Character.HealthState == HealthState.Dirty)
            {
                Character.ChangeSan(-3, false);
            }
            else
                Character.ChangeSan(-1, false);
            sanDecayCounter = 0;
        }
    }


    // 购买物品，扣除金钱，添加物品到背包，返回操作结果
    public OperationResult BuyItem(int singlePrice, ItemData item, int quantity)
    {
        return Player.BuyItem(singlePrice, item, quantity);
    }

    public OperationResult BuyCloth(int price, WardrobeSlot cloth)
    {
        return Player.BuyCloth(price, cloth);
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
        bool wasExpired = false;
        if (!isNormallyComplete.Success)
        {
            if (isNormallyComplete.Message.Contains("任务已过期。"))
            {
                wasExpired = true;
            }
            else
            {
                return isNormallyComplete;
            }
        }
        int salary = Calculators.RandomSalary(mission.Duration,mission.Difficulty,Settings.MaxSalaryFactor,Settings.HourlyWage,Settings.DifficultyBonus);
        if(wasExpired)
        {
            salary /= 2; // 过期任务只给一半报酬
        }   
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

    public void Resetmoney()
    {
        Player.ResetMoney();
    }

    // 保存当前状态
    public OperationResult SaveState()
    {
        return SaveManager.SaveStateManager(this);
    }

    public async Task<OperationResult> SaveStateAsync()
    {
        return await SaveManager.SaveStateManagerAsync(this);
    }

    // 从保存数据恢复状态
    public static OperationResult<StateManager> LoadState()
    {
        var loadResult = SaveManager.LoadStateManager();
        if (!loadResult.Success)
        {
            return OperationResult<StateManager>.Fail(loadResult.Message);
        }

        try
        {
            var saveData = loadResult.Data;
            CharacterData characterData = saveData.Character;
            if (BootSceneManager.Instance != null && BootSceneManager.Instance.CreatedCharacter != null)
            {
                characterData = BootSceneManager.Instance.CreatedCharacter;
            }

            StateManager stateManager = new StateManager(
                saveData.Player, 
                characterData,
                saveData.Settings, 
                saveData.CustomStates,
                saveData.SaveTime
            );
            
            return OperationResult<StateManager>.Complete(stateManager);
        }
        catch (System.Exception ex)
        {
            return OperationResult<StateManager>.Fail($"恢复状态失败：{ex.Message}");
        }
    }
}
