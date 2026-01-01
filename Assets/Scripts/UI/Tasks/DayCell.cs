using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayCell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private Button cellButton;
    [SerializeField] private Image selectedCircle;
    private DateTime date;

    public int index;

    public DateTime Date => date;

    public Action<int> OnCellClicked;

    public void Initialize(DateTime date, int index)
    {
        this.index = index;
        this.date = date;
        cellButton.interactable = true;
        GetComponent<Image>().color = Color.white;
        dateText.text = date.Day.ToString();

        // 确保初始时隐藏选中圈
        if (selectedCircle != null && selectedCircle.gameObject.activeSelf)
            selectedCircle.gameObject.SetActive(false);

    }

    public void OnWeekDayCellClicked()
    {
        OnCellClicked?.Invoke(this.index);
        SoundManager.Instance.PlaySfx("Type");
        cellButton.Select();
        TaskManager.Instance.OnDaySelected(date);
    }

    public void OnMonthDayCellClicked()
    {
        SoundManager.Instance.PlaySfx("HitWood");
        cellButton.Select();
        TaskManager.Instance.OnMonthDaySelected(date);
    }

    public void SetEmpty()
    {
        dateText.text = "";
        cellButton.interactable = false;
        if (selectedCircle != null && selectedCircle.gameObject.activeSelf)
            selectedCircle.gameObject.SetActive(false);
    }

    // 让外部（WeekCalendar）控制选中视觉
    public void SetSelected(bool selected)
    {
        if (selectedCircle != null)
            selectedCircle.gameObject.SetActive(selected);
    }
}
