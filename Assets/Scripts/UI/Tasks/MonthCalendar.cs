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
        // ���õ�ǰ������ʾ
        yearMonth.text = targetDate.ToString("yyyy-MM");
        
        // ��ȡ���µĵ�һ������һ��
        DateTime firstDayOfMonth = new DateTime(targetDate.Year, targetDate.Month, 1);
        DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
        
        // �����һ�����ܼ���0=����, 1=��һ, ..., 6=������
        int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
        
        // ���֮ǰ�ĵ�Ԫ��
        foreach (Transform child in monthGridParent)
            Destroy(child.gameObject);
        monthDateCells.Clear();
        
        // �����ܹ���Ҫ�ĸ����������ո��� + �·�������
        int totalCells = firstDayOfWeek + lastDayOfMonth.Day;
        
        // �������и���
        for (int i = 0; i < totalCells; i++)
        {
            GameObject dateCell = Instantiate(monthDateCellPrefab, monthGridParent);
            monthDateCells.Add(dateCell);
            
            if (i < firstDayOfWeek)
            {
                // ǰ��Ŀո��ӣ�����ʾ�κ�����
                var dayCell = dateCell.GetComponent<DayCell>();
                if (dayCell != null)
                {
                    // ��������Ϊ���ɼ�����ʾΪ��
                    dateCell.SetActive(false);
                    // �������������ʾ�ո��ӵ����ɵ����
                    // dayCell.SetEmpty();
                }
            }
            else
            {
                // ʵ�ʵ����ڸ���
                int day = i - firstDayOfWeek + 1;
                DateTime date = new DateTime(targetDate.Year, targetDate.Month, day);
                dateCell.GetComponent<DayCell>().Initialize(date);
            }
        }
    }



}
