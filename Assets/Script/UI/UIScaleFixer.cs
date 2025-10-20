using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIScaleFixer - Tool để tự động fix UI scaling issues
/// Attach vào Canvas để tự động điều chỉnh scale phù hợp
/// </summary>
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class UIScaleFixer : MonoBehaviour
{
    [Header("Auto Fix Settings")]
    [SerializeField] private bool autoFixOnStart = true;
    [SerializeField] private bool debugMode = true;
    
    [Header("Canvas Settings")]
    [SerializeField] private Vector2 referenceResolution = new Vector2(1280, 720);
    [SerializeField] private float matchWidthOrHeight = 0f; // 0 = Width priority, 1 = Height priority
    
    private Canvas canvas;
    private CanvasScaler canvasScaler;
    
    private void Start()
    {
        InitializeComponents();
        
        if (autoFixOnStart)
        {
            FixCanvasScaling();
        }
    }
    
    private void InitializeComponents()
    {
        canvas = GetComponent<Canvas>();
        canvasScaler = GetComponent<CanvasScaler>();
        
        if (canvas == null || canvasScaler == null)
        {
            Debug.LogError("UIScaleFixer: Missing Canvas or CanvasScaler component!", this);
        }
    }
    
    [ContextMenu("Fix Canvas Scaling")]
    public void FixCanvasScaling()
    {
        if (canvas == null || canvasScaler == null)
        {
            InitializeComponents();
            return;
        }
        
        // Log current settings
        if (debugMode)
        {
            Debug.Log($"Current Canvas settings before fix:", this);
            Debug.Log($"  Render Mode: {canvas.renderMode}");
            Debug.Log($"  UI Scale Mode: {canvasScaler.uiScaleMode}");
            Debug.Log($"  Reference Resolution: {canvasScaler.referenceResolution}");
            Debug.Log($"  Screen Match Mode: {canvasScaler.screenMatchMode}");
            Debug.Log($"  Match Width Or Height: {canvasScaler.matchWidthOrHeight}");
        }
        
        // Apply optimal settings
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = referenceResolution;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
        
        // Log new settings
        if (debugMode)
        {
            Debug.Log($"Canvas settings after fix:", this);
            Debug.Log($"  Reference Resolution: {canvasScaler.referenceResolution}");
            Debug.Log($"  Match: {canvasScaler.matchWidthOrHeight}");
            Debug.Log($"  Current Screen Resolution: {Screen.width}x{Screen.height}");
            Debug.Log($"  Canvas Scale Factor: {canvas.scaleFactor}");
        }
    }
    
    [ContextMenu("Auto Detect Best Settings")]
    public void AutoDetectBestSettings()
    {
        float screenAspect = (float)Screen.width / Screen.height;
        
        // Determine best reference resolution based on screen aspect
        if (screenAspect > 1.7f) // Widescreen
        {
            referenceResolution = new Vector2(1920, 1080);
            matchWidthOrHeight = 0.5f;
        }
        else if (screenAspect > 1.5f) // Standard 16:10
        {
            referenceResolution = new Vector2(1440, 900);
            matchWidthOrHeight = 0.3f;
        }
        else // More square aspect ratios
        {
            referenceResolution = new Vector2(1280, 720);
            matchWidthOrHeight = 0f;
        }
        
        if (debugMode)
        {
            Debug.Log($"Auto-detected settings for aspect ratio {screenAspect:F2}:");
            Debug.Log($"  Reference Resolution: {referenceResolution}");
            Debug.Log($"  Match: {matchWidthOrHeight}");
        }
        
        FixCanvasScaling();
    }
    
    [ContextMenu("Fix All UI Elements Size")]
    public void FixAllUIElementsSize()
    {
        // Fix MainMenuPanel
        var mainMenuPanel = transform.Find("MainMenuPanel");
        if (mainMenuPanel != null)
        {
            FixPanelSize(mainMenuPanel.GetComponent<RectTransform>(), "MainMenuPanel");
        }
        
        // Fix GameHUDPanel
        var gameHUDPanel = transform.Find("GameHUDPanel");
        if (gameHUDPanel != null)
        {
            FixPanelSize(gameHUDPanel.GetComponent<RectTransform>(), "GameHUDPanel");
        }
        
        // Fix text sizes
        FixTextSizes();
        
        // Fix button sizes
        FixButtonSizes();
    }
    
    private void FixPanelSize(RectTransform rectTransform, string panelName)
    {
        if (rectTransform == null) return;
        
        // Set to stretch fill
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        
        if (debugMode)
        {
            Debug.Log($"Fixed {panelName} size to fill canvas");
        }
    }
    
    private void FixTextSizes()
    {
        var allTexts = GetComponentsInChildren<Text>(true);
        
        foreach (var text in allTexts)
        {
            if (text.gameObject.name.Contains("Title"))
            {
                text.fontSize = Mathf.Max(text.fontSize, 48);
            }
            else if (text.gameObject.name.Contains("Button"))
            {
                text.fontSize = Mathf.Max(text.fontSize, 24);
            }
            else
            {
                text.fontSize = Mathf.Max(text.fontSize, 18);
            }
        }
        
        if (debugMode)
        {
            Debug.Log($"Fixed {allTexts.Length} text elements font sizes");
        }
    }
    
    private void FixButtonSizes()
    {
        var allButtons = GetComponentsInChildren<Button>(true);
        
        foreach (var button in allButtons)
        {
            var layoutElement = button.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = button.gameObject.AddComponent<LayoutElement>();
            }
            
            layoutElement.preferredHeight = 60f;
            layoutElement.minHeight = 40f;
        }
        
        if (debugMode)
        {
            Debug.Log($"Fixed {allButtons.Length} button sizes");
        }
    }
    
    [ContextMenu("Debug Canvas Info")]
    public void DebugCanvasInfo()
    {
        if (canvas == null || canvasScaler == null) return;
        
        Debug.Log("=== CANVAS DEBUG INFO ===", this);
        Debug.Log($"Screen Resolution: {Screen.width} x {Screen.height}");
        Debug.Log($"Screen DPI: {Screen.dpi}");
        Debug.Log($"Canvas Scale Factor: {canvas.scaleFactor}");
        Debug.Log($"Canvas Pixel Rect: {canvas.pixelRect}");
        Debug.Log($"Reference Resolution: {canvasScaler.referenceResolution}");
        Debug.Log($"Current Aspect Ratio: {(float)Screen.width / Screen.height:F2}");
        Debug.Log($"Reference Aspect Ratio: {canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y:F2}");
        Debug.Log("========================");
    }
}