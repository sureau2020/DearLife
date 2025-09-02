using UnityEngine;
using UnityEngine.U2D;

public class IconManager : MonoBehaviour
{
    [SerializeField] private SpriteAtlas foodAtlas;
    [SerializeField] private SpriteAtlas personalCareAtlas;
    [SerializeField] private SpriteAtlas medicineAtlas;
    [SerializeField] private SpriteAtlas giftAtlas;
    [SerializeField] private SpriteAtlas specialAtlas;

    private static IconManager instance;
    void Awake() => instance = this;

    public static Sprite GetIcon(ItemType type, string iconName)
    {
        Debug.Log($"Getting icon: Type={type}, Name={iconName}");
        switch (type)
        {
            case ItemType.Food: return instance.foodAtlas.GetSprite(iconName);
            case ItemType.PersonalCare: return instance.personalCareAtlas.GetSprite(iconName);
            case ItemType.Medicine: return instance.medicineAtlas.GetSprite(iconName);
            case ItemType.Gift: return instance.giftAtlas.GetSprite(iconName);
            case ItemType.Special: return instance.specialAtlas.GetSprite(iconName);
            default: return null;
        }
    }
}
