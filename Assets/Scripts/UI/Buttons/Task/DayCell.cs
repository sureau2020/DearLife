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

    // 点击事件，外部可注册
    public event Action<DateTime> OnCellClickedEvent;

    public void Initialize(DateTime date)
    {
        this.date = date;
        dateText.text = date.ToString("dd");
    }

    public void OnCellClicked()
    {
        Debug.Log($"点击了日期: {date:yyyy-MM-dd}");
        cellButton.Select();
        OnCellClickedEvent?.Invoke(date); // 触发事件
        TaskManager.Instance.OnDaySelected(date); 
    }
}
