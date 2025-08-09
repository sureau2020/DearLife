using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    // �����¼����������б���Ҫ����ʱ����
    public event Action<List<MissionData>> OnTaskListUpdated;
    private List<MissionData> lastTaskList;

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
        Debug.Log($"TaskManager �յ�ѡ������: {selectedDate.ToString("yyyy-MM-dd")}");

        // �� TaskManagerModel ��ȡ��������
        var dayMissionData = TaskManagerModel.Instance.GetMonth(selectedDate.ToString("yyyy-MM"))
            .GetDayMissionData(selectedDate.ToString("yyyy-MM-dd"));
        lastTaskList = dayMissionData.Tasks;
        OnTaskListUpdated?.Invoke(dayMissionData.Tasks);
       
    }

    public void RegisterTaskListListener(Action<List<MissionData>> listener)
    {
        OnTaskListUpdated += listener;
        if (lastTaskList != null)
        {
            listener(lastTaskList);
        }
    }
}
