using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// UIInputHandler xử lý input cho UI interactions
/// Tích hợp với Input System và UIManager
/// </summary>
public class UIInputHandler : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;
    
    private InputAction pauseAction;
    private InputAction submitAction;
    private InputAction cancelAction;

    private void Awake()
    {
        // Get input actions
        if (inputActions != null)
        {
            var playerMap = inputActions.FindActionMap("Player");
            var uiMap = inputActions.FindActionMap("UI");
            
            // Try to find pause action (we'll need to add this to the input actions)
            pauseAction = playerMap?.FindAction("Pause");
            submitAction = uiMap?.FindAction("Submit");
            cancelAction = uiMap?.FindAction("Cancel");
        }
        
        // Fallback: use built-in keyboard input if input actions not configured
        if (pauseAction == null)
        {
            Debug.LogWarning("Pause action not found in Input Actions. Using keyboard fallback.");
        }
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.Enable();
            pauseAction.performed += OnPausePerformed;
        }

        if (submitAction != null)
        {
            submitAction.Enable();
            submitAction.performed += OnSubmitPerformed;
        }

        if (cancelAction != null)
        {
            cancelAction.Enable();
            cancelAction.performed += OnCancelPerformed;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPausePerformed;
            pauseAction.Disable();
        }

        if (submitAction != null)
        {
            submitAction.performed -= OnSubmitPerformed;
            submitAction.Disable();
        }

        if (cancelAction != null)
        {
            cancelAction.performed -= OnCancelPerformed;
            cancelAction.Disable();
        }
    }

    private void Update()
    {
        // Fallback keyboard input if Input Actions not properly configured
        if (pauseAction == null)
        {
            HandleKeyboardInput();
        }
    }

    private void HandleKeyboardInput()
    {
        // ESC key for pause/cancel using Input System
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandlePauseInput();
        }

        // Enter key for submit using Input System
        if (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            HandleSubmitInput();
        }
    }

    #region Input Action Callbacks
    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        HandlePauseInput();
    }

    private void OnSubmitPerformed(InputAction.CallbackContext context)
    {
        HandleSubmitInput();
    }

    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        HandleCancelInput();
    }
    #endregion

    #region Input Handlers
    private void HandlePauseInput()
    {
        if (UIManager.Instance == null) return;

        // Different behavior based on current UI state
        if (UIManager.Instance.IsInGame)
        {
            if (UIManager.Instance.IsPaused)
            {
                UIManager.Instance.ResumeGame();
            }
            else
            {
                UIManager.Instance.PauseGame();
            }
        }
        else
        {
            // In menu, ESC might go back or quit
            HandleMenuBackInput();
        }
    }

    private void HandleSubmitInput()
    {
        // Let Unity's EventSystem handle this for UI elements
        // This is mainly for fallback behavior
    }

    private void HandleCancelInput()
    {
        HandleMenuBackInput();
    }

    private void HandleMenuBackInput()
    {
        if (UIManager.Instance == null) return;

        // Check what panel is currently active and handle accordingly
        // This is a simplified version - in a real implementation,
        // you'd want to track the UI state more precisely
        
        // For now, just handle settings menu back button
        UIManager.Instance.CloseSettings();
    }
    #endregion

    #region Public Methods
    public void EnableGameplayInput()
    {
        // Enable gameplay-related input actions
        if (inputActions != null)
        {
            var playerMap = inputActions.FindActionMap("Player");
            playerMap?.Enable();
        }
    }

    public void DisableGameplayInput()
    {
        // Disable gameplay input when in menus
        if (inputActions != null)
        {
            var playerMap = inputActions.FindActionMap("Player");
            playerMap?.Disable();
        }
    }

    public void EnableUIInput()
    {
        // Enable UI navigation input
        if (inputActions != null)
        {
            var uiMap = inputActions.FindActionMap("UI");
            uiMap?.Enable();
        }
    }

    public void DisableUIInput()
    {
        // Disable UI input when not needed
        if (inputActions != null)
        {
            var uiMap = inputActions.FindActionMap("UI");
            uiMap?.Disable();
        }
    }
    #endregion
}