using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// CameraManager quản lý main camera qua các scene khác nhau
/// Đảm bảo luôn có camera hoạt động khi chuyển scene
/// </summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool maintainCameraAcrossScenes = true;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private void Awake()
    {
        HandleCameraManagerConflict();
    }

    private void HandleCameraManagerConflict()
    {
        if (Instance == null)
        {
            // First CameraManager - set as instance
            Instance = this;
            
            if (maintainCameraAcrossScenes)
            {
                DontDestroyOnLoad(gameObject);
                DebugLog("CameraManager set to DontDestroyOnLoad");
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            InitializeCamera();
        }
        else
        {
            // CameraManager already exists - handle conflict
            DebugLog($"CameraManager conflict detected. Existing: {Instance.gameObject.scene.name}, New: {gameObject.scene.name}");
            
            // Get cameras from both managers
            Camera existingCamera = Instance.mainCamera;
            Camera newCamera = GetCameraFromThisManager();
            
            if (existingCamera == null && newCamera != null)
            {
                // Existing manager has no camera, this one does - replace existing
                DebugLog("Replacing existing CameraManager (no camera) with new one (has camera)");
                TransferCameraManagerInstance(newCamera);
            }
            else if (existingCamera != null && newCamera != null)
            {
                // Both have cameras - merge them intelligently
                DebugLog("Both CameraManagers have cameras - merging");
                MergeCameraManagers(existingCamera, newCamera);
            }
            else if (existingCamera != null && newCamera == null)
            {
                // Existing is better - destroy this one
                DebugLog("Existing CameraManager is better - destroying new one");
                Destroy(gameObject);
                return;
            }
            else
            {
                // Neither has camera - keep existing, destroy this
                DebugLog("Neither CameraManager has camera - keeping existing");
                Destroy(gameObject);
                return;
            }
        }
    }

    private Camera GetCameraFromThisManager()
    {
        // Check assigned camera
        if (mainCamera != null) return mainCamera;
        
        // Check child cameras
        Camera childCamera = GetComponentInChildren<Camera>();
        if (childCamera != null) return childCamera;
        
        // Check nearby cameras
        Camera nearbyCamera = FindObjectOfType<Camera>();
        return nearbyCamera;
    }

    private void TransferCameraManagerInstance(Camera newCamera)
    {
        // Clean up existing instance
        SceneManager.sceneLoaded -= Instance.OnSceneLoaded;
        Destroy(Instance.gameObject);
        
        // Set this as new instance
        Instance = this;
        mainCamera = newCamera;
        
        if (maintainCameraAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeCamera();
    }

    private void MergeCameraManagers(Camera existingCamera, Camera newCamera)
    {
        // Determine which camera to keep
        Camera cameraToKeep = DeterminePreferredCamera(existingCamera, newCamera);
        
        if (cameraToKeep == newCamera)
        {
            // New camera is better - transfer it to existing manager
            DebugLog($"Transferring new camera {newCamera.name} to existing CameraManager");
            
            // Move new camera to existing manager
            newCamera.transform.SetParent(Instance.transform);
            Instance.mainCamera = newCamera;
            
            // Destroy old camera if different
            if (existingCamera != newCamera && existingCamera != null)
            {
                Destroy(existingCamera.gameObject);
            }
            
            // Refresh existing manager
            Instance.InitializeCamera();
            
            // Destroy this manager
            Destroy(gameObject);
        }
        else
        {
            // Existing camera is better - just destroy this manager
            DebugLog($"Keeping existing camera {existingCamera.name}");
            Destroy(gameObject);
        }
    }

    private Camera DeterminePreferredCamera(Camera camera1, Camera camera2)
    {
        // Prefer camera that is tagged as MainCamera
        if (camera1.CompareTag("MainCamera") && !camera2.CompareTag("MainCamera"))
            return camera1;
        if (camera2.CompareTag("MainCamera") && !camera1.CompareTag("MainCamera"))
            return camera2;
            
        // Prefer camera that is active and enabled
        bool cam1Active = camera1.gameObject.activeInHierarchy && camera1.enabled;
        bool cam2Active = camera2.gameObject.activeInHierarchy && camera2.enabled;
        
        if (cam1Active && !cam2Active) return camera1;
        if (cam2Active && !cam1Active) return camera2;
        
        // Prefer camera from current scene (gameplay scene usually more important)
        string currentScene = SceneManager.GetActiveScene().name;
        if (camera2.gameObject.scene.name == currentScene && camera1.gameObject.scene.name != currentScene)
            return camera2;
            
        // Default to first camera
        return camera1;
    }

    private void Start()
    {
        EnsureCameraIsActive();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DebugLog($"CameraManager: Scene loaded - {scene.name}");
        
        // Ensure camera is active after scene load
        EnsureCameraIsActive();
        
        // Handle scene-specific camera settings
        HandleSceneSpecificSettings(scene.name);
    }

    private void InitializeCamera()
    {
        // Tìm main camera nếu chưa được assign
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            
            if (mainCamera == null)
            {
                // Tìm camera đầu tiên trong scene
                mainCamera = FindObjectOfType<Camera>();
            }
            
            if (mainCamera != null)
            {
                DebugLog($"CameraManager: Found camera - {mainCamera.name}");
                
                // Nếu maintain across scenes, di chuyển camera vào DontDestroyOnLoad object
                if (maintainCameraAcrossScenes && mainCamera.transform.parent != transform)
                {
                    // Đảm bảo camera không bị destroy khi load scene mới
                    if (mainCamera.transform.parent == null || mainCamera.transform.parent.gameObject.scene.name != "DontDestroyOnLoad")
                    {
                        mainCamera.transform.SetParent(transform);
                        DebugLog("CameraManager: Camera attached to DontDestroyOnLoad object");
                    }
                }
            }
            else
            {
                DebugLog("WARNING: CameraManager: No camera found in scene!");
            }
        }
    }

    private void EnsureCameraIsActive()
    {
        // Tìm lại camera nếu bị mất
        if (mainCamera == null)
        {
            InitializeCamera();
        }

        // Đảm bảo camera được kích hoạt
        if (mainCamera != null)
        {
            if (!mainCamera.gameObject.activeInHierarchy)
            {
                mainCamera.gameObject.SetActive(true);
                DebugLog("CameraManager: Camera was inactive, activated it");
            }

            if (!mainCamera.enabled)
            {
                mainCamera.enabled = true;
                DebugLog("CameraManager: Camera was disabled, enabled it");
            }
        }
        else
        {
            // Nếu vẫn không tìm thấy camera, tạo camera mới
            CreateNewCamera();
        }
    }

    private void CreateNewCamera()
    {
        DebugLog("CameraManager: Creating new camera");
        
        GameObject cameraObj = new GameObject("Main Camera");
        cameraObj.transform.SetParent(transform);
        
        mainCamera = cameraObj.AddComponent<Camera>();
        mainCamera.tag = "MainCamera";
        
        // Thêm AudioListener nếu không có
        if (FindObjectOfType<AudioListener>() == null)
        {
            cameraObj.AddComponent<AudioListener>();
        }
        
        // Set default camera settings
        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = Color.black;
        
        DebugLog("CameraManager: New camera created and configured");
    }

    private void HandleSceneSpecificSettings(string sceneName)
    {
        if (mainCamera == null) return;

        switch (sceneName.ToLower())
        {
            case "startmenu":
                // Camera settings for start menu
                mainCamera.transform.position = new Vector3(0, 0, -10);
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = 5;
                break;
                
            case "racinggameplay":
                // Camera settings for gameplay
                mainCamera.transform.position = new Vector3(0, 0, -10);
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = 5;
                break;
                
            default:
                // Default camera settings
                mainCamera.transform.position = new Vector3(0, 0, -10);
                break;
        }
        
        DebugLog($"CameraManager: Applied settings for scene {sceneName}");
    }

    public void SetCameraActive(bool active)
    {
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(active);
            DebugLog($"CameraManager: Camera set to {(active ? "active" : "inactive")}");
        }
    }

    public void EnableCamera(bool enabled)
    {
        if (mainCamera != null)
        {
            mainCamera.enabled = enabled;
            DebugLog($"CameraManager: Camera {(enabled ? "enabled" : "disabled")}");
        }
    }

    public Camera GetMainCamera()
    {
        return mainCamera;
    }

    public bool IsCameraActive()
    {
        return mainCamera != null && mainCamera.gameObject.activeInHierarchy && mainCamera.enabled;
    }

    private void DebugLog(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[CameraManager] {message}");
        }
    }

    private void OnDestroy()
    {
        // Cleanup scene loading event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #region Public Methods
    
    /// <summary>
    /// Force refresh camera state - useful for debugging
    /// </summary>
    public void RefreshCamera()
    {
        DebugLog("CameraManager: Force refreshing camera");
        EnsureCameraIsActive();
    }
    
    /// <summary>
    /// Switch to a specific camera
    /// </summary>
    public void SwitchCamera(Camera newCamera)
    {
        if (newCamera != null)
        {
            if (mainCamera != null)
            {
                mainCamera.enabled = false;
            }
            
            mainCamera = newCamera;
            mainCamera.enabled = true;
            
            DebugLog($"CameraManager: Switched to camera {newCamera.name}");
        }
    }
    
    #endregion
}