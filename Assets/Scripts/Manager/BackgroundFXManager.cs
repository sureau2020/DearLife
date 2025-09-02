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
        // ���ݽ�ɫ��ǰ״̬����ɫ
        switch (character.HealthState)
        {
            case HealthState.Normal:
                SetOverlayColor(new Color(0, 0, 0, 0)); 
                break;

            case HealthState.Weak:
                SetOverlayColor(new Color(0, 0, 1f, 0.25f)); // ��ɫ��͸��
                break;

            case HealthState.Crazy:
                SetOverlayColor(new Color(1f, 0, 0, 0.25f)); // ��ɫ��͸��
                break;

            case HealthState.Dirty:
                SetOverlayColor(new Color(0.5f, 0.25f, 0f, 0.2f)); // ��ɫ����ʾ��
                break;

            case HealthState.Dead:
                SetOverlayColor(new Color(0, 0, 0, 0.7f)); // ����
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
