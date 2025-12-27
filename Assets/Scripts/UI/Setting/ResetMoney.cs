using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetMoney : MonoBehaviour
{
    public void OnResetMoneyButtonClicked()
    {
        Debug.Log("REQUESTED" );
        ConfirmRequestManager.Request(
        content: "确定要将金币清零吗？",
        onConfirm: () => GameManager.Instance.StateManager.Resetmoney()
    );
    }
}
