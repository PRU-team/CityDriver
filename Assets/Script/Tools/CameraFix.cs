using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Tool để fix vấn đề "No cameras rendering" khi chuyển scene
/// Tự động setup CameraManager và đảm bảo camera luôn hoạt động
/// </summary>
public class CameraFix : MonoBehaviour
{
    [Header("Fix Options")]
    [SerializeField] private bool autoFixOnStart = true;
    [SerializeField] private bool createCameraManagerIfMissing = true;
    [SerializeField] private bool fixExistingCameras = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private void Start()
    {
        if (autoFixOnStart)
        {
            FixCameraIssues();
        }
    }

    /// <summary>
    /// Main method để fix tất cả camera issues
    /// </summary>
    [ContextMenu("Fix Camera Issues")]
    public void FixCameraIssues()
    {
        DebugLog("=== Camera Fix Tool ===");
        DebugLog("Starting camera issue diagnosis and fix...");

        // Step 1: Check current camera status
        DiagnoseCameraIssues();

        // Step 2: Create CameraManager if missing
        if (createCameraManagerIfMissing && CameraManager.Instance == null)
        {
            CreateCameraManager();
        }

        // Step 3: Fix existing cameras
        if (fixExistingCameras)
        {
            FixExistingCameras();
        }

        // Step 4: Final verification
        VerifyFix();

        DebugLog("Camera fix completed!");
    }

    private void DiagnoseCameraIssues()
    {
        DebugLog("--- Diagnosing Camera Issues ---");

        // Check for main camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            DebugLog($"Main Camera found: {mainCam.name}");
            DebugLog($"  - Active: {mainCam.gameObject.activeInHierarchy}");
            DebugLog($"  - Enabled: {mainCam.enabled}");
            DebugLog($"  - Scene: {mainCam.gameObject.scene.name}");
        }
        else
        {
            DebugLog("Main Camera NOT found!");
        }

        // Check for all cameras
        Camera[] allCameras = FindObjectsOfType<Camera>(true);
        DebugLog($"Total cameras in scene: {allCameras.Length}");

        foreach (Camera cam in allCameras)
        {
            DebugLog($"  Camera: {cam.name} - Active: {cam.gameObject.activeInHierarchy} - Enabled: {cam.enabled}");
        }

        // Check for CameraManager
        bool hasCameraManager = CameraManager.Instance != null;
        DebugLog($"CameraManager exists: {hasCameraManager}");

