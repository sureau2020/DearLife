using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class FurnitureDatabase 
{
    private readonly Dictionary<string, FurnitureData> allFurnitures = new();
    private readonly Dictionary<string, DecorData> allDecors = new();

    // 用户自定义数据路径
    private string userFurnitureDataPath => Path.Combine(Application.persistentDataPath, "UserFurnitureData.json");
    private string userDecorDataPath => Path.Combine(Application.persistentDataPath, "UserDecorData.json");

    public FurnitureDatabase()
    {
        LoadBuiltinData();
        //LoadUserData();
    }

    private void LoadBuiltinData()
    {
        var allFurnitureAssets = Resources.LoadAll<FurnitureScriptableObject>("Furnitures");

        foreach (var furniture in allFurnitureAssets)
        {
            if (!string.IsNullOrWhiteSpace(furniture.furnitureId))
            {
                allFurnitures[furniture.furnitureId] = new FurnitureData
                {
                    id = furniture.furnitureId,
                    name = furniture.furnitureName,
                    type = furniture.furnitureType,
                    occupiedCells = new List<Vector2Int>(furniture.occupiedCells),
                    renderOffset = furniture.renderOffset,
                    sprite = furniture.furnitureSprite,
                    sortingOrder = furniture.sortingOrder,
                    blocksMovement = furniture.blocksMovement,
                    prefabPath = "", // ScriptableObject不使用prefab
                    spritePath = "" // ScriptableObject不使用路径
                };
            }
        }

        var allDecorAssets = Resources.LoadAll<DecorScriptableObject>("Decors");

        foreach (var decor in allDecorAssets)
        {
            if (!string.IsNullOrWhiteSpace(decor.decorId))
            {
                allDecors[decor.decorId] = new DecorData
                {
                    id = decor.decorId,
                    name = decor.decorName,
                    type = decor.decorType,
                    renderOffset = decor.renderOffset,
                    sprite = decor.decorSprite,
                    sortingOrder = decor.sortingOrder,
                    prefabPath = "", // ScriptableObject不使用prefab
                    spritePath = "" // ScriptableObject不使用路径
                };
            }
        }

        Debug.Log($"已加载 {allFurnitures.Count} 个内置家具, {allDecors.Count} 个内置装饰");
    }
    
    private void LoadUserData()
    {
        // 加载用户自定义的家具数据
        if (File.Exists(userFurnitureDataPath))
        {
            try
            {
                string json = File.ReadAllText(userFurnitureDataPath);
                var userFurniture = JsonConvert.DeserializeObject<Dictionary<string, FurnitureData>>(json);
                if (userFurniture != null)
                {
                    foreach (var item in userFurniture)
                    {
                        allFurnitures[item.Key] = item.Value;
                    }
                    Debug.Log($"已加载 {userFurniture.Count} 个用户自定义家具");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"加载用户家具数据失败: {e.Message}");
            }
        }
        
        // 加载用户自定义的装饰数据
        if (File.Exists(userDecorDataPath))
        {
            try
            {
                string json = File.ReadAllText(userDecorDataPath);
                var userDecor = JsonConvert.DeserializeObject<Dictionary<string, DecorData>>(json);
                if (userDecor != null)
                {
                    foreach (var item in userDecor)
                    {
                        allDecors[item.Key] = item.Value;
                    }
                    Debug.Log($"已加载 {userDecor.Count} 个用户自定义装饰");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"加载用户装饰数据失败: {e.Message}");
            }
        }
    }
    
    public void SaveUserData()
    {
        try
        {
            // 分离用户自定义数据并保存
            var userFurniture = new Dictionary<string, FurnitureData>();
            var userDecor = new Dictionary<string, DecorData>();
            
            foreach (var item in allFurnitures)
            {
                if (item.Value.type == FurnitureType.Custom)
                    userFurniture[item.Key] = item.Value;
            }
            
            foreach (var item in allDecors)
            {
                if (item.Value.type == DecorType.Custom)
                    userDecor[item.Key] = item.Value;
            }
            
            string furnitureJson = JsonConvert.SerializeObject(userFurniture, Formatting.Indented);
            string decorJson = JsonConvert.SerializeObject(userDecor, Formatting.Indented);
            
            File.WriteAllText(userFurnitureDataPath, furnitureJson);
            File.WriteAllText(userDecorDataPath, decorJson);
            
            Debug.Log($"已保存 {userFurniture.Count} 个用户家具和 {userDecor.Count} 个用户装饰");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存用户数据失败: {e.Message}");
        }
    }
    
    // 公共访问方法
    public FurnitureData GetFurnitureData(string id) => allFurnitures.GetValueOrDefault(id);
    public DecorData GetDecorData(string id) => allDecors.GetValueOrDefault(id);
    
    public void AddCustomFurniture(FurnitureData data)
    {
        if (data == null) return;
        
        data.type = FurnitureType.Custom;
        allFurnitures[data.id] = data;
    }
    
    public void AddCustomDecor(DecorData data)
    {
        if (data == null) return;
        
        data.type = DecorType.Custom;
        allDecors[data.id] = data;
    }
    
    public bool RemoveFurniture(string id)
    {
        return allFurnitures.Remove(id);
    }
    
    public bool RemoveDecor(string id)
    {
        return allDecors.Remove(id);
    }
    
    // 获取所有家具列表（用于编辑器或UI显示）
    public IReadOnlyList<FurnitureData> GetAllFurniture()
    {
        return new List<FurnitureData>(allFurnitures.Values);
    }
    
    public IReadOnlyList<DecorData> GetAllDecors()
    {
        return new List<DecorData>(allDecors.Values);
    }
    
    // 按类型获取家具
    public IReadOnlyList<FurnitureData> GetFurnitureByType(FurnitureType type)
    {
        var result = new List<FurnitureData>();
        foreach (var item in allFurnitures.Values)
        {
            if (item.type == type)
                result.Add(item);
        }
        return result;
    }
    
    // 按类型获取装饰
    public IReadOnlyList<DecorData> GetDecorByType(DecorType type)
    {
        var result = new List<DecorData>();
        foreach (var item in allDecors.Values)
        {
            if (item.type == type)
                result.Add(item);
        }
        return result;
    }
    
    // 检查家具是否存在
    public bool HasFurniture(string id) => allFurnitures.ContainsKey(id);
    public bool HasDecor(string id) => allDecors.ContainsKey(id);
    
    // 获取数量统计
    public int FurnitureCount => allFurnitures.Count;
    public int DecorCount => allDecors.Count;
    
    // 验证数据完整性
    public bool ValidateData()
    {
        bool isValid = true;
        
        // 检查家具数据
        foreach (var furniture in allFurnitures.Values)
        {
            if (string.IsNullOrWhiteSpace(furniture.id))
            {
                Debug.LogError($"发现无效家具ID: {furniture.name}");
                isValid = false;
            }
            
            if (furniture.occupiedCells == null || furniture.occupiedCells.Count == 0)
            {
                Debug.LogError($"家具 {furniture.id} 缺少占用格子定义");
                isValid = false;
            }
            
            if (!furniture.HasValidSprite())
            {
                Debug.LogWarning($"家具 {furniture.id} 缺少有效的Sprite资源");
            }
        }
        
        // 检查装饰数据
        foreach (var decor in allDecors.Values)
        {
            if (string.IsNullOrWhiteSpace(decor.id))
            {
                Debug.LogError($"发现无效装饰ID: {decor.name}");
                isValid = false;
            }
            
            if (!decor.HasValidSprite())
            {
                Debug.LogWarning($"装饰 {decor.id} 缺少有效的Sprite资源");
            }
        }
        
        return isValid;
    }
}