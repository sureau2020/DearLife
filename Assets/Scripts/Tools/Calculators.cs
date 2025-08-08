using System;

public static class Calculators
{

    private static readonly Random rng = new();

    // �����������������ӡ�ʱн���ѶȽ����������н��
    public static int RandomSalary(float duration, float difficulty, float maxFactor, float hourlyWage, float difficultyBonus)
    {
        float baseReward = duration * hourlyWage + difficulty * difficultyBonus;

        float randomFactor = 1.0f + (float)rng.NextDouble() * (maxFactor - 1.0f);
        double finalReward = baseReward * randomFactor;

        return (int)Math.Round(finalReward);
    }

    // �����������ʱн���ѶȽ������ɻ���н�ʣ�����Сн�ʣ�
    public static int EstimatedMinSalary(float duration, float difficulty, float hourlyWage, float difficultyBonus)
    {
        float baseReward = duration * hourlyWage + difficulty * difficultyBonus;
        return (int)Math.Round(baseReward);
    }
}
