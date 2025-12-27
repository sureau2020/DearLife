using System;
using UnityEngine;

public class ConfirmRequestManager : MonoBehaviour
{
    public static Action<ConfirmData> OnRequestConfirm;

    public static void Request(

        string content,
        Action onConfirm,
        Action onCancel = null)
    {
        OnRequestConfirm?.Invoke(new ConfirmData
        {
            content = content,
            onConfirm = onConfirm,
            onCancel = onCancel
        });
    }
}


public struct ConfirmData
{
    public string content;
    public Action onConfirm;
    public Action onCancel;
}