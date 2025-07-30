//表示一个月的任务数据
using System.Collections.Generic;

public class MonthMissionData
{
    public string Month { get; private set; } // 表示是哪一个月的任务数据，2023-10这种格式
    private Dictionary<string, DayMissionData> dayMap = new();
}
