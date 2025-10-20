using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MenuController quản lý các tương tác UI cho main menu
/// Sử dụng với UIManager để handle menu navigation
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Menu Animations")]
    [SerializeField] private Animator menuAnimator;
    [SerializeField] private float buttonHoverScale = 1.1f;

    private void Start()
    {
        SetupButtonAnimations();
    }

    private void SetupButtonAnimations()
    {
        // Add hover effects to buttons
        AddButtonHoverEffect(startButton);
        AddButtonHoverEffect(settingsButton);
        AddButtonHoverEffect(quitButton);
    }

    private void AddButtonHoverEffect(Button button)
    {
        if (button == null) return;

        // Get or add EventTrigger component
        UnityEngine.EventSystems.EventTrigger trigger = button.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }

        // Add hover enter event
        UnityEngine.EventSystems.EventTrigger.Entry pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => {
            button.transform.localScale = Vector3.one * buttonHoverScale;
        });
        trigger.triggers.Add(pointerEnter);

        // Add hover exit event
        UnityEngine.EventSystems.EventTrigger.Entry pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => {
            button.transform.localScale = Vector3.one;
        });
        trigger.triggers.Add(pointerExit);
    }

    public void OnStartButtonClick()
    {
        // Play click sound
        AudioManager.Instance?.PlayButtonClick();
        
        // Trigger start game animation if needed
        if (menuAnimator != null)
        {
            menuAnimator.SetTrigger("StartGame");
        }
    }

    public void OnSettingsButtonClick()
    {
        AudioManager.Instance?.PlayButtonClick();
        
        if (menuAnimator != null)
        {
            menuAnimator.SetTrigger("OpenSettings");
        }
    }

    public void OnQuitButtonClick()
    {
        AudioManager.Instance?.PlayButtonClick();
        
        if (menuAnimator != null)
        {
            menuAnimator.SetTrigger("QuitGame");
        }
    }
}