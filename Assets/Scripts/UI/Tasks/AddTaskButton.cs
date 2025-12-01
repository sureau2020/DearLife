using System;
using UnityEngine;

public class AddTaskButton : MonoBehaviour
{
    [SerializeField] private InputTaskPanel addTaskPanel;

    public void OnClick()
    {
        DateTime selectedDate = TaskManager.Instance.SelectedDate;
        
        addTaskPanel.Show(selectedDate);
    }
}