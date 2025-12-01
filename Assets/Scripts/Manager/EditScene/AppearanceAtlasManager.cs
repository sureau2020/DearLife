using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AppearanceAtlasManager : MonoBehaviour
{
    public static AppearanceAtlasManager Instance;

    [SerializeField] private SpriteAtlas frontHairAtlas;
    [SerializeField] private SpriteAtlas sideHairAtlas;
    [SerializeField] private SpriteAtlas backHairAtlas;
    [SerializeField] private SpriteAtlas bodyAtlas;
    [SerializeField] private SpriteAtlas leftEyeAtlas;
    [SerializeField] private SpriteAtlas rightEyeAtlas;
    [SerializeField] private SpriteAtlas leftEyeBlancAtlas;
    [SerializeField] private SpriteAtlas rightEyeBlancAtlas;
    //[SerializeField] private SpriteAtlas eyeAtlas;
    [SerializeField] private SpriteAtlas clothesAtlas;
    [SerializeField] private SpriteAtlas headDeco1Atlas;
    [SerializeField] private SpriteAtlas headDeco2Atlas;

    private Dictionary<string, SpriteAtlas> atlasDict;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        atlasDict = new Dictionary<string, SpriteAtlas>
        {
            { "FrontHair", frontHairAtlas },
            { "SideHair", sideHairAtlas },
            { "BackHair", backHairAtlas },
            { "Body", bodyAtlas },
            //{ "Eye", eyeAtlas },
            { "LeftEye", leftEyeAtlas},
            { "RightEye", rightEyeAtlas},
            { "LeftEyeBlanc", leftEyeBlancAtlas},
            { "RightEyeBlanc", rightEyeBlancAtlas},
            { "Clothes", clothesAtlas },
            { "HeadDeco1", headDeco1Atlas },
            { "HeadDeco2", headDeco2Atlas }
        };
    }

    public Sprite GetPartSprite(string partName, int id)
    {
        if (id == 0) return null;
        if (atlasDict != null && atlasDict.TryGetValue(partName, out var atlas) && atlas != null)
        {
            return atlas.GetSprite(id.ToString());
        }
        return null;
    }

    public SpriteAtlas GetAtlasByType(string partName)
    {
        if (atlasDict != null && atlasDict.TryGetValue(partName, out var atlas))
        {
            return atlas;
        }
        return null;
    }
}
