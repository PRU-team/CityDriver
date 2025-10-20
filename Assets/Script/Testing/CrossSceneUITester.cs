using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Test script để verify cross-scene UI functionality
/// Attach vào UIManager để test automatic scene transitions
/// </summary>
public class CrossSceneUITester : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool enableTestOnStart = false;
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private float testDelay = 2f;
    
    [Header("Scene Names")]
    [SerializeField] private string startMenuScene = "StartMenu";
    [SerializeField] private string gameplayScene = "RacingGameplay";
    
    private void Start()
    {
        if (enableTestOnStart)
        {
            StartCoroutine(DelayedStartTest());
        }
        
        if (showDebugLogs)
        {
            LogCurrentSceneInfo();
        }
    }
    
    private void Update()
    {
        // Manual test controls using Input System
        if (Keyboard.current != null)
        {
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                TestCurrentSceneUI();
            }
            
            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                StartCoroutine(RunCrossSceneTest());
            }
            
            if (Keyboard.current.f3Key.wasPressedThisFrame)
            {
                TestUIManagerProperties();
            }
        }
    }
    
    private IEnumerator DelayedStartTest()
    {
        yield return new WaitForSeconds(testDelay);
        TestCurrentSceneUI();
    }
    
    [ContextMenu("Test Current Scene UI")]
    public void TestCurrentSceneUI()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        DebugLog($"=== Testing UI in scene: {currentScene} ===");
        
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            DebugLog("ERROR: UIManager instance not found!");
            return;
        }
        
        // Test basic UIManager state
        DebugLog($"UIManager IsInGame: {uiManager.IsInGame}");
        DebugLog($"UIManager IsPaused: {uiManager.IsPaused}");
        
        // Test UI element detection
        TestUIElementDetection();
        
        // Test scene-specific functionality
        if (currentScene.ToLower().Contains("menu") || currentScene.ToLower().Contains("start"))
        {
            TestMainMenuUI();
        }
        else if (currentScene.ToLower().Contains("game") || currentScene.ToLower().Contains("racing"))
        {
            TestGameplayUI();
        }
    }
    
    private void TestUIElementDetection()
    {
        DebugLog("--- Testing UI Element Detection ---");
        
        // Test common UI elements
        string[] commonElements = {
            "PlayButton", "SettingsButton", "ExitButton",
            "PauseButton", "ResumeButton", "RestartButton",
            "GameHUDPanel", "PauseMenuPanel", "SettingsPanel",
            "SpeedText", "ScoreText", "FinalScoreText"
        };
        
        foreach (string elementName in commonElements)
        {
            GameObject element = GameObject.Find(elementName);
            DebugLog($"{elementName}: {(element != null ? "Found" : "Missing")}");
        }
    }
    
    private void TestMainMenuUI()
    {
        DebugLog("--- Testing Main Menu UI ---");
        
        // Test main menu specific elements
        GameObject playButton = GameObject.Find("PlayButton");
        if (playButton != null)
        {
            DebugLog("PlayButton found - testing click simulation");
            // Could add button click simulation here
        }
        
        GameObject settingsButton = GameObject.Find("SettingsButton");
        if (settingsButton != null)
        {
            DebugLog("SettingsButton found");
        }
    }
    
    private void TestGameplayUI()
    {
        DebugLog("--- Testing Gameplay UI ---");
        
        // Test gameplay specific elements
        GameObject pauseButton = GameObject.Find("PauseButton");
        if (pauseButton != null)
        {
            DebugLog("PauseButton found - testing pause functionality");
            TestPauseFunctionality();
        }
        
        GameObject gameHUD = GameObject.Find("GameHUDPanel");
        if (gameHUD != null)
        {
            DebugLog("GameHUD found");
        }
    }
    
    private void TestPauseFunctionality()
    {
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null) return;
        
        bool wasPaused = uiManager.IsPaused;
        
        // Test pause
        if (!wasPaused)
        {
            uiManager.PauseGame();
            DebugLog($"Pause test: Game paused = {uiManager.IsPaused}");
            
            // Test resume after a short delay
            StartCoroutine(TestResumeAfterDelay());
        }
    }
    
    private IEnumerator TestResumeAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null && uiManager.IsPaused)
        {
            uiManager.ResumeGame();
            DebugLog($"Resume test: Game paused = {uiManager.IsPaused}");
        }
    }
    
    [ContextMenu("Run Cross-Scene Test")]
    public IEnumerator RunCrossSceneTest()
    {
        DebugLog("=== Starting Cross-Scene Test ===");
        
        string currentScene = SceneManager.GetActiveScene().name;
        string targetScene = currentScene.ToLower().Contains("menu") ? gameplayScene : startMenuScene;
        
        DebugLog($"Current Scene: {currentScene}");
        DebugLog($"Target Scene: {targetScene}");
        
        // Test current scene first
        TestCurrentSceneUI();
        
        yield return new WaitForSeconds(2f);
        
        // Load target scene
        DebugLog($"Loading scene: {targetScene}");
        SceneManager.LoadScene(targetScene);
        
        // Note: The test will continue in the new scene if this object survives
        yield return new WaitForSeconds(3f);
        
        // Test target scene
        TestCurrentSceneUI();
        
        DebugLog("=== Cross-Scene Test Complete ===");
    }
    
    [ContextMenu("Test UIManager Properties")]
    public void TestUIManagerProperties()
    {
        DebugLog("=== Testing UIManager Properties ===");
        
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            DebugLog("ERROR: UIManager instance not found!");
            return;
        }
        
        // Test singleton persistence
        DebugLog($"UIManager GameObject: {uiManager.gameObject.name}");
        DebugLog($"UIManager DontDestroyOnLoad: {uiManager.gameObject.scene.name == "DontDestroyOnLoad"}");
        
        // Test state properties
        DebugLog($"IsInGame: {uiManager.IsInGame}");
        DebugLog($"IsPaused: {uiManager.IsPaused}");
        
        // Test UI references
        var fields = typeof(UIManager).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(GameObject) || field.FieldType == typeof(UnityEngine.UI.Button))
            {
                var value = field.GetValue(uiManager);
                DebugLog($"{field.Name}: {(value != null ? "Assigned" : "Null")}");
            }
        }
    }
    
    private void LogCurrentSceneInfo()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        DebugLog($"Current Scene: {sceneName} (Index: {sceneIndex})");
        
        // Check for scene-specific objects
        bool hasCarController = FindObjectOfType<CarController>() != null;
        bool hasUIManager = UIManager.Instance != null;
        bool hasCanvas = FindObjectOfType<Canvas>() != null;
        
        DebugLog($"Scene Objects - CarController: {hasCarController}, UIManager: {hasUIManager}, Canvas: {hasCanvas}");
    }
    
    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[CrossSceneUITester] {message}");
        }
    }
    
    // Public methods for external testing
    public void SimulatePlayButtonClick()
    {
        DebugLog("Simulating Play button click");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.StartGame();
        }
    }
    
    public void SimulatePauseButtonClick()
    {
        DebugLog("Simulating Pause button click");
        if (UIManager.Instance != null)
        {
            if (UIManager.Instance.IsPaused)
                UIManager.Instance.ResumeGame();
            else
                UIManager.Instance.PauseGame();
        }
    }
    
    public void SimulateSettingsButtonClick()
    {
        DebugLog("Simulating Settings button click");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowSettings();
        }
    }
}