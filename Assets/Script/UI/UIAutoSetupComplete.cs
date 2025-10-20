using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// UIAutoSetup - Tự động tạo toàn bộ UI system cho CityDriver game
/// Chạy script này để tự động setup Main Menu, Game HUD, Pause Menu, Settings
/// </summary>
public class UIAutoSetup : MonoBehaviour
{
    [Header("Auto Setup Configuration")]
    [SerializeField] private bool setupMainMenu = true;
    [SerializeField] private bool setupGameHUD = true;
    [SerializeField] private bool setupPauseMenu = true;
    [SerializeField] private bool setupSettings = true;
    [SerializeField] private bool setupGameOver = true;
    
    [Header("UI Assets (Optional - từ Basic_GUI_Bundle)")]
    [SerializeField] private Sprite buttonBackground;
    [SerializeField] private Sprite panelBackground;
    [SerializeField] private Sprite sliderBackground;
    [SerializeField] private Sprite sliderFill;
    [SerializeField] private Sprite sliderHandle;
    
    [Header("Debug")]
    [SerializeField] private bool verbose = true;
    
    private Canvas mainCanvas;
    private EventSystem eventSystem;
    private Dictionary<string, GameObject> createdPanels = new Dictionary<string, GameObject>();
    
    [ContextMenu("Auto Setup All UI")]
    public void AutoSetupAllUI()
    {
        Log("🚀 Bắt đầu auto setup UI system...");
        
        SetupCanvas();
        SetupEventSystem();
        
        if (setupMainMenu) CreateMainMenuPanel();
        if (setupGameHUD) CreateGameHUDPanel();
        if (setupPauseMenu) CreatePauseMenuPanel();
        if (setupSettings) CreateSettingsPanel();
        if (setupGameOver) CreateGameOverPanel();
        
        SetupUIManager();
        SetupCanvasScaler();
        
        Log("✅ Hoàn thành auto setup UI system!");
        Log("📝 Hãy assign UI assets từ Basic_GUI_Bundle vào các Image components");
        Log("🔧 Attach các controller scripts vào các GameObjects tương ứng");
    }
    
    public void SetupCanvas()
    {
        // Tìm hoặc tạo Canvas
        mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null)
        {
            GameObject canvasGO = new GameObject("MainCanvas");
            mainCanvas = canvasGO.AddComponent<Canvas>();
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            
            Log("📱 Đã tạo Main Canvas");
        }
        
        // Setup Canvas properties
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.pixelPerfect = true;
        mainCanvas.sortingOrder = 0;
        
        SetupCanvasScaler();
        SetupEventSystem();
        
