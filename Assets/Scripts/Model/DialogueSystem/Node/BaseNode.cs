using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNode
{
    public string Id;
    public string NextNodeId;

    // ִ�нڵ�
    public abstract NodeExecResult Execute(Dictionary<string, int> parameters);

    protected BaseNode(string id, string nextNodeId)
    {
        Id = id;
        NextNodeId = nextNodeId;
    }

}
