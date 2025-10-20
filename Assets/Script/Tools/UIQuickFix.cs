using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// UIQuickFix - Tool tự động tạo tất cả UI elements cần thiết cho CityDriver game
/// Hỗ trợ MainMenu và Gameplay scenes với đầy đủ chức năng
/// Improved layout với better positioning và sizing
/// </summary>
public class UIQuickFix : MonoBehaviour
{
    [Header("Quick Fix Settings")]
    [SerializeField] private bool autoCreateOnStart = true;
    [SerializeField] private bool createMinimalUI = true;
    [SerializeField] private Canvas targetCanvas;
    
    [Header("Scene Detection")]
    [SerializeField] private bool forceMainMenuScene = false;
    [SerializeField] private bool forceGameplayScene = false;
    
    [Header("UI Styling")]
    [SerializeField] private Color primaryButtonColor = new Color(0.2f, 0.6f, 1f, 1f);
    [SerializeField] private Color panelColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private int fontSize = 18;
    
    [Header("Button Assets")]
    [SerializeField] private Sprite buttonSprite;
    [SerializeField] private Sprite buttonHoverSprite;
    [SerializeField] private Sprite buttonPressedSprite;
    [SerializeField] private Sprite panelSprite;
    [SerializeField] private Font customFont;
    
    [Header("Icon Assets")]
    [SerializeField] private Sprite playButtonIcon;
    [SerializeField] private Sprite pauseButtonIcon;
    [SerializeField] private Sprite settingsButtonIcon;
    [SerializeField] private Sprite exitButtonIcon;
    [SerializeField] private Sprite resumeButtonIcon;
    [SerializeField] private Sprite restartButtonIcon;
    [SerializeField] private Sprite menuButtonIcon;
    
    [Header("HUD Icons")]
    [SerializeField] private Sprite speedIcon;
    [SerializeField] private Sprite scoreIcon;
    [SerializeField] private Sprite fuelIcon;
    
    private bool hasCreatedUI = false;
    
    private void Start()
    {
        // Try to load assets if not assigned
        if (buttonSprite == null || playButtonIcon == null)
        {
            LoadAssetsFromResources();
            if (buttonSprite == null) // If still null, try Basic GUI Bundle
            {
                LoadAssetsFromBasicGUIBundle();
            }
        }
        
        if (autoCreateOnStart && !hasCreatedUI)
        {
            StartCoroutine(DelayedUICreation());
        }
    }
    
    private IEnumerator DelayedUICreation()
    {
        yield return new WaitForEndOfFrame();
        CreateMissingUIElements();
    }
    
    [ContextMenu("Create Missing UI Elements")]
    public void CreateMissingUIElements()
    {
        if (hasCreatedUI)
        {
            Debug.Log("UIQuickFix: UI already created, skipping...");
            return;
        }
        
        Debug.Log("UIQuickFix: Starting UI creation process...");
        
        // Step 1: Ensure Canvas and EventSystem exist
        EnsureCanvasAndEventSystem();
        
        // Step 2: Detect scene type and create appropriate UI
        bool isMainMenu = DetectMainMenuScene();
        bool isGameplay = DetectGameplayScene();
        
        if (isMainMenu)
        {
            CreateMainMenuUI();
            Debug.Log("UIQuickFix: Created Main Menu UI elements");
        }
        
        if (isGameplay)
        {
            CreateGameplayUI();
            Debug.Log("UIQuickFix: Created Gameplay UI elements");
        }
        
        if (!isMainMenu && !isGameplay)
        {
            Debug.LogWarning("UIQuickFix: Could not detect scene type. Creating basic UI...");
            CreateBasicUI();
        }
        
        hasCreatedUI = true;
        Debug.Log("UIQuickFix: UI creation completed!");
    }
    
    #region Scene Detection
    
    private bool DetectMainMenuScene()
    {
        if (forceMainMenuScene) return true;
        
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToLower();
        return sceneName.Contains("menu") || sceneName.Contains("start") || sceneName.Contains("main");
    }
    
