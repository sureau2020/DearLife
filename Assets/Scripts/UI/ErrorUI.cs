using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorUI : MonoBehaviour
{
    private TextMeshProUGUI errorText;


    public void ShowError(string message)
    {
        errorText = transform.Find("ErrorMessage").GetComponent<TextMeshProUGUI>();
        errorText.text = message;
    }


}
