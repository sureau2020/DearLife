using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonthCalendar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI yearMonth;
    [SerializeField] private Transform monthGridParent;
    [SerializeField] private GameObject monthDateCellPrefab;
    [SerializeField] private GameObject monthCalBackground;
    private List<GameObject> monthDateCells = new List<GameObject>();

    private const int MaxCells = 42; // 42格对象池

    private DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

    void OnEnable()
    {
        monthCalBackground.SetActive(true);
        GenerateMonthGrid(TaskManager.Instance.SelectedDate);
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

    public void CloseCalendar()
    {
        monthCalBackground.SetActive(false);
        gameObject.SetActive(false);
    }


    private void GenerateMonthGrid(DateTime targetDate)
    {
        currentMonth = new DateTime(targetDate.Year, targetDate.Month, 1);

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
        Debug.Log($"切换到上个月: {currentMonth.ToString("yyyy-MM")}");
        GenerateMonthGrid(currentMonth);
    }

    public void OnNextMonth()
    {
        currentMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1).AddMonths(1);
        GenerateMonthGrid(currentMonth);
    }
}
