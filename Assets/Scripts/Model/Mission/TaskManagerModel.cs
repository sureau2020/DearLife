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

    private List<RecurringMissionData> recurringMissionDatas = new();

    public void DeleteRecurringMissionData(RecurringMissionData mission)
    {
        recurringMissionDatas.Remove(mission);
        _ = SaveRecurringMissionDatas();
    }

    public void AddRecurringMissionData(RecurringMissionData mission)
    {
        recurringMissionDatas.Add(mission);
        _ = SaveRecurringMissionDatas();
    }

    public List<RecurringMissionData> GetRecurringMissionDatas()
    {

        if (recurringMissionDatas.Count != 0) {
            return recurringMissionDatas;
        }else{
            var loadResult = SaveManager.LoadRecurringMissions();
            if (loadResult.Success)
            {
                recurringMissionDatas = loadResult.Data;
            }
            else
            {
                recurringMissionDatas = new List<RecurringMissionData>();
            }
            return recurringMissionDatas;
        }
    }


    // 加载月份
    public MonthMissionData GetMonth(string month)
    {
        if (monthMap.TryGetValue(month, out var monthData))
        {
            return monthData;
        }
        else
        {
            // 尝试从文件加载
            var loadResult = SaveManager.LoadMonthTasks(month);
            MonthMissionData newMonth;
            
            if (loadResult.Success)
            {
                newMonth = loadResult.Data;
            }
            else
            {
                // 加载失败，创建新的月数据
                newMonth = new MonthMissionData(month);
            }
            
            monthMap[month] = newMonth;
            return newMonth;
        }
    }


    public async System.Threading.Tasks.Task<OperationResult> SaveRecurringMissionDatas()
    {
        try
        {
            var result = await SaveManager.SaveRecurringMissionsAsync(recurringMissionDatas);
            return result ?? OperationResult.Fail("保存返回了空结果。");
        }
        catch (Exception ex)
        {
            return OperationResult.Fail($"保存例行任务失败: {ex.Message}");
        }
    }

    // 保存单个月份
    public OperationResult SaveMonth(string month)
    {
        if (monthMap.TryGetValue(month, out var monthData))
        {
            return SaveManager.SaveMonthTasks(monthData);
        }
        return OperationResult.Fail($"月份 {month} 未加载到内存中");
    }

    // 异步保存单个月份
    public async System.Threading.Tasks.Task<OperationResult> SaveMonthAsync(string month)
    {
        if (monthMap.TryGetValue(month, out var monthData))
        {
            return await SaveManager.SaveMonthTasksAsync(monthData);
        }
        return OperationResult.Fail($"月份 {month} 未加载到内存中");
    }

    // 保存所有已加载的月份
    public OperationResult SaveAllMonths()
    {
        return SaveManager.SaveAllMonthTasks(monthMap);
    }

    // 异步保存所有已加载的月份
    public async System.Threading.Tasks.Task<OperationResult> SaveAllMonthsAsync()
    {
        return await SaveManager.SaveAllMonthTasksAsync(monthMap);
    }

    // 获取所有已加载的月份（用于保存时遍历）
    public Dictionary<string, MonthMissionData> GetAllLoadedMonths()
    {
        return new Dictionary<string, MonthMissionData>(monthMap);
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

            // 保存涉及到的月份
            _ = SaveMonthAsync(oldMonth);
            if (oldMonth != newMonth)
            {
                _ = SaveMonthAsync(newMonth);
            }
        }

        return OperationResult.Complete();
    }

    private TaskManagerModel() { }
}
