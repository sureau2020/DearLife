
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public static class LoaderManager 
{
    public const string EventsFolderPath = "Assets/GameData/Events";
    public const string ItemsFolderPath = "Assets/GameData/Items";
    
    // �� SaveManager ��ͬ������
    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() },
        SerializationBinder = new SimpleTypeBinder()
    };

    // �Զ������Ͱ������� SaveManager ��ͬ��
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

    // ���������¼�
    public static void LoadEventsFromFolder()
    {
        if (!Directory.Exists(EventsFolderPath))
        {
            Debug.LogWarning($"�¼��ļ��в����ڣ�{EventsFolderPath}");
            return;
        }
        
        string[] files = Directory.GetFiles(EventsFolderPath, "*.json", SearchOption.TopDirectoryOnly);
        Debug.Log($"�ҵ� {files.Length} ���¼��ļ�");
        
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
                    Debug.LogWarning($"�ļ� {file} ����ʧ�ܻ�EventIdΪ�ա�");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"��ȡ������¼��ļ� {file} ʱ����: {ex.Message}");
            }
        }
    }
    
    // ����������Ʒ
    public static void LoadItemsFromFolder()
    {
        if (!Directory.Exists(ItemsFolderPath))
        {
            Debug.LogWarning($"��Ʒ�ļ��в����ڣ�{ItemsFolderPath}");
            return;
        }
        
        string[] files = Directory.GetFiles(ItemsFolderPath, "*.json", SearchOption.TopDirectoryOnly);
        Debug.Log($"�ҵ� {files.Length} ����Ʒ�ļ�");
        
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
                    Debug.LogWarning($"�ļ� {file} ����ʧ�ܻ�ItemIdΪ�ա�");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"��ȡ�������Ʒ�ļ� {file} ʱ����: {ex.Message}");
            }
        }
    }
    
    // ������������
    public static void LoadAllData()
    {
        LoadEventsFromFolder();
        LoadItemsFromFolder();
    }
}
