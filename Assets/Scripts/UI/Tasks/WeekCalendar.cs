using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeekCalendar : MonoBehaviour
{
    [SerializeField] private Transform weekGridParent;
    [SerializeField] private GameObject dayCellPrefab;
    private List<GameObject> dayCells = new List<GameObject>();

    private void OnEnable()
    {
        GenerateWeekGrid();
        AutoClickToday();
    }


    private void GenerateWeekGrid()
    {
        DateTime currentDate = DateTime.Now;
        GenerateWeekGrid(currentDate);
    }

    public void GenerateWeekGrid(DateTime targetDate)
    {
        int diff = (int)targetDate.DayOfWeek;
        DateTime startOfWeek = targetDate.AddDays(-diff);

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


    private void AutoClickToday()
    {
        DateTime today = DateTime.Now.Date;
        AutoClickSpecificDay(today);
    }

    public void AutoClickSpecificDay(DateTime specificDate)
    {
        foreach (var cellObject in dayCells)
        {
            DayCell dayCell = cellObject.GetComponent<DayCell>();
            if (dayCell != null && dayCell.Date.Date == specificDate.Date)
            {
                dayCell.OnWeekDayCellClicked();
                Debug.Log($"自动选择指定日期: {specificDate.ToString("yyyy-MM-dd")}");
                TaskManager.Instance.OnDaySelected(specificDate);
                break;
            }
        }


    }
}
