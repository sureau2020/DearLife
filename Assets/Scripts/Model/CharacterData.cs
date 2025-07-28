


// 这个类用于存储游戏主要角色的数据，处理底层model的交互


using System;
using System.Collections.Generic;

public class CharacterData 
{
    private const int MAXVAR = 100;//各数值最高数
    public int Full { get; private set; } = MAXVAR;//饱腹值初始值满

    public int San { get; private set; } = MAXVAR;//理智
    public int Clean { get; private set; } = MAXVAR;//清洁
    public int Love { get; private set; } = 0;//好感度

    public List<PersonalityType> personalities { get; private set; } = new List<PersonalityType>();//性格列表

    public DateTime FirstStartTime { get; private set; }

}
