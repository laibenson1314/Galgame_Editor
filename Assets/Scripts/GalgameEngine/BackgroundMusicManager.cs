using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    static public BackgroundMusicManager Instance { get; private set; }

    private AudioSource audioSource;

    private void Awake()
    {
        if(Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip clip, int volumn = 100)
    {
        audioSource.clip = clip;
        audioSource.volume = volumn / 100f;
        audioSource.Play();
    }
    public void Stop()
    {
        audioSource.Stop();
    }
}
