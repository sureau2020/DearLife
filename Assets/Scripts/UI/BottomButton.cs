using System;
using UnityEngine;

public class BottomButton : MonoBehaviour
{
    [SerializeField] private GameObject objectToHide;
    [SerializeField] private GameObject objectToShow;
    public event Action randomDailyEvent;

    public void Click() { 
        HideOther();
        Toggle();
    }

    public void Toggle()
    {
        if (objectToShow.activeSelf)
        {
            objectToShow.GetComponent<AnimationWhenDisable>()?.HideAndDisable();
        }
        else {
            objectToShow.SetActive(true);
        }
    }

    public void HideOther() { 
        objectToHide.SetActive(false);
    }

    public void ShowRandomDailyEvent()
    {
        randomDailyEvent?.Invoke();
    }
}
