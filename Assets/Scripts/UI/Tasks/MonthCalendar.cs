using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonthCalendar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI yearMonth;
    [SerializeField] private Transform monthGridParent;
    [SerializeField] private GameObject monthDateCellPrefab;
    private List<GameObject> monthDateCells = new List<GameObject>();


    void OnEnable()
    {
        GenerateMonthGrid();
        gameObject.SetActive(true);
    }

    void OnDisable()
    {
        gameObject.SetActive(false);
    }

    private void GenerateMonthGrid()
    {
        DateTime currentDate = TimeManager.Instance != null ? TaskManager.Instance.SelectedDate : DateTime.Now;
        GenerateMonthGrid(currentDate);
    }

    private void GenerateMonthGrid(DateTime targetDate)
    {
        // 设置当前年月显示
        yearMonth.text = targetDate.ToString("yyyy-MM");
        
        // 获取当月的第一天和最后一天
        DateTime firstDayOfMonth = new DateTime(targetDate.Year, targetDate.Month, 1);
        DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
        
        // 计算第一天是周几（0=周日, 1=周一, ..., 6=周六）
        int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
        
        // 清空之前的单元格
        foreach (Transform child in monthGridParent)
            Destroy(child.gameObject);
        monthDateCells.Clear();
        
        // 计算总共需要的格子数量（空格子 + 月份天数）
        int totalCells = firstDayOfWeek + lastDayOfMonth.Day;
        
        // 生成所有格子
        for (int i = 0; i < totalCells; i++)
        {
            GameObject dateCell = Instantiate(monthDateCellPrefab, monthGridParent);
            monthDateCells.Add(dateCell);
            
            if (i < firstDayOfWeek)
            {
                // 前面的空格子，不显示任何内容
                var dayCell = dateCell.GetComponent<DayCell>();
                if (dayCell != null)
                {
                    // 可以设置为不可见或显示为空
                    dateCell.SetActive(false);
                    // 或者如果你想显示空格子但不可点击：
                    // dayCell.SetEmpty();
                }
            }
            else
            {
                // 实际的日期格子
                int day = i - firstDayOfWeek + 1;
                DateTime date = new DateTime(targetDate.Year, targetDate.Month, day);
                dateCell.GetComponent<DayCell>().Initialize(date);
            }
        }
    }



}
