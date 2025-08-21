using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNode
{
    public string Id;
    public string NextNodeId;

    // Ö´ÐÐ½Úµã
    public abstract NodeExecResult Execute();

    protected BaseNode(string id, string nextNodeId)
    {
        Id = id;
        NextNodeId = nextNodeId;
    }

}
