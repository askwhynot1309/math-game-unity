using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip backgroundMusic;
    public AudioClip balloonPopSound;
    public AudioClip BombSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMusic();
    }

    public void PlayCorrect()
    {
        sfxSource.PlayOneShot(correctSound);
    }

    public void PlayWrong()
    {
        sfxSource.PlayOneShot(wrongSound);
    }

    public void PlayMusic()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayBalloonPop()
    {
        sfxSource.PlayOneShot(balloonPopSound);
    }
    
    public void PlayBombExplosion()
    {
        sfxSource.PlayOneShot(BombSound);
    }
}
