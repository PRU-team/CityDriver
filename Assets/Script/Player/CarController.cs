using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Simple CarController:
/// - A/D moves the car left/right (world X axis) with configurable lateralSpeed.
/// - W increases game base scroll speed, S decreases it (via GameManager.ModifyBaseScrollSpeed).
/// - Keeps the car within optional horizontal bounds if set.
/// Supports both Legacy Input (Input.GetKey) and the New Input System (Keyboard.current) via preprocessor defines.
/// </summary>
public class CarController : MonoBehaviour
{
    [Header("Movement")]
    public float lateralSpeed = 6f; // units per second for A/D movement
    public float horizontalLimit = 6f; // clamp X position

    [Header("Speed Control")]
    public float speedAdjustAmount = 1f; // amount to change base scroll speed per second while holding W/S

    private GameManager gm;

    void Start()
    {
        gm = GameManager.Instance;
        if (gm == null)
            Debug.LogWarning("GameManager not found in scene. Speed controls will be disabled.");
    }

    void Update()
    {
        HandleMovement();
        HandleSpeedInput();
    }

    void HandleMovement()
    {
        float h = 0f;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) h -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
        }
        else
        {
            // Fallback to legacy if Keyboard not available
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h += 1f;
        }
#else
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h += 1f;
#endif

        Vector3 pos = transform.position;
        pos.x += h * lateralSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, -horizontalLimit, horizontalLimit);
        transform.position = pos;
    }

    void HandleSpeedInput()
    {
        if (gm == null) return;

        float delta = 0f;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed) delta += speedAdjustAmount * Time.deltaTime;
            if (kb.sKey.isPressed) delta -= speedAdjustAmount * Time.deltaTime;
        }
        else
        {
            if (Input.GetKey(KeyCode.W)) delta += speedAdjustAmount * Time.deltaTime;
            if (Input.GetKey(KeyCode.S)) delta -= speedAdjustAmount * Time.deltaTime;
        }
#else
        if (Input.GetKey(KeyCode.W)) delta += speedAdjustAmount * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) delta -= speedAdjustAmount * Time.deltaTime;
#endif

        if (Mathf.Abs(delta) > 0f)
            gm.ModifyBaseScrollSpeed(delta);
    }
}

