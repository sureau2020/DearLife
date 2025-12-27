using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptUIManager : MonoBehaviour
{
    [SerializeField] private ErrorUI errorUI;
    [SerializeField] private ConfirmPanel confirmPanel;


    void OnEnable()
    {
        ErrorNotifier.OnError += HandleError;
        ConfirmRequestManager.OnRequestConfirm += HandleRequest;
    }

    void OnDisable()
    {
        ErrorNotifier.OnError -= HandleError;
        ConfirmRequestManager.OnRequestConfirm -= HandleRequest;
    }

    private void HandleError(string message)
    {
        errorUI.gameObject.SetActive(true);
        errorUI.ShowError(message);
    }

    private void HandleRequest(ConfirmData data)
    {
        confirmPanel.gameObject.SetActive(true);
        confirmPanel.Show(data);
    }
}
