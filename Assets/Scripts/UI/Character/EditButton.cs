using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditButton : MonoBehaviour
{
    public void OnClick() { 
        BootSceneManager.Instance.EnterEditModeFromMain();
    }
}
