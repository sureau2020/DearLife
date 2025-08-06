using System;

public static class ErrorNotifier
{
    public static event Action<string> OnError;

    public static void NotifyError(string message)
    {
        OnError?.Invoke(message);
    }
}