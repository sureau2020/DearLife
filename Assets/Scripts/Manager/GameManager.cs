using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        StateManager = new StateManager(playerData, characterData, settings);
        DialogueManager = GetComponent<DialogueManager>();
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


    // parameterûд�أ�����զ��
    public OperationResult UseItemWithDialogue(string itemId, int quantity) { 
        OperationResult result = StateManager.UseItem(itemId, quantity);
        if (!result.Success) {
            return result;
        }

        return DialogueManager.ShowRandomItemDialogue(StateManager.Settings.ReplyChance, itemId, null);
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
