using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

/// <summary>
/// UIManager quản lý tất cả các UI panels và transitions cho CityDriver game
/// Hỗ trợ Main Menu, Game HUD, Pause Menu, và Settings
/// Includes Hearts and Coins management
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

    [Header("Heart UI")]
    [SerializeField] public TextMeshProUGUI heartText;   // Text hiển thị số tim
    [SerializeField] public Sprite fullHeart;
    [SerializeField] public Sprite emptyHeart;

    [Header("Coin UI")]
    [SerializeField] public TextMeshProUGUI coinText;    // Text hiển thị số coin

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
        
        // Initialize hearts and coins display if GameManager exists
        if (gameManager != null)
        {
            UpdateHearts(gameManager.GetCurrentHearts());
            UpdateCoins(gameManager.GetCoins());
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"UIManager: Scene loaded - {scene.name}");
        
        // Ensure camera is active after scene load
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.RefreshCamera();
        }
        
        // Stop all existing coroutines to prevent conflicts
        StopAllCoroutines();
        
        // Force UI refresh with proper timing
        StartCoroutine(ForceUIRefresh());
    }
    
    private IEnumerator ForceUIRefresh()
    {
        Debug.Log("UIManager: Starting ForceUIRefresh...");
        
        // Wait for scene to fully load and all objects to be instantiated
        yield return new WaitForSeconds(0.1f);
        
        // Additional wait for end of frame to ensure everything is ready
        yield return new WaitForEndOfFrame();
        
        // Force refresh all UI references
        RefreshUIReferences();
        
        // Initialize UI state for current scene
        InitializeUI();
        
        // Setup button listeners
        SetupButtonListeners();
        
        // Load audio settings
        LoadAudioSettings();
        
        Debug.Log("UIManager: ForceUIRefresh completed - All references should be updated");
        
        // Log which panels were found for debugging
        LogFoundPanels();
    }
    
    private void LogFoundPanels()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"=== UIManager Panel Status for Scene: {currentScene} ===");
        
        // Panel status with detailed info
        if (mainMenuPanel != null)
            Debug.Log($"MainMenuPanel: ✅ Found - {mainMenuPanel.name} (Active: {mainMenuPanel.activeInHierarchy})");
        else
            Debug.Log("MainMenuPanel: ❌ Missing");
            
        if (gameHUDPanel != null)
            Debug.Log($"GameHUDPanel: ✅ Found - {gameHUDPanel.name} (Active: {gameHUDPanel.activeInHierarchy})");
        else
            Debug.Log("GameHUDPanel: ❌ Missing");
            
        if (pauseMenuPanel != null)
            Debug.Log($"PauseMenuPanel: ✅ Found - {pauseMenuPanel.name} (Active: {pauseMenuPanel.activeInHierarchy})");
        else
            Debug.Log("PauseMenuPanel: ❌ Missing");
            
        if (settingsPanel != null)
            Debug.Log($"SettingsPanel: ✅ Found - {settingsPanel.name} (Active: {settingsPanel.activeInHierarchy})");
        else
            Debug.Log("SettingsPanel: ❌ Missing");
            
        if (gameOverPanel != null)
            Debug.Log($"GameOverPanel: ✅ Found - {gameOverPanel.name} (Active: {gameOverPanel.activeInHierarchy})");
        else
            Debug.Log("GameOverPanel: ❌ Missing");
        
        // Critical button status for gameplay
        Debug.Log("=== Critical Button Status ===");
        Debug.Log($"ResumeButton: {(resumeButton != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"RestartButton: {(restartButton != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"PauseButton: {(pauseButton != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"MainMenuFromPauseButton: {(mainMenuFromPauseButton != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log("================================");
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
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"UIManager: Refreshing UI references for scene: {currentScene}");
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
        
        // Find Pause Menu elements - Enhanced search
        if (pauseMenuPanel == null)
        {
            totalChecked++;
            Debug.Log("UIManager: Searching for PauseMenuPanel...");
            
            // Method 1: Direct name search
            GameObject pauseMenuObj = GameObject.Find("PauseMenuPanel");
            if (pauseMenuObj != null)
            {
                pauseMenuPanel = pauseMenuObj;
                foundCount++;
                Debug.Log($"UIManager: Found PauseMenuPanel by direct search: {pauseMenuObj.name}");
            }
            else
            {
                Debug.Log("UIManager: PauseMenuPanel not found by direct search");
                
                // Method 2: Search all GameObjects for pause menu
                GameObject[] allObjects = FindObjectsOfType<GameObject>(true); // Include inactive
                Debug.Log($"UIManager: Searching through {allObjects.Length} GameObjects for pause menu...");
                
                foreach (GameObject obj in allObjects)
                {
                    string objName = obj.name.ToLower();
                    // More specific search - prioritize exact matches
                    if (objName == "pausemenupanel" || objName == "pause menu panel" || objName == "pausemenu")
                    {
                        pauseMenuPanel = obj;
                        foundCount++;
                        Debug.Log($"UIManager: Found PauseMenuPanel by exact name match: {obj.name} (Active: {obj.activeInHierarchy})");
                        break;
                    }
                    // Fallback: Look for objects that contain "pause" but exclude buttons
                    else if (objName.Contains("pause") && objName.Contains("panel") && !objName.Contains("button"))
                    {
                        pauseMenuPanel = obj;
                        foundCount++;
                        Debug.Log($"UIManager: Found PauseMenuPanel by pattern match: {obj.name} (Active: {obj.activeInHierarchy})");
                        break;
                    }
                }
                
                if (pauseMenuPanel == null)
                {
                    Debug.LogWarning($"UIManager: PauseMenuPanel not found in scene {currentScene}!");
                    // List all UI panels found for debugging
                    Debug.Log("UIManager: Available UI objects:");
                    foreach (GameObject obj in allObjects)
                    {
                        if (obj.name.ToLower().Contains("panel") || obj.name.ToLower().Contains("menu"))
                        {
                            Debug.Log($"  - {obj.name} (Active: {obj.activeInHierarchy}, Parent: {(obj.transform.parent ? obj.transform.parent.name : "None")})");
                        }
                    }
                    
                    // DISABLED: Do not auto-create PauseMenuPanel, use existing one from scene
                    // The PauseMenuPanel should already exist in the scene hierarchy
                    Debug.LogError("UIManager: PauseMenuPanel must exist in the scene! Check the scene setup.");
                }
            }
        }
        else 
        { 
            foundCount++;
            Debug.Log($"UIManager: PauseMenuPanel already assigned: {pauseMenuPanel.name}");
        }
        
        if (resumeButton == null)
        {
            GameObject resumeButtonObj = GameObject.Find("ResumeButton");
            if (resumeButtonObj != null)
            {
                resumeButton = resumeButtonObj.GetComponent<Button>();
                foundCount++;
                Debug.Log($"UIManager: Found ResumeButton by global search");
            }
            else
            {
                // Search within PauseMenuPanel if it exists
                if (pauseMenuPanel != null)
                {
                    // Direct child search (based on Unity hierarchy structure)
                    Transform resumeTransform = pauseMenuPanel.transform.Find("ResumeButton");
                    if (resumeTransform == null)
                        resumeTransform = pauseMenuPanel.transform.Find("ButtonContainer/ResumeButton");
                    
                    if (resumeTransform != null)
                    {
                        resumeButton = resumeTransform.GetComponent<Button>();
                        if (resumeButton != null) 
                        {
                            foundCount++;
                            Debug.Log($"UIManager: Found ResumeButton in PauseMenuPanel hierarchy: {resumeTransform.name}");
                        }
                    }
                    else
                    {
                        Debug.Log("UIManager: ResumeButton not found in PauseMenuPanel hierarchy");
                    }
                }
            }
        }
        else { foundCount++; }
        totalChecked++;
        
        if (restartButton == null)
        {
            GameObject restartButtonObj = GameObject.Find("RestartButton");
            if (restartButtonObj != null)
            {
                restartButton = restartButtonObj.GetComponent<Button>();
                foundCount++;
                Debug.Log($"UIManager: Found RestartButton by global search");
            }
            else
            {
                // Search within PauseMenuPanel if it exists
                if (pauseMenuPanel != null)
                {
                    // Direct child search (based on Unity hierarchy structure)
                    Transform restartTransform = pauseMenuPanel.transform.Find("RestartButton");
                    if (restartTransform == null)
                        restartTransform = pauseMenuPanel.transform.Find("ButtonContainer/RestartButton");
                    
                    if (restartTransform != null)
                    {
                        restartButton = restartTransform.GetComponent<Button>();
                        if (restartButton != null) 
                        {
                            foundCount++;
                            Debug.Log($"UIManager: Found RestartButton in PauseMenuPanel hierarchy: {restartTransform.name}");
                        }
                    }
                    else
                    {
                        Debug.Log("UIManager: RestartButton not found in PauseMenuPanel hierarchy");
                    }
                }
            }
        }
        else { foundCount++; } 
        totalChecked++;
        
        if (mainMenuFromPauseButton == null)
        {
            GameObject mainMenuFromPauseButtonObj = GameObject.Find("MainMenuFromPauseButton");
            if (mainMenuFromPauseButtonObj != null)
            {
                mainMenuFromPauseButton = mainMenuFromPauseButtonObj.GetComponent<Button>();
                foundCount++;
                Debug.Log($"UIManager: Found MainMenuFromPauseButton by global search");
            }
            else
            {
                // Search within PauseMenuPanel if it exists
                if (pauseMenuPanel != null)
                {
                    // Direct child search (based on Unity hierarchy structure)
                    Transform mainMenuTransform = pauseMenuPanel.transform.Find("MainMenuFromPauseButton");
                    if (mainMenuTransform == null)
                        mainMenuTransform = pauseMenuPanel.transform.Find("ButtonContainer/MainMenuFromPauseButton");
                    
                    if (mainMenuTransform != null)
                    {
                        mainMenuFromPauseButton = mainMenuTransform.GetComponent<Button>();
                        if (mainMenuFromPauseButton != null) 
                        {
                            foundCount++;
                            Debug.Log($"UIManager: Found MainMenuFromPauseButton in PauseMenuPanel hierarchy: {mainMenuTransform.name}");
                        }
                    }
                    else
                    {
                        Debug.Log("UIManager: MainMenuFromPauseButton not found in PauseMenuPanel hierarchy");
                        
                        // Debug: List all children of PauseMenuPanel
                        Debug.Log("UIManager: PauseMenuPanel children:");
                        for (int i = 0; i < pauseMenuPanel.transform.childCount; i++)
                        {
                            Transform child = pauseMenuPanel.transform.GetChild(i);
                            Debug.Log($"  - Child {i}: {child.name} (Active: {child.gameObject.activeInHierarchy})");
                        }
                    }
                }
            }
        }
        else { foundCount++; }
        totalChecked++;
        
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
        Debug.Log($"UIManager: UI references refresh completed for scene '{currentScene}' - Found {foundCount}/{totalChecked} elements");
        LogFoundPanels();
        
        // Additional verification for critical panels
        if (currentScene.Contains("Gameplay") || currentScene.Contains("Game"))
        {
            if (pauseMenuPanel == null)
            {
                Debug.LogError("UIManager: CRITICAL - PauseMenuPanel is null in Gameplay scene!");
            }
            if (gameHUDPanel == null)
            {
                Debug.LogError("UIManager: CRITICAL - GameHUDPanel is null in Gameplay scene!");
            }
        }
        
        if (foundCount < totalChecked)
        {
            Debug.LogWarning($"UIManager: {totalChecked - foundCount} UI elements missing. Use UIAutoSetup tool to create them automatically.");
        }
    }
    #endregion

    private void CreatePauseMenuPanel()
    {
        Debug.Log("UIManager: Creating PauseMenuPanel...");
        
        // Find Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("UIManager: No Canvas found! Cannot create PauseMenuPanel.");
            return;
        }

        // Create main panel
        GameObject pausePanel = new GameObject("PauseMenuPanel");
        pausePanel.transform.SetParent(canvas.transform, false);
        
        // Add Image component for background
        UnityEngine.UI.Image panelImage = pausePanel.AddComponent<UnityEngine.UI.Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black

        // Set RectTransform to fill parent
        RectTransform panelRect = pausePanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;

        // Create button container
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(pausePanel.transform, false);
        
        // Add VerticalLayoutGroup to container
        UnityEngine.UI.VerticalLayoutGroup layoutGroup = buttonContainer.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
        layoutGroup.spacing = 20;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = false;

        // Set container RectTransform
        RectTransform containerRect = buttonContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(200, 300);
        containerRect.anchoredPosition = Vector2.zero;

        // Create buttons
        CreatePauseMenuButton(buttonContainer, "ResumeButton", "Resume");
        CreatePauseMenuButton(buttonContainer, "RestartButton", "Restart");
        CreatePauseMenuButton(buttonContainer, "MainMenuFromPauseButton", "Main Menu");

        // Initially deactivate the panel
        pausePanel.SetActive(false);

        // Assign to pauseMenuPanel reference
        pauseMenuPanel = pausePanel;

        Debug.Log($"✅ UIManager: Created PauseMenuPanel successfully!");
    }

    private void CreatePauseMenuButton(GameObject parent, string buttonName, string buttonText)
    {
        GameObject buttonObj = new GameObject(buttonName);
        buttonObj.transform.SetParent(parent.transform, false);

        // Add Button component
        Button button = buttonObj.AddComponent<Button>();
        UnityEngine.UI.Image buttonImage = buttonObj.AddComponent<UnityEngine.UI.Image>();
        buttonImage.color = new Color(0.2f, 0.3f, 0.8f, 0.8f); // Blue background

        // Set button size
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(180, 50);

        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        Text buttonTextComp = textObj.AddComponent<Text>();
        buttonTextComp.text = buttonText;
        buttonTextComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonTextComp.fontSize = 16;
        buttonTextComp.color = Color.white;
        buttonTextComp.alignment = TextAnchor.MiddleCenter;

        // Set text RectTransform to fill button
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        button.targetGraphic = buttonImage;

        // Assign button references
        if (buttonName == "ResumeButton") resumeButton = button;
        else if (buttonName == "RestartButton") restartButton = button;
        else if (buttonName == "MainMenuFromPauseButton") mainMenuFromPauseButton = button;

        Debug.Log($"UIManager: Created button: {buttonName}");
    }

    #region Hearts and Coins Management
    // 🩸 Cập nhật hiển thị tim (số hoặc icon)
    public void UpdateHearts(int hearts)
    {
        if (heartText != null)
            heartText.text = $" {hearts}";
    }

    // 🪙 Cập nhật hiển thị coin
    public void UpdateCoins(int coins)
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }
    #endregion

    #region Public Getters
    public bool IsPaused => isPaused;
    public bool IsInGame => isInGame;
    #endregion
}
