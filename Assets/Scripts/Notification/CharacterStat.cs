// CharacterStat.cs
using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class CharacterStat
{
    public string Name;                  // ״̬���֣���ʳ�� / ���� / ���ȣ�
    public float Value;                  // ��ǰ��ֵ
    public float DecayRatePerHour;       // ÿСʱ���ٶ�����ֵ

    // ���ĸĶ���ʹ��һ���������б����������е�֪ͨ��������
    public StatusDescriptor[] NotificationRules;

    [NonSerialized]
    public HealthState CurrentHealthState = HealthState.Normal; // ��ǰ����״̬

    /// <summary>
    /// ���ⲿͬ����ֵ
    /// </summary>
    public void SyncValue(float characterValue)
    {
        Value = characterValue;
    }

    /// <summary>
    /// ���ⲿͬ������״̬
    /// </summary>
    public void SyncHealthState(HealthState healthState)
    {
        CurrentHealthState = healthState;
    }

    /// <summary>
    /// ����ӵ�ǰ Value ˥����Ŀ����ֵ��Ҫ��ʱ�䣨Сʱ��
    /// </summary>
    public float GetTimeToReachThreshold(float threshold)
    {
        if (DecayRatePerHour <= 0 || Value <= threshold)
        {
            return -1f; // �޷��ﵽ���Ѿ�������ֵ
        }
        return (Value - threshold) / DecayRatePerHour;
    }
}



[Serializable]
public class StatusDescriptor
{
    [Tooltip("����֪ͨ���������ֽ���״̬����Ч")]
    public HealthState TargetHealthState = HealthState.Normal;

    [Tooltip("����ֵ���͵����ֵʱ������֪ͨ")]
    public float TriggerValue;

    [Tooltip("Ҫ���͵�֪ͨ��Ϣ������ʹ�� {characterName} �� {value} ռλ����")]
    [TextArea]
    public string Message;

    /// <summary>
    /// ��ʽ����Ϣ���滻ռλ��
    /// </summary>
    public string GetFormattedMessage(string characterName)
    {
        if (string.IsNullOrEmpty(Message)) return "";

        // ע�⣺��Ϊ����δ��֪ͨ����ʾ���Ǵ���ֵ�������ǵ�ǰֵ
        return Message.Replace("{characterName}", characterName)
                      .Replace("{value}", TriggerValue.ToString("F0"));
    }
}