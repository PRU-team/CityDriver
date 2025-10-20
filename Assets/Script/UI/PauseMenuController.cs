using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PauseMenuController quản lý pause menu UI và interactions
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Pause Menu Animation")]
    [SerializeField] private Animator pauseMenuAnimator;
    [SerializeField] private CanvasGroup pauseMenuCanvasGroup;

    [Header("Background")]
    [SerializeField] private Image backgroundBlur;
    [SerializeField] private float blurFadeSpeed = 2f;

    private void Start()
    {
        SetupButtonListeners();
        SetupButtonAnimations();
    }

    private void OnEnable()
    {
        // Animate pause menu entrance
        if (pauseMenuAnimator != null)
        {
            pauseMenuAnimator.SetTrigger("Show");
        }

        StartCoroutine(FadeInBackground());
    }

    private void SetupButtonListeners()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeClick);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsClick);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClick);
        }
    }

    private void SetupButtonAnimations()
    {
        AddButtonHoverEffect(resumeButton);
        AddButtonHoverEffect(settingsButton);
        AddButtonHoverEffect(mainMenuButton);
    }

    private void AddButtonHoverEffect(Button button)
    {
        if (button == null) return;

        UnityEngine.EventSystems.EventTrigger trigger = button.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }

        // Hover enter
        UnityEngine.EventSystems.EventTrigger.Entry pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => {
            AudioManager.Instance?.PlayButtonHover();
            button.transform.localScale = Vector3.one * 1.05f;
        });
        trigger.triggers.Add(pointerEnter);

        // Hover exit
        UnityEngine.EventSystems.EventTrigger.Entry pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => {
            button.transform.localScale = Vector3.one;
        });
        trigger.triggers.Add(pointerExit);
    }

    #region Button Handlers
    public void OnResumeClick()
    {
        AudioManager.Instance?.PlayButtonClick();
        UIManager.Instance?.ResumeGame();
    }

    public void OnSettingsClick()
    {
        AudioManager.Instance?.PlayButtonClick();
        UIManager.Instance?.ShowSettings();
    }

    public void OnMainMenuClick()
    {
        AudioManager.Instance?.PlayButtonClick();
        
        // Show confirmation dialog here if needed
        UIManager.Instance?.ReturnToMainMenu();
    }
    #endregion

    #region Visual Effects
    private System.Collections.IEnumerator FadeInBackground()
    {
        if (backgroundBlur != null)
        {
            Color startColor = backgroundBlur.color;
            startColor.a = 0f;
            backgroundBlur.color = startColor;

            float elapsedTime = 0f;
            float duration = 1f / blurFadeSpeed;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(0f, 0.8f, elapsedTime / duration);
                
                Color newColor = backgroundBlur.color;
                newColor.a = alpha;
                backgroundBlur.color = newColor;

                yield return null;
            }
        }
    }
    #endregion
}