
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Goods : MonoBehaviour
{
    private ItemData itemData;
    [SerializeField] private Image itemIcon;

    public void SetItemData(ItemData item)
    {
        itemData = item;
    }

    public void ShowInfo()
    {
        Sprite icon = IconManager.GetIcon(itemData.Type, itemData.ImagePath);
        if (icon != null)
            itemIcon.sprite = icon;
    }

 
    public void OnClick()
    {
        SoundManager.Instance.PlaySfx("PickItem");
        OperationResult isShow = ItemInfoManager.Instance.ShowBuyPanel(itemData);
       if (!isShow.Success)
       {
           ErrorNotifier.NotifyError(isShow.Message);
        }
    }
}
