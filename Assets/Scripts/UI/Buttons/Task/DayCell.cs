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

    public void OnWeekDayCellClicked()
    {
        cellButton.Select();
        TaskManager.Instance.OnDaySelected(date); 
    }

    public void OnMonthDayCellClicked() { 
        cellButton.Select();
        TaskManager.Instance.OnDaySelected(date);
    }
}
