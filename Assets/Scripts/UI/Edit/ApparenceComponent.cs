using System;
using UnityEngine;
using UnityEngine.UI;

public class ApparenceComponent : MonoBehaviour
{
    public int id { get; set; }
    public string type { get; set; }
    [SerializeField] private Button button;



    public void Show(int id, Sprite sp, string type) { 
        this.id = id;
        button.image.sprite = sp;
        this.type = type;
    }

    public void OnClick()
    {
        SoundManager.Instance.PlaySfx("Click");
        if (id != 0) {
            var hslObj = GameObject.Find("UI").transform.Find("ApparencePanel").transform.Find("HSL").gameObject;
            if (hslObj != null)
            {
                hslObj.SetActive(true);
            }
        }
        ApplyToAppearance();
    }

    public void ApplyToAppearance()
    {
        BootSceneManager.Instance.ApplyAppearancePart(type, id);
    }
}
