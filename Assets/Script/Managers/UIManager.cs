using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// UIManager quản lý tất cả các UI panels và transitions cho CityDriver game
/// Hỗ trợ Main Menu, Game HUD, Pause Menu, và Settings
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] public GameObject mainMenuPanel;
    [SerializeField] public GameObject gameHUDPanel;
    [SerializeField] public GameObject pauseMenuPanel;
    [SerializeField] public GameObject settingsPanel;
    [SerializeField] public GameObject gameOverPanel;

    [Header("Main Menu UI")]
    [SerializeField] public Button playButton;          // Tương thích với UIQuickFix
    [SerializeField] public Button settingsButton;
    [SerializeField] public Button exitButton;         // Tương thích với UIQuickFix

    [Header("Game HUD")]
    [SerializeField] public Text speedText;
    [SerializeField] public Text scoreText;
    [SerializeField] public Text highScoreText;        // Tương thích với UIQuickFix
    [SerializeField] public Image fuelFill;            // Tương thích với UIQuickFix
    [SerializeField] public Button pauseButton;

    [Header("Pause Menu")]
    [SerializeField] public Button resumeButton;
    [SerializeField] public Button restartButton;              // Tương thích với UIQuickFix
    [SerializeField] public Button mainMenuFromPauseButton;    // Tương thích với UIQuickFix

    [Header("Settings Menu")]
    [SerializeField] public Slider masterVolumeSlider;
    [SerializeField] public Slider musicVolumeSlider;
    [SerializeField] public Slider sfxVolumeSlider;
    [SerializeField] public Button settingsCloseButton;       // Tương thích với UIQuickFix

    [Header("Game Over")]
    [SerializeField] public Text finalScoreText;
    [SerializeField] public Button playAgainButton;              // Tương thích với UIQuickFix
    [SerializeField] public Button mainMenuFromGameOverButton;   // Tương thích với UIQuickFix

    [Header("Animation Settings")]
    [SerializeField] private float panelTransitionDuration = 0.3f;

    // Game state
    private bool isPaused = false;
    private bool isInGame = false;

    // References
    private GameManager gameManager;
    private AudioManager audioManager;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Subscribe to scene loading events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        RefreshUIReferences();
        InitializeUI();
        SetupButtonListeners();
        
        // Find GameManager reference
        gameManager = GameManager.Instance;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"UIManager: Scene loaded - {scene.name}");
        
        // Delay to ensure all objects are instantiated
        StartCoroutine(DelayedUIRefresh());
    }
    
    private IEnumerator DelayedUIRefresh()
    {
        yield return new WaitForEndOfFrame();
        
        RefreshUIReferences();
        InitializeUI();
        SetupButtonListeners();
        LoadAudioSettings();
        
        Debug.Log("UIManager: UI references refreshed for new scene");
    }

    private void Update()
    {
        // Handle ESC key for pause/unpause using Input System
        if (isInGame && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    #region UI Initialization
    private void InitializeUI()
    {
        // Determine which scene we're in
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "StartMenu")
        {
            ShowMainMenu();
        }
        else if (currentScene == "RacingGameplay")
        {
            ShowGameHUD();
            isInGame = true;
        }
    }

    private void SetupButtonListeners()
    {
        // Clear existing listeners trước khi add new ones
        ClearButtonListeners();
        
        try
        {
            // Main Menu Buttons
            if (playButton != null)
                playButton.onClick.AddListener(StartGame);
            if (settingsButton != null)
                settingsButton.onClick.AddListener(ShowSettings);
            if (exitButton != null)
                exitButton.onClick.AddListener(ExitGame);
            
            // Pause Menu Buttons
            if (resumeButton != null)
                resumeButton.onClick.AddListener(ResumeGame);
            if (restartButton != null)
                restartButton.onClick.AddListener(RestartGame);
            if (mainMenuFromPauseButton != null)
                mainMenuFromPauseButton.onClick.AddListener(BackToMainMenu);
            
            // Game HUD Buttons
            if (pauseButton != null)
                pauseButton.onClick.AddListener(PauseGame);
            
            // Settings Panel Buttons
            if (settingsCloseButton != null)
                settingsCloseButton.onClick.AddListener(HideSettings);
            
            // Volume Sliders
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
            
            // Game Over Buttons
            if (playAgainButton != null)
                playAgainButton.onClick.AddListener(PlayAgain);
            if (mainMenuFromGameOverButton != null)
                mainMenuFromGameOverButton.onClick.AddListener(BackToMainMenu);
                
            Debug.Log("UIManager: Button listeners setup completed");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"UIManager: Error setting up button listeners: {e.Message}");
        }
    }
    
    private void OnDestroy()
    {
        // Cleanup scene loading event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void ClearButtonListeners()
    {
        try
        {
            // Clear tất cả listeners để tránh duplicate
            if (playButton != null && playButton) playButton.onClick.RemoveAllListeners();
            if (settingsButton != null && settingsButton) settingsButton.onClick.RemoveAllListeners();
            if (exitButton != null && exitButton) exitButton.onClick.RemoveAllListeners();
            if (resumeButton != null && resumeButton) resumeButton.onClick.RemoveAllListeners();
            if (restartButton != null && restartButton) restartButton.onClick.RemoveAllListeners();
            if (mainMenuFromPauseButton != null && mainMenuFromPauseButton) mainMenuFromPauseButton.onClick.RemoveAllListeners();
            if (pauseButton != null && pauseButton) pauseButton.onClick.RemoveAllListeners();
            if (settingsCloseButton != null && settingsCloseButton) settingsCloseButton.onClick.RemoveAllListeners();
            if (playAgainButton != null && playAgainButton) playAgainButton.onClick.RemoveAllListeners();
            if (mainMenuFromGameOverButton != null && mainMenuFromGameOverButton) mainMenuFromGameOverButton.onClick.RemoveAllListeners();
            
            if (masterVolumeSlider != null && masterVolumeSlider) masterVolumeSlider.onValueChanged.RemoveAllListeners();
            if (musicVolumeSlider != null && musicVolumeSlider) musicVolumeSlider.onValueChanged.RemoveAllListeners();
            if (sfxVolumeSlider != null && sfxVolumeSlider) sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"UIManager: Error clearing button listeners: {e.Message}");
        }
    }
    #endregion

    #region Panel Management
    public void ShowMainMenu()
    {
        SetPanelActive(mainMenuPanel, true);
        SetPanelActive(gameHUDPanel, false);
        SetPanelActive(pauseMenuPanel, false);
        SetPanelActive(settingsPanel, false);
        SetPanelActive(gameOverPanel, false);
        
        Time.timeScale = 1f;
        isPaused = false;
        isInGame = false;
    }

    public void ShowGameHUD()
    {
        SetPanelActive(mainMenuPanel, false);
        SetPanelActive(gameHUDPanel, true);
        SetPanelActive(pauseMenuPanel, false);
        SetPanelActive(settingsPanel, false);
        SetPanelActive(gameOverPanel, false);
        
        Time.timeScale = 1f;
        isPaused = false;
        isInGame = true;
    }

    public void ShowPauseMenu()
    {
        SetPanelActive(pauseMenuPanel, true);
        SetPanelActive(gameHUDPanel, false);
        
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ShowSettings()
    {
        SetPanelActive(settingsPanel, true);
        
        // Hide other panels but remember state
        if (isInGame && !isPaused)
        {
            SetPanelActive(gameHUDPanel, false);
        }
        else if (isPaused)
        {
            SetPanelActive(pauseMenuPanel, false);
        }
        else
        {
            SetPanelActive(mainMenuPanel, false);
        }
    }

    public void ShowGameOver(float finalScore)
    {
        SetPanelActive(gameOverPanel, true);
        SetPanelActive(gameHUDPanel, false);
        
        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + finalScore.ToString("F0");
        
        Time.timeScale = 0f;
        isInGame = false;
    }

    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
    #endregion

    #region Game Controls
    public void StartGame()
    {
        SceneManager.LoadScene("RacingGameplay");
    }

    public void PauseGame()
    {
        if (isInGame && !isPaused)
        {
            ShowPauseMenu();
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            ShowGameHUD();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("RacingGameplay");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("RacingGameplay");
    }

    public void HideSettings()
    {
        CloseSettings();
    }

    public void CloseSettings()
    {
        SetPanelActive(settingsPanel, false);
        
        // Return to appropriate panel
        if (isInGame && isPaused)
        {
            ShowPauseMenu();
        }
        else if (isInGame)
        {
            ShowGameHUD();
        }
        else
        {
            ShowMainMenu();
        }
    }
    #endregion

    #region HUD Updates
    public void UpdateSpeed(float speed)
    {
        if (speedText != null)
        {
            speedText.text = speed.ToString("F0") + " km/h";
        }
    }

    public void UpdateScore(float score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString("F0");
        }
    }

    public void UpdateFuel(float fuelPercent)
    {
        if (fuelFill != null)
        {
            fuelFill.fillAmount = Mathf.Clamp01(fuelPercent);
            
            // Change color based on fuel level
            if (fuelPercent > 0.5f)
                fuelFill.color = Color.green;
            else if (fuelPercent > 0.25f)
                fuelFill.color = Color.yellow;
            else
                fuelFill.color = Color.red;
        }
    }
    #endregion

    #region Audio Settings
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        // Implement music volume control
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        // Implement SFX volume control
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void LoadAudioSettings()
    {
        // Load saved audio settings
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = masterVolume;
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = musicVolume;
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfxVolume;
            
        AudioListener.volume = masterVolume;
        
        // Apply audio settings via AudioManager if available
        if (audioManager != null)
        {
            audioManager.LoadSettings();
        }
    }
    
    // Auto-detect và assign UI elements từ scene hiện tại
    private void RefreshUIReferences()
    {
        Debug.Log("UIManager: Refreshing UI references...");
        int foundCount = 0;
        int totalChecked = 0;
        
        // Find Main Menu UI elements
        if (mainMenuPanel == null)
        {
            GameObject mainMenuObj = GameObject.Find("MainMenuPanel");
            if (mainMenuObj != null)
            {
                mainMenuPanel = mainMenuObj;
                foundCount++;
            }
        }
        else { foundCount++; }
        totalChecked++;
        
        if (playButton == null)
        {
            GameObject playButtonObj = GameObject.Find("PlayButton");
            if (playButtonObj != null)
            {
                playButton = playButtonObj.GetComponent<Button>();
                if (playButton != null) foundCount++;
            }
        }
        else { foundCount++; }
        totalChecked++;
        
        if (settingsButton == null)
        {
            GameObject settingsButtonObj = GameObject.Find("SettingsButton");
            if (settingsButtonObj != null)
                settingsButton = settingsButtonObj.GetComponent<Button>();
        }
        
        if (exitButton == null)
        {
            GameObject exitButtonObj = GameObject.Find("ExitButton");
            if (exitButtonObj != null)
                exitButton = exitButtonObj.GetComponent<Button>();
        }
        
        // Find Game HUD elements
        if (gameHUDPanel == null)
        {
            GameObject gameHUDObj = GameObject.Find("GameHUDPanel");
            if (gameHUDObj != null)
                gameHUDPanel = gameHUDObj;
        }
        
        if (speedText == null)
        {
            GameObject speedTextObj = GameObject.Find("SpeedText");
            if (speedTextObj != null)
                speedText = speedTextObj.GetComponent<Text>();
        }
        
        if (scoreText == null)
        {
            GameObject scoreTextObj = GameObject.Find("ScoreText");
            if (scoreTextObj != null)
                scoreText = scoreTextObj.GetComponent<Text>();
        }
        
        if (highScoreText == null)
        {
            GameObject highScoreTextObj = GameObject.Find("HighScoreText");
            if (highScoreTextObj != null)
                highScoreText = highScoreTextObj.GetComponent<Text>();
        }
        
        if (fuelFill == null)
        {
            GameObject fuelFillObj = GameObject.Find("FuelFill");
            if (fuelFillObj != null)
                fuelFill = fuelFillObj.GetComponent<Image>();
        }
        
        if (pauseButton == null)
        {
            GameObject pauseButtonObj = GameObject.Find("PauseButton");
            if (pauseButtonObj != null)
                pauseButton = pauseButtonObj.GetComponent<Button>();
        }
        
        // Find Pause Menu elements
        if (pauseMenuPanel == null)
        {
            GameObject pauseMenuObj = GameObject.Find("PauseMenuPanel");
            if (pauseMenuObj != null)
                pauseMenuPanel = pauseMenuObj;
        }
        
        if (resumeButton == null)
        {
            GameObject resumeButtonObj = GameObject.Find("ResumeButton");
            if (resumeButtonObj != null)
                resumeButton = resumeButtonObj.GetComponent<Button>();
        }
        
        if (restartButton == null)
        {
            GameObject restartButtonObj = GameObject.Find("RestartButton");
            if (restartButtonObj != null)
                restartButton = restartButtonObj.GetComponent<Button>();
        }
        
        if (mainMenuFromPauseButton == null)
        {
            GameObject mainMenuFromPauseButtonObj = GameObject.Find("MainMenuFromPauseButton");
            if (mainMenuFromPauseButtonObj != null)
                mainMenuFromPauseButton = mainMenuFromPauseButtonObj.GetComponent<Button>();
        }
        
        // Find Settings Panel elements
        if (settingsPanel == null)
        {
            GameObject settingsPanelObj = GameObject.Find("SettingsPanel");
            if (settingsPanelObj != null)
                settingsPanel = settingsPanelObj;
        }
        
        if (masterVolumeSlider == null)
        {
            GameObject masterVolumeSliderObj = GameObject.Find("MasterVolumeSlider");
            if (masterVolumeSliderObj != null)
                masterVolumeSlider = masterVolumeSliderObj.GetComponent<Slider>();
        }
        
        if (musicVolumeSlider == null)
        {
            GameObject musicVolumeSliderObj = GameObject.Find("MusicVolumeSlider");
            if (musicVolumeSliderObj != null)
                musicVolumeSlider = musicVolumeSliderObj.GetComponent<Slider>();
        }
        
        if (sfxVolumeSlider == null)
        {
            GameObject sfxVolumeSliderObj = GameObject.Find("SFXVolumeSlider");
            if (sfxVolumeSliderObj != null)
                sfxVolumeSlider = sfxVolumeSliderObj.GetComponent<Slider>();
        }
        
        if (settingsCloseButton == null)
        {
            GameObject settingsCloseButtonObj = GameObject.Find("SettingsCloseButton");
            if (settingsCloseButtonObj != null)
                settingsCloseButton = settingsCloseButtonObj.GetComponent<Button>();
        }
        
        // Find Game Over Panel elements
        if (gameOverPanel == null)
        {
            GameObject gameOverPanelObj = GameObject.Find("GameOverPanel");
            if (gameOverPanelObj != null)
                gameOverPanel = gameOverPanelObj;
        }
        
        if (finalScoreText == null)
        {
            GameObject finalScoreTextObj = GameObject.Find("FinalScoreText");
            if (finalScoreTextObj != null)
                finalScoreText = finalScoreTextObj.GetComponent<Text>();
        }
        
        if (playAgainButton == null)
        {
            GameObject playAgainButtonObj = GameObject.Find("PlayAgainButton");
            if (playAgainButtonObj != null)
                playAgainButton = playAgainButtonObj.GetComponent<Button>();
        }
        
        if (mainMenuFromGameOverButton == null)
        {
            GameObject mainMenuFromGameOverButtonObj = GameObject.Find("MainMenuFromGameOverButton");
            if (mainMenuFromGameOverButtonObj != null)
                mainMenuFromGameOverButton = mainMenuFromGameOverButtonObj.GetComponent<Button>();
        }
        
        // Find other managers
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }
        
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
        
        // Final report
        Debug.Log($"UIManager: UI references refresh completed - Found {foundCount}/{totalChecked} elements");
        
        if (foundCount < totalChecked)
        {
            Debug.LogWarning($"UIManager: {totalChecked - foundCount} UI elements missing. Use UIAutoSetup tool to create them automatically.");
        }
    }
    #endregion

    #region Public Getters
    public bool IsPaused => isPaused;
    public bool IsInGame => isInGame;
    #endregion
}