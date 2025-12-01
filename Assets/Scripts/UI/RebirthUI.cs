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

    public void ShowRebirthUI()
    {

        rebirthCostText.text = $"{character.Name}已经死亡\n这是{character.Pronoun}的\n第{character.DeathNum}次死亡\n在死亡时，\n你们的关系是{character.DetermineRelation()}\n你可以随时点击按钮复活{character.Name}\n但{character.Pronoun}不会忘记这次死亡。\n（好感度-200）";
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
