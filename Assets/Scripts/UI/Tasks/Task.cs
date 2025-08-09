using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Task : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskNameText; 
    [SerializeField] private TextMeshProUGUI ddlText; 
    [SerializeField] private TextMeshProUGUI estimatedMinSalaryText;
    [SerializeField] private Button completeButton; // �������ť
    [SerializeField] private Button deleteButton; // ɾ������ť
    
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
        // �����������ƣ�������������ӣ�����ɣ���ʶ
        if (string.IsNullOrEmpty(missionData.Title))
        {
            taskNameText.text = missionData.IsCompleted ? "δ������������ɣ�" : "δ��������";
        }
        else
        {
            taskNameText.text = missionData.IsCompleted ? $"{missionData.Title}������ɣ�" : missionData.Title;
        }

        // ���ý�ֹʱ��
        if (!missionData.HasDeadline)
        {
            ddlText.text = "�޽�ֹʱ��";
        }
        else
        {
            ddlText.text = missionData.Deadline.ToString("HH:mm");
        }

        // ����Ԥ��н��
        if (missionData.Duration <= 0 || missionData.Difficulty <= 0)
        {
            estimatedMinSalaryText.text = "��Ԥ��н��";
        }
        else
        {
            int estimatedSalary = Calculators.EstimatedMinSalary(
                missionData.Duration, 
                missionData.Difficulty, 
                settings.HourlyWage, 
                settings.DifficultyBonus
            );
            estimatedMinSalaryText.text = $"���н�ʣ�{estimatedSalary} ���";
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
        // ���� StateManager �������
        var result = GameManager.Instance.StateManager.CompleteMission(missionData);
        
        if (result.Success)
        {
            Debug.Log($"���� '{missionData.Title}' ��ɳɹ���");
            
            // ����UI��ʾ
            UpdateTaskDisplay();
            
            // ˢ�������б�֪ͨ TaskManager ���µ����������ʾ��
            var selectedDate = GetTaskDate();
            TaskManager.Instance.OnDaySelected(selectedDate);
        }
        else
        {
            Debug.LogWarning($"�������ʧ��: {result.Message}");
        }
    }

    public void OnDeleteTask()
    {
        var selectedDate = GetTaskDate();
        
        // �� DayMissionData ��ɾ������
        var dayMissionData = TaskManagerModel.Instance.GetMonth(selectedDate.ToString("yyyy-MM"))
            .GetDayMissionData(selectedDate.ToString("yyyy-MM-dd"));
        dayMissionData.DeleteSpecificMission(missionData);
        
        Debug.Log($"���� '{missionData.Title}' ɾ���ɹ���");
        
        // ˢ�������б�
        TaskManager.Instance.OnDaySelected(selectedDate);
    }


    private DateTime GetTaskDate()
    {
        // ��������н�ֹʱ�䣬ʹ�ý�ֹʱ�������
        if (missionData.HasDeadline)
        {
            return missionData.Deadline.Date;
        }
        
        // ���û�н�ֹʱ�䣬ʹ�õ�ǰѡ�е�����
        return TaskManager.Instance.SelectedDate;
    }

    private void OnDestroy()
    {
        // �����¼�����
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
