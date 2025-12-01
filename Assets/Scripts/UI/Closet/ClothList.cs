
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothList : MonoBehaviour
{
    [SerializeField] GameObject clothPrefab;
    [SerializeField] Transform content;
    [SerializeField] Button buyButton;
    [SerializeField] SpriteRenderer clothRenderer;
    [SerializeField] LoadHint uploadHintPanel;
    //private WardrobeSlot CurWardrobeSlot;
    private GameObject CurWardrobeSlotObj;
    private Sprite defaultClothSprite;
    private bool isChange = false;
    void OnEnable()
    {
        isChange = false;
        buyButton.gameObject.SetActive(false);
        uploadHintPanel.gameObject.SetActive(false);
        defaultClothSprite = GameManager.Instance.StateManager.Character.Appearance.ClothesId == 0 ? null : AppearanceAtlasManager.Instance.GetPartSprite("Clothes", GameManager.Instance.StateManager.Character.Appearance.ClothesId);
        //todo: curr default cloth
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        SetDefaultCloth();
        List<WardrobeSlot> clothes = WardrobeData.GetAllClothes();
        bool isEmpty = false;
        foreach (WardrobeSlot cloth in clothes)
        {
            GameObject clothItem = Instantiate(clothPrefab, content);
            ClothUI clothUI = clothItem.GetComponent<ClothUI>();

            if (clothUI != null)
            {
                //if (cloth.State == "Empty") isEmpty = true;
                if (cloth.State == "AddButton") isEmpty = true;
                clothUI.Init(cloth, OnClothItemClicked);
            }
        }
        if (!isEmpty)
            SetAddCloth();
    }

    private void OnClothItemClicked(GameObject slot)
    {
        uploadHintPanel.gameObject.SetActive(false);
        SoundManager.Instance.PlaySfx("Click");
        isChange = true;

        CurWardrobeSlotObj = slot;
        WardrobeSlot data = slot.GetComponent<ClothUI>().Data;
        if(data.State == "Empty")
        {
            uploadHintPanel.gameObject.SetActive(true);
            uploadHintPanel.Init(slot);
        }
        clothRenderer.enabled = true;
        clothRenderer.sprite = slot.GetComponent<ClothUI>().clothIcon.sprite;
        if (data.State == "Own" || data.State == "Empty")
        {
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            buyButton.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (CurWardrobeSlotObj == null || CurWardrobeSlotObj.GetComponent<ClothUI>().Data.State != "Own")
        {
            if (isChange) {
                clothRenderer.sprite = defaultClothSprite;
            }
        }
        else { 
            GameManager.Instance.StateManager.Character.Cloth = CurWardrobeSlotObj.GetComponent<ClothUI>().Data;
            GameManager.Instance.SaveGame();
        }
        
    }

    private void SetAddCloth() {
        GameObject addClothItem = Instantiate(clothPrefab, content);
        ClothUI clothUI = addClothItem.GetComponent<ClothUI>();
        if (clothUI != null)
            clothUI.CreateAddButton(OnClothItemClicked);
    }

    private void SetDefaultCloth() {
        GameObject clothItem = Instantiate(clothPrefab, content);
        ClothUI clothUI = clothItem.GetComponent<ClothUI>();
        if (clothUI != null)
            clothUI.CreateDefaultCloth(defaultClothSprite, OnClothItemClicked);
        
    }
   
    public void BuyCloth() { 
        WardrobeSlot CurWardrobeSlot = CurWardrobeSlotObj.GetComponent<ClothUI>().Data;
        OperationResult result = GameManager.Instance.StateManager.BuyCloth(CurWardrobeSlot.Price, CurWardrobeSlot);
        if (result.Success) {
            CurWardrobeSlotObj.transform.Find("money").gameObject.SetActive(false);
            CurWardrobeSlotObj.transform.Find("lock").gameObject.SetActive(false);
            SoundManager.Instance.PlaySfx("BuyItem");
            SaveManager.SaveWardrobe(WardrobeData.Slots);
            buyButton.gameObject.SetActive(false);
            if (!CurWardrobeSlot.IsBuiltIn) {
                SetAddCloth();
            }
        } else {
            ErrorNotifier.NotifyError(result.Message);
        }
    }
}
