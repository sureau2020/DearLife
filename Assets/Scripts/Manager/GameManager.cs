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

        // ��ʼ������,ĿǰӲ����
        PlayerData playerData = new PlayerData(10);// ��ʼ��Ǯ10
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
}
