using System;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI personalitiesText;
    [SerializeField] private TextMeshProUGUI appelationText;
    [SerializeField] private TextMeshProUGUI relationText;
    [SerializeField] private TextMeshProUGUI deathTimeText;
    [SerializeField] private TextMeshProUGUI meetTimeText;


    void OnEnable()
    {
        UpdateInfo();
        GameManager.Instance.StateManager.Character.OnHealthChanged += HealthChangeHandle;
    }

    private void OnDestroy()
    {
        GameManager.Instance.StateManager.Character.OnHealthChanged -= HealthChangeHandle;
    }


    public void HealthChangeHandle() {
        statusText.text = "��ǰ״̬��" + GameManager.Instance.StateManager.Character.GetHealthStateDescription();
    }


    public void UpdateInfo()
    {
        CharacterData character = GameManager.Instance.StateManager.Character;
        nameText.text = "������" + character.Name;
        statusText.text = "��ǰ״̬��" + character.GetHealthStateDescription();
        personalitiesText.text = "�ص㣺" + character.GetPersonalityDescription();
        appelationText.text = "����ĳƺ���" + character.PlayerAppellation;
        relationText.text = $"Ŀǰ����Ĺ�ϵ��{character.Relationship}";
        deathTimeText.text = "����������" + character.DeathNum;
        meetTimeText.text = $"�Ѿ����{(DateTime.Now.Date - character.FirstStartTime.Date).Days}��";
    }

}
