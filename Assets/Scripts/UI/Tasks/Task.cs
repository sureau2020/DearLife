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
            taskNameText.text = missionData.IsCompleted ? "<s>未命名任务</s>" : "未命名任务";
        }
        else
        {
            taskNameText.text = missionData.IsCompleted ? $"<s>{missionData.Title}</s>" : missionData.Title;
        }

        if (!missionData.HasDeadline)
        {
            ddlText.text = "无DDL";
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
            // 如果任务已完成，禁用完成按钮
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
            
            // 刷新任务列表
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
        // 如果任务是循环任务的实例，处理循环任务逻辑
        if (!missionData.IsCompleted && missionData.SourceRecurringId != null) { 
            //ConfirmRequestManager.Request(ToString(),
            //    new ConfirmData
            //    {
            //        content = "这是一个循环任务！",
            //        onConfirm = () =>
            //        {
            //            // 直接删除任务实例
            //            DeleteMissionInstance(selectedDate);
            //        },
            //        onCancel = () => { }
            //    });
        }
        
        // 从 DayMissionData 中删除任务
        var dayMissionData = TaskManagerModel.Instance.GetMonth(selectedDate.ToString("yyyy-MM"))
            .GetDayMissionData(selectedDate.ToString("yyyy-MM-dd"));
        dayMissionData.DeleteSpecificMission(missionData);

        // 刷新任务列表
        TaskManager.Instance.OnDaySelected(GetTaskDate());
    }


    private DateTime GetTaskDate()
    {
        // 如果任务是循环任务的实例，使用所属日期
        if (missionData.SourceRecurringId != null)
        {
            return missionData.BelongsToDate;
        }

        // 如果任务有截止时间，使用截止时间的日期
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
