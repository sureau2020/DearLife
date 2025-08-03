using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopState : MonoBehaviour
{
    private TextMeshProUGUI hungry;
    private TextMeshProUGUI clean;
    private TextMeshProUGUI san;
    private StateManager stateManager;

    // Start is called before the first frame update
    void Start()
    {
        hungry = transform.Find("Hungry").GetComponent<TextMeshProUGUI>();
        clean = transform.Find("Clean").GetComponent<TextMeshProUGUI>();
        san = transform.Find("San").GetComponent<TextMeshProUGUI>();

        stateManager = GameObject.Find("GameManager").GetComponent<GameManager>().StateManager;
        hungry.text = "������ "+ stateManager.Character.Full.ToString();
        clean.text = "��ࣺ "+ stateManager.Character.Clean.ToString();
        san.text = "���ǣ� "+ stateManager.Character.San.ToString();

    }

    void OnDestroy()
    {
        // ȡ�������¼�����ֹ�ڴ�й©
        if (stateManager != null)
        {
            stateManager.OnCharacterStateChanged -= OnCharacterStateChangedHandler;
        }
    }

    private void OnCharacterStateChangedHandler(string statName, int value)
    {
        switch (statName)
        {
            case "Full":
                hungry.text = "������ " + value.ToString();
                break;
            case "Clean":
                clean.text = "��ࣺ " + value.ToString();
                break;
            case "San":
                san.text = "���ǣ� " + value.ToString();
                break;
        }
    }

}
