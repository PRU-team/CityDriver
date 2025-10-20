using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// Tool để fix vấn đề multiple CameraManager conflicts
/// </summary>
public class CameraManagerConflictFix
{
    [MenuItem("Tools/CityDriver/Fix CameraManager Conflicts", false, 0)]
    public static void FixCameraManagerConflicts()
    {
        Debug.Log("=== Fixing CameraManager Conflicts ===");

        if (Application.isPlaying)
        {
            FixConflictsInPlayMode();
        }
        else
        {
            FixConflictsInEditMode();
        }
    }

    private static void FixConflictsInPlayMode()
    {
        Debug.Log("--- Fixing CameraManager conflicts in Play Mode ---");

        // Find all CameraManagers
        CameraManager[] cameraManagers = Object.FindObjectsOfType<CameraManager>(true);
        Debug.Log($"Found {cameraManagers.Length} CameraManager(s)");

        if (cameraManagers.Length <= 1)
        {
            Debug.Log("✅ No conflicts detected");
            return;
        }

        // Find the best CameraManager to keep
        CameraManager bestManager = null;
        Camera bestCamera = null;

        foreach (CameraManager manager in cameraManagers)
        {
            Camera managerCamera = GetCameraFromManager(manager);
            
            if (bestManager == null)
            {
                bestManager = manager;
                bestCamera = managerCamera;
                continue;
            }

            // Compare with current best
            if (IsBetterCamera(managerCamera, bestCamera))
            {
                bestManager = manager;
                bestCamera = managerCamera;
            }
        }

        // Remove all other CameraManagers
        foreach (CameraManager manager in cameraManagers)
        {
            if (manager != bestManager)
            {
                Debug.Log($"Destroying conflicting CameraManager: {manager.name}");
                Object.Destroy(manager.gameObject);
            }
        }

        Debug.Log($"✅ Kept best CameraManager: {bestManager.name} with camera: {(bestCamera != null ? bestCamera.name : "None")}");
    }

    private static void FixConflictsInEditMode()
    {
        Debug.Log("--- Fixing CameraManager conflicts in Edit Mode ---");

        // Check current scene
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"Current scene: {currentScene}");

        // Find CameraManagers in current scene
        CameraManager[] cameraManagers = Object.FindObjectsOfType<CameraManager>(true);
        Debug.Log($"Found {cameraManagers.Length} CameraManager(s) in scene");

        if (cameraManagers.Length <= 1)
        {
            if (cameraManagers.Length == 0)
            {
                Debug.Log("No CameraManager found - creating one");
                CreateOptimalCameraManager();
            }
            else
            {
                Debug.Log("✅ Single CameraManager found - no conflicts");
            }
            return;
        }

