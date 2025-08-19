
public class NodeExecResult 
{
    public NodeExecResultType Type;
    public object Payload;
}

public enum NodeExecResultType
{
    Advance,    
    ShowDialogue,  
    ShowChoices,
    NavigateEvent,
    EndEvent,       
}
