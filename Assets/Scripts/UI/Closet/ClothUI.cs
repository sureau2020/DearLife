using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ClothUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private Image lockHint;
    public Image clothIcon;
    public WardrobeSlot Data { get; private set; }
    private Button btn;
    private System.Action<GameObject> onClick;


    public void CreateDefaultCloth(Sprite icon, System.Action<GameObject> onClickCallback) {
        Data = new WardrobeSlot("Own", true, GameManager.Instance.StateManager.Character.Appearance.ClothesId.ToString(), 0);
        onClick = onClickCallback;
        if (btn == null) btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }
        lockHint.gameObject.SetActive(false);
        price.gameObject.SetActive(false);
        clothIcon.sprite = icon;
    }

    public void CreateAddButton(System.Action<GameObject> onClickCallback) {
        Data = new WardrobeSlot("AddButton", false, DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"), 99);
        WardrobeData.AddCloth(Data);
        onClick = onClickCallback;
        if (btn == null) btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }
        lockHint.gameObject.SetActive(true);
        price.gameObject.SetActive(true);
        price.text = "99";
        clothIcon.sprite = null;
        clothIcon.color = new Color(1, 1, 1, 0); // È«Í¸Ã÷
    }

    public void UploadCloth(Sprite cloth) { 
        clothIcon.color = new Color(1, 1, 1, 1);
        clothIcon.sprite = cloth;
        Data.State = "Own";
        onClick?.Invoke(gameObject);
        SaveManager.SaveCustomClothSprite(cloth,Data.Id);
        WardrobeData.AddCloth(Data);
        SaveManager.SaveWardrobe(WardrobeData.Slots);
    }


    public void Init(WardrobeSlot data, System.Action<GameObject> onClickCallback)
    {
        Data = data;
        onClick = onClickCallback;
        if (btn == null) btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }
        if (data.State == "Locked" || data.State == "AddButton")
        {
            price.text = data.Price.ToString();
            lockHint.gameObject.SetActive(true);
        }
        else
        {
            price.gameObject.SetActive(false);
            lockHint.gameObject.SetActive(false);
        }
        SetClothSprite();
    }

    private void SetClothSprite()
    {
        Sprite icon = null;
        if (Data.State == "Empty" || Data.State == "AddButton") {
            clothIcon.sprite = null;
            clothIcon.color = new Color(1, 1, 1, 0); 
        }
        else if (Data.IsBuiltIn)
        {
            icon = IconManager.GetClothIcon(Data.Id);
        }
        else {
            icon = SaveManager.LoadCustomClothSprite(Data.Id);
        }
        if (icon != null)
            clothIcon.sprite = icon;
    }

    private void OnClick()
    {
        onClick?.Invoke(gameObject);
    }
}