// CharacterStatsManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    public List<CharacterStat> Stats = new List<CharacterStat>();

    void Start()
    {
        // 启动时先取消之前的通知
        NotificationManager.CancelAll();

        // 初始化 CharacterStat 的值和健康状态
        SyncStatsWithCharacterData();

        // 根据当前状态调度新的通知
        ScheduleAllNotifications();
    }


    // 当应用进入后台或退出时
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // 重新计算并调度通知
            RecalculateAndScheduleNotifications();
        }
    }

    void OnApplicationQuit()
    {
        RecalculateAndScheduleNotifications();
    }

    private void RecalculateAndScheduleNotifications()
    {
        // 确保数据是最新
        SyncStatsWithCharacterData();
        // 重新调度
        ScheduleAllNotifications();
    }

    /// <summary>
    /// 核心方法：模拟未来的状态变化，并为每个阶段调度通知
    /// </summary>
    public void ScheduleAllNotifications()
    {
        NotificationManager.CancelAll();

        string characterName = GameManager.Instance?.StateManager?.Character?.Name ?? "你的角色";

        // --- 模拟环境初始化 ---
        var simulatedStats = Stats.ToDictionary(s => s.Name, s => s.Value);
        HealthState currentSimulatedState = GameManager.Instance.StateManager.Character.HealthState;
        float timeOffsetHours = 0f;
        int maxIterations = 10; // 增加安全锁，以防无限循环

        Debug.Log($"--- 开始离线通知模拟 ---");
        Debug.Log($"初始状态: {currentSimulatedState}, Full: {simulatedStats["Full"]}, San: {simulatedStats["San"]}, Clean: {simulatedStats["Clean"]}");

        // --- 模拟循环 ---
        for (int i = 0; i < maxIterations; i++)
        {
            float timeToNextStateChange = float.MaxValue;
            HealthState nextState = currentSimulatedState;
            string drivingStatName = null;

            // 实时获取各个数值的衰减率
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

            // 根据当前模拟的状态，决定需要监控哪些数值
            switch (currentSimulatedState)
            {
                case HealthState.Normal:
                    // Normal 状态下，Full, San, Clean 任何一个先归零都会导致状态变化
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
                    // Dirty 状态下，San 衰减加快，但 Full 和 San 归零都可能导致状态变化
                    float dirty_timeToFullZero = CalculateTimeToDepletion(simulatedStats["Full"], currentDecayRates["Full"]);
                    float dirty_timeToSanZero = CalculateTimeToDepletion(simulatedStats["San"], currentDecayRates["San"]);

                    // 根据优先级规则进行判断
                    if (dirty_timeToFullZero <= dirty_timeToSanZero)
                    {
                        // Full 先归零或同时归零，进入 Weak 状态
                        timeToNextStateChange = dirty_timeToFullZero;
                        nextState = HealthState.Weak;
                        drivingStatName = "Full";
                    }
                    else
                    {
                        // San 先归零，进入 Crazy 状态
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
                Debug.Log($"状态 {currentSimulatedState} 没有后续负向衰减，模拟结束。");
                break;
            }

            Debug.Log($"第 {i + 1} 轮模拟: 状态 '{currentSimulatedState}' 将在 {timeToNextStateChange:F2} 小时后因 '{drivingStatName}' 归零而结束。");

            // 在当前时间窗口内调度通知
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

                            NotificationManager.ScheduleNotification("状态提醒!", formattedMessage, finalFireTime, identifier);
                            Debug.Log($"[调度成功] 规则: {identifier} | 消息: '{formattedMessage}' | 将在 {finalFireTime:F2} 小时后触发。");
                        }
                    }
                }
            }

            // 为下一次模拟更新数据
            timeOffsetHours += timeToNextStateChange;

            // 模拟所有数值的衰减
            foreach (var stat in Stats)
            {
                simulatedStats[stat.Name] -= currentDecayRates[stat.Name] * timeToNextStateChange;
                if (simulatedStats[stat.Name] < 0) simulatedStats[stat.Name] = 0;
            }

            // 状态转换与数值重置
            if (drivingStatName == "Full" && nextState == HealthState.Weak)
            {
                simulatedStats["Full"] = 100f;
                Debug.Log($"状态转换! 新状态: {nextState}, 'Full' 已重置为100。总耗时: {timeOffsetHours:F2}h");
            }
            else
            {
                Debug.Log($"状态转换! 新状态: {nextState}, 无数值重置。总耗时: {timeOffsetHours:F2}h");
            }

            currentSimulatedState = nextState;

            if (currentSimulatedState == HealthState.Dead)
            {
                Debug.Log("进入死亡状态，模拟结束。");
                break;
            }
        }
        Debug.Log($"--- 离线通知模拟结束 ---");
    }

    // 辅助方法：计算数值从当前值衰减到0需要的时间
    private float CalculateTimeToDepletion(float currentValue, float decayRate)
    {
        if (decayRate <= 0) return float.MaxValue;
        return currentValue / decayRate;
    }

    // 辅助方法：计算从当前值到阈值的时间
    private float GetTimeToReachThreshold(CharacterStat stat, float currentValue, float threshold, float decayRate)
    {
        if (decayRate <= 0 || currentValue <= threshold)
            return -1f;
        return (currentValue - threshold) / decayRate;
    }
    #region 数据同步和事件订阅 (根据你的项目结构)

    //private void OnCharacterStateChanged(string stateName, int newValue)
    //{
    //    // 数据变化后，需要完全重新同步和重新调度
    //    RecalculateAndScheduleNotifications();
    //    Debug.Log($"角色状态 '{stateName}' 已改变，重新调度所有通知。");
    //}

    private void SyncStatsWithCharacterData()
    {
        if (GameManager.Instance?.StateManager?.Character == null) return;

        var character = GameManager.Instance.StateManager.Character;
        HealthState generalHealthState = character.HealthState;

        foreach (var stat in Stats)
        {
            // 同步健康状态
            stat.SyncHealthState(generalHealthState);

            // 同步具体数值
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