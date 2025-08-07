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
        // �� TimeManager ��ȡ��ǰʱ��
        DateTime currentDate = TimeManager.Instance != null ? DateTime.Now : DateTime.Now;

        // ��ȡ���ܵ���һ
        DateTime startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + 1);

        // ȷ�����������㹻
        for (int i = 0; i < 7; i++)
        {
            if (i >= dayCells.Count)
            {
                GameObject dayCell = Instantiate(dayCellPrefab, weekGridParent);
                dayCells.Add(dayCell);
            }

            // ���¸�������
            DateTime day = startOfWeek.AddDays(i);
            dayCells[i].GetComponent<DayCell>().Initialize(day);
        }
    }

   
}
