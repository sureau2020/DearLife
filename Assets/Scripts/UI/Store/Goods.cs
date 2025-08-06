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
        itemName = transform.GetComponent<TextMeshProUGUI>();
        itemName.text = itemData.Name;
    }

}
