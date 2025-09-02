using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Task : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskNameText; 
    [SerializeField] private TextMeshProUGUI ddlText; 
    [SerializeField] private TextMeshProUGUI estimatedMinSalaryText;
    [SerializeField] private Button completeButton; 
    [SerializeField] private Button deleteButton; 
    
    private MissionData missionData;
    private GameSettings settings;

    public void Initialize(MissionData missionData, GameSettings settings)
    {
        this.missionData = missionData;
        this.settings = settings;

        UpdateTaskDisplay();
        SetupButtons();
    }

    private void UpdateTaskDisplay()
    {
        if (string.IsNullOrEmpty(missionData.Title))
        {
            taskNameText.text = missionData.IsCompleted ? "<s>δ��������</s>" : "δ��������";
        }
        else
        {
            taskNameText.text = missionData.IsCompleted ? $"<s>{missionData.Title}</s>" : missionData.Title;
        }

        if (!missionData.HasDeadline)
        {
            ddlText.text = "��DDL";
        }
        else
        {
            ddlText.text = missionData.Deadline.ToString("HH:mm");
        }

        if (missionData.Duration <= 0 || missionData.Difficulty <= 0)
        {
            estimatedMinSalaryText.text = "?";
        }
        else
        {
            int estimatedSalary = Calculators.EstimatedMinSalary(
                missionData.Duration, 
                missionData.Difficulty, 
                settings.HourlyWage, 
                settings.DifficultyBonus
            );
            estimatedMinSalaryText.text = $"{estimatedSalary}+";
        }
    }

    private void SetupButtons()
    {
        if (completeButton != null)
        {
            // �����������ɣ�������ɰ�ť
            completeButton.interactable = !missionData.IsCompleted;
            completeButton.onClick.RemoveAllListeners();
            completeButton.onClick.AddListener(OnCompleteTask);
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(OnDeleteTask);
        }
    }

 
    public void OnCompleteTask()
    {
        SoundManager.Instance.PlaySfx("BuyItem");
        var result = GameManager.Instance.StateManager.CompleteMission(missionData);
        
        if (result.Success)
        {
            UpdateTaskDisplay();
            
            // ˢ�������б�
            TaskManager.Instance.OnDaySelected(GetTaskDate());
        }
        else
        {
            ErrorNotifier.NotifyError(result.Message);
        }
    }

    public void OnDeleteTask()
    {
        SoundManager.Instance.PlaySfx("Delete");
        var selectedDate = GetTaskDate();
        
        // �� DayMissionData ��ɾ������
        var dayMissionData = TaskManagerModel.Instance.GetMonth(selectedDate.ToString("yyyy-MM"))
            .GetDayMissionData(selectedDate.ToString("yyyy-MM-dd"));
        dayMissionData.DeleteSpecificMission(missionData);

        // ˢ�������б�
        TaskManager.Instance.OnDaySelected(GetTaskDate());
    }


    private DateTime GetTaskDate()
    {
        // ��������н�ֹʱ�䣬ʹ�ý�ֹʱ�������
        if (missionData.HasDeadline)
        {
            return missionData.Deadline.Date;
        }
        
        return TaskManager.Instance.SelectedDate;
    }

    private void OnDestroy()
    {
        if (completeButton != null)
        {
            completeButton.onClick.RemoveAllListeners();
        }
        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
        }
    }
}
