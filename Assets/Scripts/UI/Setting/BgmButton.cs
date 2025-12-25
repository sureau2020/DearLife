using UnityEngine;
using UnityEngine.UI;

public class BgmButton : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;      // ±≥æ∞“Ù¿÷
    [SerializeField] private Sprite MusicIcon;
    [SerializeField] private Sprite MuteIcon;
    [SerializeField] private Image buttonImage;

    private bool isPlaying = true;


    public void ToggleBgm()
    {
        if (bgmSource == null) return;

        if (isPlaying)
        {
            bgmSource.Pause();
            isPlaying = false;
            UpdateButtonIcon();
        }
        else
        {
            bgmSource.Play();
            isPlaying = true;
            UpdateButtonIcon();
        }

    }

    private void UpdateButtonIcon()
    {
        if (buttonImage == null) return;
        if (isPlaying)
        {
            buttonImage.sprite = MusicIcon;
        }
        else
        {
            buttonImage.sprite = MuteIcon;
        }
    }

}
