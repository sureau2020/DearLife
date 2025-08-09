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
    [SerializeField] private Button confirmButton;

    private DateTime targetDate;

    public void Show(DateTime date)
    {
        Debug.Log("oeaifjieia");
        targetDate = date;
        nameInput.text = "";
        hourInput.text = "";
        minuteInput.text = "";
        cartesianSelector.SetPointByValue(2, 2);
        gameObject.SetActive(true);
        Debug.Log("faeohoeiajfij");
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

        // 创建任务数据
        var mission = new MissionData(title, deadline, duration, difficulty);

        // 添加到model
        var dayMissionData = TaskManagerModel.Instance.GetMonth(targetDate.ToString("yyyy-MM"))
            .GetDayMissionData(targetDate.ToString("yyyy-MM-dd"));
        dayMissionData.AddMission(mission);

        // 通知UI刷新
        TaskManager.Instance.OnDaySelected(targetDate);

        gameObject.SetActive(false);
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}