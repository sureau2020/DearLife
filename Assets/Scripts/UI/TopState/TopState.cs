
using TMPro;
using UnityEngine;

public class TopState : MonoBehaviour
{
    private TextMeshProUGUI hungry;
    private TextMeshProUGUI clean;
    private TextMeshProUGUI san;
    private TextMeshProUGUI money;
    private StateManager stateManager;

    // Start is called before the first frame update
    void Start()
    {
        hungry = transform.Find("Hungry").GetComponent<TextMeshProUGUI>();
        clean = transform.Find("Clean").GetComponent<TextMeshProUGUI>();
        san = transform.Find("San").GetComponent<TextMeshProUGUI>();
        money = transform.Find("Money").GetComponent<TextMeshProUGUI>();

        stateManager = GameManager.Instance.StateManager;
        stateManager.Character.OnCharacterStateChanged += OnCharacterStateChangedHandler;

        InitializeStateTexts();
    }

    private void InitializeStateTexts()
    {
        hungry.text = "饱腹：" + stateManager.Character.Full.ToString();
        clean.text = "清洁：" + stateManager.Character.Clean.ToString();
        san.text = "理智：" + stateManager.Character.San.ToString();
        money.text = "金钱：" + stateManager.Player.Money.ToString();
    }

    void OnDestroy()
    {
        // 取消订阅事件，防止内存泄漏
        if (stateManager.Character != null)
        {
            stateManager.Character.OnCharacterStateChanged -= OnCharacterStateChangedHandler;
        }
    }

    private void OnCharacterStateChangedHandler(string statName, int value)
    {
        Debug.Log($"State changed: {statName} = {value}");
        switch (statName)
        {
            case "Full":
                hungry.text = $"饱腹： {value.ToString()}/100";
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
