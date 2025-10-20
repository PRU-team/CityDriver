using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper script để đảm bảo UI elements có tên đúng cho UIManager auto-detection
/// Attach script này vào UI elements và nó sẽ tự động rename chúng
/// </summary>
public class UIElementNamer : MonoBehaviour
{
    [System.Serializable]
    public enum UIElementType
    {
        // Main Menu Elements
        MainMenuPanel,
        PlayButton,
        SettingsButton,
        ExitButton,
        
        // Game HUD Elements
        GameHUDPanel,
        SpeedText,
        ScoreText,
        HighScoreText,
        FuelFill,
        PauseButton,
        
        // Pause Menu Elements
        PauseMenuPanel,
        ResumeButton,
        RestartButton,
        MainMenuFromPauseButton,
        
        // Settings Panel Elements
        SettingsPanel,
        MasterVolumeSlider,
        MusicVolumeSlider,
        SFXVolumeSlider,
        SettingsCloseButton,
        
        // Game Over Panel Elements
        GameOverPanel,
        FinalScoreText,
        PlayAgainButton,
        MainMenuFromGameOverButton,
        
        // Other Elements
        Canvas,
        EventSystem
    }
    
    [Header("UI Element Configuration")]
    [SerializeField] private UIElementType elementType;
    [SerializeField] private bool autoRenameOnStart = true;
    [SerializeField] private bool showDebugLog = true;
    
    private void Start()
    {
        if (autoRenameOnStart)
        {
            RenameElement();
        }
    }
    
    [ContextMenu("Rename Element")]
    public void RenameElement()
    {
        string newName = GetCorrectNameForType(elementType);
        
        if (gameObject.name != newName)
        {
            string oldName = gameObject.name;
            gameObject.name = newName;
            
            if (showDebugLog)
            {
                Debug.Log($"UIElementNamer: Renamed '{oldName}' to '{newName}'", this);
            }
        }
        else if (showDebugLog)
        {
            Debug.Log($"UIElementNamer: '{gameObject.name}' already has correct name", this);
        }
    }
    
    private string GetCorrectNameForType(UIElementType type)
    {
        switch (type)
        {
            // Main Menu Elements
            case UIElementType.MainMenuPanel: return "MainMenuPanel";
            case UIElementType.PlayButton: return "PlayButton";
            case UIElementType.SettingsButton: return "SettingsButton";
            case UIElementType.ExitButton: return "ExitButton";
            
            // Game HUD Elements
            case UIElementType.GameHUDPanel: return "GameHUDPanel";
            case UIElementType.SpeedText: return "SpeedText";
            case UIElementType.ScoreText: return "ScoreText";
            case UIElementType.HighScoreText: return "HighScoreText";
            case UIElementType.FuelFill: return "FuelFill";
            case UIElementType.PauseButton: return "PauseButton";
            
            // Pause Menu Elements
            case UIElementType.PauseMenuPanel: return "PauseMenuPanel";
            case UIElementType.ResumeButton: return "ResumeButton";
            case UIElementType.RestartButton: return "RestartButton";
            case UIElementType.MainMenuFromPauseButton: return "MainMenuFromPauseButton";
            
            // Settings Panel Elements
            case UIElementType.SettingsPanel: return "SettingsPanel";
            case UIElementType.MasterVolumeSlider: return "MasterVolumeSlider";
            case UIElementType.MusicVolumeSlider: return "MusicVolumeSlider";
            case UIElementType.SFXVolumeSlider: return "SFXVolumeSlider";
            case UIElementType.SettingsCloseButton: return "SettingsCloseButton";
            
            // Game Over Panel Elements
            case UIElementType.GameOverPanel: return "GameOverPanel";
            case UIElementType.FinalScoreText: return "FinalScoreText";
            case UIElementType.PlayAgainButton: return "PlayAgainButton";
            case UIElementType.MainMenuFromGameOverButton: return "MainMenuFromGameOverButton";
            
            // Other Elements
            case UIElementType.Canvas: return "Canvas";
            case UIElementType.EventSystem: return "EventSystem";
            
            default: return gameObject.name;
        }
    }
    
    // Validation methods
    private void OnValidate()
    {
        // Kiểm tra xem tên hiện tại có đúng không
        string correctName = GetCorrectNameForType(elementType);
        if (gameObject.name != correctName && showDebugLog)
        {
            Debug.LogWarning($"UIElementNamer: GameObject '{gameObject.name}' should be named '{correctName}' for auto-detection", this);
        }
    }
    
    // Utility method để rename tất cả UI elements trong scene
    [ContextMenu("Rename All UI Elements In Scene")]
    public void RenameAllUIElementsInScene()
    {
        UIElementNamer[] allNamers = FindObjectsOfType<UIElementNamer>();
        int renamedCount = 0;
        
        foreach (UIElementNamer namer in allNamers)
        {
            string oldName = namer.gameObject.name;
            namer.RenameElement();
            if (namer.gameObject.name != oldName)
            {
                renamedCount++;
            }
        }
        
        Debug.Log($"UIElementNamer: Renamed {renamedCount} UI elements in scene");
    }
}