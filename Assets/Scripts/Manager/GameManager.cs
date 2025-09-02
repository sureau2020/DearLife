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

        // ���Լ��ش浵
        Debug.Log("��ʼ���Լ��ش浵...");
        var loadResult = StateManager.LoadState();
        if (loadResult.Success)
        {
            StateManager = loadResult.Data;
            Debug.Log("�浵���سɹ�");
        }
        else
        {
            Debug.Log($"����ʧ�ܣ�{loadResult.Message}");
            if (loadResult.Message.Contains("�浵�ļ�������"))
            {
                Debug.Log("ʹ��Ĭ�ϳ�ʼ��");
                InitializeDefaultState();
            }
            else
            {
                ErrorNotifier.NotifyError($"�浵�ļ��𻵣�{loadResult.Message}");
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

        PlayerData playerData = new PlayerData(10); // ��ʼ��Ǯ10
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


    // ��ȡȫ����Ҫ��״ֵ̬�����Ի���֧�õ�
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

    //ʹ�öԻ��в�����Ч��+
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
            default: return "δ֪������";
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
                return OperationResult.Fail("������Ϸ���С��");
            }
        }
        if (int.TryParse(value, out int var)) {
            switch (key) {
                case "HourlyWage": return StateManager.Settings.ChangeHourlyWage(var);
                case "DifficultyBonus": return StateManager.Settings.ChangeDifficultyBonus(var);
                case "ReplyChance": return StateManager.Settings.ChangeReplyChance(var);
                default: return OperationResult.Fail("δ֪������");
            }
        }
        else {
            return OperationResult.Fail("������Ϸ�������");
        }
    }


    private void OnMinuteChanged(DateTime now)
    {
        Debug.Log("һ���ӹ�ȥ�ˣ���ǰʱ�䣺" + now.ToString("HH:mm"));
        StateManager.DecayStates();
        // �������ˢ��UI��֪ͨ
    }

    private void OnHourChanged(DateTime now)
    {
        Debug.Log("һСʱ��ȥ�ˣ���ǰʱ�䣺" + now.ToString("HH:mm"));
        // ÿСʱ�����߼����������񵹼�ʱ��
    }

    private void OnDayChanged(DateTime now)
    {
        Debug.Log("��һ�쿪ʼ��" + now.ToString("yyyy-MM-dd"));
        // ÿ��ˢ������״̬��
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
            Debug.Log("��Ϸ���¼��سɹ�");
            return OperationResult.Complete();
        }
        else
        {
            Debug.LogError($"���¼���ʧ�ܣ�{loadResult.Message}");
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
