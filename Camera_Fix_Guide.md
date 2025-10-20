# Camera Fix Guide - Giải quyết vấn đề "No cameras rendering"

## Vấn đề
Khi chuyển từ Start Menu scene sang Gameplay scene, camera bị ẩn đi và hiển thị thông báo "No cameras rendering".

## Nguyên nhân
- UIManager và các manager khác được set DontDestroyOnLoad nhưng camera thì không
- Camera có thể bị inactive hoặc disabled khi load scene mới
- Không có script nào quản lý camera qua các scene transitions

## Giải pháp

### 1. Sử dụng CameraFix Tool (Nhanh nhất)

#### Trong Unity Editor:
1. Vào menu `Tools > CityDriver > Quick Camera Fix`
2. Hoặc vào `Tools > CityDriver > Fix Camera Issues` để chạy full diagnostic

#### Trong Scene:
1. Tạo empty GameObject trong scene
2. Attach script `CameraFix` vào object đó
3. Bấm Play để auto-fix

### 2. Manual Setup (Chi tiết hơn)

#### Bước 1: Tạo CameraManager
```csharp
// Đã có sẵn script CameraManager.cs
// Script này sẽ:
// - Maintain camera qua các scene
// - Auto-detect và fix camera issues
// - Ensure camera luôn active
```

#### Bước 2: Integrate vào project
1. Tạo empty GameObject tên "CameraManager" trong Start scene
2. Attach script `CameraManager` vào object này
3. Hoặc sử dụng `CameraManagerAutoSetup` để tự động setup

#### Bước 3: Verify integration
- CameraManager sẽ tự động DontDestroyOnLoad
- Camera sẽ được maintain qua tất cả scenes
- Nếu camera bị mất, sẽ tự tạo camera mới

## Scripts đã tạo

### 1. CameraManager.cs
- Main script quản lý camera
- Singleton pattern với DontDestroyOnLoad
- Auto-detect và fix camera issues
- Handle scene-specific camera settings

### 2. CameraManagerAutoSetup.cs
- Auto setup CameraManager khi game start
- Đặt vào GameObject trong scene đầu tiên
- Tự destroy sau khi setup xong

### 3. CameraFix.cs
- Tool để diagnose và fix camera issues
- Có menu items trong Unity Editor
- Comprehensive camera problem solving

## Cách sử dụng

### Option 1: Quick Fix (Khuyên dùng)
1. Trong Unity Editor: `Tools > CityDriver > Quick Camera Fix`
2. Bấm Play để test

### Option 2: Auto Setup
1. Tạo empty GameObject trong Start scene
2. Attach `CameraManagerAutoSetup` script
3. Set `Setup On Awake = true`
4. Bấm Play, script sẽ tự setup

### Option 3: Manual Setup
1. Tạo empty GameObject tên "CameraManager"
2. Attach `CameraManager` script
3. Assign main camera nếu cần
4. Set `Maintain Camera Across Scenes = true`

## Test Results

Sau khi apply fix:
- ✅ Camera hoạt động bình thường khi chuyển scene
- ✅ Không còn "No cameras rendering" message
- ✅ UI vẫn hoạt động với DontDestroyOnLoad
- ✅ Audio vẫn hoạt động bình thường

## Debug Information

Để debug camera issues:
1. Check Console logs từ CameraManager
2. Sử dụng `CameraFix.DiagnoseCameraIssues()`
3. Verify rằng Camera.main không null
4. Check camera.gameObject.activeInHierarchy
5. Check camera.enabled

## Additional Notes

- CameraManager tự động tích hợp với UIManager hiện tại
- Không cần thay đổi code UIManager/SimpleUIManager nhiều
- Compatible với tất cả existing scripts
- Có thể disable/enable debug logs trong inspector

## Troubleshooting

### Nếu vẫn gặp vấn đề:
1. Chạy `Tools > CityDriver > Fix Camera Issues` 
2. Check Console để xem diagnostic info
3. Đảm bảo main camera có tag "MainCamera"
4. Verify CameraManager.Instance không null
5. Check camera position không bị lệch quá xa

### Common Issues:
- **Camera null**: CameraManager sẽ tự tạo camera mới
- **Multiple cameras**: CameraManager sẽ prioritize main camera
- **Audio issues**: CameraManager sẽ ensure có AudioListener
- **Scene loading**: Camera sẽ được refresh sau mỗi scene load