

// 这个类表示游戏中的商品数据，处理与商品底层model的交互。
using System.Collections.Generic;

public class ItemData
{
    public string Id { get; set; }
    public string Name { get; private set; } 
    public string Description { get; private set; }

    public int Price { get; private set; }

    public ItemType Type { get; private set; }

    public Dictionary<EffectType, int> Effect { get; private set; }


    

}
