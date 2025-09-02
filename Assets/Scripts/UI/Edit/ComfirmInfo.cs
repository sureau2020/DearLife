using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComfirmInfo : MonoBehaviour { 

    [SerializeField] private List<Toggle> toggles;
    [SerializeField] private GameObject InputInfoPanel;

    // Start is called before the first frame update
    private void OnEnable()
    {
        if (BootSceneManager.Instance.PersonalityTags.Count <= 0) return;
        foreach (var toggle in toggles)
        {
            if (BootSceneManager.Instance.PersonalityTags.Contains(toggle.name))
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.isOn = false;
            }
        }
    }


    public void OnComfirmButtonClicked()
    {
        SoundManager.Instance.PlaySfx("Click");
        foreach (var toggle in toggles)
        {
            if (toggle.isOn)
            {
                BootSceneManager.Instance.PersonalityTags.Add(toggle.name);
            }
        }
        InputInfoPanel.SetActive(false);
    }
}
