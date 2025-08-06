using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreButton : MonoBehaviour
{
    private GameObject Store;
    private GameObject Task;
    // Start is called before the first frame update
    void Start()
    {
        Store = GameObject.Find("UI").transform.Find("StoreUI").gameObject;
        Task = GameObject.Find("UI").transform.Find("TaskUI").gameObject;
    }

    public void Click() { 
        HideTask();
        Toggle();
    }

    public void Toggle()
    {
        Store.SetActive(!Store.activeSelf);
    }

    public void HideTask() { 
        Task.SetActive(false);
    }
}
