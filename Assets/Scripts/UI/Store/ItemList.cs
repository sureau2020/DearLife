
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

            // ����У��
            if (item == null || string.IsNullOrEmpty(item.Name))
            {
                ErrorNotifier.NotifyError("�������ݿ���Ʒ������ȱʧ���������δ�����س����ĵ�һ����Ʒ���ݡ�");
                return;
            }
            g.ShowInfo();
            
        }
    }
}
