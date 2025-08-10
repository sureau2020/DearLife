using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomButton : MonoBehaviour
{
    [SerializeField] private GameObject objectToHide;
    [SerializeField] private GameObject objectToShow;

    public void Click() { 
        HideOther();
        Toggle();
    }

    public void Toggle()
    {
        objectToShow.SetActive(!objectToShow.activeSelf);
    }

    public void HideOther() { 
        objectToHide.SetActive(false);
    }
}
