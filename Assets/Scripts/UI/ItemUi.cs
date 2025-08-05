using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUi : MonoBehaviour
{
    private ItemData item;
    private string itemId;

    // Start is called before the first frame update
    void Start()
    {
        itemId = "food001"; // ≤‚ ‘”√£¨‘› ±–¥À¿¡À
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnItemClick()
    {
        Debug.Log($"Clicked on item");
        
        OperationResult result = GameManager.Instance.StateManager.UseItem(itemId, 1);

        if (result.Success)
        {
            Debug.Log("Item purchased successfully!");
        }
        else
        {
            Debug.LogError($"Failed to purchase item: {result.Message}");
        }
    }
}
