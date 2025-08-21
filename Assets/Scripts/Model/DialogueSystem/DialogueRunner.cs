using System;
using System.Collections.Generic;

public class DialogueRunner
{
    public EventData CurrentEvent { get; private set; }
    private string currentNodeId;

    public event Action<DialoguePayload,DialogueType> OnShowDialogue;
    public event Action<DialogueType> StartDialogue;
    public event Action<List<ChoiceOption>> OnShowChoices;
    public event Action OnDialogueEnd;


    public void StartEvent(EventData eventData)
    {
        CurrentEvent = eventData;
        currentNodeId = eventData.StartNodeId;
        StartDialogue?.Invoke(eventData.Type);
        Continue();
    }

    private void Continue()
    {
        bool advancing = true;
        while (advancing)
        {
            if (string.IsNullOrEmpty(currentNodeId))
            {
                OnDialogueEnd?.Invoke();
                return;
            }

            var node = CurrentEvent.Nodes[currentNodeId];
            var result = node.Execute();

            switch (result.Type)
            {
                case NodeExecResultType.ShowDialogue:
                    OnShowDialogue?.Invoke((DialoguePayload)result.Payload, CurrentEvent.Type);
                    advancing = false;
                    break;

                case NodeExecResultType.ShowChoices:
                    OnShowChoices?.Invoke((List<ChoiceOption>)result.Payload);
                    advancing = false;
                    break;

                case NodeExecResultType.EndEvent:
                    OnDialogueEnd?.Invoke();
                    advancing = false;
                    break;

                case NodeExecResultType.Advance:
                    currentNodeId = result.Payload as string ?? node.NextNodeId;
                    break;

                case NodeExecResultType.NavigateEvent:
                    string targetEventId = result.Payload as string;
                    if (!string.IsNullOrEmpty(targetEventId))
                    {
                        var newEvent = EventDataBase.GetEvent(targetEventId);
                        if (newEvent == null)
                        {
                            ErrorNotifier.NotifyError($"无法找到事件：{targetEventId}，检查当前事件的跳转node是否有问题");
                            return;
                        }
                        CurrentEvent = newEvent;
                        currentNodeId = newEvent.StartNodeId;
                        StartDialogue?.Invoke(newEvent.Type);
                    }
                    break;
            }
        }
    }

    public void OnClickNext(string nextNodeId)
    {
        currentNodeId = nextNodeId;
        Continue();
    }

}
