

public class GameSettings 
{
    public float MaxSalaryFactor { get; private set; } = 1.2f; //任务薪水的最大随机因子
    public float MinSalaryFactor { get; private set; } = 0.8f; //任务薪水的最小随机因子

    public int HourlyWage { get; private set; } = 15;//小时工资

    public int DifficultyBonus { get; private set; } = 3; //任务难度每1，薪水增加多少

    public int CountdownWagePerHour { get; private set; } = 3; //倒计时模式的每小时低保

    public OperationResult Validate()
    {
        if (MinSalaryFactor < 0)
        {
            return OperationResult.Fail("薪水随机因子下限不得小于0");
        }

        if (MaxSalaryFactor < MinSalaryFactor)
        {
            return OperationResult.Fail("薪水随机因子下限 不能大于 薪水随机因子上限");
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
