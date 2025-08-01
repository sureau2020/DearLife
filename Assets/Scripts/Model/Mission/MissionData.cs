using System;



// 这个类用于存储任务数据，处理与任务底层model的交互
public class MissionData 
{
    public string Title { get; private set; }
    public DateTime Deadline { get; private set; }
    public float Duration { get; private set; }
    public float Difficulty { get; private set; }
    public bool IsCompleted { get; private set; }


    // REQUIRE: 0 <= duration,dificulty <= 4; deadline > DateTime.Now; title != null
    public MissionData(string title, DateTime deadline, float duration, float difficulty)
    {
        Title = title;
        Deadline = deadline;
        Duration = duration;
        Difficulty = difficulty;
        IsCompleted = false;
    }


    // 完成任务，返回操作结果
    public OperationResult CompleteMission()
    {
        if (IsCompleted)
        {
            return OperationResult.Fail("任务已完成，无法重复完成。");
        }
        if (DateTime.Now > Deadline)
        {
            return OperationResult.Fail("任务已过期，无法完成。");
        }
        IsCompleted = true;
        return OperationResult.Complete();
    }



}
