using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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


    // û�оͷ���minValue
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


    // ÿ���ӵ���һ�Σ�˥����ɫ״̬
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


    // ������Ʒ���۳���Ǯ�������Ʒ�����������ز������
    public OperationResult BuyItem(int singlePrice, ItemData item, int quantity)
    {
        return Player.BuyItem(singlePrice, item, quantity);
    }


    // ����ұ�������ȥ��Ʒ���۳�������Ӧ��Ч�������ز������
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

    // ������Ϊ��ɣ�����һ�ñ���
    public OperationResult CompleteMission(MissionData mission)
    {
        if (mission == null)
        {
            return OperationResult.Fail("���񲻴��ڣ�����������");
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



    // �������Ƿ����㹻�Ľ�Ǯ���еĻ�������������ddl�Ƴ�
    public OperationResult PushRecentMissionsDDL(MissionData mission)
    {
        if (!Player.IsMoneyEnough(Settings.DelayCost))
        {
            return OperationResult.Fail("��Ҳ��㣬�޷��ӳ�����");
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

    // ���浱ǰ״̬
    public OperationResult SaveState()
    {
        return SaveManager.SaveStateManager(this);
    }

    public async Task<OperationResult> SaveStateAsync()
    {
        return await SaveManager.SaveStateManagerAsync(this);
    }

    // �ӱ������ݻָ�״̬
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
                saveData.CustomStates
            );
            
            return OperationResult<StateManager>.Complete(stateManager);
        }
        catch (System.Exception ex)
        {
            return OperationResult<StateManager>.Fail($"�ָ�״̬ʧ�ܣ�{ex.Message}");
        }
    }
}
