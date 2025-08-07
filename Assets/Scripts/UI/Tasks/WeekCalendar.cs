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
    }

    private void GenerateWeekGrid()
    {
        // 从 TimeManager 获取当前时间
        DateTime currentDate = TimeManager.Instance != null ? DateTime.Now : DateTime.Now;

        // 获取本周的周一
        DateTime startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + 1);

        // 确保格子数量足够
        for (int i = 0; i < 7; i++)
        {
            if (i >= dayCells.Count)
            {
                GameObject dayCell = Instantiate(dayCellPrefab, weekGridParent);
                dayCells.Add(dayCell);
            }

            // 更新格子数据
            DateTime day = startOfWeek.AddDays(i);
            dayCells[i].GetComponent<DayCell>().Initialize(day);
        }
    }

   
}
