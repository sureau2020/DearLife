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

    // 保存 StateManager
    public static OperationResult SaveStateManager(StateManager stateManager)
    {
        try
        {
            if (!Directory.Exists(SaveDataFolder))
                Directory.CreateDirectory(SaveDataFolder);

            // 创建一个简单的包装对象用于序列化
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
            
            // 使用临时文件
            string tempFilePath = filePath + ".tmp";
            File.WriteAllText(tempFilePath, json);
            
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            File.Move(tempFilePath, filePath);
            
            Debug.Log($"StateManager 已保存到：{filePath}");
            return OperationResult.Complete();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"保存 StateManager 失败：{ex.Message}");
            return OperationResult.Fail($"保存失败：{ex.Message}");
        }
    }

    // 别名报错
    public static async System.Threading.Tasks.Task<OperationResult> SaveStateManagerAsync(StateManager stateManager)
    {
        // 等待获取锁，确保同时只有一个保存操作
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

            // 使用临时文件写入，然后重命名（原子操作）
            string tempFilePath = filePath + ".tmp";
            await File.WriteAllTextAsync(tempFilePath, json);
            
            // 如果原文件存在，先删除
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            // 重命名临时文件
            File.Move(tempFilePath, filePath);
            
            Debug.Log($"StateManager 已异步保存到：{filePath}");
            return OperationResult.Complete();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"异步保存 StateManager 失败：{ex.Message}");
            return OperationResult.Fail($"保存失败：{ex.Message}");
        }
        finally
        {
            // 释放锁
            stateSaveSemaphore.Release();
        }
    }

    // 加载 StateManager 数据
    public static OperationResult<StateManagerSaveData> LoadStateManager()
    {
        try
        {
            string filePath = Path.Combine(SaveDataFolder, StateManagerFileName);
            if (!File.Exists(filePath))
            {
                return OperationResult<StateManagerSaveData>.Fail("存档文件不存在"); 
            }

            string json = File.ReadAllText(filePath);
            var saveData = JsonConvert.DeserializeObject<StateManagerSaveData>(json, JsonSettings);
            
            if (saveData == null)
            {
                return OperationResult<StateManagerSaveData>.Fail("存档文件格式错误或损坏");
            }

            Debug.Log($"StateManager 数据已从 {filePath} 加载");
            return OperationResult<StateManagerSaveData>.Complete(saveData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"加载 StateManager 失败：{ex.Message}");
            return OperationResult<StateManagerSaveData>.Fail($"存档文件解析失败：{ex.Message}"); 
        }
    }

    

    // Task 系统保存/加载方法
    private static string GetMonthTaskFilePath(string month)
    {
        if (!Directory.Exists(TaskDataFolder))
            Directory.CreateDirectory(TaskDataFolder);
        return Path.Combine(TaskDataFolder, $"month_{month}.json");
    }

    // 保存单个月份数据
    public static OperationResult SaveMonthTasks(MonthMissionData monthData)
    {
        try
        {
            string json = JsonConvert.SerializeObject(monthData, JsonSettings);
            string filePath = GetMonthTaskFilePath(monthData.Month);
            File.WriteAllText(filePath, json);
            Debug.Log($"保存月份任务数据：{monthData.Month} 到 {filePath}");
            return OperationResult.Complete();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"保存月份任务失败：{ex.Message}");
            return OperationResult.Fail($"保存失败：{ex.Message}");
        }
    }

    // 异步保存单个月份数据
    public static async System.Threading.Tasks.Task<OperationResult> SaveMonthTasksAsync(MonthMissionData monthData)
    {
        try
        {
            string json = await ThreadingTask.Run(() => JsonConvert.SerializeObject(monthData, JsonSettings));
            string filePath = GetMonthTaskFilePath(monthData.Month);
            await File.WriteAllTextAsync(filePath, json);
            Debug.Log($"异步保存月份任务数据：{monthData.Month} 到 {filePath}");
            return OperationResult.Complete();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"异步保存月份任务失败：{ex.Message}");
            return OperationResult.Fail($"保存失败：{ex.Message}");
        }
    }

    // 加载单个月份数据
    public static OperationResult<MonthMissionData> LoadMonthTasks(string month)
    {
        try
        {
            string filePath = GetMonthTaskFilePath(month);
            if (!File.Exists(filePath))
            {
                // 文件不存在，创建新的月份数据
                Debug.Log($"月份任务文件不存在，创建新数据：{month}");
                MonthMissionData newMonth = new MonthMissionData(month);
                return OperationResult<MonthMissionData>.Complete(newMonth);
            }

            string json = File.ReadAllText(filePath);
            MonthMissionData monthData = JsonConvert.DeserializeObject<MonthMissionData>(json, JsonSettings);
            
            if (monthData == null)
            {
                return OperationResult<MonthMissionData>.Fail("月份任务文件格式错误");
            }

            Debug.Log($"成功加载月份任务数据：{month}");
            return OperationResult<MonthMissionData>.Complete(monthData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"加载月份任务失败：{ex.Message}");
            return OperationResult<MonthMissionData>.Fail($"加载失败：{ex.Message}");
        }
    }

    // 保存所有已加载的月份数据
    public static OperationResult SaveAllMonthTasks(Dictionary<string, MonthMissionData> monthMap)
    {
        try
        {
            foreach (var kvp in monthMap)
            {
                var result = SaveMonthTasks(kvp.Value);
                if (!result.Success)
                {
                    return result; // 如果任何一个月份保存失败，返回错误
                }
            }
            Debug.Log($"成功保存所有月份任务数据，共 {monthMap.Count} 个月份");
            return OperationResult.Complete();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"保存所有月份任务失败：{ex.Message}");
            return OperationResult.Fail($"保存失败：{ex.Message}");
        }
    }

    // 异步保存所有已加载的月份数据
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
                    return result; // 如果任何一个月份保存失败，返回错误
                }
            }

            Debug.Log($"异步保存所有月份任务数据完成，共 {monthMap.Count} 个月份");
            return OperationResult.Complete();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"异步保存所有月份任务失败：{ex.Message}");
            return OperationResult.Fail($"保存失败：{ex.Message}");
        }
    }
}

// 简单的序列化包装类
public class StateManagerSaveData
{
    public PlayerData Player { get; set; }
    public CharacterData Character { get; set; }
    public GameSettings Settings { get; set; }
    public Dictionary<string, int> CustomStates { get; set; }
    public System.DateTime SaveTime { get; set; }
}
