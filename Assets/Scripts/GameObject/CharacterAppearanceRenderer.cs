using UnityEngine;

public class CharacterAppearanceRenderer : MonoBehaviour
{
    [Header("SpriteRenderer ��Ӧ��ɫ��λ")]
    public SpriteRenderer frontHairRenderer;
    public SpriteRenderer backHairRenderer;
    public SpriteRenderer sideHairRenderer;
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer eyeRenderer;
    public SpriteRenderer clothesRenderer;
    public SpriteRenderer headDeco1Renderer;
    public SpriteRenderer headDeco2Renderer;

    [Header("�������Դ")]
    public bool useBootSceneManager = true;
    private CharacterAppearance manualAppearance = new CharacterAppearance();

    void Start()
    {
        LoadAndApplyAppearance();
    }

    /// <summary>
    /// ���ز�Ӧ�����
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
            // ����Ǳ༭ģʽ�򴴽�ģʽ��ʹ�� BootSceneManager ������
            return BootSceneManager.Instance.Appearance;
        }
        else if (GameManager.Instance != null && GameManager.Instance.StateManager != null)
        {
            // �������Ϸ������ʹ�� StateManager �Ľ�ɫ����
            return GameManager.Instance.StateManager.Character?.Appearance;
        }
        else
        {
            // �ֶ����õ��������
            return manualAppearance;
        }
    }

    /// <summary>
    /// Ӧ�����
    /// </summary>
    public void ApplyAppearance(CharacterAppearance app)
    {
        if (app == null || AppearanceAtlasManager.Instance == null) return;
        SetPart(frontHairRenderer, AppearanceAtlasManager.Instance.GetPartSprite("FrontHair", app.FrontHairId), app.FrontHairColor);
        SetPart(backHairRenderer, AppearanceAtlasManager.Instance.GetPartSprite("BackHair", app.BackHairId), app.BackHairColor);
        SetPart(sideHairRenderer, AppearanceAtlasManager.Instance.GetPartSprite("SideHair", app.SideHairId), app.SideHairColor);
        SetPart(bodyRenderer, AppearanceAtlasManager.Instance.GetPartSprite("Body", app.BodyId), app.BodyColor);
        SetPart(eyeRenderer, AppearanceAtlasManager.Instance.GetPartSprite("Eye", app.EyeId), app.EyeColor);
        SetPart(clothesRenderer, AppearanceAtlasManager.Instance.GetPartSprite("Clothes", app.ClothesId), app.ClothesColor);
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

        // HSL/HSV ��ɫ
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Hue", color.h);
        mpb.SetFloat("_Saturation", color.s);
        mpb.SetFloat("_Lightness", color.v);
        renderer.SetPropertyBlock(mpb);
    }

    /// <summary>
    /// ˢ����ۣ���������ʱ���£�
    /// </summary>
    public void RefreshAppearance()
    {
        LoadAndApplyAppearance();
    }
}