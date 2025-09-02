using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public StateManager StateManager { get; private set; }
    public DialogueManager DialogueManager {get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 尝试加载存档
        Debug.Log("开始尝试加载存档...");
        var loadResult = StateManager.LoadState();
        if (loadResult.Success)
        {
            StateManager = loadResult.Data;
            Debug.Log("存档加载成功");
        }
        else
        {
            Debug.Log($"加载失败：{loadResult.Message}");
            if (loadResult.Message.Contains("存档文件不存在"))
            {
                Debug.Log("使用默认初始化");
                InitializeDefaultState();
            }
            else
            {
                ErrorNotifier.NotifyError($"存档文件损坏：{loadResult.Message}");
                return; 
            }
        }

        DialogueManager = GetComponent<DialogueManager>();
        LoaderManager.LoadAllData();
    }

 
    private void InitializeDefaultState()
    {
        CharacterData characterData;
        
        if (BootSceneManager.Instance != null && BootSceneManager.Instance.CreatedCharacter != null)
        {
            characterData = BootSceneManager.Instance.CreatedCharacter;
        }
        else
        {
            characterData = new CharacterData();
        }

        PlayerData playerData = new PlayerData(10); // 初始金钱10
        GameSettings settings = new GameSettings();
        Dictionary<string, int> customStates = new Dictionary<string, int>();

        StateManager = new StateManager(playerData, characterData, settings, customStates);
    }

    private void Start()
    {
        TimeManager.Instance.OnMinuteChanged += OnMinuteChanged;
        TimeManager.Instance.OnHourChanged += OnHourChanged;
        TimeManager.Instance.OnDayChanged += OnDayChanged;
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged -= OnMinuteChanged;
            TimeManager.Instance.OnHourChanged -= OnHourChanged;
            TimeManager.Instance.OnDayChanged -= OnDayChanged;
        }
    }


    public OperationResult UseItemWithDialogue(string itemId, int quantity) { 
        OperationResult result = StateManager.UseItem(itemId, quantity);
        if (!result.Success) {
            return result;
        }

        return DialogueManager.ShowRandomItemDialogue(StateManager.Settings.ReplyChance, itemId);
    }


    // 获取全局需要的状态值，给对话分支用的
    public int GetValueOfState(string stateName) {
        switch(stateName) {
            case "Full":
                return StateManager.Character.Full;
            case "Clean":
                return StateManager.Character.Clean;
            case "San":
                return StateManager.Character.San;
            case "Love":
                return StateManager.Character.Love;
            case "Money":
                return StateManager.Player.Money;
            default:
                return StateManager.GetCustomState(stateName);
        }
    }

    //使用对话中产生的效果+
    public void SetValueOfState(string stateName, int value) {
        switch(stateName) {
            case "Full":
                StateManager.Character.ChangeFull(value);
                break;
            case "Clean":
                StateManager.Character.ChangeClean(value);
                break;
            case "San":
                StateManager.Character.ChangeSan(value);
                break;
            case "Love":
                StateManager.Character.ChangeLove(value);
                break;
            case "Money":
                StateManager.Player.EarnMoney(value);
                break;
            default:
                StateManager.SetCustomState(stateName, value);
                break;
        }
    }

    public string GetSetting(string key) { 
        switch(key) {
            case "MaxSalaryFactor": return StateManager.Settings.MaxSalaryFactor.ToString("F2");
            case "HourlyWage": return StateManager.Settings.HourlyWage.ToString();
            case "DifficultyBonus": return StateManager.Settings.DifficultyBonus.ToString();
            case "ReplyChance": return StateManager.Settings.ReplyChance.ToString();
            default: return "未知设置项";
        }
    }

    public OperationResult ChangeSetting(string key, string value) {
        if (key == "MaxSalaryFactor") {
            if (float.TryParse(value, out float factor))
            {
                return StateManager.Settings.ChangeMaxRandomFactor(factor);
            }
            else
            {
                return OperationResult.Fail("请输入合法的小数");
            }
        }
        if (int.TryParse(value, out int var)) {
            switch (key) {
                case "HourlyWage": return StateManager.Settings.ChangeHourlyWage(var);
                case "DifficultyBonus": return StateManager.Settings.ChangeDifficultyBonus(var);
                case "ReplyChance": return StateManager.Settings.ChangeReplyChance(var);
                default: return OperationResult.Fail("未知设置项");
            }
        }
        else {
            return OperationResult.Fail("请输入合法的整数");
        }
    }


    private void OnMinuteChanged(DateTime now)
    {
        Debug.Log("一分钟过去了，当前时间：" + now.ToString("HH:mm"));
        StateManager.DecayStates();
        // 这里可以刷新UI或发通知
    }

    private void OnHourChanged(DateTime now)
    {
        Debug.Log("一小时过去了，当前时间：" + now.ToString("HH:mm"));
        // 每小时额外逻辑，比如任务倒计时等
    }

    private void OnDayChanged(DateTime now)
    {
        Debug.Log("新一天开始：" + now.ToString("yyyy-MM-dd"));
        // 每天刷新任务、状态等
    }

    public OperationResult SaveGame()
    {
        return StateManager.SaveState();
    }

    

    public OperationResult LoadGame()
    {
        var loadResult = StateManager.LoadState();
        if (loadResult.Success)
        {
            StateManager = loadResult.Data;
            Debug.Log("游戏重新加载成功");
            return OperationResult.Complete();
        }
        else
        {
            Debug.LogError($"重新加载失败：{loadResult.Message}");
            return OperationResult.Fail(loadResult.Message);
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && StateManager != null)
        {
            SaveGame(); 
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && StateManager != null)
        {
            SaveGame(); 
        }
    }
}
