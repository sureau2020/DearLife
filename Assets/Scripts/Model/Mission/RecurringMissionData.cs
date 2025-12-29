using System;
using System.Collections;
using System.Collections.Generic;

public class RecurringMissionData 
{
    public string Title { get; set; }
    public bool HasDeadline { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime StartDate { get; set; } //任务起始日期
    public float Duration { get; set; }
    public float Difficulty { get; set; }

    public List<Days> recurrenceDays; 

    //f表示延迟完成，t表示按时完成
    public Dictionary<string, bool> completedMap;

    // REQUIRE: 0 <= duration,dificulty <= 4; 若有deadline > DateTime.Now;若没有deadline就传入DateTime.MinValue； title != null
    // 修改构造函数,deadline的年月日作为起始时间，时和分作为默认ddl时间
    public RecurringMissionData(string title, DateTime deadline, float duration, float difficulty, List<Days> recurrenceDays)
    {
        Title = title;
        Deadline = deadline;
        if (Deadline == DateTime.MinValue)
        {
            HasDeadline = false;
            // 如果没有指定归属日期，默认为今天
            StartDate = DateTime.Now.Date;
        }
        else
        {
            HasDeadline = true;
            // 有截止日期的任务，归属日期就是截止日期
            StartDate = deadline.Date;
        }
        Duration = duration;
        Difficulty = difficulty;
        completedMap = new Dictionary<string, bool>();
    }

    // 完成任务，返回操作结果
    // 修改完成任务的保存逻辑,date= 2025-07-04这样
    public OperationResult CompleteMission(string date)
    {
        if (HasDeadline && DateTime.Now > Deadline)
        {
            Title = "[迟]" + Title;
            MarkCompleted(date, false);
            //TODO save
            //_ = TaskManagerModel.Instance.SaveMonthAsync(BelongsToDate.ToString("yyyy-MM"));
            return OperationResult.Fail("任务已过期。");
        }
        MarkCompleted(date, true);
        //TODO save
        //_ = TaskManagerModel.Instance.SaveMonthAsync(BelongsToDate.ToString("yyyy-MM"));
        return OperationResult.Complete();
    }


    // 从今以后永久删除该循环任务
    public OperationResult DeleteForever()
    {
        
        return OperationResult.Complete();
    }



    public void MarkCompleted(string date, bool onTime)
    {
        if (completedMap.ContainsKey(date))
        {
            completedMap[date] = onTime;
        }
        else
        {
            completedMap.Add(date, onTime);
        }
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