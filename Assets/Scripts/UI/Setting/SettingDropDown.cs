using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingDropDown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Button editButton;
    [SerializeField] private string settingKey;
    [SerializeField] private string inputDescription;
    [SerializeField] private string valueDescription;

    private bool isEditing = false;

    void Start()
    {
        dropdown.gameObject.SetActive(false);
        valueText.text = valueDescription + GameManager.Instance.GetSetting(settingKey);
        editButton.onClick.AddListener(OnEditButtonClicked);
    }

    void OnEditButtonClicked()
    {
        SoundManager.Instance.PlaySfx("Click");
        if (!isEditing)
        {
            isEditing = true;
            if (settingKey == "Model" &&
                AIDialogueProviders.TryGet(GameManager.Instance.StateManager.Settings.API, out var provider))
            {
                dropdown.ClearOptions();
                dropdown.AddOptions(new List<string>(provider.Models));
            }
            string currentValue = GameManager.Instance.GetSetting(settingKey);
            int selected = dropdown.options.FindIndex(option => option.text == currentValue);
            dropdown.SetValueWithoutNotify(selected >= 0 ? selected : 0);
            dropdown.RefreshShownValue();
            valueText.gameObject.SetActive(false);
            dropdown.gameObject.SetActive(true);

            editButton.GetComponentInChildren<TextMeshProUGUI>().text = "OK";
        }
        else
        {
            string newValue = dropdown.options[dropdown.value].text;
            OperationResult result = GameManager.Instance.ChangeSetting(settingKey, newValue);
            if (!result.Success)
            {
                ErrorNotifier.NotifyError(result.Message);
            }
            else
            {
                valueText.text = valueDescription + newValue;
                valueText.gameObject.SetActive(true);
                dropdown.gameObject.SetActive(false);
                editButton.GetComponentInChildren<TextMeshProUGUI>().text = "¸Ä";
                isEditing = false;
            }
        }
    }


}
