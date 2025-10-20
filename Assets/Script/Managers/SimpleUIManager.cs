using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// UIManager đơn giản cho CityDriver game - tương thích với UIQuickFix
/// </summary>
public class SimpleUIManager : MonoBehaviour
{
    public static SimpleUIManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameHUDPanel;
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;

    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("Game HUD")]
    public Text speedText;
    public Text scoreText;
    public Text highScoreText;
    public Image fuelFill;
    public Button pauseButton;

    [Header("Pause Menu Buttons")]
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuFromPauseButton;

    [Header("Settings")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Button settingsCloseButton;

    [Header("Game Over")]
    public Text finalScoreText;
    public Button playAgainButton;
    public Button mainMenuFromGameOverButton;

    // Game state
    private bool isPaused = false;
    private bool isInGame = false;

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
        SetupButtonListeners();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"SimpleUIManager: Scene loaded - {scene.name}");
        StartCoroutine(DelayedUIRefresh());
    }

    private IEnumerator DelayedUIRefresh()
    {
        yield return new WaitForEndOfFrame();
        RefreshUIReferences();
        SetupButtonListeners();
        Debug.Log("SimpleUIManager: UI references refreshed for new scene");
    }

    private void RefreshUIReferences()
    {
        Debug.Log("SimpleUIManager: Refreshing UI references...");

        // Find panels
        if (mainMenuPanel == null)
            mainMenuPanel = GameObject.Find("MainMenuPanel");
        if (gameHUDPanel == null)
            gameHUDPanel = GameObject.Find("GameHUDPanel");
        if (pauseMenuPanel == null)
            pauseMenuPanel = GameObject.Find("PauseMenuPanel");
        if (settingsPanel == null)
            settingsPanel = GameObject.Find("SettingsPanel");
        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");

        // Find main menu buttons
        if (playButton == null)
        {
            GameObject obj = GameObject.Find("PlayButton");
            if (obj != null) playButton = obj.GetComponent<Button>();
        }
        if (settingsButton == null)
        {
            GameObject obj = GameObject.Find("SettingsButton");
            if (obj != null) settingsButton = obj.GetComponent<Button>();
        }
        if (exitButton == null)
        {
            GameObject obj = GameObject.Find("ExitButton");
            if (obj != null) exitButton = obj.GetComponent<Button>();
        }

        // Find game HUD elements
        if (speedText == null)
        {
            GameObject obj = GameObject.Find("SpeedText");
            if (obj != null) speedText = obj.GetComponent<Text>();
        }
        if (scoreText == null)
        {
            GameObject obj = GameObject.Find("ScoreText");
            if (obj != null) scoreText = obj.GetComponent<Text>();
        }
        if (highScoreText == null)
        {
            GameObject obj = GameObject.Find("HighScoreText");
            if (obj != null) highScoreText = obj.GetComponent<Text>();
        }
        if (fuelFill == null)
        {
            GameObject obj = GameObject.Find("FuelFill");
            if (obj != null) fuelFill = obj.GetComponent<Image>();
        }
        if (pauseButton == null)
        {
            GameObject obj = GameObject.Find("PauseButton");
            if (obj != null) pauseButton = obj.GetComponent<Button>();
        }

        // Find pause menu buttons
        if (resumeButton == null)
        {
            GameObject obj = GameObject.Find("ResumeButton");
            if (obj != null) resumeButton = obj.GetComponent<Button>();
        }
        if (restartButton == null)
        {
            GameObject obj = GameObject.Find("RestartButton");
            if (obj != null) restartButton = obj.GetComponent<Button>();
        }
        if (mainMenuFromPauseButton == null)
        {
            GameObject obj = GameObject.Find("MainMenuFromPauseButton");
            if (obj != null) mainMenuFromPauseButton = obj.GetComponent<Button>();
        }

        // Find settings elements
        if (masterVolumeSlider == null)
        {
            GameObject obj = GameObject.Find("MasterVolumeSlider");
            if (obj != null) masterVolumeSlider = obj.GetComponent<Slider>();
        }
        if (musicVolumeSlider == null)
        {
            GameObject obj = GameObject.Find("MusicVolumeSlider");
            if (obj != null) musicVolumeSlider = obj.GetComponent<Slider>();
        }
        if (sfxVolumeSlider == null)
        {
            GameObject obj = GameObject.Find("SFXVolumeSlider");
            if (obj != null) sfxVolumeSlider = obj.GetComponent<Slider>();
        }
        if (settingsCloseButton == null)
        {
            GameObject obj = GameObject.Find("SettingsCloseButton");
            if (obj != null) settingsCloseButton = obj.GetComponent<Button>();
        }

        // Find game over elements
        if (finalScoreText == null)
        {
            GameObject obj = GameObject.Find("FinalScoreText");
            if (obj != null) finalScoreText = obj.GetComponent<Text>();
        }
        if (playAgainButton == null)
        {
            GameObject obj = GameObject.Find("PlayAgainButton");
            if (obj != null) playAgainButton = obj.GetComponent<Button>();
        }
        if (mainMenuFromGameOverButton == null)
        {
            GameObject obj = GameObject.Find("MainMenuFromGameOverButton");
            if (obj != null) mainMenuFromGameOverButton = obj.GetComponent<Button>();
        }

        Debug.Log("SimpleUIManager: UI references refresh completed");
    }

    private void SetupButtonListeners()
    {
        try
        {
            // Clear existing listeners first
            ClearButtonListeners();

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

            // Game Over Buttons
            if (playAgainButton != null)
                playAgainButton.onClick.AddListener(PlayAgain);
            if (mainMenuFromGameOverButton != null)
                mainMenuFromGameOverButton.onClick.AddListener(BackToMainMenu);

            Debug.Log("SimpleUIManager: Button listeners setup completed");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"SimpleUIManager: Error setting up button listeners: {e.Message}");
        }
    }

    private void ClearButtonListeners()
    {
        try
        {
            if (playButton != null) playButton.onClick.RemoveAllListeners();
            if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
            if (exitButton != null) exitButton.onClick.RemoveAllListeners();
            if (resumeButton != null) resumeButton.onClick.RemoveAllListeners();
            if (restartButton != null) restartButton.onClick.RemoveAllListeners();
            if (mainMenuFromPauseButton != null) mainMenuFromPauseButton.onClick.RemoveAllListeners();
            if (pauseButton != null) pauseButton.onClick.RemoveAllListeners();
            if (settingsCloseButton != null) settingsCloseButton.onClick.RemoveAllListeners();
            if (playAgainButton != null) playAgainButton.onClick.RemoveAllListeners();
            if (mainMenuFromGameOverButton != null) mainMenuFromGameOverButton.onClick.RemoveAllListeners();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"SimpleUIManager: Error clearing button listeners: {e.Message}");
        }
    }

    #region UI Panel Management

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
        else if (!isInGame)
        {
            SetPanelActive(mainMenuPanel, false);
        }
    }

    public void HideSettings()
    {
        SetPanelActive(settingsPanel, false);
        
        // Restore previous state
        if (isInGame && !isPaused)
        {
            SetPanelActive(gameHUDPanel, true);
        }
        else if (!isInGame)
        {
            SetPanelActive(mainMenuPanel, true);
        }
        else if (isPaused)
        {
            SetPanelActive(pauseMenuPanel, true);
        }
    }

    public void ShowGameOver()
    {
        SetPanelActive(gameOverPanel, true);
        SetPanelActive(gameHUDPanel, false);
        SetPanelActive(pauseMenuPanel, false);

        Time.timeScale = 0f;
        isPaused = false;
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

    #region Button Event Handlers

    public void StartGame()
    {
        Debug.Log("SimpleUIManager: Starting game...");
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("RacingGameplay");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

    public void ExitGame()
    {
        Debug.Log("SimpleUIManager: Exiting game...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    #endregion

    #region HUD Updates

    public void UpdateSpeed(float speed)
    {
        if (speedText != null)
        {
            speedText.text = $"Speed: {speed:F0} km/h";
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void UpdateHighScore(int highScore)
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"Best: {highScore}";
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

    public void UpdateFinalScore(int finalScore)
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {finalScore}";
        }
    }

    #endregion

    private void OnDestroy()
    {
        // Cleanup scene loading event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}