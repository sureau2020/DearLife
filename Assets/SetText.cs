using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetText : MonoBehaviour
{
    public void SetTextValue(float value)
    {
        value = Mathf.Round(value * 100) / 100f; 
        var textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {

            textComponent.text = value.ToString();

        }
    }


}
