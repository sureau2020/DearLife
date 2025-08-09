using System;
using UnityEngine;

public class AddTaskButton : MonoBehaviour
{
    [SerializeField] private InputTaskPanel addTaskPanel;
    [SerializeField] private WeekCalendar weekCalendar;

    public void OnClick()
    {
        DateTime selectedDate = weekCalendar.GetSelectedDate();
        Debug.Log("Start");
        addTaskPanel.Show(selectedDate);
    }
}