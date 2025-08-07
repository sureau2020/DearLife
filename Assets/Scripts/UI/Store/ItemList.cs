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

            // ����У��
            if (item == null || string.IsNullOrEmpty(item.Name))
            {
                return OperationResult.Fail("�������ݿ���Ʒ������ȱʧ���������δ�����س����ĵ�һ����Ʒ���ݡ�");
            }
            g.ShowInfo();
            
        }
        return OperationResult.Complete();
    }
}
