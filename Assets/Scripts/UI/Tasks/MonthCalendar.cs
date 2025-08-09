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

    private const int MaxCells = 42; // 6周最多42格

    private DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

    void OnEnable()
    {
        GenerateMonthGrid();
    }

    void Awake()
    {
        // 只生成一次
        for (int i = 0; i < MaxCells; i++)
        {
            GameObject dateCell = Instantiate(monthDateCellPrefab, monthGridParent);
            monthDateCells.Add(dateCell);
        }
    }

    private void GenerateMonthGrid()
    {
        DateTime currentDate = DateTime.Now;
        GenerateMonthGrid(currentDate);
    }

    private void GenerateMonthGrid(DateTime targetDate)
    {
        yearMonth.text = targetDate.ToString("yyyy-MM");
        DateTime firstDayOfMonth = new DateTime(targetDate.Year, targetDate.Month, 1);
        DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
        int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
        int daysInMonth = lastDayOfMonth.Day;

        for (int i = 0; i < MaxCells; i++)
        {
            GameObject cell = monthDateCells[i];
            var dayCell = cell.GetComponent<DayCell>();
            if (i < firstDayOfWeek || i >= firstDayOfWeek + daysInMonth)
            {
                dayCell.SetEmpty();
            }
            else
            {
                // 有效日期格子
                int day = i - firstDayOfWeek + 1;
                DateTime date = new DateTime(targetDate.Year, targetDate.Month, day);
                dayCell.Initialize(date);
            }
        }
    }

    public void OnPrevMonth()
    {
        currentMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1).AddMonths(-1);
        GenerateMonthGrid(currentMonth);
    }

    public void OnNextMonth()
    {
        currentMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1).AddMonths(1);
        GenerateMonthGrid(currentMonth);
    }
}
