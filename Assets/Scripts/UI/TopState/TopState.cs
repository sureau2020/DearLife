
using TMPro;
using UnityEngine;

public class TopState : MonoBehaviour
{
    [SerializeField] private StatusBar full;
    [SerializeField] private StatusBar clean;
    [SerializeField] private StatusBar san;
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private StatusBar love;
    private StateManager stateManager;

    // Start is called before the first frame update
    void Start()
    {
        stateManager = GameManager.Instance.StateManager;
        stateManager.Character.OnCharacterStateChanged += OnCharacterStateChangedHandler;
        stateManager.Player.OnMoneyChanged += OnMoneyChangedHandler;

        InitializeState();
    }

    private void InitializeState()
    {
        full.UpdateBar(stateManager.Character.Full);
        clean.UpdateBar(stateManager.Character.Clean);
        san.UpdateBar(stateManager.Character.San);
        money.text = stateManager.Player.Money.ToString();
        love.UpdateBar(stateManager.Character.Love);
    }

    void OnDestroy()
    {
        // 取消订阅事件，防止内存泄漏
        if (stateManager.Character != null)
        {
            stateManager.Character.OnCharacterStateChanged -= OnCharacterStateChangedHandler;
            stateManager.Player.OnMoneyChanged -= OnMoneyChangedHandler;
        }
    }

    private void OnCharacterStateChangedHandler(string statName, int value)
    {
        Debug.Log($"State changed: {statName} = {value}");
        switch (statName)
        {
            case "Full":
                full.UpdateBar(stateManager.Character.Full);
                break;
            case "Clean":
                clean.UpdateBar(stateManager.Character.Clean);
                break;
            case "San":
                san.UpdateBar(stateManager.Character.San);
                break;
            case "Love":
                love.UpdateBar(stateManager.Character.Love);
                break;
        }
    }

    private void OnMoneyChangedHandler(int newMoney)
    {
        money.text = "金钱：" + newMoney.ToString();
    }

}
