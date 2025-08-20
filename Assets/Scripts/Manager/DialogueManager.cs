using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{

    private DialogueRunner runner;
    [SerializeField] private DialoguePanelUI dailyDialoguePanel;
    [SerializeField] private DialoguePanelUI characterDialoguePanel;
    [SerializeField] private ChoicePanelUI choicePanelUI;

    void Awake()
    {
        runner = new DialogueRunner();
        runner.OnShowDialogue += HandleShowDialogue;
        runner.OnShowChoices += HandleShowChoices;
        runner.OnDialogueEnd += HandleEnd;
        runner.StartDialogue += HandleStart;
        dailyDialoguePanel.OnNextClicked += HandleAdvance;
        characterDialoguePanel.OnNextClicked += HandleAdvance;
    }

    void OnDestroy()
    {
        runner.OnShowDialogue -= HandleShowDialogue;
        runner.OnShowChoices -= HandleShowChoices;
        runner.OnDialogueEnd -= HandleEnd;
        runner.StartDialogue -= HandleStart;
        dailyDialoguePanel.OnNextClicked -= HandleAdvance;
        characterDialoguePanel.OnNextClicked -= HandleAdvance;
    }


    public OperationResult StartDialogue(string eventId, Dictionary<string, int> parameters) {
        if (parameters != null) {
            runner.SetParameters(parameters);
        }
        EventData eventData = EventDataBase.GetEvent(eventId);
        if (eventData == null) {
            return OperationResult.Fail($"事件：{eventId} 没找到，检查物体事件id是否有误，检查事件数据库是否完好。");
        }
        if (runner != null) {
            Debug.Log($"开始对话：{eventId}");
        }
        runner.StartEvent(eventData);
        return OperationResult.Complete();
    }


    // REQUIRE: itemID对应的item不等于null，这点在GameManager中已经验证过了，0<=p<=5
    // 使用物品时，先使用物品,根据setting里的概率随机决定是否有对话，有对话的话随机对话，返回结果
    // todo item id 随机event ，传过去id
    public OperationResult ShowRandomItemDialogue(int p, string itemId, Dictionary<string, int> parameters) {
        if (p <= 1 || !Calculators.RandomChance(p))
        {
            return OperationResult.Complete();
        }
        else { 
            ItemData item = ItemDataBase.GetItemById(itemId);
            return StartDialogue(Calculators.RandomEvent(item.Events), parameters);
        }
    }

    private void HandleStart(DialogueType type) {
        switch(type) {
            case DialogueType.Daily:
                dailyDialoguePanel.gameObject.SetActive(true);
                break;
            case DialogueType.Item:
                characterDialoguePanel.gameObject.SetActive(true);
                break;
            default:
                dailyDialoguePanel.gameObject.SetActive(true);
                break;
        }
    }

    private void HandleShowDialogue(DialoguePayload payload, DialogueType type) { 
        switch(type) {
            case DialogueType.Daily:
                dailyDialoguePanel.ShowDialogue(payload);
                break;
            case DialogueType.Item:
                characterDialoguePanel.ShowDialogue(payload);
                break;
            default:
                dailyDialoguePanel.ShowDialogue(payload);
                break;
        }
    }

    private void HandleShowChoices(List<ChoiceOption> choices) { 
    
    }

    private void HandleEnd() { 
        dailyDialoguePanel.gameObject.SetActive(false);
        characterDialoguePanel.gameObject.SetActive(false);
        choicePanelUI.gameObject.SetActive(false);
    }

    public void HandleAdvance(string nextNodeId)
    {
        runner.OnClickNext(nextNodeId);
    }

}
