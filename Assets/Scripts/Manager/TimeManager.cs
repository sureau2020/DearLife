using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public event Action<DateTime> OnMinuteChanged;
    public event Action<DateTime> OnHourChanged;
    public event Action<DateTime> OnDayChanged;

    private DateTime lastTime;
    private DateTime nextMinuteTick;           // ��һ�Ρ������¼�������ʱ��
    private const int MinuteInterval = 12;     // ÿ12���Ӵ���һ��

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        lastTime = DateTime.Now;
        nextMinuteTick = lastTime.AddMinutes(MinuteInterval);
    }

    private void Update()
    {
        DateTime now = DateTime.Now;

        // ÿ14���Ӵ���һ�Ρ����ӱ仯���¼�
        while (now >= nextMinuteTick)
        {
            // �����Ҫ���� now
            OnMinuteChanged?.Invoke(now);
            nextMinuteTick = nextMinuteTick.AddMinutes(MinuteInterval);
        }

        //// Сʱ�仯
        //if (now.Hour != lastTime.Hour)
        //    OnHourChanged?.Invoke(now);

        //// ��仯
        //if (now.Day != lastTime.Day)
        //    OnDayChanged?.Invoke(now);

        lastTime = now;
    }
}
