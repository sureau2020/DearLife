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
            ErrorNotifier.NotifyError($"��Ʒͼ�����Ҳ���IDΪ{item.ItemID}����Ʒ�������Ƿ���Ʒͼ���Ƿ�ȱʧ��");
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
