// CharacterStatsManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    public List<CharacterStat> Stats = new List<CharacterStat>();

    void Start()
    {
        // 启动：初始化并清理所有遗留（已排程+已显示），再重排
        NotificationManager.Initialize(resetOnLaunch: true);
        SyncStatsWithCharacterData();
        ScheduleAllNotifications();
    }

    // 进入后台：按最新数据重排（先清“已排程”，避免重复）
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            RecalculateAndScheduleNotifications(cancelScheduled: true);
        }
    }

    // 退出：不再重排，避免与已存在的排程重复/覆盖
    void OnApplicationQuit()
    {
        // 留空：让后台的已排程保持生效
    }

    private void RecalculateAndScheduleNotifications(bool cancelScheduled)
    {
        SyncStatsWithCharacterData();

        if (cancelScheduled)
        {
            NotificationManager.CancelAllScheduled();
        }

        ScheduleAllNotifications();
        NotificationManager.ScheduleNotification("通知成功", "在离线期间，仍会收到角色状态变化的提醒。", 0, "StatusUpdateNotification");
    }

    /// <summary>
    /// 核心方法：模拟未来的状态变化，并为每个阶段调度通知
    /// </summary>
    public void ScheduleAllNotifications()
    {
        string characterName = GameManager.Instance?.StateManager?.Character?.Name ?? "你的角色";

        var simulatedStats = Stats.ToDictionary(s => s.Name, s => s.Value);
        HealthState currentSimulatedState = GameManager.Instance.StateManager.Character.HealthState;
        float timeOffsetHours = 0f;
        int maxIterations = 10;

        Debug.Log($"--- 开始离线通知模拟 ---");
        Debug.Log($"初始状态: {currentSimulatedState}, Full: {simulatedStats["Full"]}, San: {simulatedStats["San"]}, Clean: {simulatedStats["Clean"]}");

        for (int i = 0; i < maxIterations; i++)
        {
            float timeToNextStateChange = float.MaxValue;
            HealthState nextState = currentSimulatedState;

            var currentDecayRates = new Dictionary<string, float>();
            foreach (var stat in Stats)
            {
                float rate = stat.DecayRatePerHour;
                if (currentSimulatedState == HealthState.Crazy && stat.Name == "Full") rate *= 4f;
                if (currentSimulatedState == HealthState.Dirty && stat.Name == "San") rate *= 4f;
                currentDecayRates[stat.Name] = rate;
            }

            string driving = null;

            switch (currentSimulatedState)
            {
                case HealthState.Normal:
                    float tFull = CalculateTimeToDepletion(simulatedStats["Full"], currentDecayRates["Full"]);
                    float tSan = CalculateTimeToDepletion(simulatedStats["San"], currentDecayRates["San"]);
                    float tClean = CalculateTimeToDepletion(simulatedStats["Clean"], currentDecayRates["Clean"]);
                    timeToNextStateChange = Mathf.Min(tFull, tSan, tClean);
                    if (timeToNextStateChange == tFull) { nextState = HealthState.Weak; driving = "Full"; }
                    else if (timeToNextStateChange == tSan) { nextState = HealthState.Crazy; driving = "San"; }
                    else { nextState = HealthState.Dirty; driving = "Clean"; }
                    break;

                case HealthState.Weak:
                    timeToNextStateChange = CalculateTimeToDepletion(simulatedStats["Full"], currentDecayRates["Full"]);
                    nextState = HealthState.Dead; driving = "Full";
                    break;

                case HealthState.Crazy:
                    timeToNextStateChange = CalculateTimeToDepletion(simulatedStats["Full"], currentDecayRates["Full"]);
                    nextState = HealthState.Weak; driving = "Full";
                    break;

                case HealthState.Dirty:
                    float dtFull = CalculateTimeToDepletion(simulatedStats["Full"], currentDecayRates["Full"]);
                    float dtSan = CalculateTimeToDepletion(simulatedStats["San"], currentDecayRates["San"]);
                    if (dtFull <= dtSan) { timeToNextStateChange = dtFull; nextState = HealthState.Weak; driving = "Full"; }
                    else { timeToNextStateChange = dtSan; nextState = HealthState.Crazy; driving = "San"; }
                    break;

                case HealthState.Dead:
                    timeToNextStateChange = float.MaxValue;
                    break;
            }

            if (timeToNextStateChange == float.MaxValue) break;

            // 在当前窗口内安排通知（发送前按ID取消，幂等）
            foreach (var stat in Stats)
            {
                foreach (var rule in stat.NotificationRules)
                {
                    //Debug.Log($"stat={stat.Name}, rule.TargetHealthState={rule.TargetHealthState}, currentSimulatedState={currentSimulatedState}, rule.TriggerValue={rule.TriggerValue}, statValue={simulatedStats[stat.Name]}");
                    if (rule.TargetHealthState != currentSimulatedState) continue;

                    float timeToNotify = GetTimeToReachThreshold(simulatedStats[stat.Name], rule.TriggerValue, currentDecayRates[stat.Name]);
                    //Debug.Log($"  计算通知时间: timeToNotify={timeToNotify}");
                    if (timeToNotify <= 0 || timeToNotify >= timeToNextStateChange) continue;

                    string identifier = $"{stat.Name}_{rule.TargetHealthState}_{rule.TriggerValue}_{i}";
                    string formattedMessage = rule.GetFormattedMessage(characterName);
                    float finalFireTime = timeOffsetHours + timeToNotify;

                    // 防重复：先按ID取消（或覆盖）
                    NotificationManager.CancelByIdentifier(identifier);
                    NotificationManager.ScheduleNotification("注意", formattedMessage, finalFireTime, identifier);

                    //Debug.Log($"安排通知: [ID={identifier}] [属性={stat.Name}] [触发值={rule.TriggerValue}]");
                    //Debug.Log($"消息内容: \"{formattedMessage}\"");
                    //Debug.Log($"触发时间: {finalFireTime} 小时后 (总偏移时间: {timeOffsetHours})");
                    //Debug.Log($"当前状态: {currentSimulatedState}, 下一状态: {nextState}\n");
                    
                }
            }

            timeOffsetHours += timeToNextStateChange;

            foreach (var stat in Stats)
            {
                simulatedStats[stat.Name] -= currentDecayRates[stat.Name] * timeToNextStateChange;
                if (simulatedStats[stat.Name] < 0) simulatedStats[stat.Name] = 0;
            }

            if (driving == "Full" && nextState == HealthState.Weak)
            {
                simulatedStats["Full"] = 100f;
            }

            currentSimulatedState = nextState;
            if (currentSimulatedState == HealthState.Dead) break;
        }
        Debug.Log($"--- 离线通知模拟结束 ---");
        GetNextTaskTenLastMinutes();
        NotificationManager.ScheduleNotification("提醒", $"开始任务了咩？{characterName}需要你。", 0.1f, "MissionStartCheckNotification");
    }

    private void GetNextTaskTenLastMinutes()
    {
        MissionData a = TaskManager.Instance.GetNextMission();
        if (a == null) return;
        if (!a.HasDeadline) return;

        DateTime ddl = a.Deadline;
        DateTime now = DateTime.Now;

        // 计算在 deadline 前 10 分钟发通知的时间点（相对现在的时长）
        TimeSpan timeUntilNotify = ddl - now - TimeSpan.FromMinutes(10);

        // 唯一标识，便于幂等（先取消再安排）
        string identifier = $"Mission_DeadlineNotify_{a.Title}_{ddl.Ticks}";
        NotificationManager.CancelByIdentifier(identifier);

        // 如果已经到了或不足 10 分钟，立即（短延迟）提醒；否则按小时数排程
        if (timeUntilNotify <= TimeSpan.Zero)
        {
            string message = $"任务「{a.Title}」即将结束，请尽快完成。";
            NotificationManager.ScheduleNotification("任务快结束了", message, 0.01f, identifier);
            Debug.Log($"[通知] 立即发送截止前十分钟提醒: {a.Title}");
        }
        else
        {
            float hours = (float)timeUntilNotify.TotalHours;
            string message = $"任务「{a.Title}」将在 10 分钟后截止。";
            NotificationManager.ScheduleNotification("任务提醒", message, hours, identifier);
            Debug.Log($"[通知] 已安排在 {timeUntilNotify} 后（{hours} 小时）发送截止前十分钟提醒: {a.Title}");
        }
    }

    private float CalculateTimeToDepletion(float currentValue, float decayRate)
    {
        if (decayRate <= 0) return float.MaxValue;
        return currentValue / decayRate;
    }

    private float GetTimeToReachThreshold(float currentValue, float threshold, float decayRate)
    {
        if (decayRate <= 0 || currentValue <= threshold) return -1f;
        return (currentValue - threshold) / decayRate;
    }

    private void SyncStatsWithCharacterData()
    {
        if (GameManager.Instance?.StateManager?.Character == null) return;

        var character = GameManager.Instance.StateManager.Character;
        var hs = character.HealthState;

        foreach (var stat in Stats)
        {
            stat.SyncHealthState(hs);
            switch (stat.Name)
            {
                case "Full":  stat.SyncValue(character.Full);  break;
                case "San":   stat.SyncValue(character.San);   break;
                case "Clean": stat.SyncValue(character.Clean); break;
            }
        }
    }
}