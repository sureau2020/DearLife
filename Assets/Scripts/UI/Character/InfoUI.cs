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
        statusText.text = "当前状态：" + GameManager.Instance.StateManager.Character.GetHealthStateDescription();
    }


    public void UpdateInfo()
    {
        CharacterData character = GameManager.Instance.StateManager.Character;
        nameText.text = "姓名：" + character.Name;
        statusText.text = "当前状态：" + character.GetHealthStateDescription();
        personalitiesText.text = "特点：" + character.GetPersonalityDescription();
        appelationText.text = "对你的称呼：" + character.PlayerAppellation;
        relationText.text = $"目前与你的关系：{character.Relationship}";
        deathTimeText.text = "死亡次数：" + character.DeathNum;
        meetTimeText.text = $"已经相伴{(DateTime.Now.Date - character.FirstStartTime.Date).Days}天";
    }

}
