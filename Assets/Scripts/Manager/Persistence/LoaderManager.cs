using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class LoaderManager
{
    private const string EventsListFile = "GameData/all_events_list.json";
    private const string ItemsListFile = "GameData/all_items_list.json";

    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() },
        SerializationBinder = new SimpleTypeBinder()
    };

    private static readonly string SaveDataFolder = Path.Combine(Application.persistentDataPath, "SaveData");
    private static readonly string WardrobeFilePath = Path.Combine(SaveDataFolder, "Wardrobe.json");
    private static readonly string GridMapFilePath = Path.Combine(Application.persistentDataPath, "SaveData", "GridMap.json");

    public class SimpleTypeBinder : ISerializationBinder
    {
        public void BindToName(System.Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public System.Type BindToType(string assemblyName, string typeName)
        {
            switch (typeName)
            {
                case "DialogueNode": return typeof(DialogueNode);
                case "ChoiceNode": return typeof(ChoiceNode);
                case "ConditionNode": return typeof(ConditionNode);
                case "EffectNode": return typeof(EffectNode);
                case "NavigateNode": return typeof(NavigateNode);
                default: return null;
            }
        }
    }

    [System.Serializable]
    private class FileIndex
    {
        public List<string> files;
    }

    public static IEnumerator LoadAllData()
    {
        yield return LoadEvents();
        yield return LoadWardrobeFromSave();
        yield return LoadItems();
    }

    private static IEnumerator LoadEvents()
    {
        string relativePath = EventsListFile;
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
        string json = null;

#if UNITY_ANDROID || UNITY_IOS
        using (UnityWebRequest www = UnityWebRequest.Get(fullPath))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                json = www.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"加载事件文件失败: {fullPath}  错误: {www.error}");
                yield break;
            }
        }
#else
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"找不到事件文件: {fullPath}");
            yield break;
        }

        try
        {
            json = File.ReadAllText(fullPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"读取事件文件失败: {ex.Message}");
            yield break;
        }
#endif
        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogError("all_events_list.json 内容为空");
            yield break;
        }
        try
        {
            var events = JsonConvert.DeserializeObject<List<EventData>>(json, JsonSettings);
            foreach (var ev in events)
            {
                if (ev != null) EventDataBase.AddEvent(ev);
            }

            Debug.Log($"事件加载完成，共 {events.Count} 条");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"解析 all_events_list.json 失败: {ex.Message}");
        }

        yield break;
    }

    private static IEnumerator LoadItems()
    {
        string relativePath = ItemsListFile;
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
        string json = null;

#if UNITY_ANDROID || UNITY_IOS
        using (UnityWebRequest www = UnityWebRequest.Get(fullPath))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                json = www.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"加载物品文件失败: {fullPath}  错误: {www.error}");
                yield break;
            }
        }
#else
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"找不到物品文件: {fullPath}");
            yield break;
        }

        try
        {
            json = File.ReadAllText(fullPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"读取物品文件失败: {ex.Message}");
            yield break;
        }
#endif

        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogError("all_items_list.json 内容为空");
            yield break;
        }

        try
        {
            var items = JsonConvert.DeserializeObject<List<ItemData>>(json, JsonSettings);
            if (items == null)
            {
                Debug.LogError("解析 all_items_list.json 失败：不是 ItemData 数组格式");
                yield break;
            }

            foreach (var item in items)
            {
                if (item != null) ItemDataBase.AddItem(item);
            }

            Debug.Log($"物品加载完成，共 {items.Count} 件");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"解析 all_items_list.json 失败: {ex.Message}");
        }

        yield break;
    }

    private static IEnumerator LoadByIndex(string indexFile, string folder, System.Action<string> onJsonLoaded)
    {
        string indexPath = Path.Combine(Application.streamingAssetsPath, indexFile);
        string indexJson = null;

#if UNITY_ANDROID || UNITY_IOS
        using (UnityWebRequest www = UnityWebRequest.Get(indexPath))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
                indexJson = www.downloadHandler.text;
            else
                Debug.LogError($"加载索引文件失败: {indexPath}");
        }
#else
        indexJson = File.ReadAllText(indexPath);
#endif

        if (string.IsNullOrEmpty(indexJson)) yield break;

        FileIndex index = JsonConvert.DeserializeObject<FileIndex>(indexJson);
        foreach (var file in index.files)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, folder, file);

#if UNITY_ANDROID || UNITY_IOS
            using (UnityWebRequest www = UnityWebRequest.Get(fullPath))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    string json = www.downloadHandler.text;
                    onJsonLoaded?.Invoke(json);
                }
                else
                {
                    Debug.LogError($"加载文件失败: {fullPath}");
                }
            }
