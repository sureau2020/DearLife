using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public event Action<DateTime> OnMinuteChanged;
    public event Action<DateTime> OnHourChanged;
    public event Action<DateTime> OnDayChanged;

    private DateTime lastTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        lastTime = DateTime.Now;
    }

    private void Update()
    {
        DateTime now = DateTime.Now;

        // 每秒检查一次时间变化
        if (now.Second != lastTime.Second)
        {
            OnMinuteChanged?.Invoke(now);
        }

        //if (now.Minute != lastTime.Minute) {
        //    OnMinuteChanged?.Invoke(now);
        //}

        if (now.Hour != lastTime.Hour)
            OnHourChanged?.Invoke(now);

        if (now.Day != lastTime.Day)
            OnDayChanged?.Invoke(now);

        lastTime = now;
    }
}
