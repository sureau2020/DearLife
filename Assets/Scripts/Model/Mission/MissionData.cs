using System;



// 这个类用于存储任务数据，处理与任务底层model的交互
public class MissionData 
{
    public string Title { get; private set; }
    public DateTime Deadline { get; private set; }
    public float Duration { get; private set; }
    public float Difficulty { get; private set; }
    public bool IsCompleted { get; private set; }

    public bool HasDeadline { get; private set; }


    // REQUIRE: 0 <= duration,dificulty <= 4; 若有deadline > DateTime.Now;若没有deadline就传入DateTime.MinValue； title != null
    public MissionData(string title, DateTime deadline, float duration, float difficulty)
    {
        Title = title;
        Deadline = deadline;
        if (Deadline == DateTime.MinValue)
        {
            HasDeadline = false;
        }
        else { 
            HasDeadline = true;
        }
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
        if (HasDeadline && DateTime.Now > Deadline)
        {
            return OperationResult.Fail("任务已过期，无法完成。");
        }
        IsCompleted = true;
        return OperationResult.Complete();
    }

    // 推迟任务的DDL，如果跨天了也更新，但返回false提醒
    public bool pushMission(int delayMinutes)
    {
        DateTime newDeadline = Deadline.AddMinutes(delayMinutes);
        bool isCrossDay = newDeadline.Date != Deadline.Date;
        Deadline = newDeadline;
        return !isCrossDay;
    }


}
