

using System;

public static class Calculators
{

    private static readonly Random rng = new();

    // �����������������ӡ�ʱн���ѶȽ����������н��
    public static int RandomSalary(float duration, float difficulty, float minFactor, float maxFactor, float hourlyWage, float difficultyBonus)
    {
        float baseReward = duration * hourlyWage + difficulty * difficultyBonus;

        float randomFactor = minFactor + (float)rng.NextDouble() * (maxFactor - minFactor);
        double finalReward = baseReward * randomFactor;

        return (int)Math.Round(finalReward);
    }
}
