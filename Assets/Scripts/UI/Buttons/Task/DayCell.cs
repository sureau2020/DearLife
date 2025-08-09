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

    // ����¼����ⲿ��ע��
    public event Action<DateTime> OnCellClickedEvent;

    public void Initialize(DateTime date)
    {
        this.date = date;
        dateText.text = date.ToString("dd");
    }

    public void OnCellClicked()
    {
        Debug.Log($"���������: {date:yyyy-MM-dd}");
        cellButton.Select();
        OnCellClickedEvent?.Invoke(date); // �����¼�
        TaskManager.Instance.OnDaySelected(date); 
    }
}
