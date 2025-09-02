using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RebirthUI : MonoBehaviour
{
    [SerializeField] private Button rebirthButton;
    [SerializeField] private Button obstacle;
    [SerializeField] private TextMeshProUGUI rebirthCostText;
    private CharacterData character;

    void Start()
    {
        character = GameManager.Instance.StateManager.Character;
        character.OnHealthChanged += UpdateRebirthButton;
        rebirthButton.onClick.AddListener(OnRebirthButtonClicked);
    }

    void OnDestroy()
    {
        character.OnHealthChanged -= UpdateRebirthButton;
    }

    public void UpdateRebirthButton()
    {
        if (character.HealthState == HealthState.Dead)
        {
            ShowRebirthUI();
            
        }
    }

    private void ShowRebirthUI()
    {

        rebirthCostText.text = $"{character.Name}�Ѿ�����\n����{character.Pronoun}��\n��{character.DeathNum}������\n������ʱ��\n���ǵĹ�ϵ��{character.DetermineRelation()}\n�������ʱ�����ť����{character.Name}\n��{character.Pronoun}�����������������\n���øж�-200��";
        obstacle.gameObject.SetActive(true);
        rebirthCostText.gameObject.SetActive(true);
        rebirthButton.gameObject.SetActive(true);
    }

    private void OnRebirthButtonClicked()
    {
        character.Rebirth();
        obstacle.gameObject.SetActive(false);
        rebirthCostText.gameObject.SetActive(false);
        rebirthButton.gameObject.SetActive(false);
    }


}
