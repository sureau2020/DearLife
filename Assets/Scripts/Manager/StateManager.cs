

public class StateManager 
{
    public PlayerData Player { get; }
    public CharacterData Character { get; }
    public GameSettings Settings { get; }


    public StateManager(PlayerData player, CharacterData character, GameSettings settings)
    {
        Player = player;
        Character = character;
        Settings = settings;
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

}