        Log("⚙️ Đã cấu hình Canvas settings");
    }
    
    private void SetupCanvasScaler()
    {
        var scaler = mainCanvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0f;
            
            Log("📏 Đã tối ưu Canvas Scaler (1280x720, Match Width)");
        }
    }
    
    private void SetupEventSystem()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystem = eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            
            Log("🎮 Đã tạo Event System");
        }
    }
    
    public void SetupMainMenu()
    {
        if (mainCanvas == null) SetupCanvas();
        CreateMainMenuPanel();
    }
    
    private void CreateMainMenuPanel()
    {
        var panel = CreatePanel("MainMenuPanel", true);
        
        // Background
        SetPanelBackground(panel, new Color(0.1f, 0.1f, 0.3f, 0.9f));
        
        // Game Title
        var title = CreateText(panel, "GameTitle", "CITY DRIVER");
        SetupGameTitle(title);
        
        // Buttons Container
        var buttonContainer = CreateVerticalContainer(panel, "ButtonsContainer");
        PositionElement(buttonContainer, AnchorPresets.MiddleCenter, Vector2.zero, new Vector2(400, 300));
        
        // Buttons
        CreateMenuButton(buttonContainer, "StartButton", "START GAME");
        CreateMenuButton(buttonContainer, "SettingsButton", "SETTINGS");
        CreateMenuButton(buttonContainer, "QuitButton", "QUIT");
        
        // Version text
        var version = CreateText(panel, "VersionText", "Version 1.0");
        PositionElement(version, AnchorPresets.BottomRight, new Vector2(-50, 30), new Vector2(200, 30));
        version.GetComponent<Text>().fontSize = 18;
        version.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
        
        Log("🏠 Đã tạo Main Menu Panel với 3 buttons");
    }
    
    public void SetupGameHUD()
    {
        if (mainCanvas == null) SetupCanvas();
        CreateGameHUDPanel();
    }
    
    private void CreateGameHUDPanel()
    {
        var panel = CreatePanel("GameHUDPanel", false);
        SetPanelBackground(panel, Color.clear); // Transparent
        
        // Top Panel
        var topPanel = CreateHorizontalContainer(panel, "TopPanel");
        PositionElement(topPanel, AnchorPresets.TopStretch, Vector2.zero, new Vector2(0, 80));
        var topLayout = topPanel.GetComponent<HorizontalLayoutGroup>();
        topLayout.padding = new RectOffset(50, 50, 20, 20);
        topLayout.spacing = 20;
        
        // Score Text
        var scoreText = CreateText(topPanel, "ScoreText", "Score: 0");
        scoreText.GetComponent<Text>().fontSize = 36;
        scoreText.GetComponent<Text>().fontStyle = FontStyle.Bold;
        AddLayoutElement(scoreText, -1, -1, 1);
        
        // Pause Button
        var pauseBtn = CreateButton(topPanel, "PauseButton", "||");
        pauseBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 60);
        
        // Speedometer Panel
        var speedPanel = CreateSpeedometer(panel);
        
        // Fuel Panel
        var fuelPanel = CreateFuelPanel(panel);
        
        Log("🏁 Đã tạo Game HUD với speedometer và fuel panel");
    }
    
    public void SetupPauseMenu()
    {
        if (mainCanvas == null) SetupCanvas();
        CreatePauseMenuPanel();
    }
    
    private void CreatePauseMenuPanel()
    {
        var panel = CreatePanel("PauseMenuPanel", false);
        SetPanelBackground(panel, new Color(0, 0, 0, 0.8f)); // Semi-transparent black
        
        // Pause Dialog
        var dialog = CreateSubPanel(panel, "PauseDialog");
        PositionElement(dialog, AnchorPresets.MiddleCenter, Vector2.zero, new Vector2(400, 500));
        SetPanelBackground(dialog, new Color(0.2f, 0.2f, 0.2f, 0.95f));
        
        // Title
        var title = CreateText(dialog, "PauseTitle", "PAUSED");
        PositionElement(title, AnchorPresets.TopCenter, new Vector2(0, -50), new Vector2(300, 60));
        SetupTitle(title, 48);
        
        // Buttons Container
        var buttonContainer = CreateVerticalContainer(dialog, "PauseButtonsContainer");
        PositionElement(buttonContainer, AnchorPresets.MiddleCenter, Vector2.zero, new Vector2(300, 250));
        
        CreateMenuButton(buttonContainer, "ResumeButton", "RESUME");
        CreateMenuButton(buttonContainer, "PauseSettingsButton", "SETTINGS");
        CreateMenuButton(buttonContainer, "MainMenuButton", "MAIN MENU");
        
        Log("⏸️ Đã tạo Pause Menu với 3 options");
    }
    
    public void SetupSettingsMenu()
    {
        if (mainCanvas == null) SetupCanvas();
        CreateSettingsPanel();
    }
    
    private void CreateSettingsPanel()
    {
        var panel = CreatePanel("SettingsPanel", false);
        SetPanelBackground(panel, new Color(0.15f, 0.15f, 0.15f, 0.95f));
        
        // Settings Dialog
        var dialog = CreateSubPanel(panel, "SettingsDialog");
        PositionElement(dialog, AnchorPresets.MiddleCenter, Vector2.zero, new Vector2(600, 500));
        
        // Title
        var title = CreateText(dialog, "SettingsTitle", "SETTINGS");
        PositionElement(title, AnchorPresets.TopCenter, new Vector2(0, -40), new Vector2(400, 50));
        SetupTitle(title, 42);
        
        // Audio Panel
        var audioPanel = CreateVerticalContainer(dialog, "AudioPanel");
        PositionElement(audioPanel, AnchorPresets.MiddleCenter, new Vector2(0, 50), new Vector2(500, 300));
        
        CreateVolumeSlider(audioPanel, "MasterVolumeSlider", "Master Volume");
        CreateVolumeSlider(audioPanel, "MusicVolumeSlider", "Music Volume");
        CreateVolumeSlider(audioPanel, "SFXVolumeSlider", "SFX Volume");
        
        // Back Button
        var backBtn = CreateButton(dialog, "SettingsBackButton", "BACK");
        PositionElement(backBtn, AnchorPresets.BottomCenter, new Vector2(0, 30), new Vector2(150, 50));
        
        Log("⚙️ Đã tạo Settings Panel với audio controls");
    }
    
    public void SetupGameOverMenu()
    {
        if (mainCanvas == null) SetupCanvas();
        CreateGameOverPanel();
    }
    
    private void CreateGameOverPanel()
    {
        var panel = CreatePanel("GameOverPanel", false);
        SetPanelBackground(panel, new Color(0.1f, 0.1f, 0.1f, 0.9f));
        
        // Game Over Dialog
        var dialog = CreateSubPanel(panel, "GameOverDialog");
        PositionElement(dialog, AnchorPresets.MiddleCenter, Vector2.zero, new Vector2(500, 400));
        SetPanelBackground(dialog, new Color(0.3f, 0.1f, 0.1f, 0.95f));
        
        // Title
        var title = CreateText(dialog, "GameOverTitle", "GAME OVER");
        PositionElement(title, AnchorPresets.TopCenter, new Vector2(0, -50), new Vector2(400, 60));
        SetupTitle(title, 48);
        title.GetComponent<Text>().color = Color.red;
        
        // Final Score
        var scoreText = CreateText(dialog, "FinalScoreText", "Final Score: 0");
        PositionElement(scoreText, AnchorPresets.MiddleCenter, new Vector2(0, 20), new Vector2(350, 40));
        scoreText.GetComponent<Text>().fontSize = 32;
        
        // Buttons
        var buttonContainer = CreateHorizontalContainer(dialog, "GameOverButtonsContainer");
        PositionElement(buttonContainer, AnchorPresets.BottomCenter, new Vector2(0, 50), new Vector2(400, 60));
        
        CreateMenuButton(buttonContainer, "RestartButton", "RESTART");
        CreateMenuButton(buttonContainer, "GameOverMainMenuButton", "MAIN MENU");
        
        Log("💀 Đã tạo Game Over Panel");
    }
    
    private GameObject CreateSpeedometer(GameObject parent)
    {
        var speedPanel = CreateSubPanel(parent, "SpeedometerPanel");
        PositionElement(speedPanel, AnchorPresets.BottomLeft, new Vector2(150, 150), new Vector2(200, 200));
        SetPanelBackground(speedPanel, new Color(0, 0, 0, 0.7f));
        
        // Speed Background Circle
        var bgCircle = CreateImage(speedPanel, "SpeedBackground");
        PositionElement(bgCircle, AnchorPresets.MiddleCenter, Vector2.zero, new Vector2(180, 180));
        bgCircle.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Speed Needle
        var needle = CreateImage(speedPanel, "SpeedNeedle");
        PositionElement(needle, AnchorPresets.MiddleCenter, new Vector2(0, 20), new Vector2(4, 80));
        needle.GetComponent<Image>().color = Color.red;
        
        // Speed Text
        var speedText = CreateText(speedPanel, "SpeedText", "0 km/h");
        PositionElement(speedText, AnchorPresets.BottomCenter, new Vector2(0, 20), new Vector2(150, 30));
        speedText.GetComponent<Text>().fontSize = 24;
        speedText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        
        return speedPanel;
    }
    
    private GameObject CreateFuelPanel(GameObject parent)
    {
        var fuelPanel = CreateSubPanel(parent, "FuelPanel");
        PositionElement(fuelPanel, AnchorPresets.BottomRight, new Vector2(-150, 100), new Vector2(250, 80));
        SetPanelBackground(fuelPanel, new Color(0, 0, 0, 0.7f));
        
        // Fuel Label
        var label = CreateText(fuelPanel, "FuelLabel", "FUEL");
        PositionElement(label, AnchorPresets.TopLeft, new Vector2(10, -10), new Vector2(60, 25));
        label.GetComponent<Text>().fontSize = 18;
        
        // Fuel Slider
        var slider = CreateSlider(fuelPanel, "FuelSlider");
        PositionElement(slider, AnchorPresets.BottomStretch, Vector2.zero, new Vector2(-20, 30));
        slider.GetComponent<Slider>().value = 1f;
        
        // Fuel Warning (hidden by default)
        var warning = CreateText(fuelPanel, "FuelWarning", "LOW FUEL!");
        PositionElement(warning, AnchorPresets.MiddleCenter, Vector2.zero, new Vector2(200, 30));
        warning.GetComponent<Text>().color = Color.red;
        warning.GetComponent<Text>().fontSize = 20;
        warning.GetComponent<Text>().fontStyle = FontStyle.Bold;
        warning.SetActive(false);
        
        return fuelPanel;
    }
    
    private void SetupUIManager()
    {
        // Tìm hoặc tạo UIManager GameObject
        var uiManagerGO = GameObject.Find("UIManager");
        if (uiManagerGO == null)
        {
            uiManagerGO = new GameObject("UIManager");
            DontDestroyOnLoad(uiManagerGO);
        }
        
        // Note: UIManager script cần được attach manually
        Log("🎮 UIManager GameObject ready - hãy attach UIManager.cs script");
        Log("📋 Assign các panels đã tạo vào UIManager inspector:");
        
        foreach (var panel in createdPanels)
        {
            Log($"   - {panel.Key}: {panel.Value.name}");
        }
    }
    
    #region Helper Methods
    
    private GameObject CreatePanel(string name, bool active)
    {
        var panelGO = new GameObject(name);
        panelGO.transform.SetParent(mainCanvas.transform, false);
        
        var rectTransform = panelGO.AddComponent<RectTransform>();
        StretchToFill(rectTransform);
        
        var image = panelGO.AddComponent<Image>();
        image.raycastTarget = true;
        
        panelGO.SetActive(active);
        createdPanels[name] = panelGO;
        
        return panelGO;
    }
    
    private GameObject CreateSubPanel(GameObject parent, string name)
    {
        var panelGO = new GameObject(name);
        panelGO.transform.SetParent(parent.transform, false);
        
        var rectTransform = panelGO.AddComponent<RectTransform>();
        var image = panelGO.AddComponent<Image>();
        image.raycastTarget = true;
        
        return panelGO;
    }
    
    private GameObject CreateButton(GameObject parent, string name, string text)
    {
        var buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent.transform, false);
        
        var image = buttonGO.AddComponent<Image>();
        if (buttonBackground != null) image.sprite = buttonBackground;
        image.type = Image.Type.Sliced;
        image.color = Color.white;
        
        var button = buttonGO.AddComponent<Button>();
        
        // Text child
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        var textComponent = textGO.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 24;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = Color.black;
        
        var textRect = textGO.GetComponent<RectTransform>();
        StretchToFill(textRect);
        
        return buttonGO;
    }
    
    private GameObject CreateMenuButton(GameObject parent, string name, string text)
    {
        var button = CreateButton(parent, name, text);
        AddLayoutElement(button, -1, 60, -1);
        return button;
    }
    
    private GameObject CreateText(GameObject parent, string name, string text)
    {
        var textGO = new GameObject(name);
        textGO.transform.SetParent(parent.transform, false);
        
        var textComponent = textGO.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 24;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = Color.white;
        
        return textGO;
    }
    
    private GameObject CreateImage(GameObject parent, string name)
    {
        var imageGO = new GameObject(name);
        imageGO.transform.SetParent(parent.transform, false);
        
        var image = imageGO.AddComponent<Image>();
        image.color = Color.white;
        
        return imageGO;
    }
    
    private GameObject CreateSlider(GameObject parent, string name)
    {
        var sliderGO = new GameObject(name);
        sliderGO.transform.SetParent(parent.transform, false);
        
        var slider = sliderGO.AddComponent<Slider>();
        
        // Background
        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(sliderGO.transform, false);
        var bgImage = bgGO.AddComponent<Image>();
        if (sliderBackground != null) bgImage.sprite = sliderBackground;
        bgImage.type = Image.Type.Sliced;
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        StretchToFill(bgGO.GetComponent<RectTransform>());
        
        // Fill Area
        var fillAreaGO = new GameObject("Fill Area");
        fillAreaGO.transform.SetParent(sliderGO.transform, false);
        StretchToFill(fillAreaGO.AddComponent<RectTransform>());
        
        // Fill
        var fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(fillAreaGO.transform, false);
        var fillImage = fillGO.AddComponent<Image>();
        if (sliderFill != null) fillImage.sprite = sliderFill;
        fillImage.type = Image.Type.Sliced;
        fillImage.color = Color.green;
        StretchToFill(fillGO.GetComponent<RectTransform>());
        
        slider.fillRect = fillGO.GetComponent<RectTransform>();
        slider.value = 0.7f;
        
        return sliderGO;
    }
    
    private GameObject CreateVerticalContainer(GameObject parent, string name)
    {
        var containerGO = new GameObject(name);
        containerGO.transform.SetParent(parent.transform, false);
        
        var layout = containerGO.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 15;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        
        return containerGO;
    }
    
    private GameObject CreateHorizontalContainer(GameObject parent, string name)
    {
        var containerGO = new GameObject(name);
        containerGO.transform.SetParent(parent.transform, false);
        
        var layout = containerGO.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 15;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;
        
        return containerGO;
    }
    
    private void CreateVolumeSlider(GameObject parent, string name, string label)
    {
        var container = new GameObject(name + "Container");
        container.transform.SetParent(parent.transform, false);
        AddLayoutElement(container, -1, 60, -1);
        
        // Label
        var labelGO = CreateText(container, label.Replace(" ", "") + "Label", label);
        PositionElement(labelGO, AnchorPresets.MiddleLeft, new Vector2(20, 0), new Vector2(150, 40));
        labelGO.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
        
        // Slider
        var slider = CreateSlider(container, name);
        PositionElement(slider, AnchorPresets.MiddleStretch, Vector2.zero, new Vector2(-200, 30));
        var sliderRect = slider.GetComponent<RectTransform>();
        sliderRect.offsetMin = new Vector2(170, -15);
        sliderRect.offsetMax = new Vector2(-30, 15);
        
        // Value Text
        var valueText = CreateText(container, name + "ValueText", "70%");
        PositionElement(valueText, AnchorPresets.MiddleRight, new Vector2(-15, 0), new Vector2(60, 40));
        valueText.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
    }
    
    private void SetupGameTitle(GameObject titleGO)
    {
        PositionElement(titleGO, AnchorPresets.TopCenter, new Vector2(0, -100), new Vector2(600, 100));
        var text = titleGO.GetComponent<Text>();
        text.fontSize = 64;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
    }
    
    private void SetupTitle(GameObject titleGO, int fontSize)
    {
        var text = titleGO.GetComponent<Text>();
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
    }
    
    private void SetPanelBackground(GameObject panel, Color color)
    {
        var image = panel.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
            if (panelBackground != null)
            {
                image.sprite = panelBackground;
                image.type = Image.Type.Sliced;
            }
        }
    }
    
    private void PositionElement(GameObject element, AnchorPresets preset, Vector2 position, Vector2 size)
    {
        var rect = element.GetComponent<RectTransform>();
        if (rect == null) rect = element.AddComponent<RectTransform>();
        
        SetAnchorPreset(rect, preset);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }
    
    private void StretchToFill(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
    
    private void AddLayoutElement(GameObject go, float minWidth, float preferredHeight, float flexibleWidth)
    {
        var layoutElement = go.AddComponent<LayoutElement>();
        if (minWidth >= 0) layoutElement.minWidth = minWidth;
        if (preferredHeight >= 0) layoutElement.preferredHeight = preferredHeight;
        if (flexibleWidth >= 0) layoutElement.flexibleWidth = flexibleWidth;
    }
    
    private void SetAnchorPreset(RectTransform rect, AnchorPresets preset)
    {
        switch (preset)
        {
            case AnchorPresets.TopLeft:
                rect.anchorMin = new Vector2(0, 1); rect.anchorMax = new Vector2(0, 1);
                break;
            case AnchorPresets.TopCenter:
                rect.anchorMin = new Vector2(0.5f, 1); rect.anchorMax = new Vector2(0.5f, 1);
                break;
            case AnchorPresets.TopRight:
                rect.anchorMin = new Vector2(1, 1); rect.anchorMax = new Vector2(1, 1);
                break;
            case AnchorPresets.MiddleLeft:
                rect.anchorMin = new Vector2(0, 0.5f); rect.anchorMax = new Vector2(0, 0.5f);
                break;
            case AnchorPresets.MiddleCenter:
                rect.anchorMin = new Vector2(0.5f, 0.5f); rect.anchorMax = new Vector2(0.5f, 0.5f);
                break;
            case AnchorPresets.MiddleRight:
                rect.anchorMin = new Vector2(1, 0.5f); rect.anchorMax = new Vector2(1, 0.5f);
                break;
            case AnchorPresets.BottomLeft:
                rect.anchorMin = new Vector2(0, 0); rect.anchorMax = new Vector2(0, 0);
                break;
            case AnchorPresets.BottomCenter:
                rect.anchorMin = new Vector2(0.5f, 0); rect.anchorMax = new Vector2(0.5f, 0);
                break;
            case AnchorPresets.BottomRight:
                rect.anchorMin = new Vector2(1, 0); rect.anchorMax = new Vector2(1, 0);
                break;
            case AnchorPresets.TopStretch:
                rect.anchorMin = new Vector2(0, 1); rect.anchorMax = new Vector2(1, 1);
                break;
            case AnchorPresets.MiddleStretch:
                rect.anchorMin = new Vector2(0, 0.5f); rect.anchorMax = new Vector2(1, 0.5f);
                break;
            case AnchorPresets.BottomStretch:
                rect.anchorMin = new Vector2(0, 0); rect.anchorMax = new Vector2(1, 0);
                break;
        }
    }
    
    private void Log(string message)
    {
        if (verbose)
        {
            Debug.Log($"[UIAutoSetup] {message}");
        }
    }
    
    private enum AnchorPresets
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        BottomLeft, BottomCenter, BottomRight,
        TopStretch, MiddleStretch, BottomStretch
    }
    
    #endregion
}