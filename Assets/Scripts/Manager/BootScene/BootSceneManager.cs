using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootSceneManager : MonoBehaviour
{
    public static BootSceneManager Instance { get; private set; }
    
    // �����Ľ�ɫ����
    public CharacterData CreatedCharacter { get; private set; }
    
    // ��ʱ�洢�Ľ�ɫ��������
    public string CharacterName { get; set; } = "δ����";
    public string CharacterPronoun { get; set; } = "Ta"; 
    public string Appellation { get; set; } = "��";
    public HashSet<string> PersonalityTags { get; set; } = new HashSet<string>();
    public CharacterAppearance Appearance { get; set; } = new CharacterAppearance();
    
    public event Action<CharacterAppearance,string> OnAppearanceChanged;


    void Awake()
    {
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

    // ȷ�Ͻ�ɫ����/�༭��ͳһ����)
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

    // ������������༭ģʽ
    public void EnterEditModeFromMain()
    {   
        if (CreatedCharacter == null)
        {
            CreatedCharacter = GameManager.Instance.StateManager.Character;
        }

        SceneManager.LoadScene("EditCharacter");
    }


    // �����н�ɫ��������༭��
    private void LoadCharacterDataToEditor()
    {
        CharacterName = CreatedCharacter.Name;
        CharacterPronoun = CreatedCharacter.Pronoun;
        Appellation = CreatedCharacter.PlayerAppellation;
        PersonalityTags = new HashSet<string>(CreatedCharacter.PersonalityTags);
        Appearance = CreatedCharacter.Appearance ?? new CharacterAppearance();
    }

    // �����������
    public void SetAppearance(CharacterAppearance appearance)
    {
        Appearance = appearance;
    }

    // ����Ը��ǩ
    public void AddPersonalityTag(string tag)
    {
        PersonalityTags.Add(tag);
    }

    // �Ƴ��Ը��ǩ
    public void RemovePersonalityTag(string tag)
    {
        PersonalityTags.Remove(tag);
    }

    // ����Ը��ǩ
    public void ClearPersonalityTags()
    {
        PersonalityTags.Clear();
    }

    // ����Ƿ����Ը��ǩ
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
            case "Eye": appearance.EyeId = id; break;
            case "Clothes": appearance.ClothesId = id; break;
            case "HeadDeco1": appearance.HeadDeco1Id = id; break;
            case "HeadDeco2": appearance.HeadDeco2Id = id; break;
        }
        OnAppearanceChanged?.Invoke(Appearance,type);
    }

}
