using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayCell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private Button cellButton;
    private DateTime date;

 
    public DateTime Date => date;

    public void Initialize(DateTime date)
    {
        this.date = date;
        dateText.text = date.ToString("dd");
    }

    public void OnCellClicked()
    {
        Debug.Log($"点击了日期: {date.ToString("yyyy-MM-dd")}");
        cellButton.Select();
        TaskManager.Instance.OnDaySelected(date); 
    }
}
