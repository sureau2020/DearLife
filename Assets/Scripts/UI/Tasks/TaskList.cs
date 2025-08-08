using System.Collections.Generic;
using UnityEngine;

public class TaskList : MonoBehaviour
{
    [SerializeField] private GameObject taskPrefab;
    private List<GameObject> taskItems = new List<GameObject>();

    private void OnEnable()
    {
        // 订阅 TaskManager 的事件
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.OnTaskListUpdated += UpdateTaskList;
        }
    }

    private void OnDisable()
    {
        // 取消订阅，避免内存泄漏
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.OnTaskListUpdated -= UpdateTaskList;
        }
    }

    public void UpdateTaskList(List<MissionData> tasks)
    {
        ClearTaskList();
        GameSettings settings = GameManager.Instance.StateManager.Settings;

        foreach (var task in tasks)
        {
            GameObject taskItem = Instantiate(taskPrefab, transform);
            taskItems.Add(taskItem);

            var taskScript = taskItem.GetComponent<Task>();
            if (taskScript != null)
            {
                taskScript.Initialize(task, settings); 
            }
        }
    }

    private void ClearTaskList()
    {
        foreach (var taskItem in taskItems)
        {
            Destroy(taskItem);
        }
        taskItems.Clear();
    }
}
