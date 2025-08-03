


// 这个类用于存储游戏主要角色的数据，处理底层model的交互


using System;
using System.Collections.Generic;

public class CharacterData 
{
    private const int MaxVar = 100;//各数值最高数
    public int Full { get; private set; } = MaxVar;//饱腹值初始值满

    public int San { get; private set; } = MaxVar;//理智
    public int Clean { get; private set; } = MaxVar;//清洁
    public int Love { get; private set; } = 0;//好感度

    public List<PersonalityType> Personalities { get; private set; } = new List<PersonalityType>();//性格列表

    public DateTime FirstStartTime { get; private set; }


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
                    Full = Math.Min(MaxVar, Full + effect.Value * quantity);
                    break;
                case EffectType.San:
                    San = Math.Min(MaxVar, San + effect.Value * quantity);
                    break;
                case EffectType.Clean:
                    Clean = Math.Min(MaxVar, Clean + effect.Value * quantity);
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

}
