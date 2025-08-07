using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    private const int maxQuantity = 9;
    private const int minQuantity = 1;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private Button okButton;
    [SerializeField] private Slider quantitySlider;
    [SerializeField] private TextMeshProUGUI quantityText;
    ItemData itemData;
    private int quantity;
    private int singlePrice;



    // 商店使用item信息，背包使用itemID
    public OperationResult StoreShow(ItemData item)
    {
        quantity = minQuantity;
        itemPrice.gameObject.SetActive(true);
        try {
            itemData = item;
            singlePrice = item.Price;
            itemPrice.text = $" {singlePrice * quantity}金币";
            itemName.text = item.Name;
            itemDescription.text = item.Description;
        }catch (System.Exception e) {
            return OperationResult.Fail($"显示该物品信息失败,物品信息不全");
        }
        InitializeSlider();
        okButton.GetComponentInChildren<TextMeshProUGUI>().text = "购买";
        return OperationResult.Complete();
    }

    private void InitializeSlider()
    {
        quantitySlider.onValueChanged.RemoveAllListeners(); 
        quantitySlider.maxValue = maxQuantity;
        quantitySlider.minValue = minQuantity;
        quantitySlider.value = minQuantity;
        UpdateText((int)quantitySlider.value);

        quantitySlider.onValueChanged.AddListener(value => UpdateText((int)value));
    }

    private void UpdateText(int quantity)
    {
        this.quantity = Mathf.Clamp(quantity, minQuantity, maxQuantity); 
        quantityText.text = this.quantity.ToString();
        itemPrice.text = $" {singlePrice * this.quantity}金币";
    }

    public void OnOkButtonClick()
    {
        Debug.Log($"购买物品: {itemData.Name}, 数量: {quantity}, 单价: {singlePrice}");
        OperationResult result = GameManager.Instance.StateManager.BuyItem(singlePrice,itemData, quantity);
        if (!result.Success)
        {
            ErrorNotifier.NotifyError(result.Message);
        }
        else
        {
            HidePanel();
        }
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
