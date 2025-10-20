using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// AudioManager quản lý tất cả âm thanh trong game
/// Hỗ trợ music, SFX và voice với volume controls
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("UI Sound Effects")]
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip buttonHoverSFX;
    [SerializeField] private AudioClip menuTransitionSFX;

    [Header("Game Sound Effects")]
    [SerializeField] private AudioClip carEngineLoop;
    [SerializeField] private AudioClip carHornSFX;
    [SerializeField] private AudioClip scorePickupSFX;
    [SerializeField] private AudioClip gameOverSFX;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadAudioSettings();
        PlayMenuMusic();
    }

    private void InitializeAudio()
    {
        // Create audio sources if they don't exist
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        if (voiceSource == null)
        {
            voiceSource = gameObject.AddComponent<AudioSource>();
            voiceSource.loop = false;
            voiceSource.playOnAwake = false;
        }
    }

    #region Volume Controls
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        SaveAudioSettings();
        
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        SaveAudioSettings();
        
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        SaveAudioSettings();
        
        if (audioMixer != null)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        }
    }

    private void UpdateVolumes()
    {
        if (musicSource != null)
            musicSource.volume = masterVolume * musicVolume;
        
        if (sfxSource != null)
            sfxSource.volume = masterVolume * sfxVolume;
        
        if (voiceSource != null)
            voiceSource.volume = masterVolume * sfxVolume;
    }
    #endregion

    #region Music Controls
    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            if (musicSource.clip != clip)
            {
                musicSource.clip = clip;
                musicSource.Play();
            }
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }
    #endregion

    #region SFX Controls
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }

    public void PlayButtonHover()
    {
        PlaySFX(buttonHoverSFX);
    }

    public void PlayMenuTransition()
    {
        PlaySFX(menuTransitionSFX);
    }

    public void PlayCarHorn()
    {
        PlaySFX(carHornSFX);
    }

    public void PlayScorePickup()
    {
        PlaySFX(scorePickupSFX);
    }

    public void PlayGameOver()
    {
        PlaySFX(gameOverSFX);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlaySFX(AudioClip clip, float volumeScale)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volumeScale);
        }
    }
    #endregion

    #region Engine Sound
    public void StartEngineSound()
    {
        if (voiceSource != null && carEngineLoop != null)
        {
            voiceSource.clip = carEngineLoop;
            voiceSource.loop = true;
            voiceSource.Play();
        }
    }

    public void StopEngineSound()
    {
        if (voiceSource != null)
        {
            voiceSource.Stop();
        }
    }

    public void SetEnginePitch(float pitch)
    {
        if (voiceSource != null)
        {
            voiceSource.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
        }
    }
    #endregion

    #region Settings Persistence
    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        
        UpdateVolumes();
    }
    #endregion

    #region Scene Events
    public void OnSceneChanged(string sceneName)
    {
        if (sceneName == "StartMenu")
        {
            PlayMenuMusic();
            StopEngineSound();
        }
        else if (sceneName == "RacingGameplay")
        {
            PlayGameplayMusic();
            StartEngineSound();
        }
    }
    
    public void LoadSettings()
    {
        // Load audio settings from PlayerPrefs
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        
        // Apply loaded settings
        ApplyVolumeSettings();
        
        Debug.Log($"AudioManager: Settings loaded - Master: {masterVolume}, Music: {musicVolume}, SFX: {sfxVolume}");
    }
    
    public void SaveSettings()
    {
        // Save current settings to PlayerPrefs
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
        
        Debug.Log("AudioManager: Settings saved");
    }
    
    private void ApplyVolumeSettings()
    {
        // Apply volume settings to audio sources
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;
        if (sfxSource != null)
            sfxSource.volume = sfxVolume * masterVolume;
        if (voiceSource != null)
            voiceSource.volume = sfxVolume * masterVolume;
            
        // Apply to AudioMixer if available
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        }
        
        // Apply master volume to AudioListener as fallback
        AudioListener.volume = masterVolume;
    }
    #endregion
}