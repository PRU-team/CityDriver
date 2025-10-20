using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// GameHUD controller quản lý hiển thị thông tin game real-time
/// Bao gồm speed, score, fuel và các indicators khác
/// </summary>
public class GameHUD : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private Text speedText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private Image fuelFillImage;
    [SerializeField] private Button pauseButton;

    [Header("Speed Display")]
    [SerializeField] private GameObject speedometerPanel;
    [SerializeField] private Transform speedNeedle;
    [SerializeField] private float maxSpeedAngle = 240f;
    [SerializeField] private float maxDisplaySpeed = 200f;

    [Header("Warning Effects")]
    [SerializeField] private GameObject lowFuelWarning;
    [SerializeField] private Color lowFuelColor = Color.red;
    [SerializeField] private Color normalFuelColor = Color.green;
    [SerializeField] private float lowFuelThreshold = 0.25f;
    [SerializeField] private float warningBlinkSpeed = 2f;

    [Header("Score Effects")]
    [SerializeField] private GameObject scorePopupPrefab;
    [SerializeField] private Transform scorePopupParent;

    // Private variables
    private float currentSpeed = 0f;
    private float currentScore = 0f;
    private float currentFuel = 1f;
    private bool isLowFuelWarningActive = false;
    private Coroutine lowFuelCoroutine;

    private void Start()
    {
        InitializeHUD();
    }

    private void Update()
    {
        UpdateSpeedometer();
        CheckFuelLevel();
    }

    private void InitializeHUD()
    {
        // Set initial values
        UpdateSpeed(0f);
        UpdateScore(0f);
        UpdateFuel(1f);

        // Setup initial fuel color
        if (fuelFillImage != null)
        {
            fuelFillImage.color = normalFuelColor;
        }

        // Hide low fuel warning initially
        if (lowFuelWarning != null)
        {
            lowFuelWarning.SetActive(false);
        }
    }

    #region Public Update Methods
    public void UpdateSpeed(float speed)
    {
        currentSpeed = speed;
        
        if (speedText != null)
        {
            speedText.text = speed.ToString("F0") + " km/h";
        }
    }

    public void UpdateScore(float score)
    {
        // Show score increase popup if score increased significantly
        if (score > currentScore + 100f && scorePopupPrefab != null && scorePopupParent != null)
        {
            ShowScorePopup(score - currentScore);
        }

        currentScore = score;
        
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString("F0");
        }
    }

    public void UpdateFuel(float fuelPercent)
    {
        currentFuel = Mathf.Clamp01(fuelPercent);
        
        if (fuelSlider != null)
        {
            fuelSlider.value = currentFuel;
        }

        // Update fuel color based on level
        UpdateFuelColor();
    }
    #endregion

    #region Speedometer
    private void UpdateSpeedometer()
    {
        if (speedNeedle != null)
        {
            // Calculate needle rotation based on speed
            float speedRatio = Mathf.Clamp01(currentSpeed / maxDisplaySpeed);
            float targetAngle = speedRatio * maxSpeedAngle;
            
            // Smooth rotation
            Vector3 currentEuler = speedNeedle.eulerAngles;
            float currentAngle = currentEuler.z > 180 ? currentEuler.z - 360 : currentEuler.z;
            float newAngle = Mathf.LerpAngle(currentAngle, -targetAngle, Time.deltaTime * 5f);
            
            speedNeedle.rotation = Quaternion.Euler(0, 0, newAngle);
        }
    }
    #endregion

    #region Fuel System
    private void UpdateFuelColor()
    {
        if (fuelFillImage != null)
        {
            Color targetColor = currentFuel <= lowFuelThreshold ? lowFuelColor : normalFuelColor;
            fuelFillImage.color = Color.Lerp(fuelFillImage.color, targetColor, Time.deltaTime * 3f);
        }
    }

    private void CheckFuelLevel()
    {
        bool shouldShowWarning = currentFuel <= lowFuelThreshold;
        
        if (shouldShowWarning && !isLowFuelWarningActive)
        {
            StartLowFuelWarning();
        }
        else if (!shouldShowWarning && isLowFuelWarningActive)
        {
            StopLowFuelWarning();
        }
    }

    private void StartLowFuelWarning()
    {
        isLowFuelWarningActive = true;
        
        if (lowFuelWarning != null)
        {
            lowFuelWarning.SetActive(true);
            
            if (lowFuelCoroutine != null)
                StopCoroutine(lowFuelCoroutine);
                
            lowFuelCoroutine = StartCoroutine(BlinkLowFuelWarning());
        }
    }

    private void StopLowFuelWarning()
    {
        isLowFuelWarningActive = false;
        
        if (lowFuelWarning != null)
        {
            lowFuelWarning.SetActive(false);
        }
        
        if (lowFuelCoroutine != null)
        {
            StopCoroutine(lowFuelCoroutine);
            lowFuelCoroutine = null;
        }
    }

    private IEnumerator BlinkLowFuelWarning()
    {
        while (isLowFuelWarningActive)
        {
            if (lowFuelWarning != null)
            {
                lowFuelWarning.SetActive(!lowFuelWarning.activeSelf);
            }
            yield return new WaitForSeconds(1f / warningBlinkSpeed);
        }
    }
    #endregion

    #region Score Effects
    private void ShowScorePopup(float scoreIncrease)
    {
        GameObject popup = Instantiate(scorePopupPrefab, scorePopupParent);
        Text popupText = popup.GetComponent<Text>();
        
        if (popupText != null)
        {
            popupText.text = "+" + scoreIncrease.ToString("F0");
        }

        // Animate popup (move up and fade out)
        StartCoroutine(AnimateScorePopup(popup));
    }

    private IEnumerator AnimateScorePopup(GameObject popup)
    {
        Vector3 startPos = popup.transform.position;
        Vector3 endPos = startPos + Vector3.up * 100f;
        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = popup.AddComponent<CanvasGroup>();
        }

        float duration = 1.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Move up
            popup.transform.position = Vector3.Lerp(startPos, endPos, t);
            
            // Fade out
            canvasGroup.alpha = 1f - t;

            yield return null;
        }

        Destroy(popup);
    }
    #endregion

    #region Button Handlers
    public void OnPauseButtonClick()
    {
        AudioManager.Instance?.PlayButtonClick();
        UIManager.Instance?.PauseGame();
    }
    #endregion
}