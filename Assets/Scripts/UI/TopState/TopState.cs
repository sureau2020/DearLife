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
        hungry.text = "饱腹： "+ stateManager.Character.Full.ToString();
        clean.text = "清洁： "+ stateManager.Character.Clean.ToString();
        san.text = "理智： "+ stateManager.Character.San.ToString();

    }

    void OnDestroy()
    {
        // 取消订阅事件，防止内存泄漏
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
                hungry.text = "饱腹： " + value.ToString();
                break;
            case "Clean":
                clean.text = "清洁： " + value.ToString();
                break;
            case "San":
                san.text = "理智： " + value.ToString();
                break;
        }
    }

}
