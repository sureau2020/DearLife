using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundFXManager : MonoBehaviour
{
    [SerializeField] private Image overlayImage;
    [SerializeField] private TextMeshProUGUI statusText;
    private CharacterData character;

    private void Start()
    {
        character = GameManager.Instance.StateManager.Character;
        character.OnHealthChanged += ApplyEffect;
        ApplyEffect(); 
    }

    private void OnDestroy()
    {
        character.OnHealthChanged -= ApplyEffect;
    }

    private void ApplyEffect()
    {
        // 根据角色当前状态来调色
        switch (character.HealthState)
        {
            case HealthState.Normal:
                SetOverlayColor(new Color(0, 0, 0, 0)); 
                statusText.text = "";
                break;

            case HealthState.Weak:
                SetOverlayColor(new Color(0, 0, 1f, 0.25f)); // 蓝色半透明
                statusText.text = $"<wave>{character.Name}<br>陷入了<br><color=#000000><shake>极度饥饿状态<@shake><@wave>";
                break;

            case HealthState.Crazy:
                SetOverlayColor(new Color(1f, 0, 0, 0.25f)); // 红色半透明
                statusText.text = $"<wave>{character.Name}<br>陷入了<br><color=#FF0000><shake>疯狂状态<@shake><@wave>";
                break;

            case HealthState.Dirty:
                SetOverlayColor(new Color(0.5f, 0.25f, 0f, 0.2f)); // 棕色，表示脏
                statusText.text = $"<wave>{character.Name}<br><color=#FFFFFF><shake>急需清洁<@shake><@wave>";
                break;

            case HealthState.Dead:
                SetOverlayColor(new Color(0, 0, 0, 0.7f)); // 黑屏
                statusText.text = "";
                break;
        }
    }

    private void SetOverlayColor(Color color)
    {
        if (overlayImage != null)
        {
            overlayImage.color = color;
        }
    }
}
