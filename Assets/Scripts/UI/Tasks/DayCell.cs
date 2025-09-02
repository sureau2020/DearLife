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
        cellButton.interactable = true;
        GetComponent<Image>().color = Color.white;
        dateText.text = date.ToString("dd");
    }

    public void OnWeekDayCellClicked()
    {
        SoundManager.Instance.PlaySfx("Type");
        cellButton.Select();
        TaskManager.Instance.OnDaySelected(date); 
    }

    public void OnMonthDayCellClicked() { 
        SoundManager.Instance.PlaySfx("HitWood");
        cellButton.Select();
        TaskManager.Instance.OnMonthDaySelected(date);
    }

    public void SetEmpty()
    {
        dateText.text = "";
        cellButton.interactable = false;
    }
}
