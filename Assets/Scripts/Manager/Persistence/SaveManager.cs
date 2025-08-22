
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public static class SaveManager
{
    private const string EventsFolder = "Assets/GameData/Events";
    private const string ItemsFolder = "Assets/GameData/Items";
    
    // �����л�ѡ�����Ҫ���ô���
    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() },
        SerializationBinder = new SimpleTypeBinder()
    };

    // �Զ������Ͱ���
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
    
    // ���浥���¼�
    public static void SaveEvent(EventData eventData)
    {
        if (!Directory.Exists(EventsFolder))
            Directory.CreateDirectory(EventsFolder);
            
        string filePath = Path.Combine(EventsFolder, $"{eventData.EventId}.json");
        string json = JsonConvert.SerializeObject(eventData, JsonSettings);
        File.WriteAllText(filePath, json);
        Debug.Log($"�����¼���{eventData.EventId} �� {filePath}");
    }
    
    // ���浥����Ʒ
    public static void SaveItem(ItemData itemData)
    {
        if (!Directory.Exists(ItemsFolder))
            Directory.CreateDirectory(ItemsFolder);
            
        string filePath = Path.Combine(ItemsFolder, $"{itemData.Id}.json");
        string json = JsonConvert.SerializeObject(itemData, JsonSettings);
        File.WriteAllText(filePath, json);
        Debug.Log($"������Ʒ��{itemData.Id} �� {filePath}");
    }
    
    // ������ǰ�����¼�
    public static void ExportAllEvents()
    {
        foreach (var eventId in EventDataBase.GetAllEventIds())
        {
            EventData eventData = EventDataBase.GetEvent(eventId);
            if (eventData != null)
                SaveEvent(eventData);
        }
        Debug.Log("�ѵ��������¼�");
    }
    
    // ������ǰ������Ʒ
    public static void ExportAllItems()
    {
        foreach (var item in ItemDataBase.GetAllItems())
        {
            if (item != null)
                SaveItem(item);
        }
        Debug.Log("�ѵ���������Ʒ");
    }
}
