using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeekCalendar : MonoBehaviour
{
    [SerializeField] private Transform weekGridParent; 
    [SerializeField] private GameObject dayCellPrefab;
    private DateTime selectedDate = DateTime.Now;
    private List<GameObject> dayCells = new List<GameObject>();

    private void OnEnable()
    {
        GenerateWeekGrid();
        RegisterCellEvents();
        AutoClickToday();
    }

    private void OnDisable()
    {
        UnregisterCellEvents();
    }

    private void GenerateWeekGrid()
    {
        DateTime currentDate = TimeManager.Instance != null ? DateTime.Now : DateTime.Now;
        DateTime startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + 1);

        for (int i = 0; i < 7; i++)
        {
            if (i >= dayCells.Count)
            {
                GameObject dayCell = Instantiate(dayCellPrefab, weekGridParent);
                dayCells.Add(dayCell);
            }
            DateTime day = startOfWeek.AddDays(i);
            dayCells[i].GetComponent<DayCell>().Initialize(day);
        }
    }

    private void RegisterCellEvents()
    {
        foreach (var cellObj in dayCells)
        {
            var cell = cellObj.GetComponent<DayCell>();
            cell.OnCellClickedEvent -= OnDayCellClicked; // 防止重复注册
            cell.OnCellClickedEvent += OnDayCellClicked;
        }
    }

    private void UnregisterCellEvents()
    {
        foreach (var cellObj in dayCells)
        {
            var cell = cellObj.GetComponent<DayCell>();
            cell.OnCellClickedEvent -= OnDayCellClicked;
        }
    }

    private void OnDayCellClicked(DateTime date)
    {
        selectedDate = date;
    }

    public DateTime GetSelectedDate() => selectedDate;

    private void AutoClickToday()
    {
        DateTime today = DateTime.Now.Date;
        foreach (var cellObject in dayCells)
        {
            DayCell dayCell = cellObject.GetComponent<DayCell>();
            if (dayCell != null && dayCell.Date.Date == today)
            {
                dayCell.OnCellClicked();
                Debug.Log($"自动选择今天的日期: {today.ToString("yyyy-MM-dd")}");
                TaskManager.Instance.OnDaySelected(today);
                break;
            }
        }
    }

    
}
