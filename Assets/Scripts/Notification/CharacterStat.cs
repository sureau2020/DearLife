// CharacterStat.cs
using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class CharacterStat
{
    public string Name;                  // 状态名字（饱食度 / 理智 / 清洁度）
    public float Value;                  // 当前数值
    public float DecayRatePerHour;       // 每小时减少多少数值

    // 核心改动：使用一个描述符列表来定义所有的通知触发规则
    public StatusDescriptor[] NotificationRules;

    [NonSerialized]
    public HealthState CurrentHealthState = HealthState.Normal; // 当前健康状态

    /// <summary>
    /// 从外部同步数值
    /// </summary>
    public void SyncValue(float characterValue)
    {
        Value = characterValue;
    }

    /// <summary>
    /// 从外部同步健康状态
    /// </summary>
    public void SyncHealthState(HealthState healthState)
    {
        CurrentHealthState = healthState;
    }

    /// <summary>
    /// 计算从当前 Value 衰减到目标阈值需要的时间（小时）
    /// </summary>
    public float GetTimeToReachThreshold(float threshold)
    {
        if (DecayRatePerHour <= 0 || Value <= threshold)
        {
            return -1f; // 无法达到或已经低于阈值
        }
        return (Value - threshold) / DecayRatePerHour;
    }
}



[Serializable]
public class StatusDescriptor
{
    [Tooltip("这条通知规则在哪种健康状态下生效")]
    public HealthState TargetHealthState = HealthState.Normal;

    [Tooltip("当数值降低到这个值时，触发通知")]
    public float TriggerValue;

    [Tooltip("要发送的通知消息。可以使用 {characterName} 和 {value} 占位符。")]
    [TextArea]
    public string Message;

    /// <summary>
    /// 格式化消息，替换占位符
    /// </summary>
    public string GetFormattedMessage(string characterName)
    {
        if (string.IsNullOrEmpty(Message)) return "";

        // 注意：因为这是未来通知，显示的是触发值，而不是当前值
        return Message.Replace("{characterName}", characterName)
                      .Replace("{value}", TriggerValue.ToString("F0"));
    }
}