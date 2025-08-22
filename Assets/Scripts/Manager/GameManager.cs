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
        DontDestroyOnLoad(gameObject);

        // 初始化数据,目前硬编码
        PlayerData playerData = new PlayerData(10);// 初始金钱10
        CharacterData characterData = new CharacterData();
        GameSettings settings = new GameSettings();
        Dictionary<string, int> customStates = new Dictionary<string, int>
        {
            { "A", 100 },
            { "Thirst", 100 },
            { "Fatigue", 0 }
        };

        StateManager = new StateManager(playerData, characterData, settings, customStates);
        DialogueManager = GetComponent<DialogueManager>();
        LoaderManager.LoadAllData();
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
}
