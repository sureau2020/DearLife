

public class GameSettings 
{
    public float MaxSalaryFactor { get; private set; } = 1.2f; //任务薪水的最大随机因子

    public int HourlyWage { get; private set; } = 15;//小时工资

    public int DifficultyBonus { get; private set; } = 3; //任务难度每1，薪水增加多少

    public int CountdownWagePerHour { get; private set; } = 3; //倒计时模式的每小时低保
    //TODO：还没检测这个wage是否valid，代价和任务数和下面都没测

    public int DelayCost { get; private set; } = 5; //倒计时加时所花钱数

    public int DelayMissionNum { get; private set; } = 3; //倒计时模式加时任务数

    //0-5的数字，数字越大回复概率越高，0不回复，5肯定回复
    public int ReplyChance { get; private set; } = 5;


    public OperationResult Validate()
    {

        if (MaxSalaryFactor <= 1)
        {
            return OperationResult.Fail("薪水随机因子上限 不得 小于0");
        }

        if(HourlyWage <= 0)
        {
            return OperationResult.Fail("时薪必须大于0，不要贴钱上班哇");
        }

        if (DifficultyBonus < 0)
        {
            return OperationResult.Fail("任务难度奖励不能小于0，不要贴钱干难活哇");
        }

        return OperationResult.Complete();
    }





}
