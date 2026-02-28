using System;
using UnityEngine;
using UnityEngine.UI;

public class BottomButton : MonoBehaviour
{
    [SerializeField] private GameObject objectToHide;
    [SerializeField] private GameObject objectToShow;
    [SerializeField] private Button homeDepot;
    [SerializeField] private Button closet;
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
            homeDepot.interactable = true;
            closet.interactable = true;
        }
        else {
            objectToShow.SetActive(true);
            homeDepot.interactable = false;
            closet.interactable = false;
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
