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

    // 存储外部传入参数
    private Dictionary<string, int> parameters = new();

    public void SetParameters(Dictionary<string, int> param)
    {
        parameters = param;
    }

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
            var result = node.Execute(parameters);

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
                        StartEvent(newEvent);
                    }
                    advancing = false;
                    break;
            }
        }
    }

    public void OnClickNext(string nextNodeId = null)
    {
        currentNodeId = nextNodeId ?? currentNodeId;
        Continue();
    }

    public void OnChoiceSelected(string nextNodeId)
    {
        currentNodeId = nextNodeId;
        Continue();
    }
}
