using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Editor Window để quick fix UI issues
/// </summary>
public class UIQuickFixWindow : EditorWindow
{
    private bool autoDetectSceneType = true;
    private bool createMainMenuElements = false;
    private bool createGameplayElements = false;
    private bool createMinimalOnly = true;
    
    // Asset assignment section
    private bool showAssetSection = false;
    private Sprite buttonSprite;
    private Sprite buttonHoverSprite;
    private Sprite buttonPressedSprite;
    private Sprite panelSprite;
    private Font customFont;
    
    // Icon assets
    private Sprite playButtonIcon;
    private Sprite pauseButtonIcon;
    private Sprite settingsButtonIcon;
    private Sprite exitButtonIcon;
    private Sprite resumeButtonIcon;
    private Sprite restartButtonIcon;
    private Sprite menuButtonIcon;
    
    // HUD icons
    private Sprite speedIcon;
    private Sprite scoreIcon;
    private Sprite fuelIcon;
    
    [MenuItem("Tools/UI Quick Fix")]
    public static void ShowWindow()
    {
        GetWindow<UIQuickFixWindow>("UI Quick Fix");
    }
    
    private void OnGUI()
    {
        EditorGUILayout.LabelField("UI Quick Fix Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("This tool creates missing UI elements to prevent null reference errors in UIManager.", MessageType.Info);
        EditorGUILayout.Space();
        
        // Current scene info
        string currentScene = SceneManager.GetActiveScene().name;
        EditorGUILayout.LabelField($"Current Scene: {currentScene}");
        
        // Auto-detect settings
        autoDetectSceneType = EditorGUILayout.Toggle("Auto-detect Scene Type", autoDetectSceneType);
        
        if (!autoDetectSceneType)
        {
            createMainMenuElements = EditorGUILayout.Toggle("Create Main Menu Elements", createMainMenuElements);
            createGameplayElements = EditorGUILayout.Toggle("Create Gameplay Elements", createGameplayElements);
        }
        else
        {
            // Show what will be created
            bool isMainMenu = currentScene.ToLower().Contains("menu") || currentScene.ToLower().Contains("start");
            bool isGameplay = currentScene.ToLower().Contains("game") || currentScene.ToLower().Contains("racing") || FindObjectOfType<CarController>() != null;
            
            EditorGUILayout.LabelField("Will create:", EditorStyles.boldLabel);
            if (isMainMenu)
                EditorGUILayout.LabelField("• Main Menu Elements (PlayButton, SettingsButton, etc.)");
            if (isGameplay)
                EditorGUILayout.LabelField("• Gameplay Elements (GameHUD, PauseMenu, etc.)");
            if (!isMainMenu && !isGameplay)
                EditorGUILayout.LabelField("• No elements detected for this scene type");
        }
        
        EditorGUILayout.Space();
        createMinimalOnly = EditorGUILayout.Toggle("Create Minimal UI Only", createMinimalOnly);
        
        EditorGUILayout.Space();
        
        // Asset section
        showAssetSection = EditorGUILayout.Foldout(showAssetSection, "Custom Assets (Optional)", EditorStyles.foldoutHeader);
        if (showAssetSection)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.LabelField("Button Assets:", EditorStyles.boldLabel);
            buttonSprite = (Sprite)EditorGUILayout.ObjectField("Button Sprite", buttonSprite, typeof(Sprite), false);
            buttonHoverSprite = (Sprite)EditorGUILayout.ObjectField("Button Hover Sprite", buttonHoverSprite, typeof(Sprite), false);
            buttonPressedSprite = (Sprite)EditorGUILayout.ObjectField("Button Pressed Sprite", buttonPressedSprite, typeof(Sprite), false);
            panelSprite = (Sprite)EditorGUILayout.ObjectField("Panel Sprite", panelSprite, typeof(Sprite), false);
            customFont = (Font)EditorGUILayout.ObjectField("Custom Font", customFont, typeof(Font), false);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Button Icons:", EditorStyles.boldLabel);
            playButtonIcon = (Sprite)EditorGUILayout.ObjectField("Play Icon", playButtonIcon, typeof(Sprite), false);
            pauseButtonIcon = (Sprite)EditorGUILayout.ObjectField("Pause Icon", pauseButtonIcon, typeof(Sprite), false);
            settingsButtonIcon = (Sprite)EditorGUILayout.ObjectField("Settings Icon", settingsButtonIcon, typeof(Sprite), false);
            exitButtonIcon = (Sprite)EditorGUILayout.ObjectField("Exit Icon", exitButtonIcon, typeof(Sprite), false);
            resumeButtonIcon = (Sprite)EditorGUILayout.ObjectField("Resume Icon", resumeButtonIcon, typeof(Sprite), false);
            restartButtonIcon = (Sprite)EditorGUILayout.ObjectField("Restart Icon", restartButtonIcon, typeof(Sprite), false);
            menuButtonIcon = (Sprite)EditorGUILayout.ObjectField("Menu Icon", menuButtonIcon, typeof(Sprite), false);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("HUD Icons:", EditorStyles.boldLabel);
            speedIcon = (Sprite)EditorGUILayout.ObjectField("Speed Icon", speedIcon, typeof(Sprite), false);
            scoreIcon = (Sprite)EditorGUILayout.ObjectField("Score Icon", scoreIcon, typeof(Sprite), false);
            fuelIcon = (Sprite)EditorGUILayout.ObjectField("Fuel Icon", fuelIcon, typeof(Sprite), false);
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load from Resources"))
            {
                LoadAssetsFromResources();
            }
            if (GUILayout.Button("Load from Basic GUI Bundle"))
            {
                LoadAssetsFromBasicGUIBundle();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        
        // Quick fix button
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Quick Fix Missing UI Elements", GUILayout.Height(40)))
        {
            QuickFixUI();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        // Individual tools
        EditorGUILayout.LabelField("Individual Tools:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Canvas"))
        {
            CreateCanvas();
        }
        if (GUILayout.Button("Create EventSystem"))
        {
            CreateEventSystem();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Main Menu"))
        {
            CreateMainMenuElements();
        }
        if (GUILayout.Button("Create Game HUD"))
        {
            CreateGameplayElements();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Cleanup tools
        EditorGUILayout.LabelField("Cleanup Tools:", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Remove All Quick Fix UI"))
        {
            if (EditorUtility.DisplayDialog("Remove Quick Fix UI", 
                "This will remove all UI elements created by Quick Fix. Are you sure?", 
                "Yes", "Cancel"))
            {
                RemoveQuickFixUI();
            }
        }
        GUI.backgroundColor = Color.white;
    }
    
    private void QuickFixUI()
    {
        // Add UIQuickFix component to scene temporarily
        GameObject quickFixObj = new GameObject("UIQuickFix_Temp");
        UIQuickFix quickFix = quickFixObj.AddComponent<UIQuickFix>();
        
        // Configure based on settings
        if (!autoDetectSceneType)
        {
            // Set manual configuration using reflection
            var isMainMenuField = typeof(UIQuickFix).GetField("forceMainMenuScene", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isGameplayField = typeof(UIQuickFix).GetField("forceGameplayScene", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (isMainMenuField != null) isMainMenuField.SetValue(quickFix, createMainMenuElements);
            if (isGameplayField != null) isGameplayField.SetValue(quickFix, createGameplayElements);
        }
        
        // Apply custom assets if provided
        ApplyCustomAssets(quickFix);
        
        // Run quick fix
        quickFix.CreateMissingUIElements();
        
        // Remove temporary object
        DestroyImmediate(quickFixObj);
        
        EditorUtility.SetDirty(FindObjectOfType<Canvas>());
        Debug.Log("UIQuickFix: UI elements created successfully!");
    }
    
    private void CreateCanvas()
    {
        if (FindObjectOfType<Canvas>() != null)
        {
            Debug.LogWarning("Canvas already exists in scene");
            return;
        }
        
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        EditorUtility.SetDirty(canvasObj);
        Debug.Log("Canvas created successfully");
    }
    
    private void CreateEventSystem()
    {
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null)
        {
            Debug.LogWarning("EventSystem already exists in scene");
            return;
        }
        
        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        
        EditorUtility.SetDirty(eventSystemObj);
        Debug.Log("EventSystem created successfully");
    }
    
    private void CreateMainMenuElements()
    {
        GameObject quickFixObj = new GameObject("UIQuickFix_Temp");
        UIQuickFix quickFix = quickFixObj.AddComponent<UIQuickFix>();
        
        // Force main menu creation
        var isMainMenuField = typeof(UIQuickFix).GetField("forceMainMenuScene", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (isMainMenuField != null) isMainMenuField.SetValue(quickFix, true);
        
        quickFix.CreateMissingUIElements();
        DestroyImmediate(quickFixObj);
        
        EditorUtility.SetDirty(FindObjectOfType<Canvas>());
        Debug.Log("Main Menu elements created");
    }
    
    private void CreateGameplayElements()
    {
        GameObject quickFixObj = new GameObject("UIQuickFix_Temp");
        UIQuickFix quickFix = quickFixObj.AddComponent<UIQuickFix>();
        
        // Force gameplay creation
        var isGameplayField = typeof(UIQuickFix).GetField("forceGameplayScene", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (isGameplayField != null) isGameplayField.SetValue(quickFix, true);
        
        quickFix.CreateMissingUIElements();
        DestroyImmediate(quickFixObj);
        
        EditorUtility.SetDirty(FindObjectOfType<Canvas>());
        Debug.Log("Gameplay elements created");
    }
    
    private void RemoveQuickFixUI()
    {
        // List of UI elements created by QuickFix
        string[] quickFixElements = {
            "MainMenuPanel", "PlayButton", "SettingsButton", "ExitButton",
            "GameHUDPanel", "SpeedText", "ScoreText", "HighScoreText", "FuelFill", "PauseButton",
            "PauseMenuPanel", "ResumeButton", "RestartButton", "MainMenuFromPauseButton",
            "SettingsPanel", "MasterVolumeSlider", "MusicVolumeSlider", "SFXVolumeSlider", "SettingsCloseButton",
            "GameOverPanel", "FinalScoreText", "PlayAgainButton", "MainMenuFromGameOverButton"
        };
        
        int removedCount = 0;
        foreach (string elementName in quickFixElements)
        {
            GameObject element = GameObject.Find(elementName);
            if (element != null)
            {
                DestroyImmediate(element);
                removedCount++;
            }
        }
        
        Debug.Log($"Removed {removedCount} Quick Fix UI elements");
    }
    
    private void ApplyCustomAssets(UIQuickFix quickFix)
    {
        // Use reflection to apply custom assets to the UIQuickFix component
        var type = typeof(UIQuickFix);
        var bindingFlags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        
        // Button assets
        if (buttonSprite != null)
            type.GetField("buttonSprite", bindingFlags)?.SetValue(quickFix, buttonSprite);
        if (buttonHoverSprite != null)
            type.GetField("buttonHoverSprite", bindingFlags)?.SetValue(quickFix, buttonHoverSprite);
        if (buttonPressedSprite != null)
            type.GetField("buttonPressedSprite", bindingFlags)?.SetValue(quickFix, buttonPressedSprite);
        if (panelSprite != null)
            type.GetField("panelSprite", bindingFlags)?.SetValue(quickFix, panelSprite);
        if (customFont != null)
            type.GetField("customFont", bindingFlags)?.SetValue(quickFix, customFont);
        
        // Button icons
        if (playButtonIcon != null)
            type.GetField("playButtonIcon", bindingFlags)?.SetValue(quickFix, playButtonIcon);
        if (pauseButtonIcon != null)
            type.GetField("pauseButtonIcon", bindingFlags)?.SetValue(quickFix, pauseButtonIcon);
        if (settingsButtonIcon != null)
            type.GetField("settingsButtonIcon", bindingFlags)?.SetValue(quickFix, settingsButtonIcon);
        if (exitButtonIcon != null)
            type.GetField("exitButtonIcon", bindingFlags)?.SetValue(quickFix, exitButtonIcon);
        if (resumeButtonIcon != null)
            type.GetField("resumeButtonIcon", bindingFlags)?.SetValue(quickFix, resumeButtonIcon);
        if (restartButtonIcon != null)
            type.GetField("restartButtonIcon", bindingFlags)?.SetValue(quickFix, restartButtonIcon);
        if (menuButtonIcon != null)
            type.GetField("menuButtonIcon", bindingFlags)?.SetValue(quickFix, menuButtonIcon);
        
        // HUD icons
        if (speedIcon != null)
            type.GetField("speedIcon", bindingFlags)?.SetValue(quickFix, speedIcon);
        if (scoreIcon != null)
            type.GetField("scoreIcon", bindingFlags)?.SetValue(quickFix, scoreIcon);
        if (fuelIcon != null)
            type.GetField("fuelIcon", bindingFlags)?.SetValue(quickFix, fuelIcon);
    }
    
    private void LoadAssetsFromResources()
    {
        Debug.Log("Loading assets from Resources folder...");
        
        // Load button sprites
        if (buttonSprite == null)
            buttonSprite = Resources.Load<Sprite>("UI/button");
        if (buttonHoverSprite == null)
            buttonHoverSprite = Resources.Load<Sprite>("UI/button_hover");
        if (buttonPressedSprite == null)
            buttonPressedSprite = Resources.Load<Sprite>("UI/button_pressed");
        if (panelSprite == null)
            panelSprite = Resources.Load<Sprite>("UI/panel");
        
        // Load custom font
        if (customFont == null)
            customFont = Resources.Load<Font>("Fonts/UIFont");
        
        // Load icon sprites
        if (playButtonIcon == null)
            playButtonIcon = Resources.Load<Sprite>("UI/Icons/play_icon");
        if (pauseButtonIcon == null)
            pauseButtonIcon = Resources.Load<Sprite>("UI/Icons/pause_icon");
        if (settingsButtonIcon == null)
            settingsButtonIcon = Resources.Load<Sprite>("UI/Icons/settings_icon");
        if (exitButtonIcon == null)
            exitButtonIcon = Resources.Load<Sprite>("UI/Icons/exit_icon");
        if (resumeButtonIcon == null)
            resumeButtonIcon = Resources.Load<Sprite>("UI/Icons/resume_icon");
        if (restartButtonIcon == null)
            restartButtonIcon = Resources.Load<Sprite>("UI/Icons/restart_icon");
        if (menuButtonIcon == null)
            menuButtonIcon = Resources.Load<Sprite>("UI/Icons/menu_icon");
        
        // Load HUD icons
        if (speedIcon == null)
            speedIcon = Resources.Load<Sprite>("UI/Icons/speed_icon");
        if (scoreIcon == null)
            scoreIcon = Resources.Load<Sprite>("UI/Icons/score_icon");
        if (fuelIcon == null)
            fuelIcon = Resources.Load<Sprite>("UI/Icons/fuel_icon");
        
        Debug.Log("Asset loading from Resources completed!");
    }
    
    private void LoadAssetsFromBasicGUIBundle()
    {
        Debug.Log("Loading assets from Basic_GUI_Bundle...");
        
        // Load from Basic_GUI_Bundle path
        string basePath = "Basic_GUI_Bundle/";
        
        // Load button sprites
        if (buttonSprite == null)
            buttonSprite = Resources.Load<Sprite>(basePath + "Buttons/button_01") ?? 
                          Resources.Load<Sprite>(basePath + "Banners/banner_01");
        
        if (panelSprite == null)
            panelSprite = Resources.Load<Sprite>(basePath + "Boxes/box_01");
        
        // Try to load icons from the bundle
        if (playButtonIcon == null)
            playButtonIcon = Resources.Load<Sprite>(basePath + "Icons/icon_play") ?? 
                            Resources.Load<Sprite>(basePath + "Icons/icon_01");
        
        if (pauseButtonIcon == null)
            pauseButtonIcon = Resources.Load<Sprite>(basePath + "Icons/icon_pause") ?? 
                             Resources.Load<Sprite>(basePath + "Icons/icon_02");
        
        if (settingsButtonIcon == null)
            settingsButtonIcon = Resources.Load<Sprite>(basePath + "Icons/icon_settings") ?? 
                                Resources.Load<Sprite>(basePath + "Icons/icon_03");
        
        Debug.Log("Basic GUI Bundle asset loading completed!");
    }
}