
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
        hungry.text = "������" + stateManager.Character.Full.ToString();
        clean.text = "��ࣺ" + stateManager.Character.Clean.ToString();
        san.text = "���ǣ�" + stateManager.Character.San.ToString();
        money.text = "��Ǯ��" + stateManager.Player.Money.ToString();
    }

    void OnDestroy()
    {
        // ȡ�������¼�����ֹ�ڴ�й©
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
                hungry.text = $"������ {value.ToString()}/100";
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
