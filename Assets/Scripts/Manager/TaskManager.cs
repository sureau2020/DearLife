using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [SerializeField] private MonthCalendar monthCalendar;
    [SerializeField] private WeekCalendar weekCalendar;

    // �����¼����������б���Ҫ����ʱ����
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
        Debug.Log($"TaskManager �յ�ѡ������: {selectedDate:yyyy-MM-dd}");

        // �� TaskManagerModel ��ȡ��������
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


    public void ActiveMonthCalendar()
    {
        monthCalendar.gameObject.SetActive(true);
    }

    public void DeactiveMonthCalendar()
    {
        monthCalendar.gameObject.SetActive(false);

    }
}
