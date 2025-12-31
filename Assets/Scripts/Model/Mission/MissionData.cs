using System;



// 这个类用于存储任务数据，处理与任务底层model的交互
public class MissionData 
{
    public string Title { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime BelongsToDate { get; set; } //任务归属日期
    public float Duration { get; set; }
    public float Difficulty { get; set; }
    public bool IsCompleted { get; set; }
    public bool HasDeadline { get; set; }

    public string SourceRecurringId { get; set; } // nullable，如果是从重复任务生成的任务，这里存储源任务的ID


    // REQUIRE: 0 <= duration,dificulty <= 4; 若有deadline > DateTime.Now;若没有deadline就传入DateTime.MinValue； title != null
    // 修改构造函数
    public MissionData(string title, DateTime deadline, float duration, float difficulty, DateTime belongsToDate)
    {
        Title = title;
        Deadline = deadline;
        if (Deadline == DateTime.MinValue)
        {
            HasDeadline = false;
            // 如果没有指定归属日期，默认为今天
            BelongsToDate = belongsToDate;
        }
        else { 
            HasDeadline = true;
            // 有截止日期的任务，归属日期就是截止日期
            BelongsToDate = deadline.Date;
        }
        Duration = duration;
        Difficulty = difficulty;
        IsCompleted = false;
    }


    // 用于从重复任务生成的任务
    public MissionData(string title, DateTime deadline, float duration, float difficulty, DateTime belongsToDate, string sourceRecurringId)
        : this(title, deadline, duration, difficulty, belongsToDate)
    {
        BelongsToDate = belongsToDate;
        IsCompleted = false;
        SourceRecurringId = sourceRecurringId;
    }

    public MissionData() { }

    // 完成任务，返回操作结果
    // 修改完成任务的保存逻辑
    public OperationResult CompleteMission()
    {
        if (IsCompleted)
        {
            return OperationResult.Fail("任务已完成，无法重复完成。");
        }
        if (HasDeadline && ((SourceRecurringId == null && DateTime.Now > Deadline) || (SourceRecurringId != null && IsPassedDeadline())))
        {
            Title = "[迟]" + Title;
            IsCompleted = true;
            _ = TaskManagerModel.Instance.SaveMonthAsync(BelongsToDate.ToString("yyyy-MM"));
            return OperationResult.Fail("任务已过期。");
        }
        IsCompleted = true;
        _ = TaskManagerModel.Instance.SaveMonthAsync(BelongsToDate.ToString("yyyy-MM"));
        return OperationResult.Complete();
    }


    //private void TransferToNormalMission()
    //{
    //    DayMissionData dayMissionData = TaskManagerModel.Instance.GetMonth(BelongsToDate.ToString("yyyy-MM"))
    //        .GetDayMissionData(BelongsToDate.ToString("yyyy-MM-dd"));
    //    dayMissionData.TransferRecurringMissionsToNormalMissions(this);
    //}

    private bool IsPassedDeadline()
    {
        return (DateTime.Now >= BelongsToDate && DateTime.Now.Hour > Deadline.Hour) || (DateTime.Now >= BelongsToDate && DateTime.Now.Hour == Deadline.Hour && DateTime.Now.Minute > Deadline.Minute);
    }

    // 推迟任务的DDL，如果跨天了也更新，但返回false提醒
    public bool pushMission(int delayMinutes)
    {
        if (!HasDeadline) return false; // 无 DDL 任务不能推迟
        
        DateTime newDeadline = Deadline.AddMinutes(delayMinutes);
        bool isCrossDay = newDeadline.Date != Deadline.Date;
        Deadline = newDeadline;
        BelongsToDate = newDeadline.Date; // 更新归属日期
        return !isCrossDay;
    }
}
