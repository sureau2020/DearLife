using TMPro;
using UnityEngine;

public class ErrorUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorText;


    public void ShowError(string message)
    {
        errorText.text = message;
    }

    public void HideError()
    {
        gameObject.SetActive(false);
    }


}
