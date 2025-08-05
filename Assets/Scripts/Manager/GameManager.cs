using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public StateManager StateManager { get; private set; }

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
