
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public static class LoaderManager 
{
    public const string EventsFolderPath = "Assets/GameData/Events";
    public const string ItemsFolderPath = "Assets/GameData/Items";
    
    // 与 SaveManager 相同的设置
    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() },
        SerializationBinder = new SimpleTypeBinder()
    };

    // 自定义类型绑定器（与 SaveManager 相同）
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
                case "DialogueNode":
                    return typeof(DialogueNode);
                case "ChoiceNode":
                    return typeof(ChoiceNode);
                case "ConditionNode":
                    return typeof(ConditionNode);
                case "EffectNode":
                    return typeof(EffectNode);
                case "NavigateNode":
                    return typeof(NavigateNode);
                default:
                    return null;
            }
        }
    }

    // 加载所有事件
    public static void LoadEventsFromFolder()
    {
        if (!Directory.Exists(EventsFolderPath))
        {
            Debug.LogWarning($"事件文件夹不存在：{EventsFolderPath}");
            return;
        }
        
        string[] files = Directory.GetFiles(EventsFolderPath, "*.json", SearchOption.TopDirectoryOnly);
        Debug.Log($"找到 {files.Length} 个事件文件");
        
        foreach (var file in files)
        {
            try
            {
                string json = File.ReadAllText(file);
                EventData eventData = JsonConvert.DeserializeObject<EventData>(json, JsonSettings);
                
                if (eventData != null && !string.IsNullOrEmpty(eventData.EventId))
                {
                    EventDataBase.AddEvent(eventData);
                    
                }
                else
                {
                    Debug.LogWarning($"文件 {file} 解析失败或EventId为空。");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"读取或解析事件文件 {file} 时出错: {ex.Message}");
            }
        }
    }
    
    // 加载所有物品
    public static void LoadItemsFromFolder()
    {
        if (!Directory.Exists(ItemsFolderPath))
        {
            Debug.LogWarning($"物品文件夹不存在：{ItemsFolderPath}");
            return;
        }
        
        string[] files = Directory.GetFiles(ItemsFolderPath, "*.json", SearchOption.TopDirectoryOnly);
        Debug.Log($"找到 {files.Length} 个物品文件");
        
        foreach (var file in files)
        {
            try
            {
                string json = File.ReadAllText(file);
                ItemData itemData = JsonConvert.DeserializeObject<ItemData>(json, JsonSettings);
                
                if (itemData != null && !string.IsNullOrEmpty(itemData.Id))
                {
                    ItemDataBase.AddItem(itemData);
                    
                }
                else
                {
                    Debug.LogWarning($"文件 {file} 解析失败或ItemId为空。");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"读取或解析物品文件 {file} 时出错: {ex.Message}");
            }
        }
    }
    
    // 加载所有数据
    public static void LoadAllData()
    {
        LoadEventsFromFolder();
        LoadItemsFromFolder();
    }
}
