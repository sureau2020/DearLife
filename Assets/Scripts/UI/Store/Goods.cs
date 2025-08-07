using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Goods : MonoBehaviour
{
    private ItemData itemData;
    private TextMeshProUGUI itemName;

    public void SetItemData(ItemData item)
    {
        itemData = item;
    }

    public void ShowInfo()
    {
        itemName = GetComponentInChildren<TextMeshProUGUI>();
        itemName.text = itemData.Name;
    }

 
    public void OnClick()
    {
       OperationResult isShow = ItemInfoManager.Instance.ShowBuyPanel(itemData);
       if (!isShow.Success)
       {
           ErrorNotifier.NotifyError(isShow.Message);
        }
    }
}
