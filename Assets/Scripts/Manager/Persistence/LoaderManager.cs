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
}
