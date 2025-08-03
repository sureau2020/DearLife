


// 这个类用于存储游戏主要角色的数据，处理底层model的交互


using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData 
{
    private const int MaxVar = 100;//各数值最高数
    public int Full { get; private set; } = MaxVar;//饱腹值初始值满

    public int San { get; private set; } = MaxVar;//理智
    public int Clean { get; private set; } = MaxVar;//清洁

    public HealthState HealthState { get; private set; } = HealthState.Normal;//健康状态
    public int Love { get; private set; } = 0;//好感度

    public List<PersonalityType> Personalities { get; private set; } = new List<PersonalityType>();//性格列表

    public DateTime FirstStartTime { get; private set; }

    public CharacterData()
    {
        FirstStartTime = DateTime.Now; // 记录第一次开始游戏的时间
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
                    Love += effect.Value * quantity;
                    break;
                default:
                    return OperationResult.Fail("物品效果包含未知类型，请检查物品效果。");
            }
        }
        return OperationResult.Complete();
    }

    // TODO:使用物品后显示对话！


    public void ChangeFull(int delta)
    {
        Full = Mathf.Clamp(Full + delta, 0, 100);
    }

    public void ChangeClean(int delta)
    {
        Clean = Mathf.Clamp(Clean + delta, 0, 100);
    }

    public void ChangeSan(int delta)
    {
        San = Mathf.Clamp(San + delta, 0, 100);
    }



}
