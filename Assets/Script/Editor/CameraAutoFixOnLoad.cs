using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Script tự động chạy khi Unity Editor mở scene
/// Fix camera issues ngay lập tức mà không cần chạy game
/// </summary>
[InitializeOnLoad]
public static class CameraAutoFixOnLoad
{
    static CameraAutoFixOnLoad()
    {
        // Subscribe to scene opened event
        EditorSceneManager.sceneOpened += OnSceneOpened;
        
        // Also check when entering play mode
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        
        Debug.Log("[CameraAutoFix] Auto-fix system initialized");
    }

    private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
        Debug.Log($"[CameraAutoFix] Scene opened: {scene.name}");
        
        // Delay execution to ensure scene is fully loaded
        EditorApplication.delayCall += () => {
            CheckAndFixCameraIssues(scene.name);
        };
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Fix camera issues before entering play mode
            Debug.Log("[CameraAutoFix] Checking camera before play mode...");
            CheckAndFixCameraIssues("Pre-PlayMode");
        }
    }

    private static void CheckAndFixCameraIssues(string sceneName)
    {
        // Only auto-fix if enabled in preferences
        bool autoFixEnabled = EditorPrefs.GetBool("CityDriver.AutoFixCamera", true);
        if (!autoFixEnabled)
        {
            return;
        }

        Debug.Log($"[CameraAutoFix] Checking camera issues in {sceneName}...");

        bool issueFixed = false;

        // Check for main camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = Object.FindObjectOfType<Camera>();
        }

        if (mainCamera == null)
        {
            // No camera found, create one
            CreateMainCamera();
            issueFixed = true;
        }
        else
        {
            // Check if camera is active and enabled
            if (!mainCamera.gameObject.activeInHierarchy)
            {
                mainCamera.gameObject.SetActive(true);
                EditorUtility.SetDirty(mainCamera.gameObject);
                Debug.Log($"[CameraAutoFix] Activated camera: {mainCamera.name}");
                issueFixed = true;
            }

            if (!mainCamera.enabled)
            {
                mainCamera.enabled = true;
                EditorUtility.SetDirty(mainCamera);
                Debug.Log($"[CameraAutoFix] Enabled camera: {mainCamera.name}");
                issueFixed = true;
            }

            // Ensure it has MainCamera tag
            if (!mainCamera.CompareTag("MainCamera"))
            {
                mainCamera.tag = "MainCamera";
                EditorUtility.SetDirty(mainCamera.gameObject);
                Debug.Log($"[CameraAutoFix] Set MainCamera tag on: {mainCamera.name}");
                issueFixed = true;
            }
        }

        // Check for CameraManager
        CameraManager cameraManager = Object.FindObjectOfType<CameraManager>();
        if (cameraManager == null)
        {
            CreateCameraManager();
            issueFixed = true;
        }

        // Check for AudioListener
        AudioListener audioListener = Object.FindObjectOfType<AudioListener>();
        if (audioListener == null && mainCamera != null)
        {
            mainCamera.gameObject.AddComponent<AudioListener>();
            EditorUtility.SetDirty(mainCamera.gameObject);
            Debug.Log("[CameraAutoFix] Added AudioListener to main camera");
            issueFixed = true;
        }

        if (issueFixed)
        {
            Debug.Log($"[CameraAutoFix] Camera issues fixed in {sceneName}");
            
            // Mark scene as dirty to save changes
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }

    private static void CreateMainCamera()
    {
        Debug.Log("[CameraAutoFix] Creating new main camera...");

        GameObject cameraObj = new GameObject("Main Camera");
        Camera camera = cameraObj.AddComponent<Camera>();
        AudioListener audioListener = cameraObj.AddComponent<AudioListener>();

        // Set camera properties
        camera.tag = "MainCamera";
        camera.transform.position = new Vector3(0, 0, -10);
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;
        camera.orthographic = true;
        camera.orthographicSize = 5;

        // Mark as dirty
        EditorUtility.SetDirty(cameraObj);

        // Register undo
        Undo.RegisterCreatedObjectUndo(cameraObj, "Auto-create Main Camera");

        Debug.Log("[CameraAutoFix] Main camera created successfully");
    }

    private static void CreateCameraManager()
    {
        Debug.Log("[CameraAutoFix] Creating CameraManager...");

        GameObject managerObj = new GameObject("CameraManager");
        CameraManager manager = managerObj.AddComponent<CameraManager>();

        // Attach existing camera if any
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(managerObj.transform);
            Debug.Log($"[CameraAutoFix] Attached {mainCamera.name} to CameraManager");
        }

        // Mark as dirty
        EditorUtility.SetDirty(managerObj);

        // Register undo
        Undo.RegisterCreatedObjectUndo(managerObj, "Auto-create Camera Manager");

        Debug.Log("[CameraAutoFix] CameraManager created successfully");
    }

    // Menu item to toggle auto-fix
    [MenuItem("Tools/CityDriver/Auto-Fix Settings/Enable Auto Camera Fix")]
    private static void EnableAutoFix()
    {
        EditorPrefs.SetBool("CityDriver.AutoFixCamera", true);
        Debug.Log("[CameraAutoFix] Auto-fix enabled");
    }

    [MenuItem("Tools/CityDriver/Auto-Fix Settings/Enable Auto Camera Fix", true)]
    private static bool EnableAutoFixValidate()
    {
        return !EditorPrefs.GetBool("CityDriver.AutoFixCamera", true);
    }

    [MenuItem("Tools/CityDriver/Auto-Fix Settings/Disable Auto Camera Fix")]
    private static void DisableAutoFix()
    {
        EditorPrefs.SetBool("CityDriver.AutoFixCamera", false);
        Debug.Log("[CameraAutoFix] Auto-fix disabled");
    }

    [MenuItem("Tools/CityDriver/Auto-Fix Settings/Disable Auto Camera Fix", true)]
    private static bool DisableAutoFixValidate()
    {
        return EditorPrefs.GetBool("CityDriver.AutoFixCamera", true);
    }

    [MenuItem("Tools/CityDriver/Auto-Fix Settings/Force Check Camera Now")]
    private static void ForceCheckNow()
    {
        CheckAndFixCameraIssues("Manual Check");
    }
}