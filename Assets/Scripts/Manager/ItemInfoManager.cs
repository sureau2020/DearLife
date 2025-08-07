using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoManager : MonoBehaviour
{
    public static ItemInfoManager Instance { get; private set; }

    [SerializeField] private ItemInfoPanel infoPanel;

    private void Awake()
    {
        Instance = this;
    }

    public OperationResult ShowBuyPanel(ItemData item)
    {
        infoPanel.gameObject.SetActive(true);
        return infoPanel.StoreShow(item);
    }

    public void HideBuyPanel()
    {
        infoPanel.gameObject.SetActive(false);
    }
}
