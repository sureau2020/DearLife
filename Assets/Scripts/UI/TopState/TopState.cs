
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TopState : MonoBehaviour
{
    [SerializeField] private StatusBar full;
    [SerializeField] private StatusBar clean;
    [SerializeField] private StatusBar san;
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private StatusBar love;
    private int yOffset = 610;
    private float slideInDuration = 0.5f;
    private StateManager stateManager;
    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
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
        love.UpdateLoveBar(stateManager.Character.Love);
        if (rectTransform != null)
        {
            Vector2 cur = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = new Vector2(cur.x, yOffset);
            rectTransform.DOAnchorPosY(30f, slideInDuration).SetEase(Ease.OutCubic);
        }
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
                love.UpdateLoveBar(stateManager.Character.Love);
                break;
        }
    }

    private void OnMoneyChangedHandler(int newMoney)
    {
        money.text = newMoney.ToString();
    }

}
