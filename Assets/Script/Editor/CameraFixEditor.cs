using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor tool để fix camera issues ngay trong Editor mode
/// Chạy được cả khi không Play
/// </summary>
public class CameraFixEditor : EditorWindow
{
    private bool showDebugInfo = true;
    private Vector2 scrollPosition;
    private string logText = "";

    [MenuItem("Tools/CityDriver/Camera Fix Editor")]
    public static void ShowWindow()
    {
        CameraFixEditor window = GetWindow<CameraFixEditor>("Camera Fix Tool");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Camera Fix Tool - Editor Mode", EditorStyles.boldLabel);
        GUILayout.Space(10);

        showDebugInfo = EditorGUILayout.Toggle("Show Debug Info", showDebugInfo);
        GUILayout.Space(10);

        // Quick Fix Button
        if (GUILayout.Button("Quick Camera Fix (Editor)", GUILayout.Height(30)))
        {
            QuickCameraFixEditor();
        }

        GUILayout.Space(5);

        // Full Diagnosis Button
        if (GUILayout.Button("Full Camera Diagnosis & Fix", GUILayout.Height(30)))
        {
            FullCameraFixEditor();
        }

        GUILayout.Space(5);

        // Setup Persistent Camera Button
        if (GUILayout.Button("Setup Persistent Camera System", GUILayout.Height(30)))
        {
            SetupPersistentCameraSystem();
        }

        GUILayout.Space(10);
        GUILayout.Label("Debug Log:", EditorStyles.boldLabel);

        // Scroll area for debug log
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        EditorGUILayout.TextArea(logText, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("Clear Log"))
        {
            logText = "";
        }
    }

    private void QuickCameraFixEditor()
    {
        LogMessage("=== Quick Camera Fix (Editor Mode) ===");

        // Find current camera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            mainCam = FindObjectOfType<Camera>();
        }

        if (mainCam != null)
        {
            // Ensure camera is active
            mainCam.gameObject.SetActive(true);
            mainCam.enabled = true;
            
            // Set as main camera if not already
            if (!mainCam.CompareTag("MainCamera"))
            {
                mainCam.tag = "MainCamera";
            }

            LogMessage($"Fixed existing camera: {mainCam.name}");
            EditorUtility.SetDirty(mainCam.gameObject);
        }
        else
        {
            // Create new camera
            CreateMainCameraEditor();
        }

        // Ensure we have CameraManager in the scene
        EnsureCameraManagerInScene();

