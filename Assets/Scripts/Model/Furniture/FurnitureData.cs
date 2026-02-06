using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FurnitureData
{
    public string id;
    public string name;
    public FurnitureType type;
    
    // 家具占用的格子形状 - 相对于锚点(0,0)的偏移
    public List<Vector2Int> occupiedCells = new List<Vector2Int> { Vector2Int.zero };
    
    // 渲染相关
    public Vector2 renderOffset = Vector2.zero; // 相对于锚点格子的渲染偏移
    
    [System.NonSerialized] // Sprite不能序列化，运行时使用
    public Sprite sprite; // 直接的Sprite引用（用于ScriptableObject）
    
    public string prefabPath; // prefab路径（保留给自定义家具使用）
    public string spritePath; // 用户自定义图片路径（保留给自定义家具使用）
    public int sortingOrder = 0; // 渲染层级
    
    // 游戏逻辑
    public bool blocksMovement = true;
    
    // 获取家具的包围盒（用于检查放置空间）
    public Vector2Int GetBounds()
    {
        if (occupiedCells.Count == 0) return Vector2Int.one;
        
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;
        
        foreach (var cell in occupiedCells)
        {
            minX = Mathf.Min(minX, cell.x);
            maxX = Mathf.Max(maxX, cell.x);
            minY = Mathf.Min(minY, cell.y);
            maxY = Mathf.Max(maxY, cell.y);
        }
        
        return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
    }
    
    // 检查是否有可用的图片资源
    public bool HasValidSprite()
    {
        return sprite != null || !string.IsNullOrEmpty(spritePath);
    }
}

[System.Serializable]
public class DecorData
{
    public string id;
    public string name;
    public DecorType type;
    public Vector2 renderOffset = Vector2.zero;
    
    [System.NonSerialized] // Sprite不能序列化，运行时使用
    public Sprite sprite; // 直接的Sprite引用（用于ScriptableObject）
    
    public string spritePath; // 用户自定义图片路径
    public string prefabPath; // prefab路径（保留给自定义装饰使用）
    public int sortingOrder = 100;
    
    // 检查是否有可用的图片资源
    public bool HasValidSprite()
    {
        return sprite != null || !string.IsNullOrEmpty(spritePath);
    }
}

public enum FurnitureType
{
    BuiltIn,
    Custom
}

public enum DecorType
{
    BuiltIn,
    Custom
}