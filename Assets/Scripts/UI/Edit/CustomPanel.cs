using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPanel : MonoBehaviour
{
    [SerializeField] private Transform buttons;
    public float yOffset = 30f;
    public void Show(Vector2 pos)
    {
        // Implementation for showing the custom panel
        gameObject.SetActive(true);
        buttons.transform.position = new Vector2(pos.x, pos.y + yOffset);
        // Additional logic based on type and id can be added here
    }


    public void ClosePanel()
    {
        SoundManager.Instance.PlaySfx("Click");
        gameObject.SetActive(false);
    }
}
