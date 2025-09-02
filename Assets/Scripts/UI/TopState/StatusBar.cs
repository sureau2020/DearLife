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

    public void UpdateLoveBar(int cur)
    {
        int level = cur >= 0 ? cur / maxValue : ((cur + 1) / maxValue) - 1;
        int remainder = ((cur % maxValue) + maxValue) % maxValue;
        fillImage.fillAmount = remainder / (float)maxValue;
        TMPro.TextMeshProUGUI levelText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        levelText.text = "Lv " + level.ToString();
    }
}
