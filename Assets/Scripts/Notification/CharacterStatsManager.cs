// CharacterStatsManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    public List<CharacterStat> Stats = new List<CharacterStat>();

    void Start()
    {
        // ����ʱ��ȡ��֮ǰ��֪ͨ
        NotificationManager.CancelAll();

        // ��ʼ�� CharacterStat ��ֵ�ͽ���״̬
        SyncStatsWithCharacterData();

        // ���ݵ�ǰ״̬�����µ�֪ͨ
        ScheduleAllNotifications();
    }


    // ��Ӧ�ý����̨���˳�ʱ
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // ���¼��㲢����֪ͨ
            RecalculateAndScheduleNotifications();
        }
    }

    void OnApplicationQuit()
    {
        RecalculateAndScheduleNotifications();
    }

    private void RecalculateAndScheduleNotifications()
    {
        // ȷ������������
        SyncStatsWithCharacterData();
        // ���µ���
        ScheduleAllNotifications();
    }

    /// <summary>
    /// ���ķ�����ģ��δ����״̬�仯����Ϊÿ���׶ε���֪ͨ
    /// </summary>
    public void ScheduleAllNotifications()
    {
        NotificationManager.CancelAll();

        string characterName = GameManager.Instance?.StateManager?.Character?.Name ?? "��Ľ�ɫ";

        // --- ģ�⻷����ʼ�� ---
        var simulatedStats = Stats.ToDictionary(s => s.Name, s => s.Value);
        HealthState currentSimulatedState = GameManager.Instance.StateManager.Character.HealthState;
        float timeOffsetHours = 0f;
        int maxIterations = 10; // ���Ӱ�ȫ�����Է�����ѭ��

        Debug.Log($"--- ��ʼ����֪ͨģ�� ---");
        Debug.Log($"��ʼ״̬: {currentSimulatedState}, Full: {simulatedStats["Full"]}, San: {simulatedStats["San"]}, Clean: {simulatedStats["Clean"]}");

        // --- ģ��ѭ�� ---
        for (int i = 0; i < maxIterations; i++)
        {
            float timeToNextStateChange = float.MaxValue;
            HealthState nextState = currentSimulatedState;
            string drivingStatName = null;

            // ʵʱ��ȡ������ֵ��˥����
            var currentDecayRates = new Dictionary<string, float>();
            foreach (var stat in Stats)
            {
                float rate = stat.DecayRatePerHour;
                if (currentSimulatedState == HealthState.Crazy && stat.Name == "Full")
                {
                    rate *= 4f;
                }
                if (currentSimulatedState == HealthState.Dirty && stat.Name == "San")
                {
                    rate *= 4f;
                }
                currentDecayRates[stat.Name] = rate;
            }

            // ���ݵ�ǰģ���״̬��������Ҫ�����Щ��ֵ
            switch (currentSimulatedState)
            {
                case HealthState.Normal:
                    // Normal ״̬�£�Full, San, Clean �κ�һ���ȹ��㶼�ᵼ��״̬�仯
                    float timeToFullZero = CalculateTimeToDepletion(simulatedStats["Full"], currentDecayRates["Full"]);
                    float timeToSanZero = CalculateTimeToDepletion(simulatedStats["San"], currentDecayRates["San"]);
                    float timeToCleanZero = CalculateTimeToDepletion(simulatedStats["Clean"], currentDecayRates["Clean"]);

                    timeToNextStateChange = Mathf.Min(timeToFullZero, timeToSanZero, timeToCleanZero);

                    if (timeToNextStateChange == timeToFullZero)
                    {
                        nextState = HealthState.Weak;
                        drivingStatName = "Full";
                    }
                    else if (timeToNextStateChange == timeToSanZero)
                    {
                        nextState = HealthState.Crazy;
                        drivingStatName = "San";
                    }
                    else
                    {
                        nextState = HealthState.Dirty;
                        drivingStatName = "Clean";
                    }
                    break;

                case HealthState.Weak:
                    timeToNextStateChange = CalculateTimeToDepletion(simulatedStats["Full"], currentDecayRates["Full"]);
                    nextState = HealthState.Dead;
                    drivingStatName = "Full";
                    break;

                case HealthState.Crazy:
                    timeToNextStateChange = CalculateTimeToDepletion(simulatedStats["Full"], currentDecayRates["Full"]);
                    nextState = HealthState.Weak;
                    drivingStatName = "Full";
                    break;

                case HealthState.Dirty:
                    // Dirty ״̬�£�San ˥���ӿ죬�� Full �� San ���㶼���ܵ���״̬�仯
                    float dirty_timeToFullZero = CalculateTimeToDepletion(simulatedStats["Full"], currentDecayRates["Full"]);
                    float dirty_timeToSanZero = CalculateTimeToDepletion(simulatedStats["San"], currentDecayRates["San"]);

                    // �������ȼ���������ж�
                    if (dirty_timeToFullZero <= dirty_timeToSanZero)
                    {
                        // Full �ȹ����ͬʱ���㣬���� Weak ״̬
                        timeToNextStateChange = dirty_timeToFullZero;
                        nextState = HealthState.Weak;
                        drivingStatName = "Full";
                    }
                    else
                    {
                        // San �ȹ��㣬���� Crazy ״̬
                        timeToNextStateChange = dirty_timeToSanZero;
                        nextState = HealthState.Crazy;
                        drivingStatName = "San";
                    }
                    break;

                case HealthState.Dead:
                    timeToNextStateChange = float.MaxValue;
                    break;
            }

            if (timeToNextStateChange == float.MaxValue)
            {
                Debug.Log($"״̬ {currentSimulatedState} û�к�������˥����ģ�������");
                break;
            }

            Debug.Log($"�� {i + 1} ��ģ��: ״̬ '{currentSimulatedState}' ���� {timeToNextStateChange:F2} Сʱ���� '{drivingStatName}' �����������");

            // �ڵ�ǰʱ�䴰���ڵ���֪ͨ
            foreach (var stat in Stats)
            {
                foreach (var rule in stat.NotificationRules)
                {
                    if (rule.TargetHealthState == currentSimulatedState)
                    {
                        float timeToNotify = GetTimeToReachThreshold(stat, simulatedStats[stat.Name], rule.TriggerValue, currentDecayRates[stat.Name]);

                        if (timeToNotify > 0 && timeToNotify < timeToNextStateChange)
                        {
                            string identifier = $"{stat.Name}_{rule.TargetHealthState}_{rule.TriggerValue}_{i}";
                            string formattedMessage = rule.GetFormattedMessage(characterName);
                            float finalFireTime = timeOffsetHours + timeToNotify;

                            NotificationManager.ScheduleNotification("״̬����!", formattedMessage, finalFireTime, identifier);
                            Debug.Log($"[���ȳɹ�] ����: {identifier} | ��Ϣ: '{formattedMessage}' | ���� {finalFireTime:F2} Сʱ�󴥷���");
                        }
                    }
                }
            }

            // Ϊ��һ��ģ���������
            timeOffsetHours += timeToNextStateChange;

            // ģ��������ֵ��˥��
            foreach (var stat in Stats)
            {
                simulatedStats[stat.Name] -= currentDecayRates[stat.Name] * timeToNextStateChange;
                if (simulatedStats[stat.Name] < 0) simulatedStats[stat.Name] = 0;
            }

            // ״̬ת������ֵ����
            if (drivingStatName == "Full" && nextState == HealthState.Weak)
            {
                simulatedStats["Full"] = 100f;
                Debug.Log($"״̬ת��! ��״̬: {nextState}, 'Full' ������Ϊ100���ܺ�ʱ: {timeOffsetHours:F2}h");
            }
            else
            {
                Debug.Log($"״̬ת��! ��״̬: {nextState}, ����ֵ���á��ܺ�ʱ: {timeOffsetHours:F2}h");
            }

            currentSimulatedState = nextState;

            if (currentSimulatedState == HealthState.Dead)
            {
                Debug.Log("��������״̬��ģ�������");
                break;
            }
        }
        Debug.Log($"--- ����֪ͨģ����� ---");
    }

    // ����������������ֵ�ӵ�ǰֵ˥����0��Ҫ��ʱ��
    private float CalculateTimeToDepletion(float currentValue, float decayRate)
    {
        if (decayRate <= 0) return float.MaxValue;
        return currentValue / decayRate;
    }

    // ��������������ӵ�ǰֵ����ֵ��ʱ��
    private float GetTimeToReachThreshold(CharacterStat stat, float currentValue, float threshold, float decayRate)
    {
        if (decayRate <= 0 || currentValue <= threshold)
            return -1f;
        return (currentValue - threshold) / decayRate;
    }
    #region ����ͬ�����¼����� (���������Ŀ�ṹ)

    //private void OnCharacterStateChanged(string stateName, int newValue)
    //{
    //    // ���ݱ仯����Ҫ��ȫ����ͬ�������µ���
    //    RecalculateAndScheduleNotifications();
    //    Debug.Log($"��ɫ״̬ '{stateName}' �Ѹı䣬���µ�������֪ͨ��");
    //}

    private void SyncStatsWithCharacterData()
    {
        if (GameManager.Instance?.StateManager?.Character == null) return;

        var character = GameManager.Instance.StateManager.Character;
        HealthState generalHealthState = character.HealthState;

        foreach (var stat in Stats)
        {
            // ͬ������״̬
            stat.SyncHealthState(generalHealthState);

            // ͬ��������ֵ
            switch (stat.Name)
            {
                case "Full":
                    stat.SyncValue(character.Full);
                    break;
                case "San":
                    stat.SyncValue(character.San);
                    break;
                case "Clean":
                    stat.SyncValue(character.Clean);
                    break;
            }
        }
    }

    //private void SubscribeToCharacterEvents()
    //{
    //    if (GameManager.Instance?.StateManager?.Character != null)
    //    {
    //        GameManager.Instance.StateManager.Character.OnCharacterStateChanged += OnCharacterStateChanged;
    //    }
    //}

    //private void UnsubscribeFromCharacterEvents()
    //{
    //    if (GameManager.Instance?.StateManager?.Character != null)
    //    {
    //        GameManager.Instance.StateManager.Character.OnCharacterStateChanged -= OnCharacterStateChanged;
    //    }
    //}

    #endregion
}