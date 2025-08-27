


// 这个类用于存储游戏主要角色的数据，处理底层model的交互


using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData 
{
    public const int MaxVar = 100;//各数值最高数

    public string Name { get; set; } = "主角";//角色名称//目前写死硬编码todo

    public int Full { get; private set; } = MaxVar;//饱腹值初始值满

    public int San { get; private set; } = MaxVar;//理智
    public int Clean { get; private set; } = MaxVar;//清洁

    public HealthState HealthState { get; private set; } = HealthState.Normal;//健康状态
    public int Love { get; private set; } = 0;//好感度

    public HashSet<string> PersonalityTags { get; private set; }

    public DateTime FirstStartTime { get; private set; }

    public event Action<string, int> OnCharacterStateChanged;// 角色状态变化事件，参数为状态名称和变化值


    // 目前硬编码，写死了一个角色的性格标签
    public CharacterData()
    {
        FirstStartTime = DateTime.Now; // 记录第一次开始游戏的时间
        PersonalityTags = new HashSet<string> { "mature", "calm" }; 
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


    public void ChangeFull(int delta)
    {
        Full = Mathf.Clamp(Full + delta, 0, 100);
        OnCharacterStateChanged?.Invoke("Full", Full);
    }

    public void ChangeClean(int delta)
    {
        Clean = Mathf.Clamp(Clean + delta, 0, 100);
        OnCharacterStateChanged?.Invoke("Clean", Clean);
    }

    public void ChangeSan(int delta)
    {
        San = Mathf.Clamp(San + delta, 0, 100);
        OnCharacterStateChanged?.Invoke("San", San);
    }

    public void ChangeLove(int delta)
    {
        Love = Mathf.Max(0, Love + delta); // Love不能小于0
        OnCharacterStateChanged?.Invoke("Love", Love);
    }


}
