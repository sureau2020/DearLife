using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUi : MonoBehaviour
{
    private string itemId;

    public void SetItemId(string id)
    {
        itemId = id;
    }

    public void OnItemClick()
    {
        OperationResult result = GameManager.Instance.UseItemWithDialogue(itemId, 1);
        if (!result.Success)
        {
            ErrorNotifier.NotifyError(result.Message);
        }
        else
        {
            SoundManager.Instance.PlaySfx("LittleType");
            // �ɹ�ʹ����Ʒ�󣬸��±���UI
            BackPackUI backpackUI = gameObject.transform.parent.GetComponent<BackPackUI>();
            if (backpackUI != null)
            {
                backpackUI.RefreshBackPackUI();
            }
        }
    }
}
