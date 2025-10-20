using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// Tool để fix UI MainMenuPanel không hiển thị
/// </summary>
public class UIMainMenuFix
{
    [MenuItem("Tools/CityDriver/Fix MainMenuPanel (Quick)", false, 0)]
    public static void FixMainMenuPanel()
    {
        Debug.Log("=== Fixing MainMenuPanel ===");

        bool success = false;

        // Step 1: Tìm MainMenuPanel
        GameObject mainMenuPanel = GameObject.Find("MainMenuPanel");
        
        if (mainMenuPanel != null)
        {
            // Panel exists but might be inactive
            if (!mainMenuPanel.activeInHierarchy)
            {
                mainMenuPanel.SetActive(true);
                Debug.Log("✅ Activated existing MainMenuPanel");
                success = true;
            }
            else
            {
                Debug.Log("✅ MainMenuPanel already active");
                success = true;
            }
        }
        else
        {
            // Panel doesn't exist - create it
            success = CreateMainMenuPanelFromScratch();
        }

        // Step 2: Ensure UIManager shows main menu
        if (success)
        {
            ForceShowMainMenu();
        }

        // Step 3: Report result
        if (success)
        {
            EditorUtility.DisplayDialog(
                "MainMenuPanel Fixed!", 
                "✅ MainMenuPanel is now visible!\n✅ UIManager state reset\n\nYour main menu should be working now.", 
                "OK"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Fix Failed", 
                "❌ Could not fix MainMenuPanel.\n\nTry running 'Create UI System' first.", 
                "OK"
            );
        }
    }

    [MenuItem("Tools/CityDriver/Create Complete UI System", false, 1)]
    public static void CreateCompleteUISystem()
    {
        Debug.Log("=== Creating Complete UI System ===");

        // Step 1: Create Canvas if missing
        Canvas canvas = EnsureCanvasExists();

        // Step 2: Create EventSystem if missing
        EnsureEventSystemExists();

        // Step 3: Create MainMenuPanel
        GameObject mainMenuPanel = CreateMainMenuPanelFromScratch();

        // Step 4: Ensure UIManager exists
        EnsureUIManagerExists();

        // Step 5: Force show main menu
        ForceShowMainMenu();

        Debug.Log("✅ Complete UI System created!");
        
        EditorUtility.DisplayDialog(
            "UI System Created", 
            "✅ Canvas created\n✅ EventSystem created\n✅ MainMenuPanel created\n✅ UIManager setup\n\nYour UI should be working now!", 
            "OK"
        );
    }

