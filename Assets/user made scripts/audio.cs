using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton pattern
    
    [Header("Audio Clips")]
    public AudioClip backgroundMusic; // Drag your background music here
    public AudioClip buttonClickSound; // Drag your button sound here
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    private AudioSource musicSource;
    private AudioSource sfxSource;
    
    void Awake()
    {
        // Singleton pattern - only keep one AudioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Only start music if this is the main instance
        if (Instance == this)
        {
            PlayBackgroundMusic();
        }
    }
    
    void CreateAudioSources()
    {
        // Create music source
        GameObject musicObject = new GameObject("MusicSource");
        musicObject.transform.SetParent(transform);
        musicSource = musicObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        
        // Create SFX source
        GameObject sfxObject = new GameObject("SFXSource");
        sfxObject.transform.SetParent(transform);
        sfxSource = sfxObject.AddComponent<AudioSource>();
        sfxSource.volume = sfxVolume;
    }
    
    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }
    
    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    
    public void PlayButtonSound()
    {
        if (sfxSource != null && buttonClickSound != null)
        {
            sfxSource.PlayOneShot(buttonClickSound);
        }
    }
    
    public void PlaySoundEffect(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
}