        // Multiple CameraManagers found - merge them
        Debug.Log($"Multiple CameraManagers detected - merging into one");
        MergeCameraManagersInEditMode(cameraManagers);
    }

    private static void CreateOptimalCameraManager()
    {
        // Find best camera in scene
        Camera bestCamera = Camera.main;
        if (bestCamera == null)
        {
            bestCamera = Object.FindObjectOfType<Camera>();
        }

        if (bestCamera == null)
        {
            // No camera exists - create one
            GameObject cameraObj = new GameObject("Main Camera");
            bestCamera = cameraObj.AddComponent<Camera>();
            bestCamera.tag = "MainCamera";
            
            if (Object.FindObjectOfType<AudioListener>() == null)
            {
                cameraObj.AddComponent<AudioListener>();
            }

            // Set camera properties
            bestCamera.transform.position = new Vector3(0, 0, -10);
            bestCamera.clearFlags = CameraClearFlags.SolidColor;
            bestCamera.backgroundColor = Color.black;
            bestCamera.orthographic = true;
            bestCamera.orthographicSize = 5;

            Debug.Log("✅ Created new Main Camera");
        }

        // Create CameraManager
        GameObject managerObj = new GameObject("CameraManager");
        CameraManager manager = managerObj.AddComponent<CameraManager>();

        // Attach camera to manager
        bestCamera.transform.SetParent(managerObj.transform);

        Debug.Log("✅ Created optimal CameraManager setup");
        
        EditorUtility.SetDirty(managerObj);
        EditorUtility.SetDirty(bestCamera.gameObject);
    }

    private static void MergeCameraManagersInEditMode(CameraManager[] managers)
    {
        // Find best manager and camera
        CameraManager bestManager = null;
        Camera bestCamera = null;

        foreach (CameraManager manager in managers)
        {
            Camera managerCamera = GetCameraFromManager(manager);
            
            if (bestManager == null || IsBetterCamera(managerCamera, bestCamera))
            {
                bestManager = manager;
                bestCamera = managerCamera;
            }
        }

        // Collect all cameras from other managers
        foreach (CameraManager manager in managers)
        {
            if (manager != bestManager)
            {
                Camera managerCamera = GetCameraFromManager(manager);
                
                if (managerCamera != null && managerCamera != bestCamera)
                {
                    // Move camera to best manager or destroy if redundant
                    if (bestCamera == null)
                    {
                        managerCamera.transform.SetParent(bestManager.transform);
                        Debug.Log($"Moved camera {managerCamera.name} to best manager");
                    }
                    else
                    {
                        Debug.Log($"Destroying redundant camera: {managerCamera.name}");
                        Object.DestroyImmediate(managerCamera.gameObject);
                    }
                }

                // Destroy the manager
                Debug.Log($"Destroying redundant CameraManager: {manager.name}");
                Object.DestroyImmediate(manager.gameObject);
            }
        }

        Debug.Log($"✅ Merged into single CameraManager: {bestManager.name}");
        EditorUtility.SetDirty(bestManager.gameObject);
    }

    private static Camera GetCameraFromManager(CameraManager manager)
    {
        // Get camera from manager's private field
        var field = typeof(CameraManager).GetField("mainCamera", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Camera camera = field?.GetValue(manager) as Camera;
        
        if (camera != null) return camera;

        // Check children
        return manager.GetComponentInChildren<Camera>();
    }

    private static bool IsBetterCamera(Camera newCamera, Camera currentBest)
    {
        if (newCamera == null) return false;
        if (currentBest == null) return true;

        // Prefer MainCamera tag
        if (newCamera.CompareTag("MainCamera") && !currentBest.CompareTag("MainCamera"))
            return true;
        if (currentBest.CompareTag("MainCamera") && !newCamera.CompareTag("MainCamera"))
            return false;

        // Prefer active and enabled
        bool newActive = newCamera.gameObject.activeInHierarchy && newCamera.enabled;
        bool currentActive = currentBest.gameObject.activeInHierarchy && currentBest.enabled;

        if (newActive && !currentActive) return true;
        if (currentActive && !newActive) return false;

        // Prefer camera from current scene
        string currentScene = SceneManager.GetActiveScene().name;
        if (newCamera.gameObject.scene.name == currentScene && currentBest.gameObject.scene.name != currentScene)
            return true;

        return false;
    }

    [MenuItem("Tools/CityDriver/Remove All CameraManagers", false, 1)]
    public static void RemoveAllCameraManagers()
    {
        Debug.Log("=== Removing All CameraManagers ===");

        CameraManager[] managers = Object.FindObjectsOfType<CameraManager>(true);
        
        foreach (CameraManager manager in managers)
        {
            Debug.Log($"Destroying CameraManager: {manager.name}");
            
            if (Application.isPlaying)
            {
                Object.Destroy(manager.gameObject);
            }
            else
            {
                Object.DestroyImmediate(manager.gameObject);
            }
        }

        Debug.Log($"✅ Removed {managers.Length} CameraManager(s)");
        
        EditorUtility.DisplayDialog(
            "CameraManagers Removed", 
            $"All {managers.Length} CameraManager(s) have been removed.\n\nYou can now set up a clean CameraManager system.", 
            "OK"
        );
    }

    [MenuItem("Tools/CityDriver/Create Single CameraManager", false, 2)]
    public static void CreateSingleCameraManager()
    {
        Debug.Log("=== Creating Single CameraManager ===");

        // Remove existing managers first
        CameraManager[] existingManagers = Object.FindObjectsOfType<CameraManager>(true);
        foreach (CameraManager manager in existingManagers)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(manager.gameObject);
            }
            else
            {
                Object.DestroyImmediate(manager.gameObject);
            }
        }

        // Create new optimal setup
        CreateOptimalCameraManager();

        EditorUtility.DisplayDialog(
            "Single CameraManager Created", 
            "✅ Clean CameraManager system created\n✅ Main Camera configured\n✅ Ready for scene transitions\n\nTest your scene transitions now!", 
            "OK"
        );
    }

    [MenuItem("Tools/CityDriver/Debug CameraManager State", false, 3)]
    public static void DebugCameraManagerState()
    {
        Debug.Log("=== CameraManager Debug Info ===");

        CameraManager[] managers = Object.FindObjectsOfType<CameraManager>(true);
        
        string report = $"CameraManager Debug Report:\n\n";
        report += $"Total CameraManagers: {managers.Length}\n\n";

        for (int i = 0; i < managers.Length; i++)
        {
            CameraManager manager = managers[i];
            Camera camera = GetCameraFromManager(manager);
            
            report += $"CameraManager {i + 1}:\n";
            report += $"  - Name: {manager.name}\n";
            report += $"  - Scene: {manager.gameObject.scene.name}\n";
            report += $"  - Has Camera: {(camera != null ? camera.name : "None")}\n";
            report += $"  - Is Instance: {(CameraManager.Instance == manager ? "Yes" : "No")}\n\n";
        }

        if (CameraManager.Instance != null)
        {
            Camera instanceCamera = GetCameraFromManager(CameraManager.Instance);
            report += $"Current Instance Camera: {(instanceCamera != null ? instanceCamera.name : "None")}\n";
        }
        else
        {
            report += "No CameraManager.Instance set\n";
        }

        Debug.Log(report);
        EditorUtility.DisplayDialog("CameraManager Debug", report, "OK");
    }
}