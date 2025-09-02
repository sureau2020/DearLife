using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using System.Threading;
using ThreadingTask = System.Threading.Tasks.Task;

public static class SaveManager
{
    private const string EventsFolder = "Assets/GameData/Events";
    private const string ItemsFolder = "Assets/GameData/Items";
    private const string SaveDataFolder = "Assets/GameData/SaveData";
    private const string StateManagerFileName = "StateManager.json";
    private const string TaskDataFolder = "Assets/GameData/TaskData";
    private static readonly SemaphoreSlim stateSaveSemaphore = new SemaphoreSlim(1, 1);


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

    // ���� StateManager
    public static OperationResult SaveStateManager(StateManager stateManager)
    {
        try
        {
            if (!Directory.Exists(SaveDataFolder))
                Directory.CreateDirectory(SaveDataFolder);

            // ����һ���򵥵İ�װ�����������л�
            var saveData = new
            {
                Player = stateManager.Player,
                Character = stateManager.Character,
                Settings = stateManager.Settings,
                CustomStates = stateManager.CustomStates,
                SaveTime = System.DateTime.Now
            };

            string json = JsonConvert.SerializeObject(saveData, JsonSettings);
            string filePath = Path.Combine(SaveDataFolder, StateManagerFileName);
            
            // ʹ����ʱ�ļ�
            string tempFilePath = filePath + ".tmp";
            File.WriteAllText(tempFilePath, json);
            
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            File.Move(tempFilePath, filePath);
            
            Debug.Log($"StateManager �ѱ��浽��{filePath}");
            return OperationResult.Complete();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"���� StateManager ʧ�ܣ�{ex.Message}");
            return OperationResult.Fail($"����ʧ�ܣ�{ex.Message}");
        }
    }

    // ��������
    public static async System.Threading.Tasks.Task<OperationResult> SaveStateManagerAsync(StateManager stateManager)
    {
        // �ȴ���ȡ����ȷ��ͬʱֻ��һ���������
        await stateSaveSemaphore.WaitAsync();
        
        try
        {
            if (!Directory.Exists(SaveDataFolder))
                Directory.CreateDirectory(SaveDataFolder);

            var saveData = new
            {
                Player = stateManager.Player,
                Character = stateManager.Character,
                Settings = stateManager.Settings,
                CustomStates = stateManager.CustomStates,
                SaveTime = System.DateTime.Now
            };

            string json = await System.Threading.Tasks.Task.Run(() =>
                JsonConvert.SerializeObject(saveData, JsonSettings));
            string filePath = Path.Combine(SaveDataFolder, StateManagerFileName);

            // ʹ����ʱ�ļ�д�룬Ȼ����������ԭ�Ӳ�����
            string tempFilePath = filePath + ".tmp";
            await File.WriteAllTextAsync(tempFilePath, json);
            
            // ���ԭ�ļ����ڣ���ɾ��
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            // ��������ʱ�ļ�
            File.Move(tempFilePath, filePath);
            
            Debug.Log($"StateManager ���첽���浽��{filePath}");
            return OperationResult.Complete();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"�첽���� StateManager ʧ�ܣ�{ex.Message}");
            return OperationResult.Fail($"����ʧ�ܣ�{ex.Message}");
        }
        finally
        {
            // �ͷ���
            stateSaveSemaphore.Release();
        }
    }

    // ���� StateManager ����
    public static OperationResult<StateManagerSaveData> LoadStateManager()
    {
        try
        {
            string filePath = Path.Combine(SaveDataFolder, StateManagerFileName);
            if (!File.Exists(filePath))
            {
                return OperationResult<StateManagerSaveData>.Fail("�浵�ļ�������"); 
            }

            string json = File.ReadAllText(filePath);
            var saveData = JsonConvert.DeserializeObject<StateManagerSaveData>(json, JsonSettings);
            
            if (saveData == null)
            {
                return OperationResult<StateManagerSaveData>.Fail("�浵�ļ���ʽ�������");
            }

            Debug.Log($"StateManager �����Ѵ� {filePath} ����");
            return OperationResult<StateManagerSaveData>.Complete(saveData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"���� StateManager ʧ�ܣ�{ex.Message}");
            return OperationResult<StateManagerSaveData>.Fail($"�浵�ļ�����ʧ�ܣ�{ex.Message}"); 
        }
    }

    

    // Task ϵͳ����/���ط���
    private static string GetMonthTaskFilePath(string month)
    {
        if (!Directory.Exists(TaskDataFolder))
            Directory.CreateDirectory(TaskDataFolder);
        return Path.Combine(TaskDataFolder, $"month_{month}.json");
    }

    // ���浥���·�����
    public static OperationResult SaveMonthTasks(MonthMissionData monthData)
    {
        try
        {
            string json = JsonConvert.SerializeObject(monthData, JsonSettings);
            string filePath = GetMonthTaskFilePath(monthData.Month);
            File.WriteAllText(filePath, json);
            Debug.Log($"�����·��������ݣ�{monthData.Month} �� {filePath}");
            return OperationResult.Complete();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"�����·�����ʧ�ܣ�{ex.Message}");
            return OperationResult.Fail($"����ʧ�ܣ�{ex.Message}");
        }
    }

    // �첽���浥���·�����
    public static async System.Threading.Tasks.Task<OperationResult> SaveMonthTasksAsync(MonthMissionData monthData)
    {
        try
        {
            string json = await ThreadingTask.Run(() => JsonConvert.SerializeObject(monthData, JsonSettings));
            string filePath = GetMonthTaskFilePath(monthData.Month);
            await File.WriteAllTextAsync(filePath, json);
            Debug.Log($"�첽�����·��������ݣ�{monthData.Month} �� {filePath}");
            return OperationResult.Complete();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"�첽�����·�����ʧ�ܣ�{ex.Message}");
            return OperationResult.Fail($"����ʧ�ܣ�{ex.Message}");
        }
    }

    // ���ص����·�����
    public static OperationResult<MonthMissionData> LoadMonthTasks(string month)
    {
        try
        {
            string filePath = GetMonthTaskFilePath(month);
            if (!File.Exists(filePath))
            {
                // �ļ������ڣ������µ��·�����
                Debug.Log($"�·������ļ������ڣ����������ݣ�{month}");
                MonthMissionData newMonth = new MonthMissionData(month);
                return OperationResult<MonthMissionData>.Complete(newMonth);
            }

            string json = File.ReadAllText(filePath);
            MonthMissionData monthData = JsonConvert.DeserializeObject<MonthMissionData>(json, JsonSettings);
            
            if (monthData == null)
            {
                return OperationResult<MonthMissionData>.Fail("�·������ļ���ʽ����");
            }

            Debug.Log($"�ɹ������·��������ݣ�{month}");
            return OperationResult<MonthMissionData>.Complete(monthData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"�����·�����ʧ�ܣ�{ex.Message}");
            return OperationResult<MonthMissionData>.Fail($"����ʧ�ܣ�{ex.Message}");
        }
    }

    // ���������Ѽ��ص��·�����
    public static OperationResult SaveAllMonthTasks(Dictionary<string, MonthMissionData> monthMap)
    {
        try
        {
            foreach (var kvp in monthMap)
            {
                var result = SaveMonthTasks(kvp.Value);
                if (!result.Success)
                {
                    return result; // ����κ�һ���·ݱ���ʧ�ܣ����ش���
                }
            }
            Debug.Log($"�ɹ����������·��������ݣ��� {monthMap.Count} ���·�");
            return OperationResult.Complete();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"���������·�����ʧ�ܣ�{ex.Message}");
            return OperationResult.Fail($"����ʧ�ܣ�{ex.Message}");
        }
    }

    // �첽���������Ѽ��ص��·�����
    public static async System.Threading.Tasks.Task<OperationResult> SaveAllMonthTasksAsync(Dictionary<string, MonthMissionData> monthMap)
    {
        try
        {
            var tasks = new List<System.Threading.Tasks.Task<OperationResult>>();
            foreach (var kvp in monthMap)
            {
                tasks.Add(SaveMonthTasksAsync(kvp.Value));
            }

            var results = await ThreadingTask.WhenAll(tasks);
            
            foreach (var result in results)
            {
                if (!result.Success)
                {
                    return result; // ����κ�һ���·ݱ���ʧ�ܣ����ش���
                }
            }

            Debug.Log($"�첽���������·�����������ɣ��� {monthMap.Count} ���·�");
            return OperationResult.Complete();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"�첽���������·�����ʧ�ܣ�{ex.Message}");
            return OperationResult.Fail($"����ʧ�ܣ�{ex.Message}");
        }
    }
}

// �򵥵����л���װ��
public class StateManagerSaveData
{
    public PlayerData Player { get; set; }
    public CharacterData Character { get; set; }
    public GameSettings Settings { get; set; }
    public Dictionary<string, int> CustomStates { get; set; }
    public System.DateTime SaveTime { get; set; }
}
