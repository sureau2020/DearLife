
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChoicesUI : MonoBehaviour
{
    [SerializeField] private GameObject ChoicePrefab;
    [SerializeField] private Transform gridParent;
    public event Action<string> OnChoiceClicked;


    void OnEnable()
    {
        ChoicePrefab.GetComponent<ChoiceUI>().OnChoiceClicked += HandleChoiceClicked;
    }

    void OnDisable()
    {
        ChoicePrefab.GetComponent<ChoiceUI>().OnChoiceClicked -= HandleChoiceClicked;
    }

    public void GenerateChoices(List<ChoiceOption> choiceOptions) { 
        foreach (Transform child in gridParent) {
            Destroy(child.gameObject);
        }
        foreach (var option in choiceOptions) {
            GameObject choiceObj = Instantiate(ChoicePrefab, gridParent);
            ChoiceUI choiceUI = choiceObj.GetComponent<ChoiceUI>();
            choiceUI.SetChoice(option); 
        }
    }



    private void HandleChoiceClicked(string nextNodeId)
    {
        OnChoiceClicked?.Invoke(nextNodeId);
        gameObject.SetActive(false);
    }
}
