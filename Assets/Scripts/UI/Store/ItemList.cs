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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateAllGoodsByType(ItemType type) { 
        ItemDataBase.GetItemsByType(type).ForEach(item => {
            GameObject go = Instantiate(goodsPrefab, transform);
            Goods g = go.GetComponent<Goods>();
            g.SetItemData(item);
            g.ShowInfo();
        });
    }
}
