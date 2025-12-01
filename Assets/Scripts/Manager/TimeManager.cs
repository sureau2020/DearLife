using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public event Action<DateTime> OnMinuteChanged;
    public event Action<DateTime> OnHourChanged;
    public event Action<DateTime> OnDayChanged;

    private DateTime lastTime;
    private DateTime nextMinuteTick;           // 下一次“分钟事件”触发时间
    private const int MinuteInterval = 12;     // 每12分钟触发一次
    //private float secondTimer = 0f;

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

        while (now >= nextMinuteTick)
        {
            // 按你的要求传入 now
            OnMinuteChanged?.Invoke(now);
            nextMinuteTick = nextMinuteTick.AddMinutes(MinuteInterval);
        }

        //secondTimer += Time.deltaTime;
        //if (secondTimer >= 0.1f)
        //{
        //    OnMinuteChanged?.Invoke(now);
        //    secondTimer = 0f;
        //}

        //// 小时变化
        //if (now.Hour != lastTime.Hour)
        //    OnHourChanged?.Invoke(now);

        //// 天变化
        //if (now.Day != lastTime.Day)
        //    OnDayChanged?.Invoke(now);

        lastTime = now;
    }
}