    private static Canvas EnsureCanvasExists()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            Debug.Log("✅ Created Canvas");
        }
        return canvas;
    }

    private static void EnsureEventSystemExists()
    {
        UnityEngine.EventSystems.EventSystem eventSystem = Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            Debug.Log("✅ Created EventSystem");
        }
    }

    private static GameObject CreateMainMenuPanelFromScratch()
    {
        Canvas canvas = EnsureCanvasExists();
        
        // Remove existing MainMenuPanel if any
        GameObject existingPanel = GameObject.Find("MainMenuPanel");
        if (existingPanel != null)
        {
            Object.DestroyImmediate(existingPanel);
            Debug.Log("Removed existing MainMenuPanel");
        }

        // Create new MainMenuPanel
        GameObject panel = new GameObject("MainMenuPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Dark background

        // Create Game Title
        CreateUIText("GameTitle", "CITY DRIVER", panel.transform, 
            new Vector2(0, 250), new Vector2(600, 80), 48, Color.white);

        // Create buttons
        CreateUIButton("PlayButton", "PLAY", panel.transform, 
            new Vector2(0, 100), new Vector2(250, 60), Color.green);
            
        CreateUIButton("SettingsButton", "SETTINGS", panel.transform, 
            new Vector2(0, 20), new Vector2(250, 60), Color.blue);
            
        CreateUIButton("ExitButton", "EXIT", panel.transform, 
            new Vector2(0, -60), new Vector2(250, 60), Color.red);

        // Mark as dirty for saving
        EditorUtility.SetDirty(panel);
        
        Debug.Log("✅ Created new MainMenuPanel with all buttons");
        return panel;
    }

    private static void CreateUIText(string name, string text, Transform parent, Vector2 position, Vector2 size, int fontSize, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchoredPosition = position;
        textRect.sizeDelta = size;
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = color;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        EditorUtility.SetDirty(textObj);
    }

    private static void CreateUIButton(string name, string text, Transform parent, Vector2 position, Vector2 size, Color buttonColor)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = size;

        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = buttonColor;

        Button button = buttonObj.AddComponent<Button>();
        
        // Create button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.fontSize = 20;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        EditorUtility.SetDirty(buttonObj);
    }

    private static void EnsureUIManagerExists()
    {
        UIManager uiManager = Object.FindObjectOfType<UIManager>();
        SimpleUIManager simpleUIManager = Object.FindObjectOfType<SimpleUIManager>();

        if (uiManager == null && simpleUIManager == null)
        {
            // Create SimpleUIManager (easier to work with)
            GameObject uiManagerObj = new GameObject("SimpleUIManager");
            simpleUIManager = uiManagerObj.AddComponent<SimpleUIManager>();
            Debug.Log("✅ Created SimpleUIManager");
        }
    }

    private static void ForceShowMainMenu()
    {
        Debug.Log("--- Forcing UI to show main menu ---");

        // Find and activate MainMenuPanel directly
        GameObject mainMenuPanel = GameObject.Find("MainMenuPanel");
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            Debug.Log("✅ MainMenuPanel force activated");
        }

        // Try to call ShowMainMenu on any UIManager
        UIManager uiManager = Object.FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowMainMenu();
            Debug.Log("✅ UIManager.ShowMainMenu() called");
            return;
        }

        SimpleUIManager simpleUIManager = Object.FindObjectOfType<SimpleUIManager>();
        if (simpleUIManager != null)
        {
            simpleUIManager.ShowMainMenu();
            Debug.Log("✅ SimpleUIManager.ShowMainMenu() called");
            return;
        }

        Debug.Log("⚠️ No UIManager found - but MainMenuPanel should still be visible");
    }

    [MenuItem("Tools/CityDriver/Diagnose UI Problem", false, 2)]
    public static void DiagnoseUIProblem()
    {
        Debug.Log("=== UI Problem Diagnosis ===");

        string report = "UI Diagnosis Report:\n\n";

        // Check Canvas
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        report += $"Canvas: {(canvas != null ? "✅ Found" : "❌ Missing")}\n";

        // Check MainMenuPanel
        GameObject mainMenuPanel = GameObject.Find("MainMenuPanel");
        if (mainMenuPanel != null)
        {
            report += $"MainMenuPanel: ✅ Found\n";
            report += $"  - Active: {(mainMenuPanel.activeInHierarchy ? "✅ Yes" : "❌ No")}\n";
            report += $"  - Parent: {(mainMenuPanel.transform.parent != null ? mainMenuPanel.transform.parent.name : "None")}\n";
        }
        else
        {
            report += "MainMenuPanel: ❌ Not Found\n";
        }

        // Check UIManagers
        UIManager uiManager = Object.FindObjectOfType<UIManager>();
        SimpleUIManager simpleUIManager = Object.FindObjectOfType<SimpleUIManager>();
        
        if (uiManager != null)
        {
            report += "UIManager: ✅ Found\n";
            report += $"  - MainMenuPanel ref: {(uiManager.mainMenuPanel != null ? "✅ Set" : "❌ Null")}\n";
        }
        else if (simpleUIManager != null)
        {
            report += "SimpleUIManager: ✅ Found\n";
            report += $"  - MainMenuPanel ref: {(simpleUIManager.mainMenuPanel != null ? "✅ Set" : "❌ Null")}\n";
        }
        else
        {
            report += "UIManager: ❌ Missing\n";
        }

        // Check Camera
        Camera mainCamera = Camera.main;
        report += $"Main Camera: {(mainCamera != null && mainCamera.enabled ? "✅ Working" : "❌ Problem")}\n";

        Debug.Log(report);
        
        EditorUtility.DisplayDialog("UI Diagnosis", report, "OK");
    }
}