using UnityEngine;

/// <summary>
/// Auto setup CameraManager khi game bắt đầu
/// Đặt script này vào một GameObject trong scene đầu tiên
/// </summary>
public class CameraManagerAutoSetup : MonoBehaviour
{
    [Header("Setup Settings")]
    [SerializeField] private bool setupOnAwake = true;
    [SerializeField] private bool destroyAfterSetup = true;
    
    private void Awake()
    {
        if (setupOnAwake)
        {
            SetupCameraManager();
        }
    }

    private void SetupCameraManager()
    {
        // Kiểm tra xem đã có CameraManager chưa
        if (CameraManager.Instance == null)
        {
            // Tạo GameObject mới cho CameraManager
            GameObject cameraManagerObj = new GameObject("CameraManager");
            
            // Thêm CameraManager component
            CameraManager cameraManager = cameraManagerObj.AddComponent<CameraManager>();
            
            Debug.Log("CameraManagerAutoSetup: CameraManager created successfully");
            
            // Tìm và assign main camera hiện tại
            Camera existingCamera = Camera.main;
            if (existingCamera == null)
            {
                existingCamera = FindObjectOfType<Camera>();
            }
            
            if (existingCamera != null)
            {
                Debug.Log($"CameraManagerAutoSetup: Found existing camera: {existingCamera.name}");
            }
        }
        else
        {
            Debug.Log("CameraManagerAutoSetup: CameraManager already exists");
        }
        
        // Destroy object này sau khi setup xong
        if (destroyAfterSetup)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Manual setup method - có thể gọi từ editor hoặc script khác
    /// </summary>
    [ContextMenu("Setup Camera Manager")]
    public void ManualSetup()
    {
        SetupCameraManager();
    }
}