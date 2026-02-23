using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FurnishItemEvents
{
    public static System.Action<string, FurnishCategory, int> OnItemClicked;

    public static void TriggerItemClicked(string id, FurnishCategory category,int index)
    {
        OnItemClicked?.Invoke(id, category, index);
    }
}