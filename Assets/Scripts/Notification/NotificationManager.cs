using UnityEngine;
using Unity.Notifications.Android;
//using Unity.Notifications.iOS;
using System;

public static class NotificationManager
{
    private const string ChannelId = "default_channel";

    static NotificationManager()
    {
#if UNITY_ANDROID
        // ³õÊ¼»¯ Android ÇþµÀ
        var channel = new AndroidNotificationChannel()
        {
            Id = ChannelId,
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Game notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
    }

    public static void ScheduleNotification(string title, string body, float hoursLater, string identifier)
    {
#if UNITY_ANDROID
        var notification = new AndroidNotification()
        {
            Title = title,
            Text = body,
            FireTime = DateTime.Now.AddHours(hoursLater),
        };

        AndroidNotificationCenter.SendNotificationWithExplicitID(
            notification, ChannelId, identifier.GetHashCode()
        );

//#elif UNITY_IOS
//        var trigger = new iOSNotificationTimeIntervalTrigger()
//        {
//            TimeInterval = TimeSpan.FromHours(hoursLater),
//            Repeats = false
//        };

//        var notification = new iOSNotification()
//        {
//            Identifier = identifier,
//            Title = title,
//            Body = body,
//            ShowInForeground = true,
//            ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
//            Trigger = trigger
//        };

//        iOSNotificationCenter.ScheduleNotification(notification);
#endif
    }

    public static void CancelAll()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllScheduledNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
    }
}
