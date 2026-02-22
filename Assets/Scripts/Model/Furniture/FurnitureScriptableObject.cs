using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Furniture", menuName = "Furniture/Furniture Data", order = 1)]
public class FurnitureScriptableObject : ScriptableObject
{
    [Header("基础信息")]
    public string furnitureId;
    public string furnitureName;
    public FurnitureType furnitureType;
    
    [Header("占用格子")]
    [Tooltip("家具占用的格子形状，相对于锚点(0,0)的偏移")]
    public List<Vector2Int> occupiedCells = new List<Vector2Int> { Vector2Int.zero };
    
    [Header("渲染设置")]
    [Tooltip("家具的Sprite图片")]
    public Sprite furnitureSprite;
    
    [Tooltip("相对于锚点格子的渲染偏移")]
    public Vector2 renderOffset = Vector2.zero;
    
    [Tooltip("排序层级")]
    public int sortingOrder = 0;
    
    [Header("游戏逻辑")]
    [Tooltip("是否阻挡移动")]
    public bool blocksMovement = true;
    
    // 转换为运行时数据
    public FurnitureData ToFurnitureData()
    {
        return new FurnitureData
        {
            id = furnitureId,
            displayName = furnitureName,
            type = furnitureType,
            occupiedCells = new List<Vector2Int>(occupiedCells),
            renderOffset = renderOffset,
            prefabPath = "", // 不再使用prefab
            spritePath = "", // 不再使用路径，直接用Sprite引用
            sortingOrder = sortingOrder,
            blocksMovement = blocksMovement,
            sprite = furnitureSprite // 添加Sprite引用
        };
    }
    
    private void OnValidate()
    {
        // 确保ID不为空
        if (string.IsNullOrWhiteSpace(furnitureId))
        {
            furnitureId = name.ToLower().Replace(" ", "_");
        }
        
        // 确保至少有一个占用格子
        if (occupiedCells == null || occupiedCells.Count == 0)
        {
            occupiedCells = new List<Vector2Int> { Vector2Int.zero };
        }
    }
}