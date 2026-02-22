using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureCell : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    private string id;
    private FurnishCategory category;
    public void SetFurnitureData(Sprite sprite, string id, FurnishCategory category) { 
         if (sprite != null)
         {
             nameText.text = id;
             icon.sprite = sprite;
             var fitter = icon.GetComponent<AspectRatioFitter>();
             if (fitter != null)
             {
                 fitter.aspectRatio = (float)sprite.rect.width / sprite.rect.height;
             }
            this.id = id;
            this.category = category;
        }
    }
}
