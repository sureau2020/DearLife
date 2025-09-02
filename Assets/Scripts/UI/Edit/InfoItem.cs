using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button editButton;
    [SerializeField] private string infoKey;

    private bool isEditing = false;

    void Start()
    {
        inputField.gameObject.SetActive(false);
        valueText.text = BootSceneManager.Instance.GetInfo(infoKey);
        editButton.onClick.AddListener(OnEditButtonClicked);
    }

    void OnEditButtonClicked()
    {
        SoundManager.Instance.PlaySfx("Click");
        if (!isEditing)
        {
            isEditing = true;
            inputField.text = "";
            valueText.gameObject.SetActive(false);
            inputField.gameObject.SetActive(true);

            editButton.GetComponentInChildren<TextMeshProUGUI>().text = "»∑»œ";
        }
        else
        {
            string newValue = inputField.text;
            if (!string.IsNullOrWhiteSpace(newValue))
            {
                BootSceneManager.Instance.SetInfo(infoKey, newValue);
                valueText.text = newValue;
            }
            valueText.gameObject.SetActive(true);
            inputField.gameObject.SetActive(false);
            editButton.GetComponentInChildren<TextMeshProUGUI>().text = "±‡º≠";
            isEditing = false;
            
        }
    }


    
}
