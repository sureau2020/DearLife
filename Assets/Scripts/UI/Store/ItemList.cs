
using UnityEngine;

public class ItemList : MonoBehaviour
{
    [SerializeField] private GameObject goodsPrefab;


    void Start()
    {
        GenerateAllGoodsByType(ItemType.Food);
    }

 


    public void GenerateAllGoodsByType(ItemType type)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        var items = ItemDataBase.GetItemsByType(type);
        foreach (var item in items)
        {
            GameObject go = Instantiate(goodsPrefab, transform);
            Goods g = go.GetComponent<Goods>();
            g.SetItemData(item);

            // 数据校验
            if (item == null || string.IsNullOrEmpty(item.Name))
            {
                ErrorNotifier.NotifyError("疑似数据库商品数据有缺失，检查该类别未被加载出来的第一个商品数据。");
                return;
            }
            g.ShowInfo();
            
        }
    }
}
