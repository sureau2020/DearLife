using UnityEngine;
using UnityEngine.UI;

public class HSVSliderController : MonoBehaviour
{
    [Header("要控制的角色部位")]
    public string partName; // "FrontHair", "Body", "Clothes" ...
    public CharacterAppearance appearance;
    public AppearanceLoader loader;

    [Header("Slider 引用")]
    public Slider hueSlider;
    public Slider satSlider;
    public Slider valSlider;

    void Start()
    {
        BootSceneManager.Instance.OnAppearanceChanged += InitSlider;
        // 初始化 Slider 范围和默认值
        hueSlider.minValue = -1f;
        hueSlider.maxValue = 1f;
        satSlider.minValue = 0f;
        satSlider.maxValue = 2f;
        valSlider.minValue = 0f;
        valSlider.maxValue = 2f;

        //InitSlider(appearance, partName);

        // 添加监听
        hueSlider.onValueChanged.AddListener(OnHueChanged);
        satSlider.onValueChanged.AddListener(OnSatChanged);
        valSlider.onValueChanged.AddListener(OnValChanged);
    }

    public void InitSlider(CharacterAppearance app, string type)
    {
        appearance = app;
        partName = type;
        HSV currentHSV = GetHSV();
        hueSlider.SetValueWithoutNotify(currentHSV.h);
        satSlider.SetValueWithoutNotify(currentHSV.s);
        valSlider.SetValueWithoutNotify(currentHSV.v);
    }


    HSV GetHSV()
    {
        return partName switch
        {
            "FrontHair" => appearance.FrontHairColor,
            "BackHair" => appearance.BackHairColor,
            "SideHair" => appearance.SideHairColor,
            "Body" => appearance.BodyColor,
            "Eye" => appearance.EyeColor,
            "Clothes" => appearance.ClothesColor,
            "HeadDeco1" => appearance.HeadDeco1Color,
            "HeadDeco2" => appearance.HeadDeco2Color,
            _ => new HSV()
        };
    }

    void SetHSV(HSV h)
    {
        switch (partName)
        {
            case "FrontHair": appearance.FrontHairColor = h; break;
            case "BackHair": appearance.BackHairColor = h; break;
            case "SideHair": appearance.SideHairColor = h; break;
            case "Body": appearance.BodyColor = h; break;
            case "Eye": appearance.EyeColor = h; break;
            case "Clothes": appearance.ClothesColor = h; break;
            case "HeadDeco1": appearance.HeadDeco1Color = h; break;
            case "HeadDeco2": appearance.HeadDeco2Color = h; break;
        }
        loader.ApplyAppearance(appearance, "empty");
    }

    void OnHueChanged(float val)
    {
        HSV h = GetHSV();
        h.h = val;
        SetHSV(h);
    }

    void OnSatChanged(float val)
    {
        HSV h = GetHSV();
        h.s = val;
        SetHSV(h);
    }

    void OnValChanged(float val)
    {
        HSV h = GetHSV();
        h.v = val;
        SetHSV(h);
    }
}
