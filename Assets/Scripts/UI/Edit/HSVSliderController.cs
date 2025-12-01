using UnityEngine;
using UnityEngine.UI;

public class HSVSliderController : MonoBehaviour
{
    [Header("要控制的角色部位")]
    public string partName; // "FrontHair", "Body", "Clothes" ..
    public CharacterAppearance appearance;
    public AppearanceLoader loader;

    [SerializeField] private Button syncEye;

    [Header("Slider 引用")]
    public Slider hueSlider;
    public Slider satSlider;
    public Slider valSlider;
    public SetText hueText;
    public SetText satText;
    public SetText valText;

    void Start()
    {
        // 初始化 Slider 范围和默认值
        hueSlider.minValue = -1f;
        hueSlider.maxValue = 1f;
        satSlider.minValue = 0f;
        satSlider.maxValue = 2f;
        valSlider.minValue = 0f;
        valSlider.maxValue = 2f;

        // 添加监听
        hueSlider.onValueChanged.AddListener(OnHueChanged);
        satSlider.onValueChanged.AddListener(OnSatChanged);
        valSlider.onValueChanged.AddListener(OnValChanged);
        
        // 延迟订阅事件，确保所有组件都已初始化
        BootSceneManager.Instance.OnAppearanceChanged += InitSlider;
    }

    void OnDestroy()
    {
        // 取消事件订阅，防止内存泄漏和对已销毁对象的引用
        if (BootSceneManager.Instance != null)
            BootSceneManager.Instance.OnAppearanceChanged -= InitSlider;
    }

    public void InitSlider(CharacterAppearance app, string type)
    {
        appearance = app;
        partName = type;
        HSV currentHSV = GetHSV();
        
        // 添加空值检查
        if (hueText != null) hueText.SetTextValue(currentHSV.h);
        if (satText != null) satText.SetTextValue(currentHSV.s);
        if (valText != null) valText.SetTextValue(currentHSV.v);
        
        if (hueSlider != null) hueSlider.SetValueWithoutNotify(currentHSV.h);
        if (satSlider != null) satSlider.SetValueWithoutNotify(currentHSV.s);
        if (valSlider != null) valSlider.SetValueWithoutNotify(currentHSV.v);
    }


    HSV GetHSV()
    {
        return partName switch
        {
            "FrontHair" => appearance.FrontHairColor,
            "BackHair" => appearance.BackHairColor,
            "SideHair" => appearance.SideHairColor,
            "Body" => appearance.BodyColor,
            //"Eye" => appearance.EyeColor,
            "LeftEye" => appearance.LeftEyeColor,
            "RightEye" => appearance.RightEyeColor,
            "LeftEyeBlanc" => appearance.LeftEyeBlancColor,
            "RightEyeBlanc" => appearance.RightEyeBlancColor,
            "Clothes" => appearance.ClothesColor,
            "HeadDeco1" => appearance.HeadDeco1Color,
            "HeadDeco2" => appearance.HeadDeco2Color,
            _ => new HSV()
        };
    }

    public void SyncEye() { 
        HSV h = GetHSV();
        appearance.LeftEyeColor = h;
        appearance.RightEyeColor = h;
        loader.ApplyAppearance(appearance, "empty");
    }

    void SetHSV(HSV h)
    {
        switch (partName)
        {
            case "FrontHair": appearance.FrontHairColor = h; break;
            case "BackHair": appearance.BackHairColor = h; break;
            case "SideHair": appearance.SideHairColor = h; break;
            case "Body": appearance.BodyColor = h; break;
            //case "Eye": appearance.EyeColor = h; break;
            case "LeftEye": appearance.LeftEyeColor = h; break;
            case "RightEye": appearance.RightEyeColor = h; break;
            case "LeftEyeBlanc": appearance.LeftEyeBlancColor = h; break;
            case "RightEyeBlanc": appearance.RightEyeBlancColor = h; break;
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
        hueText.SetTextValue(val);
        SetHSV(h);
    }

    void OnSatChanged(float val)
    {
        HSV h = GetHSV();
        h.s = val;
        satText.SetTextValue(val);
        SetHSV(h);
    }

    void OnValChanged(float val)
    {
        HSV h = GetHSV();
        h.v = val;
        valText.SetTextValue(val);
        SetHSV(h);
    }
}
