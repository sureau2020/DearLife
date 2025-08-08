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
            taskNameText.text = "δ��������";
        }
        else
        {
            taskNameText.text = missionData.Title;
        }

        if (!missionData.HasDeadline)
        {
            ddlText.text = "�޽�ֹʱ��";
        }
        else
        {
            ddlText.text = missionData.Deadline.ToString("HH:mm");
        }

        if (missionData.Duration <= 0 || missionData.Difficulty <= 0)
        {
            estimatedMinSalaryText.text = "��Ԥ��н��";
        }
        else
        {
            int estimatedSalary = Calculators.EstimatedMinSalary(missionData.Duration, missionData.Difficulty, settings.HourlyWage, settings.DifficultyBonus);
            estimatedMinSalaryText.text = $"���н�ʣ�{estimatedSalary:F2} ���";
        }
    }





}
