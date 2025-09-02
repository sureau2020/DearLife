using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button editButton;
    [SerializeField] private string settingKey;
    [SerializeField] private string inputDescription;
    [SerializeField] private string valueDescription;

    private bool isEditing = false;

    void Start()
    {
        inputField.gameObject.SetActive(false);
        valueText.text = valueDescription + GameManager.Instance.GetSetting(settingKey);
        editButton.onClick.AddListener(OnEditButtonClicked);
    }

    void OnEditButtonClicked()
    {
        SoundManager.Instance.PlaySfx("Click");
        if (!isEditing)
        {
            isEditing = true;
            inputField.text = "";
            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = inputDescription;
            valueText.gameObject.SetActive(false);
            inputField.gameObject.SetActive(true);

            editButton.GetComponentInChildren<TextMeshProUGUI>().text = "OK";
        }
        else
        {
            string newValue = inputField.text;
            OperationResult result = GameManager.Instance.ChangeSetting(settingKey, newValue);
            if (!result.Success)
            {
                ErrorNotifier.NotifyError(result.Message);
            }
            else
            {
                valueText.text = valueDescription + newValue;
                valueText.gameObject.SetActive(true);
                inputField.gameObject.SetActive(false);
                editButton.GetComponentInChildren<TextMeshProUGUI>().text = "¸Ä";
                isEditing = false;
            }
        }
    }


    
}