    private bool DetectGameplayScene()
    {
        if (forceGameplayScene) return true;
        
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToLower();
        bool hasCarController = FindObjectOfType<CarController>() != null;
        bool hasGameManager = FindObjectOfType<GameManager>() != null;
        
        return sceneName.Contains("game") || sceneName.Contains("racing") || sceneName.Contains("play") || 
               hasCarController || hasGameManager;
    }
    
    #endregion
    
    #region Canvas and EventSystem Setup
    
    private void EnsureCanvasAndEventSystem()
    {
        // Find or create Canvas
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                targetCanvas = CreateCanvas();
            }
        }
        
        // Ensure EventSystem exists
        if (FindObjectOfType<EventSystem>() == null)
        {
            CreateEventSystem();
        }
    }
    
    private Canvas CreateCanvas()
    {
        Debug.Log("UIQuickFix: Creating Canvas...");
        
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); // Updated to 1080p
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        return canvas;
    }
    
    private void CreateEventSystem()
    {
        Debug.Log("UIQuickFix: Creating EventSystem...");
        
        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<EventSystem>();
        eventSystemObj.AddComponent<StandaloneInputModule>();
    }
    
    #endregion
    
    #region Main Menu UI Creation
    
    private void CreateMainMenuUI()
    {
        Debug.Log("UIQuickFix: Creating Main Menu UI...");
        
        // Create Main Menu Panel - better sized and centered
        if (GameObject.Find("MainMenuPanel") == null)
        {
            GameObject mainMenuPanel = CreatePanel("MainMenuPanel", new Vector2(600, 800));
            
            // Title with better positioning
            CreateText("GameTitle", "CITY DRIVER", mainMenuPanel.transform, new Vector2(0, 300), 36, TextAnchor.MiddleCenter);
            
            // Main buttons with better spacing and size
            CreateButtonWithIcon("PlayButton", "PLAY", mainMenuPanel.transform, new Vector2(0, 150), new Vector2(300, 80), playButtonIcon);
            CreateButtonWithIcon("SettingsButton", "SETTINGS", mainMenuPanel.transform, new Vector2(0, 50), new Vector2(300, 80), settingsButtonIcon);
            CreateButtonWithIcon("ExitButton", "EXIT", mainMenuPanel.transform, new Vector2(0, -50), new Vector2(300, 80), exitButtonIcon);
        }
        
        // Create Settings Panel with improved layout
        if (GameObject.Find("SettingsPanel") == null)
        {
            GameObject settingsPanel = CreatePanel("SettingsPanel", new Vector2(700, 600));
            settingsPanel.SetActive(false);
            
            // Settings title
            CreateText("SettingsTitle", "SETTINGS", settingsPanel.transform, new Vector2(0, 250), 28, TextAnchor.MiddleCenter);
            
            // Volume sliders with better spacing
            CreateSlider("MasterVolumeSlider", "Master Volume", settingsPanel.transform, new Vector2(0, 150));
            CreateSlider("MusicVolumeSlider", "Music Volume", settingsPanel.transform, new Vector2(0, 80));
            CreateSlider("SFXVolumeSlider", "SFX Volume", settingsPanel.transform, new Vector2(0, 10));
            
            // Close button
            CreateButtonWithIcon("SettingsCloseButton", "CLOSE", settingsPanel.transform, new Vector2(0, -150), new Vector2(200, 60), exitButtonIcon);
        }
    }
    
    #endregion
    
    #region Gameplay UI Creation
    
    private void CreateGameplayUI()
    {
        Debug.Log("UIQuickFix: Creating Gameplay UI...");
        
        // Create Game HUD Panel - repositioned to top-left with better size
        if (GameObject.Find("GameHUDPanel") == null)
        {
            GameObject gameHUDPanel = CreatePanel("GameHUDPanel", new Vector2(350, 150));
            
            // Position at top-left with proper anchoring
            RectTransform hudRect = gameHUDPanel.GetComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(0, 1);
            hudRect.anchorMax = new Vector2(0, 1);
            hudRect.pivot = new Vector2(0, 1);
            hudRect.anchoredPosition = new Vector2(20, -20);
            
            // HUD elements with better layout
            CreateHUDElement("SpeedText", "Speed: 0 km/h", gameHUDPanel.transform, new Vector2(0, -20), speedIcon, 16);
            CreateHUDElement("ScoreText", "Score: 0", gameHUDPanel.transform, new Vector2(0, -50), scoreIcon, 16);
            CreateText("HighScoreText", "Best: 0", gameHUDPanel.transform, new Vector2(0, -80), 14, TextAnchor.MiddleCenter);
            
            // Fuel bar with better positioning
            CreateFuelBar(gameHUDPanel.transform, new Vector2(0, -110));
        }
        
        // Create Pause Button - repositioned to top-right
        if (GameObject.Find("PauseButton") == null)
        {
            GameObject pauseButton = CreateButtonWithIcon("PauseButton", pauseButtonIcon != null ? "" : "||", targetCanvas.transform, new Vector2(-80, -80), new Vector2(70, 70), pauseButtonIcon);
            
            // Position at top-right with proper anchoring
            RectTransform pauseRect = pauseButton.GetComponent<RectTransform>();
            pauseRect.anchorMin = new Vector2(1, 1);
            pauseRect.anchorMax = new Vector2(1, 1);
            pauseRect.pivot = new Vector2(1, 1);
        }
        
        // Create Pause Menu Panel - better centered and sized
        if (GameObject.Find("PauseMenuPanel") == null)
        {
            GameObject pauseMenuPanel = CreatePanel("PauseMenuPanel", new Vector2(500, 600));
            pauseMenuPanel.SetActive(false);
            
            // Pause title
            CreateText("PauseTitle", "PAUSED", pauseMenuPanel.transform, new Vector2(0, 220), 32, TextAnchor.MiddleCenter);
            
            // Pause menu buttons with better spacing
            CreateButtonWithIcon("ResumeButton", "RESUME", pauseMenuPanel.transform, new Vector2(0, 120), new Vector2(280, 70), resumeButtonIcon);
            CreateButtonWithIcon("RestartButton", "RESTART", pauseMenuPanel.transform, new Vector2(0, 30), new Vector2(280, 70), restartButtonIcon);
            CreateButtonWithIcon("MainMenuFromPauseButton", "MAIN MENU", pauseMenuPanel.transform, new Vector2(0, -60), new Vector2(280, 70), menuButtonIcon);
        }
        
        // Create Game Over Panel - better centered and sized
        if (GameObject.Find("GameOverPanel") == null)
        {
            GameObject gameOverPanel = CreatePanel("GameOverPanel", new Vector2(600, 500));
            gameOverPanel.SetActive(false);
            
            // Game over title
            CreateText("GameOverTitle", "GAME OVER", gameOverPanel.transform, new Vector2(0, 180), 32, TextAnchor.MiddleCenter);
            
            // Final score
            CreateText("FinalScoreText", "Final Score: 0", gameOverPanel.transform, new Vector2(0, 100), 22, TextAnchor.MiddleCenter);
            
            // Game over buttons with better spacing
            CreateButtonWithIcon("PlayAgainButton", "PLAY AGAIN", gameOverPanel.transform, new Vector2(0, 20), new Vector2(280, 70), playButtonIcon);
            CreateButtonWithIcon("MainMenuFromGameOverButton", "MAIN MENU", gameOverPanel.transform, new Vector2(0, -60), new Vector2(280, 70), menuButtonIcon);
        }
    }
    
    #endregion
    
    #region Basic UI Creation
    
    private void CreateBasicUI()
    {
        Debug.Log("UIQuickFix: Creating basic UI elements...");
        
        // Create a simple panel with basic elements
        if (GameObject.Find("MainPanel") == null)
        {
            GameObject mainPanel = CreatePanel("MainPanel", new Vector2(400, 300));
            CreateText("InfoText", "Basic UI Created", mainPanel.transform, new Vector2(0, 50), 18, TextAnchor.MiddleCenter);
            CreateButton("BasicButton", "CLICK ME", mainPanel.transform, new Vector2(0, 0), new Vector2(200, 60));
        }
    }
    
    #endregion
    
    #region Improved UI Helper Methods
    
    private GameObject CreatePanel(string name, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(targetCanvas.transform, false);
        
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        
        // Center the panel properly
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = Vector2.zero;
        
        Image image = panel.AddComponent<Image>();
        
        // Use custom sprite if available, otherwise use color
        if (panelSprite != null)
        {
            image.sprite = panelSprite;
            image.type = Image.Type.Sliced;
        }
        else
        {
            image.color = panelColor;
        }
        
        return panel;
    }
    
    private GameObject CreateButton(string name, string text, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;
        // Center relative to parent
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        
        Image image = buttonObj.AddComponent<Image>();
        Button button = buttonObj.AddComponent<Button>();
        
        // Configure button appearance based on available assets
        if (buttonSprite != null)
        {
            image.sprite = buttonSprite;
            image.type = Image.Type.Sliced;
            
            // Setup sprite state for button
            SpriteState spriteState = new SpriteState();
            if (buttonHoverSprite != null)
                spriteState.highlightedSprite = buttonHoverSprite;
            if (buttonPressedSprite != null)
                spriteState.pressedSprite = buttonPressedSprite;
            
            button.spriteState = spriteState;
            button.transition = Selectable.Transition.SpriteSwap;
        }
        else
        {
            // Fallback to color-based buttons
            image.color = primaryButtonColor;
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(primaryButtonColor.r * 1.2f, primaryButtonColor.g * 1.2f, primaryButtonColor.b * 1.2f, 1f);
            colors.pressedColor = new Color(primaryButtonColor.r * 0.8f, primaryButtonColor.g * 0.8f, primaryButtonColor.b * 0.8f, 1f);
            button.colors = colors;
            button.transition = Selectable.Transition.ColorTint;
        }
        
        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = Vector2.zero;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = customFont != null ? customFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = fontSize;
        textComponent.color = textColor;
        textComponent.alignment = TextAnchor.MiddleCenter;
        
        return buttonObj;
    }
    
    private GameObject CreateButtonWithIcon(string name, string text, Transform parent, Vector2 position, Vector2 size, Sprite icon = null)
    {
        GameObject buttonObj = CreateButton(name, text, parent, position, size);
        
        // Add icon if provided
        if (icon != null)
        {
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(buttonObj.transform, false);
            
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(size.y * 0.6f, size.y * 0.6f); // Icon size based on button height
            iconRect.anchorMin = new Vector2(0, 0.5f);
            iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.anchoredPosition = new Vector2(20, 0); // Position from left edge
            
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.sprite = icon;
            iconImage.color = textColor;
            
            // Adjust text position to make room for icon
            Transform textTransform = buttonObj.transform.Find("Text");
            if (textTransform != null)
            {
                RectTransform textRect = textTransform.GetComponent<RectTransform>();
                textRect.offsetMin = new Vector2(size.y * 0.8f, 0); // Move text to the right
            }
        }
        
        return buttonObj;
    }
    
    private GameObject CreateText(string name, string text, Transform parent, Vector2 position, int textSize = 0, TextAnchor alignment = TextAnchor.MiddleLeft)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300, 40);
        rectTransform.anchoredPosition = position;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = customFont != null ? customFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = textSize > 0 ? textSize : fontSize;
        textComponent.color = textColor;
        textComponent.alignment = alignment;
        
        return textObj;
    }
    
    private GameObject CreateHUDElement(string name, string text, Transform parent, Vector2 position, Sprite icon = null, int textSize = 0)
    {
        GameObject hudElement = new GameObject(name + "Container");
        hudElement.transform.SetParent(parent, false);
        
        RectTransform containerRect = hudElement.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(320, 25);
        containerRect.anchoredPosition = position;
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        
        // Add icon if provided
        if (icon != null)
        {
            GameObject iconObj = new GameObject(name + "Icon");
            iconObj.transform.SetParent(hudElement.transform, false);
            
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(20, 20);
            iconRect.anchorMin = new Vector2(0, 0.5f);
            iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.anchoredPosition = new Vector2(10, 0);
            
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.sprite = icon;
            iconImage.color = textColor;
        }
        
        // Create text
        GameObject textObj = CreateText(name, text, hudElement.transform, 
            icon != null ? new Vector2(40, 0) : new Vector2(0, 0), 
            textSize, TextAnchor.MiddleLeft);
        
        // Adjust text anchoring for HUD
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0.5f);
        textRect.anchorMax = new Vector2(1, 0.5f);
        textRect.pivot = new Vector2(0, 0.5f);
        
        return hudElement;
    }
    
    private GameObject CreateSlider(string name, string labelText, Transform parent, Vector2 position)
    {
        // Create container with better sizing
        GameObject sliderContainer = new GameObject(name + "Container");
        sliderContainer.transform.SetParent(parent, false);
        
        RectTransform containerRect = sliderContainer.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(500, 50);
        containerRect.anchoredPosition = position;
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        
        // Create label
        CreateText(name + "Label", labelText, sliderContainer.transform, new Vector2(-150, 0), 16, TextAnchor.MiddleCenter);
        
        // Create slider with better positioning
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(sliderContainer.transform, false);
        
        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(250, 30);
        sliderRect.anchoredPosition = new Vector2(100, 0);
        sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
        sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0.8f; // Default to 80%
        
        // Create background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderObj.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Create fill area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.sizeDelta = new Vector2(-20, 0);
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(10, 0);
        fillAreaRect.offsetMax = new Vector2(-10, 0);
        
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = primaryButtonColor;
        
        // Create handle area
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.sizeDelta = new Vector2(-20, 0);
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(10, 0);
        handleAreaRect.offsetMax = new Vector2(-10, 0);
        
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(25, 25);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        
        // Configure slider
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        
        return sliderContainer;
    }
    
    private void CreateFuelBar(Transform parent, Vector2 position)
    {
        // Create fuel bar container with better layout
        GameObject fuelBarContainer = new GameObject("FuelBarContainer");
        fuelBarContainer.transform.SetParent(parent, false);
        
        RectTransform containerRect = fuelBarContainer.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(300, 20);
        containerRect.anchoredPosition = position;
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        
        // Create fuel label with icon
        if (fuelIcon != null)
        {
            GameObject fuelIconObj = new GameObject("FuelIcon");
            fuelIconObj.transform.SetParent(fuelBarContainer.transform, false);
            
            RectTransform iconRect = fuelIconObj.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(18, 18);
            iconRect.anchorMin = new Vector2(0, 0.5f);
            iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.anchoredPosition = new Vector2(5, 0);
            
            Image iconImage = fuelIconObj.AddComponent<Image>();
            iconImage.sprite = fuelIcon;
            iconImage.color = textColor;
            
            CreateText("FuelLabel", "Fuel:", fuelBarContainer.transform, new Vector2(-90, 0), 14, TextAnchor.MiddleRight);
        }
        else
        {
            CreateText("FuelLabel", "Fuel:", fuelBarContainer.transform, new Vector2(-110, 0), 14, TextAnchor.MiddleRight);
        }
        
        // Create fuel bar background
        GameObject fuelBarBG = new GameObject("FuelBarBackground");
        fuelBarBG.transform.SetParent(fuelBarContainer.transform, false);
        
        RectTransform bgRect = fuelBarBG.AddComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(150, 12);
        bgRect.anchorMin = new Vector2(0.5f, 0.5f);
        bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        bgRect.anchoredPosition = new Vector2(50, 0);
        
        Image bgImage = fuelBarBG.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Create fuel fill
        GameObject fuelFill = new GameObject("FuelFill");
        fuelFill.transform.SetParent(fuelBarBG.transform, false);
        
        RectTransform fillRect = fuelFill.AddComponent<RectTransform>();
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        Image fillImage = fuelFill.AddComponent<Image>();
        fillImage.color = Color.green;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
    }
    
    #endregion
    
    #region Asset Loading Methods
    
    [ContextMenu("Load Assets from Resources")]
    public void LoadAssetsFromResources()
    {
        Debug.Log("UIQuickFix: Loading assets from Resources folder...");
        
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
        
        Debug.Log("UIQuickFix: Asset loading completed!");
    }
    
    [ContextMenu("Load Assets from Basic GUI Bundle")]
    public void LoadAssetsFromBasicGUIBundle()
    {
        Debug.Log("UIQuickFix: Loading assets from Basic_GUI_Bundle...");
        
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
        
        Debug.Log("UIQuickFix: Basic GUI Bundle asset loading completed!");
    }
    
    #endregion
    
    #region Utility Methods
    
    [ContextMenu("Remove All Quick Fix UI")]
    public void RemoveAllQuickFixUI()
    {
        string[] uiElements = {
            "MainMenuPanel", "PlayButton", "SettingsButton", "ExitButton", "GameTitle",
            "SettingsPanel", "MasterVolumeSlider", "MusicVolumeSlider", "SFXVolumeSlider", "SettingsCloseButton", "SettingsTitle",
            "GameHUDPanel", "SpeedText", "ScoreText", "HighScoreText", "FuelFill", "PauseButton",
            "PauseMenuPanel", "ResumeButton", "RestartButton", "MainMenuFromPauseButton", "PauseTitle",
            "GameOverPanel", "FinalScoreText", "PlayAgainButton", "MainMenuFromGameOverButton", "GameOverTitle",
            "MainPanel", "InfoText", "BasicButton"
        };
        
        int removedCount = 0;
        foreach (string elementName in uiElements)
        {
            GameObject element = GameObject.Find(elementName);
            if (element != null)
            {
                DestroyImmediate(element);
                removedCount++;
            }
        }
        
        hasCreatedUI = false;
        Debug.Log($"UIQuickFix: Removed {removedCount} UI elements");
    }
    
    [ContextMenu("Reset and Recreate UI")]
    public void ResetAndRecreateUI()
    {
        RemoveAllQuickFixUI();
        hasCreatedUI = false;
        CreateMissingUIElements();
    }
    
    [ContextMenu("Preview Asset Status")]
    public void PreviewAssetStatus()
    {
        Debug.Log("=== UIQuickFix Asset Status ===");
        Debug.Log($"Button Sprite: {(buttonSprite != null ? buttonSprite.name : "Not Assigned")}");
        Debug.Log($"Button Hover Sprite: {(buttonHoverSprite != null ? buttonHoverSprite.name : "Not Assigned")}");
        Debug.Log($"Button Pressed Sprite: {(buttonPressedSprite != null ? buttonPressedSprite.name : "Not Assigned")}");
        Debug.Log($"Panel Sprite: {(panelSprite != null ? panelSprite.name : "Not Assigned")}");
        Debug.Log($"Custom Font: {(customFont != null ? customFont.name : "Not Assigned")}");
        
        Debug.Log("--- Button Icons ---");
        Debug.Log($"Play Icon: {(playButtonIcon != null ? playButtonIcon.name : "Not Assigned")}");
        Debug.Log($"Pause Icon: {(pauseButtonIcon != null ? pauseButtonIcon.name : "Not Assigned")}");
        Debug.Log($"Settings Icon: {(settingsButtonIcon != null ? settingsButtonIcon.name : "Not Assigned")}");
        Debug.Log($"Exit Icon: {(exitButtonIcon != null ? exitButtonIcon.name : "Not Assigned")}");
        
        Debug.Log("--- HUD Icons ---");
        Debug.Log($"Speed Icon: {(speedIcon != null ? speedIcon.name : "Not Assigned")}");
        Debug.Log($"Score Icon: {(scoreIcon != null ? scoreIcon.name : "Not Assigned")}");
        Debug.Log($"Fuel Icon: {(fuelIcon != null ? fuelIcon.name : "Not Assigned")}");
        Debug.Log("========================");
    }
    
    #endregion
}