        // Check for AudioListener
        AudioListener[] listeners = FindObjectsOfType<AudioListener>(true);
        DebugLog($"AudioListeners found: {listeners.Length}");
    }

    private void CreateCameraManager()
    {
        DebugLog("--- Creating CameraManager ---");

        GameObject cameraManagerObj = new GameObject("CameraManager");
        CameraManager cameraManager = cameraManagerObj.AddComponent<CameraManager>();

        DebugLog("CameraManager created successfully");

        // Set CameraManager to not be destroyed
        DontDestroyOnLoad(cameraManagerObj);
    }

    private void FixExistingCameras()
    {
        DebugLog("--- Fixing Existing Cameras ---");

        Camera[] allCameras = FindObjectsOfType<Camera>(true);

        if (allCameras.Length == 0)
        {
            DebugLog("No cameras found, creating new main camera...");
            CreateNewMainCamera();
            return;
        }

        // Fix all inactive cameras
        foreach (Camera cam in allCameras)
        {
            if (!cam.gameObject.activeInHierarchy)
            {
                cam.gameObject.SetActive(true);
                DebugLog($"Activated camera: {cam.name}");
            }

            if (!cam.enabled)
            {
                cam.enabled = true;
                DebugLog($"Enabled camera: {cam.name}");
            }
        }

        // Ensure we have a main camera
        if (Camera.main == null)
        {
            Camera firstCamera = allCameras[0];
            firstCamera.tag = "MainCamera";
            DebugLog($"Set {firstCamera.name} as MainCamera");
        }
    }

    private void CreateNewMainCamera()
    {
        GameObject cameraObj = new GameObject("Main Camera");
        Camera newCamera = cameraObj.AddComponent<Camera>();
        AudioListener audioListener = cameraObj.AddComponent<AudioListener>();

        // Set camera tag
        newCamera.tag = "MainCamera";

        // Set default position and settings
        newCamera.transform.position = new Vector3(0, 0, -10);
        newCamera.clearFlags = CameraClearFlags.SolidColor;
        newCamera.backgroundColor = Color.black;
        newCamera.orthographic = true;
        newCamera.orthographicSize = 5;

        DebugLog("New main camera created with default settings");

        // If CameraManager exists, attach camera to it
        if (CameraManager.Instance != null)
        {
            cameraObj.transform.SetParent(CameraManager.Instance.transform);
            DebugLog("Camera attached to CameraManager");
        }
    }

    private void VerifyFix()
    {
        DebugLog("--- Verifying Fix ---");

        Camera mainCam = Camera.main;
        bool cameraWorking = mainCam != null && mainCam.gameObject.activeInHierarchy && mainCam.enabled;

        if (cameraWorking)
        {
            DebugLog("✓ Camera fix successful! Main camera is working properly.");
        }
        else
        {
            DebugLog("✗ Camera fix failed! Issues still exist.");
        }

        // Additional checks
        bool hasCameraManager = CameraManager.Instance != null;
        bool hasAudioListener = FindObjectOfType<AudioListener>() != null;

        DebugLog($"✓ CameraManager: {(hasCameraManager ? "Present" : "Missing")}");
        DebugLog($"✓ AudioListener: {(hasAudioListener ? "Present" : "Missing")}");
    }

    /// <summary>
    /// Quick fix method for immediate use
    /// </summary>
    [ContextMenu("Quick Camera Fix")]
    public void QuickCameraFix()
    {
        // Tìm camera hiện tại
        Camera cam = Camera.main;
        if (cam == null)
        {
            cam = FindObjectOfType<Camera>();
        }

        if (cam != null)
        {
            // Kích hoạt camera
            cam.gameObject.SetActive(true);
            cam.enabled = true;
            DebugLog($"Quick fix applied to camera: {cam.name}");
        }
        else
        {
            // Tạo camera mới
            CreateNewMainCamera();
            DebugLog("Quick fix: Created new main camera");
        }

        // Đảm bảo có CameraManager
        if (CameraManager.Instance == null)
        {
            CreateCameraManager();
        }
    }

    private void DebugLog(string message)
    {
        if (showDebugInfo)
        {
            Debug.Log($"[CameraFix] {message}");
        }
    }

    // Unity Editor methods
    #if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/CityDriver/Fix Camera Issues")]
    public static void MenuFixCameraIssues()
    {
        // Tìm hoặc tạo CameraFix object
        CameraFix cameraFix = FindObjectOfType<CameraFix>();
        if (cameraFix == null)
        {
            GameObject fixObj = new GameObject("CameraFix_Temp");
            cameraFix = fixObj.AddComponent<CameraFix>();
        }

        cameraFix.FixCameraIssues();

        // Cleanup nếu là temporary object
        if (cameraFix.gameObject.name == "CameraFix_Temp")
        {
            UnityEditor.EditorApplication.delayCall += () => {
                if (cameraFix != null && cameraFix.gameObject != null)
                {
                    DestroyImmediate(cameraFix.gameObject);
                }
            };
        }
    }

    [UnityEditor.MenuItem("Tools/CityDriver/Quick Camera Fix")]
    public static void MenuQuickCameraFix()
    {
        // Tìm hoặc tạo CameraFix object
        CameraFix cameraFix = FindObjectOfType<CameraFix>();
        if (cameraFix == null)
        {
            GameObject fixObj = new GameObject("CameraFix_Temp");
            cameraFix = fixObj.AddComponent<CameraFix>();
        }

        cameraFix.QuickCameraFix();

        // Cleanup nếu là temporary object
        if (cameraFix.gameObject.name == "CameraFix_Temp")
        {
            UnityEditor.EditorApplication.delayCall += () => {
                if (cameraFix != null && cameraFix.gameObject != null)
                {
                    DestroyImmediate(cameraFix.gameObject);
                }
            };
        }
    }
    #endif
}