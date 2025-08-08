using System;

public static class Calculators
{

    private static readonly Random rng = new();

    // 根据设置里的随机因子、时薪、难度奖励随机生成薪资
    public static int RandomSalary(float duration, float difficulty, float maxFactor, float hourlyWage, float difficultyBonus)
    {
        float baseReward = duration * hourlyWage + difficulty * difficultyBonus;

        float randomFactor = 1.0f + (float)rng.NextDouble() * (maxFactor - 1.0f);
        double finalReward = baseReward * randomFactor;

        return (int)Math.Round(finalReward);
    }

    // 根据设置里的时薪、难度奖励生成基础薪资（即最小薪资）
    public static int EstimatedMinSalary(float duration, float difficulty, float hourlyWage, float difficultyBonus)
    {
        float baseReward = duration * hourlyWage + difficulty * difficultyBonus;
        return (int)Math.Round(baseReward);
    }
}
