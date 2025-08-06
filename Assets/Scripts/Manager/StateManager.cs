

using System;
using System.Collections.Generic;
using System.Diagnostics;

public class StateManager 
{
    public PlayerData Player { get; }
    public CharacterData Character { get; }
    public GameSettings Settings { get; }

    private const int DelayTime = 10;

    public event Action<string,int> OnCharacterStateChanged;// ��ɫ״̬�仯�¼�������Ϊ״̬���ƺͱ仯ֵ

    private int cleanDecayCounter = 0;
    private int sanDecayCounter = 0;


    public StateManager(PlayerData player, CharacterData character, GameSettings settings)
    {
        Player = player;
        Character = character;
        Settings = settings;
    }



    // ÿ���ӵ���һ�Σ�˥����ɫ״̬
    public void DecayStates()
    {
        Character.ChangeFull(-1);
        OnCharacterStateChanged?.Invoke("Full", Character.Full);

        cleanDecayCounter++;
        if (cleanDecayCounter >= 2)
        {
            Character.ChangeClean(-1);
            OnCharacterStateChanged?.Invoke("Clean", Character.Clean);
            cleanDecayCounter = 0;
        }

        sanDecayCounter++;
        if (sanDecayCounter >= 4)
        {
            Character.ChangeSan(-1);
            OnCharacterStateChanged?.Invoke("San", Character.San);
            sanDecayCounter = 0;
        }
    }


    // ������Ʒ���۳���Ǯ�������Ʒ�����������ز������
    public OperationResult BuyItem(int cost, ItemData item, int quantity)
    {
        return Player.BuyItem(cost, item, quantity);
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
        int salary = Calculators.RandomSalary(mission.Duration,mission.Difficulty,Settings.MinSalaryFactor,Settings.MaxSalaryFactor,Settings.HourlyWage,Settings.DifficultyBonus);
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
}
