using System;
using UnityEngine;
using System.Collections;

#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif


public static class NotificationManager
{
    private const string ChannelId = "default_channel"; // Android 专用
    private static bool s_Initialized;

    // 稳定哈希（FNV-1a 32-bit），跨会话一致
    public static int StableId(string key)
    {
        unchecked
        {
            const int p = 16777619;
            int hash = (int)2166136261;
            for (int i = 0; i < key.Length; i++)
            {
                hash ^= key[i];
                hash *= p;
            }
            return hash == 0 ? 1 : hash; // 避免0
        }
    }

    public static void Initialize(bool resetOnLaunch = true)
    {
#if UNITY_ANDROID
        if (s_Initialized) return;

        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

        var channel = new AndroidNotificationChannel()
        {
            Id = ChannelId,
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Game notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        if (resetOnLaunch)
        {
            CancelAllScheduled();
            CancelAllDelivered();
        }

        s_Initialized = true;

#elif UNITY_IOS
    if (s_Initialized) return;

    s_Initialized = true; // 先标记初始化，防止重复调用

    // 请求 iOS 通知权限（2.4.1 新版 API）
    var authOptions = AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound;
    var request = new AuthorizationRequest(authOptions, true);

        // 启动协程等待授权完成
        CoroutineRunner.Run(WaitForAuthorization(request, resetOnLaunch));


#endif
    }

#if UNITY_IOS
    private static IEnumerator WaitForAuthorization(AuthorizationRequest request, bool resetOnLaunch)
    {
        while (!request.IsFinished)
        {
            yield return null;
        }

        Debug.Log($"iOS 通知授权完成 → granted: {request.Granted}, deviceToken: {request.DeviceToken}, error: {request.Error}");

        if (resetOnLaunch)
        {
            CancelAllScheduled();
            CancelAllDelivered();
        }
    }
#endif

    public static void ScheduleNotification(string title, string body, float hoursLater, string identifier)
    {
#if UNITY_ANDROID
        if (!s_Initialized) Initialize();
        if (hoursLater < 0f) return;

        var fireTime = EnsureFutureTime(DateTime.Now.AddHours(hoursLater), 5);

        var notification = new AndroidNotification()
        {
            Title = title,
            Text = body,
            FireTime = fireTime,
        };

        int id = StableId(identifier);
        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, ChannelId, id);

#elif UNITY_IOS
        if (!s_Initialized) Initialize();
        if (hoursLater < 0f) return;

        var trigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = TimeSpan.FromHours(hoursLater),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            Identifier = identifier,
            Title = title,
            Body = body,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            Trigger = trigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
#endif
    }

    public static void ScheduleAt(string title, string body, DateTime fireAt, string identifier)
    {
#if UNITY_ANDROID
        if (!s_Initialized) Initialize();

        var safeTime = EnsureFutureTime(fireAt, 5);
        var notification = new AndroidNotification()
        {
            Title = title,
            Text = body,
            FireTime = safeTime
        };
        int id = StableId(identifier);
        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, ChannelId, id);

#elif UNITY_IOS
        if (!s_Initialized) Initialize();

        var now = DateTime.Now;
        var interval = fireAt > now ? fireAt - now : TimeSpan.FromSeconds(5);

        var trigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = interval,
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            Identifier = identifier,
            Title = title,
            Body = body,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            Trigger = trigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
#endif
    }

    public static void CancelByIdentifier(string identifier)
    {
#if UNITY_ANDROID
        int id = StableId(identifier);
        AndroidNotificationCenter.CancelScheduledNotification(id);
        AndroidNotificationCenter.CancelDisplayedNotification(id);

#elif UNITY_IOS
        iOSNotificationCenter.RemoveScheduledNotification(identifier);
        iOSNotificationCenter.RemoveDeliveredNotification(identifier);
#endif
    }

    public static void CancelAllScheduled()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllScheduledNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
    }

    public static void CancelAllDelivered()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
#endif
    }

    public static void CancelAll()
    {
#if UNITY_ANDROID
        CancelAllScheduled();
        CancelAllDelivered();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
#endif
    }

#if UNITY_ANDROID
    private static DateTime EnsureFutureTime(DateTime time, int minDelaySeconds)
    {
        var min = DateTime.Now.AddSeconds(minDelaySeconds);
        return time <= min ? min : time;
    }
#endif
}
