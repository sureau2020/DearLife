using System;
using System.Collections.Generic;
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

        currentTasks = dayMissionData.Tasks ?? new List<MissionData>();
        OnTaskListUpdated?.Invoke(currentTasks);
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
