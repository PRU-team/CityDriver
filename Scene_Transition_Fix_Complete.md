# Scene Transition Fix - Giải pháp hoàn chỉnh

## 🎯 Vấn đề đã xác định
- ✅ Start scene: MainMenuPanel hiển thị OK
- ❌ Chuyển sang gameplay scene: MainMenuPanel bị ẩn
- ❌ UIManager không biết hiển thị panel nào cho scene nào

## ✅ Giải pháp đã implemented

### 1. Cập nhật SimpleUIManager
- ✅ Thêm `InitializeUIForCurrentScene()` method
- ✅ Auto-detect scene và hiển thị UI tương ứng
- ✅ Start scene → ShowMainMenu()
- ✅ Gameplay scene → ShowGameHUD()

### 2. Logic mới trong SimpleUIManager:
```csharp
private void InitializeUIForCurrentScene()
{
    string currentScene = SceneManager.GetActiveScene().name;
    
    if (currentScene.Contains("menu") || currentScene.Contains("start"))
    {
        ShowMainMenu();  // Hiện MainMenuPanel
    }
    else if (currentScene.Contains("game") || currentScene.Contains("racing"))
    {
        ShowGameHUD();   // Ẩn MainMenuPanel, hiện GameHUD
    }
}
```

### 3. Tích hợp vào scene loading:
- ✅ Gọi `InitializeUIForCurrentScene()` trong `Start()`
- ✅ Gọi `InitializeUIForCurrentScene()` trong `DelayedUIRefresh()`
- ✅ Tự động setup UI state mỗi khi load scene

## 🛠 Tools hỗ trợ testing

### SceneTransitionTester.cs
```
Tools → CityDriver → Test Scene Transition
Tools → CityDriver → Force UI State Reset  
Tools → CityDriver → Debug UI State
```

## 🚀 Test ngay

### Bước 1: Test trong Editor
```
Tools → CityDriver → Force UI State Reset
```

### Bước 2: Test trong Play mode
1. Bấm Play ở Start scene
2. Click Play button để chuyển sang Gameplay
3. MainMenuPanel sẽ tự ẩn đi ở Gameplay scene

### Bước 3: Verify
```
Tools → CityDriver → Debug UI State
```

## 🔍 Expected Results

### Start Menu Scene:
- ✅ MainMenuPanel: Visible
- ✅ GameHUDPanel: Hidden  
- ✅ isInGame: false

### Gameplay Scene:
- ✅ MainMenuPanel: Hidden
- ✅ GameHUDPanel: Visible (nếu có)
- ✅ isInGame: true

## 🐛 Nếu vẫn có vấn đề

### Quick Fixes:
1. `Tools → CityDriver → Force UI State Reset`
2. `Tools → CityDriver → Test Scene Transition`
3. Check Console logs để xem InitializeUIForCurrentScene() có chạy không

### Debug Commands:
```csharp
// Check in Console:
SimpleUIManager.Instance.ShowMainMenu();  // Force show main menu
SimpleUIManager.Instance.ShowGameHUD();   // Force show game HUD
```

---

**Kết quả:** MainMenuPanel sẽ tự động ẩn khi chuyển sang Gameplay scene! 🎮