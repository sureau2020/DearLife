using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [Range(0, 100)]
    public int value;
    public int maxValue = CharacterData.MaxVar; 

    public void UpdateBar(int current)
    {
        value = Mathf.Clamp(current, 0, maxValue);
        fillImage.fillAmount = (float)value / maxValue;
    }
}
