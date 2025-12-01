using System;
using System.Collections.Generic;
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
        var character = GameManager.Instance.StateManager.Character;
        var replacements = new Dictionary<string, string>
        {
            { "{characterName}", character.Name },
            { "{appellation}", character.PlayerAppellation },
            { "{playerAppellation}", character.PlayerAppellation },
            { "{characterPronoun}", character.Pronoun }
        };

        string text = choiceOption.Text;
        foreach (var pair in replacements)
        {
            text = text.Replace(pair.Key, pair.Value);
        }
        choiceText.text = text;
    }

    public void OnClickChoice()
    {
        SoundManager.Instance.PlaySfx("Click");
        OnChoiceClicked?.Invoke(nextNodeId);
    }

}
