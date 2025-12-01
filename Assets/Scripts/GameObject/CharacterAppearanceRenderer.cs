using UnityEngine;

public class CharacterAppearanceRenderer : MonoBehaviour
{
    [Header("SpriteRenderer 对应角色部位")]
    public SpriteRenderer frontHairRenderer;
    public SpriteRenderer backHairRenderer;
    public SpriteRenderer sideHairRenderer;
    public SpriteRenderer bodyRenderer;
    //public SpriteRenderer eyeRenderer;
    public SpriteRenderer leftEyeRenderer;
    public SpriteRenderer rightEyeRenderer;
    public SpriteRenderer leftEyeBlancRenderer;
    public SpriteRenderer rightEyeBlancRenderer;
    public SpriteRenderer clothesRenderer;
    public SpriteRenderer headDeco1Renderer;
    public SpriteRenderer headDeco2Renderer;

    [Header("外观数据源")]
    public bool useBootSceneManager = true;
    private CharacterAppearance manualAppearance = new CharacterAppearance();

    void Start()
    {
        LoadAndApplyAppearance();
    }

    /// <summary>
    /// 加载并应用外观
    /// </summary>
    public void LoadAndApplyAppearance()
    {
        CharacterAppearance appearance = GetAppearanceData();
        if (appearance != null)
        {
            ApplyAppearance(appearance);
        } 
    }


    private CharacterAppearance GetAppearanceData()
    {
        if (useBootSceneManager && BootSceneManager.Instance != null)
        {
            // 如果是编辑模式或创建模式，使用 BootSceneManager 的数据
            return BootSceneManager.Instance.Appearance;
        }
        else if (GameManager.Instance != null && GameManager.Instance.StateManager != null)
        {
            return GameManager.Instance.StateManager.Character?.Appearance;
        }
        else
        {
            // 手动设置的外观数据
            return manualAppearance;
        }
    }

    /// <summary>
    /// 应用外观
    /// </summary>
    public void ApplyAppearance(CharacterAppearance app)
    {
        if (app == null || AppearanceAtlasManager.Instance == null) return;
        SetPart(frontHairRenderer, AppearanceAtlasManager.Instance.GetPartSprite("FrontHair", app.FrontHairId), app.FrontHairColor);
        SetPart(backHairRenderer, AppearanceAtlasManager.Instance.GetPartSprite("BackHair", app.BackHairId), app.BackHairColor);
        SetPart(sideHairRenderer, AppearanceAtlasManager.Instance.GetPartSprite("SideHair", app.SideHairId), app.SideHairColor);
        SetPart(bodyRenderer, AppearanceAtlasManager.Instance.GetPartSprite("Body", app.BodyId), app.BodyColor);
        //SetPart(eyeRenderer, AppearanceAtlasManager.Instance.GetPartSprite("Eye", app.EyeId), app.EyeColor);
        SetPart(leftEyeRenderer, AppearanceAtlasManager.Instance.GetPartSprite("LeftEye", app.LeftEyeId), app.LeftEyeColor);
        SetPart(rightEyeRenderer, AppearanceAtlasManager.Instance.GetPartSprite("RightEye", app.RightEyeId), app.RightEyeColor);
        SetPart(leftEyeBlancRenderer, AppearanceAtlasManager.Instance.GetPartSprite("LeftEyeBlanc", app.LeftEyeBlancId), app.LeftEyeBlancColor);
        SetPart(rightEyeBlancRenderer, AppearanceAtlasManager.Instance.GetPartSprite("RightEyeBlanc", app.RightEyeBlancId), app.RightEyeBlancColor);
        WardrobeSlot slot = GameManager.Instance.StateManager.Character.Cloth;
        //Debug.Log("Applying Clothes: " + (slot?.Id != "0" ? slot.Id.ToString() : app.ClothesId.ToString()));
        if (slot != null)
        {
            string clothId = slot?.Id != "0" ? slot.Id.ToString() : app.ClothesId.ToString();
            if (slot.IsBuiltIn)
            {
                Debug.Log("Getting cloth icon for ID: " + clothId);
                
                Sprite a = IconManager.GetClothIcon(clothId);
                Debug.Log("Built-in Cloth Sprite: " + (a != null ? a.name : "null"));
                
                if (a == null)
                {
                    Debug.Log("Failed to get cloth icon for ID: " + slot.Id);
                    a = IconManager.GetClothIcon("-1");
                }
                
                SetPart(clothesRenderer, a, app.ClothesColor);
            }
            else
            {
                Sprite a = SaveManager.LoadCustomClothSprite(slot.Id);
                Debug.Log("Custom Cloth Sprite: " + (a != null ? a.name : "null"));
                if (a == null)
                {
                    a = IconManager.GetClothIcon("-1");
                }
                SetPart(clothesRenderer, a, app.ClothesColor);
            }
        }
        else {
            SetPart(clothesRenderer, AppearanceAtlasManager.Instance.GetPartSprite("Clothes", app.ClothesId), app.ClothesColor);
        }
        SetPart(headDeco1Renderer, AppearanceAtlasManager.Instance.GetPartSprite("HeadDeco1", app.HeadDeco1Id), app.HeadDeco1Color);
        SetPart(headDeco2Renderer, AppearanceAtlasManager.Instance.GetPartSprite("HeadDeco2", app.HeadDeco2Id), app.HeadDeco2Color);
    }

    private void SetPart(SpriteRenderer renderer, Sprite sprite, HSV color)
    {
        if (renderer == null) return;

        if (sprite == null)
        {
            renderer.enabled = false;
            return;
        }

        renderer.enabled = true;
        renderer.sprite = sprite;

        // HSL/HSV 调色
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Hue", color.h);
        mpb.SetFloat("_Saturation", color.s);
        mpb.SetFloat("_Lightness", color.v);
        renderer.SetPropertyBlock(mpb);
    }

    /// <summary>
    /// 刷新外观（用于运行时更新）
    /// </summary>
    public void RefreshAppearance()
    {
        LoadAndApplyAppearance();
    }
}