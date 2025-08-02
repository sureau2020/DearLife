//表示一个月的任务数据
using System.Collections.Generic;

public class MonthMissionData
{
    public string Month { get; private set; } // 表示是哪一个月的任务数据，2023-10这种格式
    private Dictionary<string, DayMissionData> dayMap = new();


    //string month 是2023-10这种格式
    public MonthMissionData(string month)
    {
       Month = month;
    }

    public DayMissionData GetDayMissionData(string day)
    {
        if (!dayMap.ContainsKey(day))
        {
            DayMissionData newDate = new DayMissionData(day);
            dayMap[day] = newDate;
        }
        return dayMap[day];
    }


}
