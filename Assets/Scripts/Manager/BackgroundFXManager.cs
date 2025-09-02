using UnityEngine;
using UnityEngine.UI;

public class BackgroundFXManager : MonoBehaviour
{
    [SerializeField] private Image overlayImage; 
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
                break;

            case HealthState.Weak:
                SetOverlayColor(new Color(0, 0, 1f, 0.25f)); // 蓝色半透明
                break;

            case HealthState.Crazy:
                SetOverlayColor(new Color(1f, 0, 0, 0.25f)); // 红色半透明
                break;

            case HealthState.Dirty:
                SetOverlayColor(new Color(0.5f, 0.25f, 0f, 0.2f)); // 棕色，表示脏
                break;

            case HealthState.Dead:
                SetOverlayColor(new Color(0, 0, 0, 0.7f)); // 黑屏
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
