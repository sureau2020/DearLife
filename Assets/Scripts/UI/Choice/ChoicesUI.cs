using System;
using System.Collections.Generic;
using UnityEngine;

public class ChoicesUI : MonoBehaviour
{
    [SerializeField] private GameObject ChoicePrefab;
    [SerializeField] private Transform gridParent;
    public event Action<string> OnChoiceClicked;


    public void GenerateChoices(List<ChoiceOption> choiceOptions) { 
        foreach (Transform child in gridParent) {
            var choiceUI = child.GetComponent<ChoiceUI>();
            if (choiceUI != null)
                choiceUI.OnChoiceClicked -= HandleChoiceClicked;
            Destroy(child.gameObject);
        }
        foreach (var option in choiceOptions) {
            GameObject choiceObj = Instantiate(ChoicePrefab, gridParent);
            ChoiceUI choiceUI = choiceObj.GetComponent<ChoiceUI>();
            choiceUI.SetChoice(option); 
            choiceUI.OnChoiceClicked += HandleChoiceClicked;
        }
    }



    private void HandleChoiceClicked(string nextNodeId)
    {
        OnChoiceClicked?.Invoke(nextNodeId);
        gameObject.SetActive(false);
    }
}
