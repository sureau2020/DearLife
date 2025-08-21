using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{

    private DialogueRunner runner;
    [SerializeField] private DialoguePanelUI dailyDialoguePanel;
    [SerializeField] private DialoguePanelUI characterDialoguePanel;
    [SerializeField] private ChoicesUI choicePanelUI;
    [SerializeField] private BottomButton randomDailyEventGenerator;

    void Awake()
    {
        runner = new DialogueRunner();
        runner.OnShowDialogue += HandleShowDialogue;
        runner.OnShowChoices += HandleShowChoices;
        runner.OnDialogueEnd += HandleEnd;
        runner.StartDialogue += HandleStart;
        dailyDialoguePanel.OnNextClicked += HandleAdvance;
        characterDialoguePanel.OnNextClicked += HandleAdvance;
        choicePanelUI.OnChoiceClicked += HandleAdvance;
        randomDailyEventGenerator.randomDailyEvent += StartRandomDailyDialogue;
    }

    void OnDestroy()
    {
        runner.OnShowDialogue -= HandleShowDialogue;
        runner.OnShowChoices -= HandleShowChoices;
        runner.OnDialogueEnd -= HandleEnd;
        runner.StartDialogue -= HandleStart;
        dailyDialoguePanel.OnNextClicked -= HandleAdvance;
        characterDialoguePanel.OnNextClicked -= HandleAdvance;
        choicePanelUI.OnChoiceClicked -= HandleAdvance;
        randomDailyEventGenerator.randomDailyEvent -= StartRandomDailyDialogue;
    }


    public OperationResult StartDialogue(string eventId) {
        if (eventId == null)
        {
            return OperationResult.Complete();//没有随机到事件，直接返回成功
        }
        EventData eventData = EventDataBase.GetEvent(eventId);
        if (eventData == null) {
            return OperationResult.Fail($"事件：{eventId} 没找到，检查物体事件id是否有误，检查事件数据库是否完好。");
        }
        runner.StartEvent(eventData);
        return OperationResult.Complete();
    }

    public void StartRandomDailyDialogue() {
        string eventId = Calculators.RandomEvent(EventDataBase.GetCandidateDailyEventIds());
        OperationResult result = StartDialogue(eventId);
        if (!result.Success) {
            ErrorNotifier.NotifyError(result.Message);
        }
    }


    // REQUIRE: itemID对应的item不等于null，这点在GameManager中已经验证过了，0<=p<=5
    // 使用物品时，先使用物品,根据setting里的概率随机决定是否有对话，有对话的话随机对话，返回结果
    public OperationResult ShowRandomItemDialogue(int p, string itemId) {
        if (p <= 1 || !Calculators.RandomChance(p))
        {
            return OperationResult.Complete();
        }
        else { 
            ItemData item = ItemDataBase.GetItemById(itemId);
            return StartDialogue(Calculators.RandomEvent(item.FilteredEventIds));
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
                if (dailyDialoguePanel.isActiveAndEnabled)
                {
                    dailyDialoguePanel.ShowDialogue(payload);
                }
                else { 
                    characterDialoguePanel.ShowDialogue(payload);
                }
                break;
            case DialogueType.Item:
                if (characterDialoguePanel.isActiveAndEnabled)
                {
                    characterDialoguePanel.ShowDialogue(payload);
                }
                else {
                    dailyDialoguePanel.ShowDialogue(payload);
                }
                break;
            default:
                dailyDialoguePanel.gameObject.SetActive(true);
                dailyDialoguePanel.ShowDialogue(payload);
                break;
        }
    }

    private void HandleShowChoices(List<ChoiceOption> choices) {
        choicePanelUI.gameObject.SetActive(true);
        choicePanelUI.GenerateChoices(choices);
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
