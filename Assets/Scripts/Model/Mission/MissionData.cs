

using System;


// ��������ڴ洢�������ݣ�����������ײ�model�Ľ���
public class MissionData 
{
    private static readonly Random rng = new();
    public string Title { get; private set; }
    public DateTime Deadline { get; private set; }
    public int Salary { get; private set; }
    public bool IsCompleted { get; private set; }


    // REQUIRE: 0 <= duration,dificulty <= 4,deadline > DateTime.Now,title != null
    public MissionData(string title, DateTime deadline, float duration, float difficulty)
    {
        Title = title;
        Deadline = deadline;
        Salary = RandomSalary(duration, difficulty);
        IsCompleted = false;
    }


    // �������н�ʣ�Ŀǰд���ˣ�֮��Ӧ�������û������������
    public int RandomSalary(float duration, float difficulty)
    {
        double baseReward = duration * 15 + difficulty * 3;

        double randomFactor = 0.8 + rng.NextDouble() * 0.4; // 0.8 ~ 1.2
        double finalReward = baseReward * randomFactor;

        return (int)Math.Round(finalReward);
    }

}
