using System;
using System.Collections;
using System.Collections.Generic;

public class RecurringMissionData 
{
    public string Id { get; set; }
    public string Title { get; set; }
    public bool HasDeadline { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime StartDate { get; set; } //任务起始日期
    public float Duration { get; set; }
    public float Difficulty { get; set; }

    public List<DayOfWeek> recurrenceDays;

    // REQUIRE：recurrenceDays 实际上不得为空
    // REQUIRE: 0 <= duration,dificulty <= 4; 若有deadline > DateTime.Now;若没有deadline就传入DateTime.MinValue； title != null
    // 修改构造函数,deadline的年月日作为起始时间，时和分作为默认ddl时间
    public RecurringMissionData(string title, DateTime deadline, float duration, float difficulty, List<DayOfWeek> recurrenceDays)
    {
        Id = Guid.NewGuid().ToString();
        Title = title;
        Deadline = deadline;
        this.recurrenceDays = recurrenceDays ?? new List<DayOfWeek>();
        if (Deadline == DateTime.MinValue)
        {
            HasDeadline = false;
            Deadline = DateTime.MinValue;
            // 如果没有指定归属日期，默认为今天
            StartDate = DateTime.Now.Date;
        }
        else
        {
            HasDeadline = true;
            Deadline = deadline;
            // 有截止日期的任务，归属日期就是截止日期
            StartDate = deadline.Date;
        }
        Duration = duration;
        Difficulty = difficulty;
    }

    // 完成任务，返回操作结果
    // 修改完成任务的保存逻辑,date= 2025-07-04这样注意查这个
    //public OperationResult CompleteMission(string date)
    //{
    //    if (HasDeadline && IsPassedDeadline())
    //    {
    //        Title = "[迟]" + Title;
    //        TransferToNormalMission(date);
    //        _ = TaskManagerModel.Instance.SaveMonthAsync(DateTime.Parse(date).ToString("yyyy-MM"));
    //        return OperationResult.Fail("任务已过期。");
    //    }
    //    TransferToNormalMission(date);
    //    _ = TaskManagerModel.Instance.SaveMonthAsync(DateTime.Parse(date).ToString("yyyy-MM"));
    //    return OperationResult.Complete();
    //}

    //private bool IsPassedDeadline()
    //{
    //    return (DateTime.Now.Hour > Deadline.Hour) || (DateTime.Now.Hour == Deadline.Hour && DateTime.Now.Minute > Deadline.Minute);
    //}



    //private void TransferToNormalMission(string date)
    //{
    //    MissionData mission = new MissionData(Title, Deadline, Duration, Difficulty, DateTime.Parse(date), Id);
    //    DayMissionData dayMissionData = TaskManagerModel.Instance.GetMonth(DateTime.Parse(date).ToString("yyyy-MM"))
    //        .GetDayMissionData(date);
    //    dayMissionData.AddMission(mission);
    //}


    // 判断任务是否在指定日期发生
    public bool IsOccurringOnDate(DateTime date)
    {
        if (date.Date < StartDate.Date)
        {
            return false;
        }
        return recurrenceDays.Contains(date.DayOfWeek);
    }


}




public enum Days
{
    Sunday,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday
}