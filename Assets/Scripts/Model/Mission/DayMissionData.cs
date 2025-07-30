// 表示一天的任务数据
using System.Collections.Generic;

public class DayMissionData 
{
    public string Day { get; private set; } // 表示是哪一天的任务数据，2023-10-10这种
    public List<MissionData> Tasks { get; private set; } // 任务列表
}
