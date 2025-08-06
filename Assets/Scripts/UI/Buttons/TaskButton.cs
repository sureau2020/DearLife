using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskButton : MonoBehaviour
{
    private GameObject Store;
    private GameObject Task;
    void Start()
    {
        Store = GameObject.Find("UI").transform.Find("StoreUI").gameObject;
        Task = GameObject.Find("UI").transform.Find("TaskUI").gameObject;
    }

    public void Click()
    {
        HideStore();
        Toggle();
    }

    public void Toggle()
    {
        Task.SetActive(!Task.activeSelf);
    }

    public void HideStore()
    {
        Store.SetActive(false);
    }
}
