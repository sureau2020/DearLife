using UnityEngine;

public class AppearanceLoader : MonoBehaviour
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


    void Start()
    {
        BootSceneManager.Instance.OnAppearanceChanged += ApplyAppearance;
        var appearance = BootSceneManager.Instance.Appearance;
        if (appearance != null)
        {
            ApplyAppearance(appearance, "empty");
        }
    }

    void OnDestroy()
    {
        if (BootSceneManager.Instance != null)
        {
            BootSceneManager.Instance.OnAppearanceChanged -= ApplyAppearance;
        }
    }


    public void ApplyAppearance(CharacterAppearance app, string type)
    {
        SetPart(frontHairRenderer, AppearanceAtlasManager.Instance.GetPartSprite("FrontHair", app.FrontHairId),
                app.FrontHairColor);

        SetPart(backHairRenderer, AppearanceAtlasManager.Instance.GetPartSprite("BackHair", app.BackHairId),
                app.BackHairColor);

        SetPart(sideHairRenderer, AppearanceAtlasManager.Instance.GetPartSprite("SideHair", app.SideHairId),
                app.SideHairColor);

        SetPart(bodyRenderer, AppearanceAtlasManager.Instance.GetPartSprite("Body", app.BodyId),
                app.BodyColor);

        //SetPart(eyeRenderer, AppearanceAtlasManager.Instance.GetPartSprite("Eye", app.EyeId),
                //app.EyeColor);
        SetPart(leftEyeRenderer, AppearanceAtlasManager.Instance.GetPartSprite("LeftEye", app.LeftEyeId),
                app.LeftEyeColor);
        SetPart(rightEyeRenderer, AppearanceAtlasManager.Instance.GetPartSprite("RightEye", app.RightEyeId),
                app.RightEyeColor);
        SetPart(leftEyeBlancRenderer, AppearanceAtlasManager.Instance.GetPartSprite("LeftEyeBlanc", app.LeftEyeBlancId),
                app.LeftEyeBlancColor);
        SetPart(rightEyeBlancRenderer, AppearanceAtlasManager.Instance.GetPartSprite("RightEyeBlanc", app.RightEyeBlancId),
                app.RightEyeBlancColor);

        SetPart(clothesRenderer, AppearanceAtlasManager.Instance.GetPartSprite("Clothes", app.ClothesId),
                app.ClothesColor);

        SetPart(headDeco1Renderer, AppearanceAtlasManager.Instance.GetPartSprite("HeadDeco1", app.HeadDeco1Id),
                app.HeadDeco1Color);

        SetPart(headDeco2Renderer, AppearanceAtlasManager.Instance.GetPartSprite("HeadDeco2", app.HeadDeco2Id),
                app.HeadDeco2Color);
    }

    private void SetPart(SpriteRenderer renderer, Sprite sprite, HSV color)
    {
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
}
