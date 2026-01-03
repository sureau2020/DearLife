using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomPanel : MonoBehaviour
{
    [SerializeField] private Transform buttons;
    [SerializeField] private Button wearButton;
    private GameObject customComponent;
    [SerializeField] private LoadHint loadHint;
    public float yOffset = 30f;
    public void Show(Vector2 pos, bool wearable, GameObject customComponent)
    {
        this.customComponent = customComponent;
        gameObject.SetActive(true);
        wearButton.interactable = wearable;
        buttons.transform.position = new Vector2(pos.x, pos.y + yOffset);
    }

    public void OpenLoadHint()
    {
        SoundManager.Instance.PlaySfx("LittleType");
        loadHint.OpenPanel(customComponent);
        gameObject.SetActive(false);
    }

    public void ClosePanel()
    {
        SoundManager.Instance.PlaySfx("Click");
        gameObject.SetActive(false);
    }

    public void WearCustomComponent()
    {
        customComponent.GetComponent<ApparenceComponent>().OnClick();
        gameObject.SetActive(false);
    }

}
