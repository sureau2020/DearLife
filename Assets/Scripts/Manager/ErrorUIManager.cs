using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorUIManager : MonoBehaviour
{
    private GameObject errorPanel;

    void Start()
    {
        errorPanel = GameObject.Find("UI").transform.Find("ErrorPanel").gameObject;
    }

    void OnEnable()
    {
        ErrorNotifier.OnError += HandleError;
    }

    void OnDisable()
    {
        ErrorNotifier.OnError -= HandleError;
    }

    private void HandleError(string message)
    {
        errorPanel.SetActive(true);
        ErrorUI errorUI = errorPanel.GetComponent<ErrorUI>();
        errorUI.ShowError(message);
    }
}
