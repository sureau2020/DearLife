using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TypeButton : MonoBehaviour
{
    [SerializeField] private string type;
    [SerializeField] private Transform componentList;
    [SerializeField] private ApparenceComponent componentPrefab;
    [SerializeField] private GameObject Slider;


    public void OnClick()
    {
        SoundManager.Instance.PlaySfx("LittleType");
        Slider.SetActive(false);
        foreach (Transform child in componentList)
        {
            Destroy(child.gameObject);
        }
        //��һ���ǿյ�
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
            if (int.TryParse(cleanName, out int spriteId))
            {
                newComponent = Instantiate(componentPrefab.gameObject, componentList);
                instance = newComponent.GetComponent<ApparenceComponent>();
                instance.Show(spriteId, sprite,type);
            }
        }
    }
}
