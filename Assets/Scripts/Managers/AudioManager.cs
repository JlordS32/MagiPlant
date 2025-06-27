using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; set; }

    // References
    AudioSource _soundSource;
    AudioSource _musicSource;

    void Awake()
    {
        _soundSource = GetComponent<AudioSource>();
        _musicSource = transform.GetChild(0).GetComponent<AudioSource>();   // BG Music Component must be nested under manager.

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        _soundSource.Stop();
        _soundSource.clip = clip;
        _soundSource.Play();
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (_musicSource.clip == clip) return;
        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.Play();
    }

    public void PauseMusic()
    {
        if (_musicSource.isPlaying)
            _musicSource.Pause();
        else
            _musicSource.Play();
    }

    public void ResetMusic()
    {
        _musicSource.Stop();
        _musicSource.time = 0f;
        _musicSource.Play();
    }
}