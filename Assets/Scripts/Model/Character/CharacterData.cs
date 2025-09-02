// 这个类用于存储游戏主要角色的数据，处理底层model的交互


using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class CharacterData 
{
    public const int MaxVar = 100;//各数值最高数

    public string Name { get; set; }

    public string Pronoun { get; set; }

// 临时改为 public set 测试反序列化
    public int Full { get; set; } = MaxVar;
    public int San { get; set; } = MaxVar;
    public int Clean { get; set; } = MaxVar;

    public CharacterAppearance Appearance { get; set; }
    public HealthState HealthState { get; set; } = HealthState.Normal;
    public int Love { get; set; } = 0;
    public HashSet<string> PersonalityTags { get; set; }
    public DateTime FirstStartTime { get; set; }

    public int DeathNum { get; set; } = 0;

    public string Relationship { get; set; } = "礼貌";
    public string PlayerAppellation { get; set; }

    public event Action<string, int> OnCharacterStateChanged;// 角色状态变化事件，参数为状态名称和变化值
    public event Action OnHealthChanged;


    // 目前硬编码，写死了一个角色的性格标签
    public CharacterData()
    {
        FirstStartTime = DateTime.Now; 
        PersonalityTags = new HashSet<string> { "mature", "calm" }; 
    }

    public CharacterData(string name, string pronoun, CharacterAppearance appearance, HashSet<string> personalityTags, string playerAppellation)
    {
        Name = name;
        Pronoun = pronoun;
        Appearance = appearance;
        PersonalityTags = personalityTags;
        PlayerAppellation = playerAppellation;

        // 尝试从 SaveManager 加载已有的状态数据
        var loadResult = SaveManager.LoadStateManager();
        if (loadResult.Success && loadResult.Data?.Character != null)
        {
            var savedCharacter = loadResult.Data.Character;
            
            // 使用保存的状态数据
            FirstStartTime = savedCharacter.FirstStartTime;
            DeathNum = savedCharacter.DeathNum;
            Full = savedCharacter.Full;
            San = savedCharacter.San;
            Clean = savedCharacter.Clean;
            Love = savedCharacter.Love;
            HealthState = savedCharacter.HealthState;
            Relationship = savedCharacter.Relationship;
            
        }
        else
        {
            FirstStartTime = DateTime.Now;
            DeathNum = 0;
            Full = MaxVar;
            San = MaxVar;
            Clean = MaxVar;
            Love = 0;
            HealthState = HealthState.Normal;
            Relationship = DetermineRelation();
        }
    }


    // 在主角身上真正使用物品，应用效果，返回操作结果
    public OperationResult ApplyItemEffect(string itemId, int quantity) {
        ItemData item = ItemDataBase.GetItemById(itemId);
        if (item == null)
        {
            return OperationResult.Fail("物品不存在。疑似物品数据库损坏，尝试重启或还原数据库json文件。");
        }

        foreach (var effect in item.Effect)
        {
            switch (effect.Key)
            {
                case EffectType.Full:
                    ChangeFull(effect.Value * quantity);
                    break;
                case EffectType.San:
                    ChangeSan(effect.Value * quantity);
                    break;
                case EffectType.Clean:
                    ChangeClean(effect.Value * quantity);
                    break;
                case EffectType.Love:
                    ChangeLove(effect.Value * quantity);
                    break;
                default:
                    return OperationResult.Fail("物品效果包含未知类型，请检查物品效果。");
            }
        }
        return OperationResult.Complete();
    }

    // TODO:使用物品后显示对话！

    public OperationResult ApplyEffect(EffectType type, int quantity)
    {
        switch (type)
        {
            case EffectType.Full:
                ChangeFull(quantity);
                break;
            case EffectType.San:
                ChangeSan(quantity);
                break;
            case EffectType.Clean:
                ChangeClean(quantity);
                break;
            case EffectType.Love:
                ChangeLove(quantity);
                break;
            default:
                return OperationResult.Fail("未知效果类型，请检查当前事件");
        }
        return OperationResult.Complete();
    }

    public void SetRelationShip() {
        Relationship = DetermineRelation();
    } 

    public string DetermineRelation() {
        if (Love >= 10000)
            return "无可替代";
        else if (Love >= 8000)
            return "挚友";
        else if (Love >= 5000)
            return "好友";
        else if (Love >= 3000)
            return "普通朋友";
        else if (Love >= 2000)
            return "礼貌朋友";
        else if (Love >= 1000)
            return "初相识";
        else if (Love >= 0)
            return "礼貌";
        else if (Love >= -300)
            return "警惕";
        else if (Love >= -1000)
            return "敌对";
        else if (Love >= -2000)
            return "死敌";
        else
            return "无可挽回";
    }


    public string GetHealthStateDescription()
    {
        switch (HealthState)
        {
            case HealthState.Normal:
                return "健康";
            case HealthState.Weak:
                return "虚弱";
            case HealthState.Sick:
                return "生病";
            case HealthState.Crazy:
                return "疯狂";
            case HealthState.Dead:
                return "死亡";
            case HealthState.Dirty:
                return "肮脏";
            default:
                return "未知状态";
        }
    }


    //返回中文性格描述
    public string GetPersonalityDescription()
    {
        if (PersonalityTags == null || PersonalityTags.Count == 0)
            return "无";

        // 英文tag到中文的映射
        Dictionary<string, string> personalityDescriptions = new Dictionary<string, string>
        {
            { "calm", "冷静" },
            { "innocent", "天真" },
            { "mature", "成熟" },
            { "lively", "活泼" },
            { "realistic", "现实" },
            { "denpa", "电波" },
            { "crazy", "疯狂" }
        };

        List<string> descList = new List<string>();
        foreach (var tag in PersonalityTags)
        {
            if (personalityDescriptions.TryGetValue(tag, out string desc))
                descList.Add(desc);
            else
                descList.Add(tag); // 未定义的tag直接显示英文
        }
        return string.Join("、", descList);
    }

    // true represents promoted
    void UpdateStatus()
    {
        if (HealthState == HealthState.Normal && Full <= 0)
        {
            SwitchState(HealthState.Weak, false);
        }
        else if (HealthState == HealthState.Weak && Full <= 0)
        {
            SwitchState(HealthState.Dead, false);
        }
        else if (HealthState == HealthState.Normal && San <= 0)
        {
            SwitchState(HealthState.Crazy, false);
        }
        else if (HealthState == HealthState.Normal && Clean <= 0)
        {
            SwitchState(HealthState.Dirty, false);
        }
        else if ((HealthState == HealthState.Weak && Full >= 100) 
            || (HealthState == HealthState.Crazy && San >= 70)
            || (HealthState == HealthState.Dirty && Clean >= 70))
        {
            SwitchState(HealthState.Normal, true);
        }
    }

    void SwitchState(HealthState newState, bool isPromoted)
    {
        HealthState = newState;
        if (isPromoted) {
            ZeroStateValue(newState);
        }
        else
        {
            FillStateValue(newState);
        }
        OnHealthChanged?.Invoke();
    }

    private void FillStateValue(HealthState state)
    {
        if (state == HealthState.Weak) Full = 99; 
        if (state == HealthState.Dead) DeathNum++;;
    }

    private void ZeroStateValue(HealthState state) {
        if (state == HealthState.Normal) { 
            Full = 20;
            Clean = 20;
            San = 20;
        } 
    }

    public void Rebirth()
    {
        if (HealthState != HealthState.Dead)
            return;
        ChangeLove(-200);
        Full = MaxVar;
        San = MaxVar;
        Clean = MaxVar;
        HealthState = HealthState.Normal;
        OnHealthChanged?.Invoke();
    }



    public void ChangeFull(int delta)
    {
        Full = Mathf.Clamp(Full + delta, 0, 100);
        OnCharacterStateChanged?.Invoke("Full", Full);
        UpdateStatus();
        
        _ = GameManager.Instance.StateManager.SaveStateAsync();
    }

    public void ChangeClean(int delta)
    {
        Clean = Mathf.Clamp(Clean + delta, 0, 100);
        OnCharacterStateChanged?.Invoke("Clean", Clean);
        UpdateStatus();
        
        _ = GameManager.Instance.StateManager.SaveStateAsync();
    }

    public void ChangeSan(int delta)
    {
        San = Mathf.Clamp(San + delta, 0, 100);
        OnCharacterStateChanged?.Invoke("San", San);
        UpdateStatus();
        
        _ = GameManager.Instance.StateManager.SaveStateAsync();
    }

    public void ChangeLove(int delta)
    {
        Love = Love + delta;
        Relationship = DetermineRelation();
        OnCharacterStateChanged?.Invoke("Love", Love);
        
        _ = GameManager.Instance.StateManager.SaveStateAsync();
    }


}


