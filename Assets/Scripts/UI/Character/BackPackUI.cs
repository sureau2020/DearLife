using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BackPackUI : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab; 
    private StateManager stateManager;

    public void GenerateItems(List<ItemEntryData> items)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in items)
        {
            bool flowControl = TryCreateSingleItem(item);
            if (!flowControl)
            {
                return;
            }

        }
    }

    private bool TryCreateSingleItem(ItemEntryData item)
    {
        ItemData data = ItemDataBase.GetItemById(item.ItemID);
        if (data == null)
        {
            ErrorNotifier.NotifyError($"物品图鉴里找不到ID为{item.ItemID}的物品，请检查是否物品图鉴是否缺失。");
            return false;
        }
        GameObject go = Instantiate(itemPrefab, transform);
        go.GetComponent<ItemUi>().SetItemId(item.ItemID);
        go.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = data.Name;
        go.transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = item.Count.ToString();
        return true;
    }

 
     void OnEnable()
    {
        RefreshBackPackUI();
    }

    public void RefreshBackPackUI()
    {
        stateManager = GameManager.Instance.StateManager;
        GenerateItems(stateManager.GetAllItemsInBackPack());
    }
}
