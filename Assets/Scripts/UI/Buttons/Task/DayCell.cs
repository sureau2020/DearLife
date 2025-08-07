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
    /// �������ʱ����
    /// </summary>
    public void OnCellClicked()
    {
        Debug.Log($"���������: {date.ToString("yyyy-MM-dd")}");

        // �� MissionManager ��ȡ�������������
        //var dayMissionData = TaskManagerModel.Instance.GetDayMissionData(date);
        //if (dayMissionData != null)
        //{
        //    // TODO: ��ʾ��������
        //    Debug.Log($"��������: {dayMissionData.Tasks.Count}");
        //}
        //else
        //{
        //    Debug.Log("����û������");
        //}
    }
}
