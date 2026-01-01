using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomPanel : MonoBehaviour
{
    [SerializeField] private Transform buttons;
    [SerializeField] private Button wearButton;
    public float yOffset = 30f;
    public void Show(Vector2 pos, bool wearable)
    {
        // Implementation for showing the custom panel
        gameObject.SetActive(true);
        wearButton.interactable = wearable;
        buttons.transform.position = new Vector2(pos.x, pos.y + yOffset);
        
    }


    public void ClosePanel()
    {
        SoundManager.Instance.PlaySfx("Click");
        gameObject.SetActive(false);
    }
}
