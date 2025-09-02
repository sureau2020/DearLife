using UnityEngine;
using UnityEngine.UI;

public class BgmButton : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;      // �ϱ������ֵ� AudioSource

    private bool isPlaying = true;


    public void ToggleBgm()
    {
        if (bgmSource == null) return;

        if (isPlaying)
        {
            bgmSource.Pause();
            isPlaying = false;
        }
        else
        {
            bgmSource.Play();
            isPlaying = true;
        }

    }

}
