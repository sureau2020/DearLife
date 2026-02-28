using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestMap : MonoBehaviour
{
    public void OnResetMapButtonClicked()
    {
        Debug.Log("REQUESTED");
        ConfirmRequestManager.Request(
        content: "确定要将小屋恢复到初始状态吗？",
        onConfirm: () => GameManager.Instance.ResetRoom()
    );
    }
}
