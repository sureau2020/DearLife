using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Furniture Database", menuName = "Furniture/Furniture Database", order = 0)]
public class FurnitureDataBaseScriptableObject : ScriptableObject
{
    [Header("内置家具")]
    public List<FurnitureScriptableObject> builtinFurnitures = new();
    
    [Header("内置装饰")]
    public List<DecorScriptableObject> builtinDecors = new();
    
    // 获取所有家具数据的字典
    public Dictionary<string, FurnitureData> GetFurnitureDataDictionary()
    {
        var dict = new Dictionary<string, FurnitureData>();
        
        foreach (var furniture in builtinFurnitures)
        {
            if (furniture != null && !string.IsNullOrWhiteSpace(furniture.furnitureId))
            {
                dict[furniture.furnitureId] = furniture.ToFurnitureData();
            }
        }
        
        return dict;
    }
    
    // 获取所有装饰数据的字典
    public Dictionary<string, DecorData> GetDecorDataDictionary()
    {
        var dict = new Dictionary<string, DecorData>();
        
        foreach (var decor in builtinDecors)
        {
            if (decor != null && !string.IsNullOrWhiteSpace(decor.decorId))
            {
                dict[decor.decorId] = decor.ToDecorData();
            }
        }
        
        return dict;
    }
    
    private void OnValidate()
    {
        // 检查重复ID
        var furnitureIds = new HashSet<string>();
        var decorIds = new HashSet<string>();
        
        foreach (var furniture in builtinFurnitures)
        {
            if (furniture != null && !string.IsNullOrWhiteSpace(furniture.furnitureId))
            {
                if (furnitureIds.Contains(furniture.furnitureId))
                {
                    Debug.LogWarning($"重复的家具ID: {furniture.furnitureId}");
                }
                else
                {
                    furnitureIds.Add(furniture.furnitureId);
                }
            }
        }
        
        foreach (var decor in builtinDecors)
        {
            if (decor != null && !string.IsNullOrWhiteSpace(decor.decorId))
            {
                if (decorIds.Contains(decor.decorId))
                {
                    Debug.LogWarning($"重复的装饰ID: {decor.decorId}");
                }
                else
                {
                    decorIds.Add(decor.decorId);
                }
            }
        }
    }
}