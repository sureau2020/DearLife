using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputRecurringTaskPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField hourInput;
    [SerializeField] private TMP_InputField minuteInput;
    [SerializeField] private CartesianSelector cartesianSelector;
    [SerializeField] private GameObject inputTaskBackGround;
    [SerializeField] private Button confirmButton;
    [SerializeField] private InputTaskPanel inputTaskPanel;
    [SerializeField] private Button toInputTaskButton;
    [SerializeField] private List<Toggle> days;

    private DateTime targetDate;

    public void Show(DateTime date, string title, string hour, string min, float duration, float difficulty)
    {
        targetDate = date;

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
        gameObject.SetActive(true);
    }

    public void OnToInputTaskButtonClicked()
    {
        SoundManager.Instance.PlaySfx("Click");
        string title = nameInput.text;
        string hour = hourInput.text;
        string min = minuteInput.text;
        float duration = cartesianSelector.Duration;
        float difficulty = cartesianSelector.Difficulty;
        inputTaskPanel.Show(title, hour, min, duration, difficulty);
        CloseRPanel();
    }

    public void OnConfirmButtonClicked()
    {
        SoundManager.Instance.PlaySfx("Click");
        string title = nameInput.text;
        int hour = int.TryParse(hourInput.text, out var h) ? Mathf.Clamp(h, 0, 23) : -1;
        int minute = int.TryParse(minuteInput.text, out var m) ? Mathf.Clamp(m, 0, 59) : -1;
        float duration = cartesianSelector.Duration;
        float difficulty = cartesianSelector.Difficulty;
        DateTime deadline = DateTime.MinValue;
        if (hour >= 0 || minute >= 0)
        {
            deadline = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, hour, minute, 0);
        }
        List<Days> selectedDays = GetSelectedDays();
        if (selectedDays.Count == 0)
        {
            ErrorNotifier.NotifyError("请至少选择一个日子！");
            return;
        }
        SetSelectedDays(new bool[7]); // 重置选择
        RecurringMissionData recurringMission = new RecurringMissionData(title,deadline,duration,difficulty,selectedDays);
        TaskManagerModel.Instance.AddRecurringMissionData(recurringMission);
        //TaskManager.Instance.OnDaySelected(targetDate);
        inputTaskPanel.Close();
    }

    public void CloseRPanel()
    {
        gameObject.SetActive(false);
    }

    // ---------- Toggle / 星期 相关工具方法 ----------
    // 约定：days 列表索引 0..6 对应 Monday..Sunday

    private bool ValidateDaysList()
    {
        if (days == null || days.Count != 7)
        {
            Debug.LogWarning($"InputRecurringTaskPanel: 'days' 未正确配置（需要 7 个 Toggle，顺序 Sunday..Saturday）。当前 count={(days == null ? 0 : days.Count)}");
            return false;
        }
        return true;
    }

    // 将 System.DayOfWeek 映射到 days 列表索引（Sunday->0 ... Saturday->6）
    private int DayIndexFromDayOfWeek(DayOfWeek day)
    {
        return (int)day;
    }

    // 获取对应 DayOfWeek 的 Toggle（可能为 null）
    public Toggle GetToggleForDay(DayOfWeek day)
    {
        if (!ValidateDaysList()) return null;
        int idx = DayIndexFromDayOfWeek(day);
        return days[idx];
    }

    // 获取对应 Days 的 Toggle（可能为 null）
    public Toggle GetToggleForDay(Days day)
    {
        if (!ValidateDaysList()) return null;
        int idx = (int)day;
        return days[idx];
    }

    // 获取按 Sunday..Saturday 排序的布尔数组，长度为 7
    public bool[] GetSelectedDaysArray()
    {
        var result = new bool[7];
        if (!ValidateDaysList()) return result;
        for (int i = 0; i < 7; i++)
        {
            result[i] = days[i] != null && days[i].isOn;
        }
        return result;
    }

    // 返回被选中天的索引列表，索引按 Sunday..Saturday 的 0..6
    public List<int> GetSelectedDayIndices()
    {
        var list = new List<int>();
        if (!ValidateDaysList()) return list;
        for (int i = 0; i < 7; i++)
        {
            if (days[i] != null && days[i].isOn) list.Add(i);
        }
        return list;
    }

    // 返回被选中天的枚举列表，按 Sunday..Saturday
    public List<Days> GetSelectedDays()
    {
        var list = new List<Days>();
        if (!ValidateDaysList()) return list;
        for (int i = 0; i < 7; i++)
        {
            if (days[i] != null && days[i].isOn) list.Add((Days)i);
        }
        return list;
    }

    // 批量设置（传入长度 7 的布尔数组，按 Sunday..Saturday）
    public void SetSelectedDays(bool[] selections)
    {
        if (selections == null) return;
        if (!ValidateDaysList()) return;
        if (selections.Length != 7)
        {
            Debug.LogWarning("SetSelectedDays: selections 长度必须为 7（Sunday..Saturday）。");
            return;
        }
        for (int i = 0; i < 7; i++)
        {
            if (days[i] != null) days[i].isOn = selections[i];
        }
    }
}
