using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComfirmInfo : MonoBehaviour { 

    [SerializeField] private List<Toggle> toggles;
    [SerializeField] private TextMeshProUGUI nameLabel;
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
        nameLabel.text = BootSceneManager.Instance.CharacterName;
        foreach (var toggle in toggles)
        {
            if (toggle.isOn)
            {
                BootSceneManager.Instance.PersonalityTags.Add(toggle.name);
            }
            else { 
                BootSceneManager.Instance.PersonalityTags.Remove(toggle.name);
            }
        }
    }
}
