using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    // 定义事件，当任务列表需要更新时触发
    public event Action<List<MissionData>> OnTaskListUpdated;

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
        Debug.Log($"TaskManager 收到选中日期: {selectedDate.ToString("yyyy-MM-dd")}");

        // 从 TaskManagerModel 获取任务数据
        var dayMissionData = TaskManagerModel.Instance.GetMonth(selectedDate.ToString("yyyy-MM"))
            .GetDayMissionData(selectedDate.ToString("yyyy-MM-dd"));

        OnTaskListUpdated?.Invoke(dayMissionData.Tasks);
       
    }
}
