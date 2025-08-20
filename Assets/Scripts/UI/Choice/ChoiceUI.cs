
using System;
using TMPro;
using UnityEngine;

public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI choiceText;
    private string nextNodeId;
    public event Action<string> OnChoiceClicked;

    public void SetChoice(ChoiceOption choiceOption)
    {
        nextNodeId = choiceOption.NextNodeId;
        if (choiceOption.Text.Contains("{characterName}"))
        {
            choiceText.text = choiceOption.Text.Replace("{characterName}", GameManager.Instance.StateManager.Character.Name);
        }
        else {
            choiceText.text = choiceOption.Text;
        }
    }

    public void OnClickChoice()
    {
        OnChoiceClicked?.Invoke(nextNodeId);
    }

}
