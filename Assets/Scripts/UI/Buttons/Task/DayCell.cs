using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayCell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dateText; 
    private DateTime date; 


    public void Initialize(DateTime date)
    {
        this.date = date;
        dateText.text = date.ToString("dd");
    }

    /// <summary>
    /// 点击格子时触发
    /// </summary>
    public void OnCellClicked()
    {
        Debug.Log($"点击了日期: {date.ToString("yyyy-MM-dd")}");

        // 从 MissionManager 获取当天的任务数据
        //var dayMissionData = TaskManagerModel.Instance.GetDayMissionData(date);
        //if (dayMissionData != null)
        //{
        //    // TODO: 显示任务数据
        //    Debug.Log($"任务数量: {dayMissionData.Tasks.Count}");
        //}
        //else
        //{
        //    Debug.Log("当天没有任务");
        //}
    }
}
