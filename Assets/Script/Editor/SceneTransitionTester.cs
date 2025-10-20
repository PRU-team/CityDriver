using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// Tool để test scene transition và UI state management
/// </summary>
public class SceneTransitionTester
{
    [MenuItem("Tools/CityDriver/Test Scene Transition", false, 10)]
    public static void TestSceneTransition()
    {
        if (Application.isPlaying)
        {
            // Test in play mode
            TestTransitionInPlayMode();
        }
        else
        {
            // Setup for testing
            SetupForSceneTransitionTest();
        }
    }

    private static void TestTransitionInPlayMode()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string targetScene = "";

        if (currentScene.ToLower().Contains("menu") || currentScene.ToLower().Contains("start"))
        {
            targetScene = "RacingGameplay";
        }
        else if (currentScene.ToLower().Contains("game") || currentScene.ToLower().Contains("racing"))
        {
            targetScene = "StartMenu";
        }
        else
        {
            Debug.LogWarning("Unknown scene - cannot determine target scene for testing");
            return;
        }

        Debug.Log($"Testing scene transition: {currentScene} → {targetScene}");
        
        // Check current state before transition
        LogCurrentUIState();
        
        // Perform transition
        SceneManager.LoadScene(targetScene);
    }

    private static void SetupForSceneTransitionTest()
    {
        Debug.Log("=== Setting up Scene Transition Test ===");

        // Ensure both camera and UI systems are properly setup
        bool setupComplete = true;

        // Check Camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null || !mainCamera.enabled)
        {
            Debug.LogWarning("Camera system needs setup - run Camera Fix first");
            setupComplete = false;
        }

        // Check UIManager
        SimpleUIManager simpleUIManager = Object.FindObjectOfType<SimpleUIManager>();
        UIManager uiManager = Object.FindObjectOfType<UIManager>();
        
        if (simpleUIManager == null && uiManager == null)
        {
            Debug.LogWarning("No UIManager found - creating SimpleUIManager");
            GameObject uiManagerObj = new GameObject("SimpleUIManager");
            simpleUIManager = uiManagerObj.AddComponent<SimpleUIManager>();
            setupComplete = false;
        }

        // Check MainMenuPanel in current scene
        GameObject mainMenuPanel = GameObject.Find("MainMenuPanel");
        if (mainMenuPanel == null)
        {
            Debug.LogWarning("MainMenuPanel not found in current scene");
            setupComplete = false;
        }

        if (setupComplete)
        {
            EditorUtility.DisplayDialog(
                "Scene Transition Test Setup", 
                "✅ Camera system: OK\n✅ UIManager: OK\n✅ MainMenuPanel: OK\n\nYou can now test scene transitions in Play mode.\n\nRun this tool again while in Play mode to test transitions.", 
                "OK"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Setup Required", 
                "Some components need setup before testing:\n\n" +
                "1. Run 'Fix MainMenuPanel (Quick)' first\n" +
                "2. Run 'Setup Complete Camera System' if needed\n" +
                "3. Then try this test again", 
                "OK"
            );
        }
    }

    private static void LogCurrentUIState()
    {
        Debug.Log("=== Current UI State ===");
        
        // Check panels
        GameObject mainMenuPanel = GameObject.Find("MainMenuPanel");
        GameObject gameHUDPanel = GameObject.Find("GameHUDPanel");
        
        Debug.Log($"MainMenuPanel: {(mainMenuPanel != null ? (mainMenuPanel.activeInHierarchy ? "Active" : "Inactive") : "Not Found")}");
        Debug.Log($"GameHUDPanel: {(gameHUDPanel != null ? (gameHUDPanel.activeInHierarchy ? "Active" : "Inactive") : "Not Found")}");
        
        // Check UIManager state
        SimpleUIManager simpleUIManager = Object.FindObjectOfType<SimpleUIManager>();
        if (simpleUIManager != null)
        {
            Debug.Log($"SimpleUIManager: Found, IsInGame={GetPrivateField(simpleUIManager, "isInGame")}");
        }
        
        UIManager uiManager = Object.FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            Debug.Log($"UIManager: Found, IsInGame={uiManager.IsInGame}");
        }
        
        // Check Camera
        Camera mainCamera = Camera.main;
        Debug.Log($"Main Camera: {(mainCamera != null && mainCamera.enabled ? "Working" : "Problem")}");
    }

    private static object GetPrivateField(object obj, string fieldName)
    {
        try
        {
            var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field?.GetValue(obj);
        }
        catch
        {
            return "Unknown";
        }
    }

    [MenuItem("Tools/CityDriver/Force UI State Reset", false, 11)]
    public static void ForceUIStateReset()
    {
        Debug.Log("=== Force UI State Reset ===");

        string currentScene = SceneManager.GetActiveScene().name;
        
        // Find UIManager
        SimpleUIManager simpleUIManager = Object.FindObjectOfType<SimpleUIManager>();
        UIManager uiManager = Object.FindObjectOfType<UIManager>();

        if (currentScene.ToLower().Contains("menu") || currentScene.ToLower().Contains("start"))
        {
            // Force show main menu
            GameObject mainMenuPanel = GameObject.Find("MainMenuPanel");
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
                Debug.Log("✅ MainMenuPanel force activated");
            }

            if (simpleUIManager != null)
            {
                simpleUIManager.ShowMainMenu();
                Debug.Log("✅ SimpleUIManager.ShowMainMenu() called");
            }
            else if (uiManager != null)
            {
                uiManager.ShowMainMenu();
                Debug.Log("✅ UIManager.ShowMainMenu() called");
            }
        }
        else if (currentScene.ToLower().Contains("game") || currentScene.ToLower().Contains("racing"))
        {
            // Force show game HUD
            if (simpleUIManager != null)
            {
                simpleUIManager.ShowGameHUD();
                Debug.Log("✅ SimpleUIManager.ShowGameHUD() called");
            }
            else if (uiManager != null)
            {
                uiManager.ShowGameHUD();
                Debug.Log("✅ UIManager.ShowGameHUD() called");
            }

            // Hide main menu if it exists
            GameObject mainMenuPanel = GameObject.Find("MainMenuPanel");
            if (mainMenuPanel != null && mainMenuPanel.activeInHierarchy)
            {
                mainMenuPanel.SetActive(false);
                Debug.Log("✅ MainMenuPanel hidden in gameplay scene");
            }
        }

        if (Application.isPlaying)
        {
            EditorUtility.DisplayDialog(
                "UI State Reset", 
                "UI state has been reset for current scene.\n\nCheck the scene view to see the changes.", 
                "OK"
            );
        }
    }

    [MenuItem("Tools/CityDriver/Debug UI State", false, 12)]
    public static void DebugUIState()
    {
        LogCurrentUIState();
        
        string report = "Check Console for detailed UI state information.\n\n";
        report += "Current Scene: " + SceneManager.GetActiveScene().name + "\n";
        
        GameObject mainMenuPanel = GameObject.Find("MainMenuPanel");
        report += $"MainMenuPanel: {(mainMenuPanel != null ? (mainMenuPanel.activeInHierarchy ? "✅ Visible" : "❌ Hidden") : "❌ Missing")}\n";
        
        Camera mainCamera = Camera.main;
        report += $"Camera: {(mainCamera != null && mainCamera.enabled ? "✅ Working" : "❌ Problem")}\n";

        EditorUtility.DisplayDialog("UI State Debug", report, "OK");
    }
}