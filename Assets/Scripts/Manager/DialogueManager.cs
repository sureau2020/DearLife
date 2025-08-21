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
            return OperationResult.Complete();//û��������¼���ֱ�ӷ��سɹ�
        }
        EventData eventData = EventDataBase.GetEvent(eventId);
        if (eventData == null) {
            return OperationResult.Fail($"�¼���{eventId} û�ҵ�����������¼�id�Ƿ����󣬼���¼����ݿ��Ƿ���á�");
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


    // REQUIRE: itemID��Ӧ��item������null�������GameManager���Ѿ���֤���ˣ�0<=p<=5
    // ʹ����Ʒʱ����ʹ����Ʒ,����setting��ĸ�����������Ƿ��жԻ����жԻ��Ļ�����Ի������ؽ��
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