public struct HSV
{
    public float h; // 色相 -1..1
    public float s; // 饱和度 -1..1
    public float v; // 明度 -1..1
}


public class CharacterAppearance
{
    public int FrontHairId;
    public HSV FrontHairColor;

    public int BodyId;
    public HSV BodyColor;

    public int BackHairId;
    public HSV BackHairColor;

    public int EyeId;
    public HSV EyeColor;

    public int ClothesId;
    public HSV ClothesColor;

    public int SideHairId;
    public HSV SideHairColor;

    public int HeadDeco1Id;
    public HSV HeadDeco1Color;

    public int HeadDeco2Id;
    public HSV HeadDeco2Color;

    public CharacterAppearance()
    {
        FrontHairId = 0;
        FrontHairColor = new HSV { h = 0, s = 1, v = 1 };
        BodyId = 1;
        BodyColor = new HSV { h = 0, s = 1, v = 1 };
        BackHairId = 0;
        BackHairColor = new HSV { h = 0, s = 1, v = 1 };
        EyeId = 0;
        EyeColor = new HSV { h = 0, s = 1, v = 1 };
        ClothesId = 0;
        ClothesColor = new HSV { h = 0, s = 1, v = 1 };
        SideHairId = 0;
        SideHairColor = new HSV { h = 0, s = 1, v = 1 };
        HeadDeco1Id = 0;
        HeadDeco1Color = new HSV { h = 0, s = 1, v = 1 };
        HeadDeco2Id = 0;
        HeadDeco2Color = new HSV { h = 0, s = 1, v = 1 };
    }
}
