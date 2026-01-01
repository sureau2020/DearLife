using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TypeButton : MonoBehaviour
{
    [SerializeField] private string type;
    [SerializeField] private Transform componentList;
    [SerializeField] private ApparenceComponent componentPrefab;
    [SerializeField] private GameObject customComponent;
    [SerializeField] private GameObject Slider;
    [SerializeField] private GameObject SyncEyeButton; 


    public void OnClick() { 
        

        SoundManager.Instance.PlaySfx("LittleType");
        Slider.SetActive(false);
        SyncEyeButton.SetActive(false);
        foreach (Transform child in componentList)
        {
            Destroy(child.gameObject);
        }
        //第一个是空的
        GameObject newComponent = Instantiate(componentPrefab.gameObject, componentList);
        var instance = newComponent.GetComponent<ApparenceComponent>();
        instance.Show(0, null, type);

        SpriteAtlas atlas = AppearanceAtlasManager.Instance.GetAtlasByType(type);
        if (atlas == null) return;

        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);
        foreach (var sprite in sprites)
        {
            if (sprite == null) continue;
            string cleanName = sprite.name.Replace("(Clone)", "").Trim();
            if (int.TryParse(cleanName, out int spriteId) && spriteId > 0)//这个有没有问题看一眼
            {
                newComponent = Instantiate(componentPrefab.gameObject, componentList);
                instance = newComponent.GetComponent<ApparenceComponent>();
                instance.Show(spriteId, sprite,type);
            }
        }
        if (type != "Clothes") {
            GenerateCustomComponent();
        }
    }


    private void GenerateCustomComponent()
    {
        if (customComponent != null)
        {
            GameObject newComponent = Instantiate(customComponent, componentList);
            ApparenceComponent instance = newComponent.GetComponent<ApparenceComponent>();
            instance.Show(type);
        }
    }
}


