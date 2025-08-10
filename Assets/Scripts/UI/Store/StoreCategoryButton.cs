using UnityEngine;

public class StoreCategoryButton : MonoBehaviour
{
    [SerializeField] private GameObject goodsList;
    [SerializeField] private ItemType itemType;

    public void OnClick()
    {
        goodsList.GetComponent<ItemList>().GenerateAllGoodsByType(itemType);
    }
}