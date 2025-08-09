using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

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
}
