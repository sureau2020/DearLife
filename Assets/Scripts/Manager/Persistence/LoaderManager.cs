using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;

public static class LoaderManager
{
    private const string EventsIndexFile = "GameData/events_index.json";
    private const string ItemsIndexFile = "GameData/items_index.json";

    private const string EventsFolder = "GameData/Events";
    private const string ItemsFolder = "GameData/Items";

    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() },
        SerializationBinder = new SimpleTypeBinder()
    };

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
        yield return LoadItems();
    }

    private static IEnumerator LoadEvents()
    {
        yield return LoadByIndex(EventsIndexFile, EventsFolder, (json) =>
        {
            EventData data = JsonConvert.DeserializeObject<EventData>(json, JsonSettings);
            if (data != null) EventDataBase.AddEvent(data);
        });
        Debug.Log("事件加载完成");
    }

    private static IEnumerator LoadItems()
    {
        yield return LoadByIndex(ItemsIndexFile, ItemsFolder, (json) =>
        {
            ItemData data = JsonConvert.DeserializeObject<ItemData>(json, JsonSettings);
            if (data != null) ItemDataBase.AddItem(data);
        });
        Debug.Log("物品加载完成");
    }

    private static IEnumerator LoadByIndex(string indexFile, string folder, System.Action<string> onJsonLoaded)
    {
        string indexPath = Path.Combine(Application.streamingAssetsPath, indexFile);
        string indexJson = null;

#if UNITY_ANDROID
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

#if UNITY_ANDROID
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
}
