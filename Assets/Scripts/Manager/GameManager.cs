using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public StateManager StateManager { get; private set; }
    public DialogueManager DialogueManager {get; private set; }
    public RoomManager RoomManager { get; private set; }

    public FurnitureDatabase FurnitureDatabase { get; private set; } 
    public TileDataBase TileDataBase { get; private set; }

    public MapDatabase MapDataBase { get; private set; } 


    [SerializeField]private RebirthUI rebirthUI;
    [SerializeField] private GameObject loadingUI;
    [SerializeField] private CharacterAppearanceRenderer appearanceRenderer;

    private void Awake()
    {
        FurnitureDatabase = new FurnitureDatabase();
        TileDataBase = new TileDataBase();
        MapDataBase = new MapDatabase();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 尝试加载存档
        var loadResult = StateManager.LoadState();
        if (loadResult.Success)
        {
            StateManager = loadResult.Data;
            BootSceneManager.Instance.Appearance = StateManager.Character.Appearance;
            
            CalculatePassTime();
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
        RoomManager = GetComponent<RoomManager>();//TODO暂时不用管加载，弄完后加载得放在LoadAndCloseUI里
        StartCoroutine(LoadAndCloseUI());
    }

    IEnumerator LoadAndCloseUI()
    {
        yield return StartCoroutine(LoaderManager.LoadAllData());
        loadingUI.SetActive(false);
        appearanceRenderer.LoadAndApplyAppearance();
    }


    public void CalculatePassTime()
    {
        DateTime now = DateTime.Now;
        TimeSpan timePassed = now - StateManager.SaveTime;
        int totalMinutes = (int)timePassed.TotalMinutes;
        totalMinutes = totalMinutes / 12;
        for (int i = 0; i < (totalMinutes-1); i++)
        {
            StateManager.DecayStatesWithoutNotify();
            if (StateManager.Character.HealthState == HealthState.Dead)
            {
                break;
            }
        }
        StateManager.DecayStates();
        
        StateManager.SaveTime = now;
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
        StateManager = new StateManager(playerData, characterData, settings, customStates, DateTime.Now);
    }

    private void Start()
    {

        if (StateManager.Character.HealthState == HealthState.Dead && rebirthUI != null)
        {
            rebirthUI.ShowRebirthUI();
        }
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
        GameSettings tmp = StateManager.Settings;
        if (tmp.IsAIEnabled)
        {
            return DialogueManager.ShowAIItemDialogue(StateManager.Settings.ReplyChance, itemId, StateManager.Character);
        }
        else {
            return DialogueManager.ShowRandomItemDialogue(StateManager.Settings.ReplyChance, itemId);
        }
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

    public void ResetRoom() {
        RoomManager.ResetRoom();
    }

    public string GetSetting(string key) { 
        switch(key) {
            case "MaxSalaryFactor": return StateManager.Settings.MaxSalaryFactor.ToString("F2");
            case "HourlyWage": return StateManager.Settings.HourlyWage.ToString();
            case "DifficultyBonus": return StateManager.Settings.DifficultyBonus.ToString();
            case "ReplyChance": return StateManager.Settings.ReplyChance.ToString();
            case "API": return StateManager.Settings.API;
            case "Key": return StateManager.Settings.Key;
            case "Model": return StateManager.Settings.Model;
            case "Prompt": return StateManager.Settings.Prompt;
            case "isAIEnabled": return StateManager.Settings.IsAIEnabled.ToString();
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
        if (key == "isAIEnabled") {
            if (bool.TryParse(value, out bool isEnabled))
            {
                return StateManager.Settings.ChangeAIEnabled(isEnabled);
            }
            else
            {
                return OperationResult.Fail("卧槽你咋搞到这个错误的");
            }
        }
        if (int.TryParse(value, out int var)) {
            switch (key) {
                case "HourlyWage": return StateManager.Settings.ChangeHourlyWage(var);
                case "DifficultyBonus": return StateManager.Settings.ChangeDifficultyBonus(var);
                case "ReplyChance": return StateManager.Settings.ChangeReplyChance(var);
                case "API": return StateManager.Settings.ChangeAPI(value);
                case "Key": return StateManager.Settings.ChangeKey(value);
                case "Model": return StateManager.Settings.ChangeModel(value);
                case "Prompt": return StateManager.Settings.ChangePrompt(value);
                default: return OperationResult.Fail("未知设置项");
            }
        }
        else {
            switch (key) {
                case "API": return StateManager.Settings.ChangeAPI(value);
                case "Key": return StateManager.Settings.ChangeKey(value);
                case "Model": return StateManager.Settings.ChangeModel(value);
                case "Prompt": return StateManager.Settings.ChangePrompt(value);
                default: return OperationResult.Fail("请输入合法的整数");
            }
        }
    }


    private void OnMinuteChanged(DateTime now)
    {
        StateManager.DecayStates();
        // 这里可以刷新UI或发通知
    }

    private void OnHourChanged(DateTime now)
    {
    }

    private void OnDayChanged(DateTime now)
    {
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