        LogMessage("Quick fix completed!");
    }

    private void FullCameraFixEditor()
    {
        LogMessage("=== Full Camera Diagnosis & Fix (Editor Mode) ===");

        // Diagnosis
        DiagnoseCameraIssuesEditor();

        // Fix all issues
        FixAllCameraIssuesEditor();

        // Setup persistent system
        SetupPersistentCameraSystem();

        LogMessage("Full camera fix completed!");
    }

    private void DiagnoseCameraIssuesEditor()
    {
        LogMessage("--- Camera Diagnosis ---");

        // Check for main camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            LogMessage($"Main Camera: {mainCam.name}");
            LogMessage($"  - Active: {mainCam.gameObject.activeInHierarchy}");
            LogMessage($"  - Enabled: {mainCam.enabled}");
            LogMessage($"  - Scene: {mainCam.gameObject.scene.name}");
        }
        else
        {
            LogMessage("Main Camera: NOT FOUND!");
        }

        // Check all cameras
        Camera[] allCameras = FindObjectsOfType<Camera>(true);
        LogMessage($"Total cameras found: {allCameras.Length}");

        foreach (Camera cam in allCameras)
        {
            LogMessage($"  - {cam.name}: Active={cam.gameObject.activeInHierarchy}, Enabled={cam.enabled}");
        }

        // Check for CameraManager
        CameraManager cameraManager = FindObjectOfType<CameraManager>(true);
        LogMessage($"CameraManager exists: {cameraManager != null}");

        // Check AudioListener
        AudioListener[] listeners = FindObjectsOfType<AudioListener>(true);
        LogMessage($"AudioListeners: {listeners.Length}");
    }

    private void FixAllCameraIssuesEditor()
    {
        LogMessage("--- Fixing All Camera Issues ---");

        Camera[] allCameras = FindObjectsOfType<Camera>(true);

        if (allCameras.Length == 0)
        {
            LogMessage("No cameras found, creating new main camera...");
            CreateMainCameraEditor();
            return;
        }

        // Fix all cameras
        foreach (Camera cam in allCameras)
        {
            if (!cam.gameObject.activeInHierarchy)
            {
                cam.gameObject.SetActive(true);
                LogMessage($"Activated camera: {cam.name}");
                EditorUtility.SetDirty(cam.gameObject);
            }

            if (!cam.enabled)
            {
                cam.enabled = true;
                LogMessage($"Enabled camera: {cam.name}");
                EditorUtility.SetDirty(cam);
            }
        }

        // Ensure main camera
        if (Camera.main == null && allCameras.Length > 0)
        {
            allCameras[0].tag = "MainCamera";
            LogMessage($"Set {allCameras[0].name} as MainCamera");
            EditorUtility.SetDirty(allCameras[0].gameObject);
        }
    }

    private void CreateMainCameraEditor()
    {
        LogMessage("Creating new Main Camera...");

        GameObject cameraObj = new GameObject("Main Camera");
        Camera newCamera = cameraObj.AddComponent<Camera>();
        
        // Add AudioListener if none exists
        if (FindObjectOfType<AudioListener>() == null)
        {
            cameraObj.AddComponent<AudioListener>();
            LogMessage("Added AudioListener to camera");
        }

        // Set camera properties
        newCamera.tag = "MainCamera";
        newCamera.transform.position = new Vector3(0, 0, -10);
        newCamera.clearFlags = CameraClearFlags.SolidColor;
        newCamera.backgroundColor = Color.black;
        newCamera.orthographic = true;
        newCamera.orthographicSize = 5;

        LogMessage($"Created new main camera: {cameraObj.name}");

        // Mark as dirty for saving
        EditorUtility.SetDirty(cameraObj);

        // Register undo
        Undo.RegisterCreatedObjectUndo(cameraObj, "Create Main Camera");
    }

    private void SetupPersistentCameraSystem()
    {
        LogMessage("--- Setting up Persistent Camera System ---");

        // Check if CameraManager already exists
        CameraManager existingManager = FindObjectOfType<CameraManager>(true);
        if (existingManager != null)
        {
            LogMessage("CameraManager already exists in scene");
            return;
        }

        // Create CameraManager GameObject
        GameObject cameraManagerObj = new GameObject("CameraManager");
        CameraManager cameraManager = cameraManagerObj.AddComponent<CameraManager>();

        LogMessage("Created CameraManager in scene");

        // Find and assign main camera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            mainCam = FindObjectOfType<Camera>();
        }

        if (mainCam != null)
        {
            // Move camera to be child of CameraManager for persistence
            mainCam.transform.SetParent(cameraManagerObj.transform);
            LogMessage($"Attached camera {mainCam.name} to CameraManager");
        }

        // Mark as dirty for saving
        EditorUtility.SetDirty(cameraManagerObj);

        // Register undo
        Undo.RegisterCreatedObjectUndo(cameraManagerObj, "Create Camera Manager");

        LogMessage("Persistent camera system setup completed!");
    }

    private void EnsureCameraManagerInScene()
    {
        CameraManager existingManager = FindObjectOfType<CameraManager>(true);
        if (existingManager == null)
        {
            GameObject cameraManagerObj = new GameObject("CameraManager");
            CameraManager cameraManager = cameraManagerObj.AddComponent<CameraManager>();
            
            LogMessage("Created CameraManager in scene");
            EditorUtility.SetDirty(cameraManagerObj);
            Undo.RegisterCreatedObjectUndo(cameraManagerObj, "Create Camera Manager");
        }
    }

    private void LogMessage(string message)
    {
        if (showDebugInfo)
        {
            string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
            logText += $"[{timestamp}] {message}\n";
            
            // Auto-scroll to bottom
            scrollPosition.y = Mathf.Infinity;
            
            // Also log to console
            Debug.Log($"[CameraFixEditor] {message}");
        }
    }

    // Static methods for menu items
    [MenuItem("Tools/CityDriver/Quick Fix Camera (Editor)", false, 1)]
    public static void MenuQuickFixCamera()
    {
        CameraFixEditor window = GetWindow<CameraFixEditor>("Camera Fix Tool");
        window.QuickCameraFixEditor();
        window.Show();
    }

    [MenuItem("Tools/CityDriver/Setup Persistent Camera System", false, 2)]
    public static void MenuSetupPersistentCamera()
    {
        CameraFixEditor window = GetWindow<CameraFixEditor>("Camera Fix Tool");
        window.SetupPersistentCameraSystem();
        window.Show();
    }

    [MenuItem("Tools/CityDriver/Diagnose Camera Issues", false, 3)]
    public static void MenuDiagnoseCameraIssues()
    {
        CameraFixEditor window = GetWindow<CameraFixEditor>("Camera Fix Tool");
        window.DiagnoseCameraIssuesEditor();
        window.Show();
    }
}