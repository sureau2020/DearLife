using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootSceneManager : MonoBehaviour
{
    public static BootSceneManager Instance { get; private set; }
    
    // 创建的角色数据
    public CharacterData CreatedCharacter { get; private set; }
    
    // 临时存储的角色创建数据
    public string CharacterName { get; set; } = "未命名";
    public string CharacterPronoun { get; set; } = "Ta"; 
    public string Appellation { get; set; } = "你";
    public HashSet<string> PersonalityTags { get; set; } = new HashSet<string>();
    public CharacterAppearance Appearance { get; set; } = new CharacterAppearance();
    
    public event Action<CharacterAppearance,string> OnAppearanceChanged;


    void Awake()
    {
        Application.targetFrameRate = 30;
        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
        if (Instance != null) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        OperationResult<StateManagerSaveData> result = SaveManager.LoadStateManager();
        if (result.Success) { 
            SceneManager.LoadScene("Room");
            return;
        }

        if (Appearance == null)
        {
            Appearance = new CharacterAppearance();
        }
        SceneManager.LoadScene("EditCharacter");
    }

    // 确认角色创建/编辑（统一处理)
    public void ConfirmCharacter()
    {
        CreateCharacterFromData();
        SceneManager.LoadScene("Room");
    }

    private void CreateCharacterFromData()
    {
        CharacterData characterData = new CharacterData(CharacterName, CharacterPronoun, Appearance, PersonalityTags, Appellation);
        CreatedCharacter = characterData;
    }

    // 从主场景进入编辑模式
    public void EnterEditModeFromMain()
    {   
        CreatedCharacter = GameManager.Instance.StateManager.Character;
        LoadCharacterDataToEditor();

        SceneManager.LoadScene("EditCharacter");
    }


    // 将现有角色数据载入编辑器
    private void LoadCharacterDataToEditor()
    {
        CharacterName = CreatedCharacter.Name;
        CharacterPronoun = CreatedCharacter.Pronoun;
        Appellation = CreatedCharacter.PlayerAppellation;
        PersonalityTags = new HashSet<string>(CreatedCharacter.PersonalityTags);
        Appearance = CreatedCharacter.Appearance ?? new CharacterAppearance();
    }

    // 设置外观数据
    public void SetAppearance(CharacterAppearance appearance)
    {
        Appearance = appearance;
    }

    // 添加性格标签
    public void AddPersonalityTag(string tag)
    {
        PersonalityTags.Add(tag);
    }

    // 移除性格标签
    public void RemovePersonalityTag(string tag)
    {
        PersonalityTags.Remove(tag);
    }

    // 清空性格标签
    public void ClearPersonalityTags()
    {
        PersonalityTags.Clear();
    }

    // 检查是否有性格标签
    public bool HasPersonalityTag(string tag)
    {
        return PersonalityTags.Contains(tag);
    }


    public string GetInfo(string key)
    {
        switch (key)
        {
            case "Name":
                return string.IsNullOrWhiteSpace(CharacterName) ? "" : CharacterName;
            case "Pronoun":
                return string.IsNullOrWhiteSpace(CharacterPronoun) ? "" : CharacterPronoun;
            case "Appellation":
                return string.IsNullOrWhiteSpace(Appellation) ? "" : Appellation;
            default:
                return "";
        }
    }

    public void SetInfo(string key, string value)
    {
        switch (key)
        {
            case "Name":
                CharacterName = value;
                break;
            case "Pronoun":
                CharacterPronoun = value;
                break;
            case "Appellation":
                Appellation = value;
                break;
        }
    }

    public void ApplyAppearancePart(string type, int id)
    {
        var appearance = Appearance;
        if (appearance == null) return;

        switch (type)
        {
            case "FrontHair": appearance.FrontHairId = id; break;
            case "SideHair": appearance.SideHairId = id; break;
            case "BackHair": appearance.BackHairId = id; break;
            case "Body": appearance.BodyId = id; break;
            //case "Eye": appearance.EyeId = id; break;
            case "LeftEye": appearance.LeftEyeId = id; break;
            case "RightEye": appearance.RightEyeId = id; break;
            case "LeftEyeBlanc": appearance.LeftEyeBlancId = id; break;
            case "RightEyeBlanc": appearance.RightEyeBlancId = id; break;
            case "Clothes": appearance.ClothesId = id; break;
            case "HeadDeco1": appearance.HeadDeco1Id = id; break;
            case "HeadDeco2": appearance.HeadDeco2Id = id; break;
        }
        OnAppearanceChanged?.Invoke(Appearance,type);
    }

}