#else
            string json = File.ReadAllText(fullPath);
            onJsonLoaded?.Invoke(json);
#endif
        }
    }

    private static IEnumerator LoadSingleFile(string relativePath, System.Action<string> onJsonLoaded)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
        string json = null;

#if UNITY_ANDROID || UNITY_IOS
        using (UnityWebRequest www = UnityWebRequest.Get(fullPath))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
                json = www.downloadHandler.text;
            else
                Debug.LogError($"加载文件失败: {fullPath}");
        }
#else
    if (File.Exists(fullPath))
        json = File.ReadAllText(fullPath);
    else
        Debug.LogError($"文件不存在: {fullPath}");
#endif

        if (!string.IsNullOrEmpty(json))
            onJsonLoaded?.Invoke(json);
    }

    private static IEnumerator LoadWardrobe()
    {
        yield return LoadSingleFile(WardrobeFilePath, (json) =>
        {
            var slots = JsonConvert.DeserializeObject<Dictionary<string, WardrobeSlot>>(json, JsonSettings);
            if (slots != null)
            {
                foreach (var kv in slots)
                {
                    if (kv.Value != null)
                        WardrobeData.AddCloth(kv.Value);
                }
                Debug.Log($"衣柜数据加载完成，共 {slots.Count} 件");
            }
            else
            {
                Debug.LogError("衣柜文件格式错误或为空，创建默认数据");
                CreateDefaultWardrobeData();
            }
        });
    }

    // 加载衣柜数据（服装）
    public static IEnumerator LoadWardrobeFromSave()
    {
        Debug.Log("开始加载衣柜数据");
        string filePath = WardrobeFilePath;
        Debug.Log($"衣柜文件路径: {filePath}");

        bool needLoadFromStreaming = false;

        if (!File.Exists(filePath))
        {
            needLoadFromStreaming = true;
        }
        else
        {
            string jsonFromSave = null;
            try
            {
                jsonFromSave = File.ReadAllText(filePath);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"读取衣柜文件失败: {ex.Message}");
                needLoadFromStreaming = true;
            }

            if (string.IsNullOrEmpty(jsonFromSave))
            {
                needLoadFromStreaming = true;
            }
            else
            {
                var slotsFromSave = JsonConvert.DeserializeObject<Dictionary<string, WardrobeSlot>>(jsonFromSave, JsonSettings);
                if (slotsFromSave != null && slotsFromSave.Count > 0)
                {
                    foreach (var kv in slotsFromSave)
                    {
                        if (kv.Value != null)
                            WardrobeData.AddCloth(kv.Value);
                    }
                    Debug.Log($"衣柜数据加载完成，共{slotsFromSave.Count}件");
                    yield break;
                }
                else
                {
                    needLoadFromStreaming = true;
                }
            }
            
        }

        if (needLoadFromStreaming)
        {
            Debug.Log("存档无效，尝试从StreamingAssets加载初始衣柜数据");
            string streamingPath = Path.Combine(Application.streamingAssetsPath, "GameData/clothes.json");
            string json = null;

#if UNITY_ANDROID
            using (UnityWebRequest www = UnityWebRequest.Get(streamingPath))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    json = www.downloadHandler.text;
                }
                else
                {
                    Debug.LogError("找不到初始衣柜数据文件: " + streamingPath);
                    yield break;
                }
            }
#else
            if (File.Exists(streamingPath))
            {
                json = File.ReadAllText(streamingPath);
            }
            else
            {
                Debug.LogError("找不到初始衣柜数据文件: " + streamingPath);
                yield break;
            }
