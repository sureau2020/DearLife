using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [SerializeField] private MonthCalendar monthCalendar;
    [SerializeField] private WeekCalendar weekCalendar;

    // 定义事件，当任务列表需要更新时触发
    public event Action<List<MissionData>> OnTaskListUpdated;

    private DateTime selectedDate = DateTime.Now;
    private List<MissionData> currentTasks = new List<MissionData>();

    public DateTime SelectedDate => selectedDate;
    public List<MissionData> CurrentTasks => currentTasks;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OnDaySelected(DateTime selectedDate)
    {
        this.selectedDate = selectedDate;
        Debug.Log($"TaskManager 收到选中日期: {selectedDate:yyyy-MM-dd}");

        // 从 TaskManagerModel 获取任务数据
        var dayMissionData = TaskManagerModel.Instance.GetMonth(selectedDate.ToString("yyyy-MM"))
            .GetDayMissionData(selectedDate.ToString("yyyy-MM-dd"));

        currentTasks = new List<MissionData>(dayMissionData.Tasks);


        HashSet<string> recurringTasksId = GetArchievedTasksIDMap(currentTasks);

        Debug.Log($"已归档的循环任务ID数量: {recurringTasksId.Count}" );
        foreach (var id in recurringTasksId)
        {
            Debug.Log($"归档的循环任务ID: {id}");
        }

        List<MissionData> generatedRecurringTasks = GenerateRecurringTasksOnSpecificDate(selectedDate, recurringTasksId);
        
        foreach (var task in currentTasks)
        {
            Debug.Log($"先生成的普通任务: {task.Title} ");
        }
        
        currentTasks.AddRange(generatedRecurringTasks);
        
        foreach (var task in currentTasks)
        {
            Debug.Log($"加上生成的循环任务: {task.Title} ");
        }
        
        SortTasksByDeadlineTime(currentTasks);
        
        OnTaskListUpdated?.Invoke(currentTasks);
    }


    //TODO 生成当天的循环任务列表、合并列表、显示UI、删除、完成任务（转为普通任务）

    public HashSet<string> GetArchievedTasksIDMap(List<MissionData> currentTasks)
    {
        return currentTasks.Where(t => t.SourceRecurringId != null).Select(t => t.SourceRecurringId).ToHashSet();
    }


    public List<MissionData> GenerateRecurringTasksOnSpecificDate(DateTime date, HashSet<string> archievedId)
    {
        List<RecurringMissionData> recurringMissions = TaskManagerModel.Instance.GetRecurringMissionDatas();
        List<MissionData> generatedMissions = new List<MissionData>();
        if (recurringMissions.Count == 0)
        {
            return generatedMissions;
        }
        else
        {
            foreach (RecurringMissionData recurringMission in recurringMissions)
            {
                if (!recurringMission.IsOccurringOnDate(date)) continue;
                if (archievedId.Contains(recurringMission.Id)) continue;
                Debug.Log($"生成循环任务: {recurringMission.Title} 于 {date:yyyy-MM-dd}");
                // 生成新的 MissionData 实例
                MissionData newMission = new MissionData(
                            recurringMission.Title,
                            recurringMission.HasDeadline ? recurringMission.Deadline : DateTime.MinValue,
                            recurringMission.Duration,
                            recurringMission.Difficulty,
                            date,
                            recurringMission.Id // 设置来源循环任务ID
                        );
                generatedMissions.Add(newMission);
            }
        }
        return generatedMissions;
    }

    // 辅助：按截止时间（小时+分钟）排序，有 deadline 的在前，无 deadline 的放最后
    private void SortTasksByDeadlineTime(List<MissionData> tasks)
    {
        if (tasks == null || tasks.Count <= 1) return;

        tasks.Sort((a, b) =>
        {
            bool aHas = a.HasDeadline;
            bool bHas = b.HasDeadline;

            if (aHas && bHas)
            {
                // 只比较 time-of-day（忽略日期）
                var at = a.Deadline.TimeOfDay;
                var bt = b.Deadline.TimeOfDay;
                return at.CompareTo(bt);
            }
            if (aHas && !bHas) return -1; // a 在前
            if (!aHas && bHas) return 1;  // b 在前
            return 0; // 都无 deadline，保持相对顺序
        });
    }


    public void OnMonthDaySelected(DateTime selectedDate) { 
        DeactiveMonthCalendar();
        weekCalendar.GenerateWeekGrid(selectedDate);
        weekCalendar.AutoClickSpecificDay(selectedDate);
    }

    public MissionData GetNextMission()
    {
       DayMissionData day = TaskManagerModel.Instance.GetMonth(DateTime.Now.ToString("yyyy-MM")).GetDayMissionData(DateTime.Now.ToString("yyyy-MM-dd"));
       return day.GetFirstUncompletedMission();
    }


    public void ActiveMonthCalendar()
    {
        monthCalendar.gameObject.SetActive(true);
    }

    public void DeactiveMonthCalendar()
    {
        monthCalendar.CloseCalendar();

    }
}
