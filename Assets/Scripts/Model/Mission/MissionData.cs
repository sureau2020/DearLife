

using System;


// 这个类用于存储任务数据，处理与任务底层model的交互
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


    // 随机生成薪资，目前写死了，之后应该允许用户在设置里调整
    public int RandomSalary(float duration, float difficulty)
    {
        double baseReward = duration * 15 + difficulty * 3;

        double randomFactor = 0.8 + rng.NextDouble() * 0.4; // 0.8 ~ 1.2
        double finalReward = baseReward * randomFactor;

        return (int)Math.Round(finalReward);
    }

}
