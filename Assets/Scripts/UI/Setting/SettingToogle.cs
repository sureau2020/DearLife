using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingToogle : MonoBehaviour
{
    [SerializeField] private Toggle toogle;

    void OnEnable()
    {
        toogle.isOn = GameManager.Instance.GetSetting("isAIEnabled") == "True";
    }

    public void OnToogleValueChanged(bool value)
    {
        SoundManager.Instance.PlaySfx("Click");
        string newValue = value ? "true" : "false";
        OperationResult result = GameManager.Instance.ChangeSetting("isAIEnabled", newValue);
        if (!result.Success)
        {
            ErrorNotifier.NotifyError(result.Message);
            toogle.isOn = !value; 
        }
    }
}
