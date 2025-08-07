using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    [SerializeField] private GameObject goodsPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GenerateAllGoodsByType(ItemType.Food);
    }



    private OperationResult GenerateAllGoodsByType(ItemType type)
    {
        var items = ItemDataBase.GetItemsByType(type);
        foreach (var item in items)
        {
            GameObject go = Instantiate(goodsPrefab, transform);
            Goods g = go.GetComponent<Goods>();
            g.SetItemData(item);

            // 数据校验
            if (item == null || string.IsNullOrEmpty(item.Name))
            {
                return OperationResult.Fail("疑似数据库商品数据有缺失，检查该类别未被加载出来的第一个商品数据。");
            }
            g.ShowInfo();
            
        }
        return OperationResult.Complete();
    }
}
