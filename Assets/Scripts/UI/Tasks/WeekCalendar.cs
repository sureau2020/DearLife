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

    // 维护当前被选中的索引，-1 表示无选中
    private int selectedIndex = -1;

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
        // 重置选中状态（新的一周重新选择）
        selectedIndex = -1;

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
            var dc = dayCells[i].GetComponent<DayCell>();
            dc.Initialize(day, i);

            // 清除老订阅，订阅新的点击处理
            dc.OnCellClicked = null;
            dc.OnCellClicked += OnDayCellClicked;
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
                // 触发 DayCell 自身的点击逻辑（会通知 TaskManager）
                dayCell.OnWeekDayCellClicked();

                // 更新选中视觉（通过 OnDayCellClicked，不会重复通知 TaskManager）
                OnDayCellClicked(dayCell.index);

                break;
            }
        }
    }

    // 仅在选中变化时更新：先比较索引，相同则不操作；不同则只切换旧的和新的两个 cell
    private void OnDayCellClicked(int index)
    {
        if (index == selectedIndex)
            return;

        // 取消之前的选中
        if (selectedIndex >= 0 && selectedIndex < dayCells.Count)
        {
            var prev = dayCells[selectedIndex].GetComponent<DayCell>();
            if (prev != null)
                prev.SetSelected(false);
        }

        // 设置新的选中
        if (index >= 0 && index < dayCells.Count)
        {
            var current = dayCells[index].GetComponent<DayCell>();
            if (current != null)
                current.SetSelected(true);
            selectedIndex = index;
        }
        else
        {
            selectedIndex = -1;
        }
    }
}
