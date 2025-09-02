using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public event Action<DateTime> OnMinuteChanged;
    public event Action<DateTime> OnHourChanged;
    public event Action<DateTime> OnDayChanged;

    private DateTime lastTime;
    private float minuteTimer = 0f; // 新增计时器

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        lastTime = DateTime.Now;
    }

    private void Update()
    {
        DateTime now = DateTime.Now;

        // 每0.2秒触发一次
        minuteTimer += Time.deltaTime;
        if (minuteTimer >= 5f)
        {
            OnMinuteChanged?.Invoke(now);
            minuteTimer = 0f;
        }

        if (now.Hour != lastTime.Hour)
            OnHourChanged?.Invoke(now);

        if (now.Day != lastTime.Day)
            OnDayChanged?.Invoke(now);

        lastTime = now;
    }
}
