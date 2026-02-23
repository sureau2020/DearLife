using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureCell : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image kuang;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button _button;
    private string id;
    private FurnishCategory category;
    public void SetFurnitureData(Sprite sprite, string id, FurnishCategory category, int index) { 
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
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => FurnishItemEvents.TriggerItemClicked(id,category, index));
        }
    }

    public void SetSelected(bool selected) {
        kuang.gameObject.SetActive(selected);
    }

}
