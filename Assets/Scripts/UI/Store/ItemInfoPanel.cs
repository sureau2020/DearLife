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
    private int quantity;
    private int singlePrice;



    // �̵�ʹ��item��Ϣ������ʹ��itemID
    public OperationResult StoreShow(ItemData item)
    {
        quantity = minQuantity;
        itemPrice.gameObject.SetActive(true);
        try {
            singlePrice = item.Price;
            itemPrice.text = $" {singlePrice * quantity}���";
            itemName.text = item.Name;
            itemDescription.text = item.Description;
        }catch (System.Exception e) {
            return OperationResult.Fail($"��ʾ����Ʒ��Ϣʧ��,��Ʒ��Ϣ��ȫ");
        }
        InitializeSlider();
        okButton.GetComponent<TextMeshProUGUI>().text = "����";
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
        itemPrice.text = $" {singlePrice * this.quantity}���";
    }
}
