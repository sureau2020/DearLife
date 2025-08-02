using System;
using System.Collections.Generic;

public class TaskManagerModel
{

    private static TaskManagerModel _instance;
    public static TaskManagerModel Instance
    {
        get
        {
            if (_instance == null)
                _instance = new TaskManagerModel();
            return _instance;
        }
    }


    //所有已加载的月份数据（键是 "2025-07"）
    private Dictionary<string, MonthMissionData> monthMap = new();


    // 加载月份
    public MonthMissionData GetMonth(string month)
    {
        if (monthMap.TryGetValue(month, out var monthData))
        {
            return monthData;
        }
        else
        {
            // 如果没有找到，创建一个新的月数据
            var newMonth = new MonthMissionData(month);
            monthMap[month] = newMonth;
            return newMonth;
        }
    }



    //推后任务的DDL
    public OperationResult pushDDL(MissionData mission, int delayTime)
    {
        if (mission == null)
        {
            return OperationResult.Fail("任务不存在。");
        }

        DateTime oldTime = mission.Deadline;
        bool isCross = mission.pushMission(delayTime); // 推迟任务，更新Deadline

        // 如果任务跨天或跨月
        if (isCross)
        {
            string oldDay = oldTime.ToString("yyyy-MM-dd");
            string oldMonth = oldTime.ToString("yyyy-MM");
            string newDay = mission.Deadline.ToString("yyyy-MM-dd");
            string newMonth = mission.Deadline.ToString("yyyy-MM");

            // 获取原月和原日
            MonthMissionData oldMonthData = GetMonth(oldMonth);
            DayMissionData oldDayData = oldMonthData.GetDayMissionData(oldDay);

            // 获取新月和新日（自动创建）
            MonthMissionData newMonthData = GetMonth(newMonth);
            DayMissionData newDayData = newMonthData.GetDayMissionData(newDay);

            // 从原日删除任务
            oldDayData.DeleteSpecificMission(mission);
            // 添加到新日
            newDayData.AddMission(mission);
        }

        return OperationResult.Complete();
        

    }

    private TaskManagerModel() { }
}
