using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelChanger : MonoBehaviour
{
    [SerializeField] Transform backpackTab;
    [SerializeField] Transform infoTab;
    [SerializeField] GameObject info;
    [SerializeField] GameObject backpack;

    private bool isBackPackTabHigh = false;
    private bool isInfoTabHigh = true;
    private int yOffset = 10;

    public void ShowBackpack()
    {
        SoundManager.Instance.PlaySfx("SmallClick");
        backpack.SetActive(true);
        info.SetActive(false);
        if (!isBackPackTabHigh)
        {
            MoveToHigh(backpackTab);
            isBackPackTabHigh = true;
        }
        if (isInfoTabHigh)
        {
            MoveToLow(infoTab);
            isInfoTabHigh = false;
        }
    }

    public void ShowInfo()
    {
        SoundManager.Instance.PlaySfx("SmallClick");
        backpack.SetActive(false);
        info.SetActive(true);
        if (!isInfoTabHigh)
        {
            MoveToHigh(infoTab);
            isInfoTabHigh = true;
        }
        if (isBackPackTabHigh)
        {
            MoveToLow(backpackTab);
            isBackPackTabHigh = false;
        }
    }

    public void MoveToHigh(Transform obj) {
        Vector3 newPos = obj.position;
        newPos.y += yOffset;
        obj.position = newPos;
    }

    public void MoveToLow(Transform obj)
    {
        Vector3 newPos = obj.position;
        newPos.y -= yOffset;
        obj.position = newPos;
    }


}
