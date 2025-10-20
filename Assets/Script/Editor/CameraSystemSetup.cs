using UnityEngine;
using UnityEditor;

/// <summary>
/// Script để tạo Camera System Prefab và setup automatic
/// Chạy một lần để setup toàn bộ hệ thống
/// </summary>
public class CameraSystemSetup
{
    [MenuItem("Tools/CityDriver/Setup Complete Camera System")]
    public static void SetupCompleteCameraSystem()
    {
        Debug.Log("=== Setting up Complete Camera System ===");

        // Step 1: Create or fix main camera
        Camera mainCamera = SetupMainCamera();

        // Step 2: Create CameraManager
        CameraManager cameraManager = SetupCameraManager(mainCamera);

        // Step 3: Setup scene for persistence
        SetupScenePersistence();

        // Step 4: Create prefab for future use
        CreateCameraSystemPrefab(cameraManager.gameObject);

        Debug.Log("✅ Complete Camera System setup finished!");
        
        // Show success dialog
        EditorUtility.DisplayDialog(
            "Camera System Setup Complete", 
            "Camera system has been setup successfully!\n\n" +
            "✅ Main Camera created/fixed\n" +
            "✅ CameraManager installed\n" +
            "✅ Auto-fix system enabled\n" +
            "✅ Scene configured for persistence\n\n" +
            "You can now test scene transitions without camera issues.", 
            "OK"
        );
    }

    private static Camera SetupMainCamera()
    {
        Debug.Log("Setting up Main Camera...");

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = Object.FindObjectOfType<Camera>();
        }

        if (mainCamera == null)
        {
            // Create new main camera
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            
            // Add AudioListener if none exists
            if (Object.FindObjectOfType<AudioListener>() == null)
            {
                cameraObj.AddComponent<AudioListener>();
            }

            // Set camera properties
            mainCamera.tag = "MainCamera";
            mainCamera.transform.position = new Vector3(0, 0, -10);
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = Color.black;
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 5;

            Debug.Log("✅ New Main Camera created");
        }
        else
        {
            // Fix existing camera
            mainCamera.gameObject.SetActive(true);
            mainCamera.enabled = true;
            
            if (!mainCamera.CompareTag("MainCamera"))
            {
                mainCamera.tag = "MainCamera";
            }

            // Ensure AudioListener exists
            if (Object.FindObjectOfType<AudioListener>() == null)
            {
                mainCamera.gameObject.AddComponent<AudioListener>();
            }

            Debug.Log("✅ Existing Main Camera fixed");
        }

        EditorUtility.SetDirty(mainCamera.gameObject);
        return mainCamera;
    }

    private static CameraManager SetupCameraManager(Camera mainCamera)
    {
        Debug.Log("Setting up CameraManager...");

        // Remove existing CameraManager if any
        CameraManager existingManager = Object.FindObjectOfType<CameraManager>();
        if (existingManager != null)
        {
            Debug.Log("Removing existing CameraManager...");
            Object.DestroyImmediate(existingManager.gameObject);
        }

        // Create new CameraManager
        GameObject managerObj = new GameObject("CameraManager");
        CameraManager cameraManager = managerObj.AddComponent<CameraManager>();

        // Move main camera to be child of CameraManager
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(managerObj.transform);
            Debug.Log("✅ Main Camera attached to CameraManager");
        }

        EditorUtility.SetDirty(managerObj);
        Debug.Log("✅ CameraManager created");

        return cameraManager;
    }

    private static void SetupScenePersistence()
    {
        Debug.Log("Setting up scene persistence...");

        // Enable auto-fix system
        EditorPrefs.SetBool("CityDriver.AutoFixCamera", true);

        // Mark scene as dirty to save changes
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );

        Debug.Log("✅ Scene persistence configured");
    }

    private static void CreateCameraSystemPrefab(GameObject cameraSystemObj)
    {
        Debug.Log("Creating Camera System Prefab...");

        // Create Prefabs folder if it doesn't exist
        string prefabFolder = "Assets/Prefab";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Prefab");
        }

        // Create prefab path
        string prefabPath = $"{prefabFolder}/CameraSystem.prefab";

        try
        {
            // Delete existing prefab if any
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                AssetDatabase.DeleteAsset(prefabPath);
            }

            // Create new prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(cameraSystemObj, prefabPath);
            
            if (prefab != null)
            {
                Debug.Log($"✅ Camera System Prefab created at: {prefabPath}");
            }
            else
            {
                Debug.LogWarning("Failed to create Camera System Prefab");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Could not create prefab: {e.Message}");
        }
    }

    [MenuItem("Tools/CityDriver/Quick Setup Camera (One Click)")]
    public static void QuickSetupCamera()
    {
        Debug.Log("=== Quick Camera Setup ===");

        // Quick setup without prefab creation
        Camera mainCamera = SetupMainCamera();
        CameraManager cameraManager = SetupCameraManager(mainCamera);
        
        // Enable auto-fix
        EditorPrefs.SetBool("CityDriver.AutoFixCamera", true);

        Debug.Log("✅ Quick Camera Setup completed!");
        
        EditorUtility.DisplayDialog(
            "Quick Setup Complete", 
            "Camera system has been quickly setup!\n\n" +
            "✅ Camera fixed and ready\n" +
            "✅ CameraManager installed\n" +
            "✅ Auto-fix enabled\n\n" +
            "Test your scene transitions now!", 
            "OK"
        );
    }

    [MenuItem("Tools/CityDriver/Validate Camera Setup")]
    public static void ValidateCameraSetup()
    {
        Debug.Log("=== Validating Camera Setup ===");

        bool isValid = true;
        string report = "Camera Setup Validation Report:\n\n";

        // Check main camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.gameObject.activeInHierarchy && mainCamera.enabled)
        {
            report += "✅ Main Camera: OK\n";
        }
        else
        {
            report += "❌ Main Camera: PROBLEM\n";
            isValid = false;
        }

        // Check CameraManager
        CameraManager cameraManager = Object.FindObjectOfType<CameraManager>();
        if (cameraManager != null)
        {
            report += "✅ CameraManager: OK\n";
        }
        else
        {
            report += "❌ CameraManager: MISSING\n";
            isValid = false;
        }

        // Check AudioListener
        AudioListener audioListener = Object.FindObjectOfType<AudioListener>();
        if (audioListener != null)
        {
            report += "✅ AudioListener: OK\n";
        }
        else
        {
            report += "❌ AudioListener: MISSING\n";
            isValid = false;
        }

        // Check auto-fix setting
        bool autoFixEnabled = EditorPrefs.GetBool("CityDriver.AutoFixCamera", false);
        if (autoFixEnabled)
        {
            report += "✅ Auto-Fix: ENABLED\n";
        }
        else
        {
            report += "⚠️ Auto-Fix: DISABLED\n";
        }

        report += $"\nOverall Status: {(isValid ? "✅ GOOD" : "❌ NEEDS FIX")}";

        Debug.Log(report);
        
        EditorUtility.DisplayDialog(
            "Camera Setup Validation", 
            report, 
            "OK"
        );
    }
}