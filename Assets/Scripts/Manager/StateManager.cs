

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

    // 购买物品，扣除金钱，添加物品到背包，返回操作结果
    public OperationResult BuyItem(int cost, ItemData item, int quantity)
    {
        return Player.BuyItem(cost, item, quantity);
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
        int salary = Calculators.RandomSalary(mission.Duration,mission.Difficulty,Settings.MinSalaryFactor,Settings.MaxSalaryFactor,Settings.HourlyWage,Settings.DifficultyBonus);
        return Player.EarnMoney(salary);
    }

}
