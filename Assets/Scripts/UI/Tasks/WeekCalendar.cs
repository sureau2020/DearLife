using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeekCalendar : MonoBehaviour
{
    [SerializeField] private Transform weekGridParent; 
    [SerializeField] private GameObject dayCellPrefab;
    //private DateTime selectedDate = DateTime.Now;
    private List<GameObject> dayCells = new List<GameObject>();

    private void OnEnable()
    {
        GenerateWeekGrid();
        AutoClickToday();
    }


    private void GenerateWeekGrid()
    {
        // 无参数版本，使用当前时间
        DateTime currentDate = TimeManager.Instance != null ? DateTime.Now : DateTime.Now;
        GenerateWeekGrid(currentDate);
    }

    private void GenerateWeekGrid(DateTime targetDate)
    {
        // 根据传入的日期计算该周的周一
        DateTime startOfWeek = targetDate.AddDays(-(int)targetDate.DayOfWeek + 1);

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
        foreach (var cellObject in dayCells)
        {
            DayCell dayCell = cellObject.GetComponent<DayCell>();
            if (dayCell != null && dayCell.Date.Date == today)
            {
                dayCell.OnWeekDayCellClicked();
                Debug.Log($"自动选择今天的日期: {today.ToString("yyyy-MM-dd")}");
                TaskManager.Instance.OnDaySelected(today);
                break;
            }
        }
    }

    
}