#endif

            var slots = JsonConvert.DeserializeObject<Dictionary<string, WardrobeSlot>>(json, JsonSettings);
            if (slots != null)
            {
                foreach (var kv in slots)
                {
                    if (kv.Value != null)
                        WardrobeData.AddCloth(kv.Value);
                }
                SaveManager.SaveWardrobe(slots);
                Debug.Log($"已从StreamingAssets加载初始衣柜数据，共{slots.Count}件");
            }
            else
            {
                Debug.LogError("初始衣柜数据格式错误或为空");
            }
        }
    }

    private static void CreateDefaultWardrobeData()
    {
        // 创建默认衣柜数据
        var defaultSlots = new Dictionary<string, WardrobeSlot>();
        
        // 保存默认数据
        SaveManager.SaveWardrobe(defaultSlots);
        Debug.Log("创建并保存默认衣柜数据");
    }

    // 加载 GridMap
    public static OperationResult<GridMap> LoadGridMap()
    {
        try
        {
            if (!File.Exists(GridMapFilePath))
            {
                Debug.Log("GridMap 存档文件不存在，将创建新的 GridMap");
                return OperationResult<GridMap>.Fail("存档文件不存在");
            }

            string json = File.ReadAllText(GridMapFilePath);
            var saveData = JsonConvert.DeserializeObject<SaveManager.GridMapSaveData>(json, JsonSettings);
            
            if (saveData == null)
            {
                Debug.LogError("GridMap 存档格式错误");
                return OperationResult<GridMap>.Fail("存档格式错误");
            }

            // 创建新的 GridMap 实例（不使用默认初始化）
            var gridMap = CreateEmptyGridMap();
            
            // 恢复 CameraLimitMax
            typeof(GridMap).GetProperty("CameraLimitMax").SetValue(gridMap, saveData.CameraLimitMax);
            
            // 恢复家具实例
            var furnitureField = typeof(GridMap).GetField("furnitureInstances", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var furnitureInstances = (Dictionary<string, FurnitureInstance>)furnitureField.GetValue(gridMap);
            
            foreach (var furnitureSave in saveData.FurnitureInstances.Values)
            {
                var furniture = new FurnitureInstance
                {
                    instanceId = furnitureSave.InstanceId,
                    furnitureDataId = furnitureSave.FurnitureDataId,
                    anchorPos = furnitureSave.AnchorPos,
                    occupiedCells = new List<Vector2Int>(furnitureSave.OccupiedCells)
                };
                furnitureInstances[furniture.instanceId] = furniture;
            }
            
            // 恢复装饰实例
            var decorField = typeof(GridMap).GetField("decorInstances", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var decorInstances = (Dictionary<string, DecorInstance>)decorField.GetValue(gridMap);
            
            foreach (var decorSave in saveData.DecorInstances.Values)
            {
                var decor = new DecorInstance
                {
                    instanceId = decorSave.InstanceId,
                    decorId = decorSave.DecorId,
                    position = decorSave.Position
                };
                decorInstances[decor.instanceId] = decor;
            }
            
            // 恢复 Cell 数据
            foreach (var cellPair in saveData.Cells)
            {
                var coords = cellPair.Key.Split(',');
                var pos = new Vector2Int(int.Parse(coords[0]), int.Parse(coords[1]));
                var cellSave = cellPair.Value;
                
                ref CellData cell = ref gridMap.GetCellRef(pos);
                cell.flags = cellSave.Flags;
                cell.furnitureInstanceId = cellSave.FurnitureInstanceId;
                cell.floorTileId = cellSave.FloorTileId;
                cell.decorInstanceId = cellSave.DecorInstanceId;
            }
            
            // 恢复 ID 计数器
            var nextFurnitureIdField = typeof(GridMap).GetField("nextFurnitureInstanceId", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            nextFurnitureIdField.SetValue(gridMap, saveData.NextFurnitureInstanceId);
            
            var nextDecorIdField = typeof(GridMap).GetField("nextDecorInstanceId", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            nextDecorIdField.SetValue(gridMap, saveData.NextDecorInstanceId);
            
            Debug.Log($"GridMap 成功从 {GridMapFilePath} 加载");
            return OperationResult<GridMap>.Complete(gridMap);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"加载 GridMap 失败：{ex.Message}");
            return OperationResult<GridMap>.Fail($"加载失败：{ex.Message}");
        }
    }

    // 创建空的 GridMap（跳过默认初始化）
    private static GridMap CreateEmptyGridMap()
    {
        // 使用反射创建 GridMap 实例，避免调用构造函数中的初始化逻辑
        var gridMap = System.Activator.CreateInstance<GridMap>();
        
        // 手动初始化必要的字段
        var worldField = typeof(GridMap).GetField("world", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        worldField.SetValue(gridMap, new ChunkWorld());
        
        var furnitureField = typeof(GridMap).GetField("furnitureInstances", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        furnitureField.SetValue(gridMap, new Dictionary<string, FurnitureInstance>());
        
        var decorField = typeof(GridMap).GetField("decorInstances", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        decorField.SetValue(gridMap, new Dictionary<string, DecorInstance>());
        
        // 初始化数据库引用
        var tileField = typeof(GridMap).GetField("tileDataBase", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        tileField.SetValue(gridMap, GameManager.Instance.TileDataBase);
        
        var furnitureDbField = typeof(GridMap).GetField("furnitureDatabase", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        furnitureDbField.SetValue(gridMap, GameManager.Instance.FurnitureDatabase);
        
        return gridMap;
    }
}
