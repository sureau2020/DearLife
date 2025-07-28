using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 这个类表示玩家（使用这个软件的用户）数据，处理与玩家底层model的交互
public class PlayerData 
{
    public int Money { get; private set; } = 0;//金钱

    public Dictionary<string, int> Items { get; private set; } = new Dictionary<string, int>();//背包里有啥物品
}
