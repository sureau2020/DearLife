using System;
using TMPro;
using UnityEngine;

public class Task : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskNameText; 
    [SerializeField] private TextMeshProUGUI ddlText; 
    [SerializeField] private TextMeshProUGUI estimatedMinSalaryText; 
    private MissionData missionData;

    public void Initialize(MissionData missionData, GameSettings settings)
    {
        this.missionData = missionData;

        if (string.IsNullOrEmpty(missionData.Title))
        {
            taskNameText.text = "未命名任务";
        }
        else
        {
            taskNameText.text = missionData.Title;
        }

        if (!missionData.HasDeadline)
        {
            ddlText.text = "无截止时间";
        }
        else
        {
            ddlText.text = missionData.Deadline.ToString("HH:mm");
        }

        if (missionData.Duration <= 0 || missionData.Difficulty <= 0)
        {
            estimatedMinSalaryText.text = "无预估薪资";
        }
        else
        {
            int estimatedSalary = Calculators.EstimatedMinSalary(missionData.Duration, missionData.Difficulty, settings.HourlyWage, settings.DifficultyBonus);
            estimatedMinSalaryText.text = $"最低薪资：{estimatedSalary:F2} 金币";
        }
    }





}
