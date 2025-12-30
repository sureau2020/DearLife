using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InputTaskPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField hourInput;
    [SerializeField] private TMP_InputField minuteInput;
    [SerializeField] private CartesianSelector cartesianSelector;
    [SerializeField] private GameObject inputTaskBackGround;
    [SerializeField] private Button confirmButton;
    [SerializeField] private InputRecurringTaskPanel inputRecurringTaskPanel;

    private DateTime targetDate;

    public void Show(DateTime date)
    {
        targetDate = date;
        nameInput.text = "";
        hourInput.text = "";
        minuteInput.text = "";
        cartesianSelector.SetPointByValue(2, 2);
        inputTaskBackGround.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Show(string title, string hour, string min, float duration, float difficulty) { 
        nameInput.text = string.IsNullOrEmpty(title) ? "" : title;
        hourInput.text = string.IsNullOrEmpty(hour) ? "" : hour;
        minuteInput.text = string.IsNullOrEmpty(min) ? "" : min;
        float d = 2f;
        float diff = 2f;
        bool validDuration = !float.IsNaN(duration) && !float.IsInfinity(duration) && duration >= 0f && duration <= 4f;
        bool validDifficulty = !float.IsNaN(difficulty) && !float.IsInfinity(difficulty) && difficulty >= 0f && difficulty <= 4f;
        if (validDuration) d = duration;
        if (validDifficulty) diff = difficulty;
        cartesianSelector.SetPointByValue(d, diff);
    }


    public void OnConfirm()
    {
        string title = nameInput.text;
        int hour = int.TryParse(hourInput.text, out var h) ? Mathf.Clamp(h, 0, 23) : -1;
        int minute = int.TryParse(minuteInput.text, out var m) ? Mathf.Clamp(m, 0, 59) : -1;
        float duration = cartesianSelector.Duration;
        float difficulty = cartesianSelector.Difficulty;
        DateTime deadline = DateTime.MinValue;
        if (hour >= 0 || minute >= 0) { 
            deadline = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, hour, minute, 0); 
        }

        var mission = new MissionData(title, deadline, duration, difficulty, targetDate);

        var dayMissionData = TaskManagerModel.Instance.GetMonth(targetDate.ToString("yyyy-MM"))
            .GetDayMissionData(targetDate.ToString("yyyy-MM-dd"));
        dayMissionData.AddMission(mission);

        TaskManager.Instance.OnDaySelected(targetDate);

        Close();
    }

    public void Close()
    {
        inputTaskBackGround.SetActive(false);
        inputRecurringTaskPanel.CloseRPanel();
        gameObject.SetActive(false);
    }

    public void OnToRecurringTaskButtonClicked()
    {
        SoundManager.Instance.PlaySfx("Click");
        string title = nameInput.text;
        string hour = hourInput.text;
        string min = minuteInput.text;
        float duration = cartesianSelector.Duration;
        float difficulty = cartesianSelector.Difficulty;
        inputRecurringTaskPanel.Show(targetDate,title,hour,min,duration,difficulty);
    }



}