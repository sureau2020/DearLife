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

    public void ShowBuyPanel(ItemData item)
    {
        infoPanel.gameObject.SetActive(true);
        infoPanel.Show(item);
    }

    public void HideBuyPanel()
    {
        infoPanel.gameObject.SetActive(false);
    }
}
