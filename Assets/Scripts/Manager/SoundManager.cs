using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<AudioClip> sfxClips;

    private Dictionary<string, AudioClip> clipDict = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var clip in sfxClips)
        {
            clipDict[clip.name] = clip;
        }
    }

    public void PlaySfx(string clipName)
    {
        if (clipDict.TryGetValue(clipName, out var clip))
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
