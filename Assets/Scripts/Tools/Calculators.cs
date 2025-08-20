using System;
using System.Collections.Generic;

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


    //�������һ��1-5�����֣����С�ڵ���p�򷵻�true�����򷵻�false
    public static bool RandomChance(int p)
    {
        return rng.Next(1, 6) <= p;
    }

    public static string RandomEvent(List<string> events) { 
        int index = rng.Next(events.Count);
        return events[index];
    }
}
