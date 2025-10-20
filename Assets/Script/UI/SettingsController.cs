using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SettingsController quản lý settings menu với audio/graphics options
/// </summary>
public class SettingsController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Text masterVolumeText;
    [SerializeField] private Text musicVolumeText;
    [SerializeField] private Text sfxVolumeText;

    [Header("Graphics Settings")]
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown resolutionDropdown;

    [Header("Controls")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button resetButton;

    [Header("Tabs")]
    [SerializeField] private Button audioTabButton;
    [SerializeField] private Button graphicsTabButton;
    [SerializeField] private Button controlsTabButton;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject graphicsPanel;
    [SerializeField] private GameObject controlsPanel;

    private Resolution[] availableResolutions;
    private int currentTabIndex = 0; // 0: Audio, 1: Graphics, 2: Controls

    private void Start()
    {
        InitializeSettings();
        SetupEventListeners();
        LoadSettings();
        ShowTab(0); // Show audio tab by default
    }

    private void InitializeSettings()
    {
        // Initialize resolutions
        availableResolutions = Screen.resolutions;
        
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            
            System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();
            int currentResolutionIndex = 0;
            
            for (int i = 0; i < availableResolutions.Length; i++)
            {
                string option = availableResolutions[i].width + " x " + availableResolutions[i].height;
                options.Add(option);
                
                if (availableResolutions[i].width == Screen.currentResolution.width &&
                    availableResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        // Initialize quality settings
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.RefreshShownValue();
        }
    }

    private void SetupEventListeners()
    {
        // Audio sliders
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // Graphics controls
        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        // Tab buttons
        if (audioTabButton != null)
            audioTabButton.onClick.AddListener(() => ShowTab(0));
        
        if (graphicsTabButton != null)
            graphicsTabButton.onClick.AddListener(() => ShowTab(1));
        
        if (controlsTabButton != null)
            controlsTabButton.onClick.AddListener(() => ShowTab(2));

        // Control buttons
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClick);
        
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetButtonClick);
    }

    #region Tab Management
    public void ShowTab(int tabIndex)
    {
        currentTabIndex = tabIndex;

        // Hide all panels
        if (audioPanel != null) audioPanel.SetActive(false);
        if (graphicsPanel != null) graphicsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);

        // Show selected panel
        switch (tabIndex)
        {
            case 0:
                if (audioPanel != null) audioPanel.SetActive(true);
                break;
            case 1:
                if (graphicsPanel != null) graphicsPanel.SetActive(true);
                break;
            case 2:
                if (controlsPanel != null) controlsPanel.SetActive(true);
                break;
        }

        // Update tab button colors
        UpdateTabButtonColors();
        
        AudioManager.Instance?.PlayButtonClick();
    }

    private void UpdateTabButtonColors()
    {
        Color selectedColor = Color.white;
        Color normalColor = new Color(0.8f, 0.8f, 0.8f, 1f);

        if (audioTabButton != null)
            audioTabButton.GetComponent<Image>().color = currentTabIndex == 0 ? selectedColor : normalColor;
        
        if (graphicsTabButton != null)
            graphicsTabButton.GetComponent<Image>().color = currentTabIndex == 1 ? selectedColor : normalColor;
        
        if (controlsTabButton != null)
            controlsTabButton.GetComponent<Image>().color = currentTabIndex == 2 ? selectedColor : normalColor;
    }
    #endregion

    #region Audio Settings
    public void OnMasterVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMasterVolume(value);
        
        if (masterVolumeText != null)
            masterVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
        
        if (musicVolumeText != null)
            musicVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
        
        if (sfxVolumeText != null)
            sfxVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }
    #endregion

    #region Graphics Settings
    public void OnQualityChanged(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        AudioManager.Instance?.PlayButtonClick();
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        AudioManager.Instance?.PlayButtonClick();
    }

    public void OnResolutionChanged(int resolutionIndex)
    {
        Resolution resolution = availableResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        AudioManager.Instance?.PlayButtonClick();
    }
    #endregion

    #region Button Handlers
    public void OnBackButtonClick()
    {
        AudioManager.Instance?.PlayButtonClick();
        UIManager.Instance?.CloseSettings();
    }

    public void OnResetButtonClick()
    {
        AudioManager.Instance?.PlayButtonClick();
        ResetToDefaults();
    }

    private void ResetToDefaults()
    {
        // Reset audio settings
        if (masterVolumeSlider != null) masterVolumeSlider.value = 1f;
        if (musicVolumeSlider != null) musicVolumeSlider.value = 0.7f;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = 0.8f;

        // Reset graphics settings
        if (qualityDropdown != null) qualityDropdown.value = QualitySettings.GetQualityLevel();
        if (fullscreenToggle != null) fullscreenToggle.isOn = true;

        SaveSettings();
    }
    #endregion

    #region Settings Persistence
    private void LoadSettings()
    {
        // Load audio settings
        if (masterVolumeSlider != null)
        {
            float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
            masterVolumeSlider.value = masterVol;
            OnMasterVolumeChanged(masterVol);
        }

        if (musicVolumeSlider != null)
        {
            float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            musicVolumeSlider.value = musicVol;
            OnMusicVolumeChanged(musicVol);
        }

        if (sfxVolumeSlider != null)
        {
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            sfxVolumeSlider.value = sfxVol;
            OnSFXVolumeChanged(sfxVol);
        }

        // Load graphics settings
        if (qualityDropdown != null)
        {
            int quality = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
            qualityDropdown.value = quality;
        }

        if (fullscreenToggle != null)
        {
            bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            fullscreenToggle.isOn = fullscreen;
        }

        if (resolutionDropdown != null)
        {
            int resIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
            resolutionDropdown.value = resIndex;
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.Save();
    }
    #endregion
}