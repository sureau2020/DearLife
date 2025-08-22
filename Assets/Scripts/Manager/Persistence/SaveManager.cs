
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public static class SaveManager
{
    private const string EventsFolder = "Assets/GameData/Events";
    private const string ItemsFolder = "Assets/GameData/Items";
    
    // 简化序列化选项，不需要引用处理
    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() },
        SerializationBinder = new SimpleTypeBinder()
    };

    // 自定义类型绑定器
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
    
    // 保存单个事件
    public static void SaveEvent(EventData eventData)
    {
        if (!Directory.Exists(EventsFolder))
            Directory.CreateDirectory(EventsFolder);
            
        string filePath = Path.Combine(EventsFolder, $"{eventData.EventId}.json");
        string json = JsonConvert.SerializeObject(eventData, JsonSettings);
        File.WriteAllText(filePath, json);
        Debug.Log($"保存事件：{eventData.EventId} 到 {filePath}");
    }
    
    // 保存单个物品
    public static void SaveItem(ItemData itemData)
    {
        if (!Directory.Exists(ItemsFolder))
            Directory.CreateDirectory(ItemsFolder);
            
        string filePath = Path.Combine(ItemsFolder, $"{itemData.Id}.json");
        string json = JsonConvert.SerializeObject(itemData, JsonSettings);
        File.WriteAllText(filePath, json);
        Debug.Log($"保存物品：{itemData.Id} 到 {filePath}");
    }
    
    // 导出当前所有事件
    public static void ExportAllEvents()
    {
        foreach (var eventId in EventDataBase.GetAllEventIds())
        {
            EventData eventData = EventDataBase.GetEvent(eventId);
            if (eventData != null)
                SaveEvent(eventData);
        }
        Debug.Log("已导出所有事件");
    }
    
    // 导出当前所有物品
    public static void ExportAllItems()
    {
        foreach (var item in ItemDataBase.GetAllItems())
        {
            if (item != null)
                SaveItem(item);
        }
        Debug.Log("已导出所有物品");
    }
}
