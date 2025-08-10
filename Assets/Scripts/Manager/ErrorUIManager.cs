using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorUIManager : MonoBehaviour
{
    [SerializeField] private ErrorUI errorUI;


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
        errorUI.gameObject.SetActive(true);
        errorUI.ShowError(message);
    }
}
