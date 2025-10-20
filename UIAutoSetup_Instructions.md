# 🚀 Hướng dẫn sử dụng UIAutoSetup

## 🎯 Tự động tạo toàn bộ UI cho CityDriver

Script `UIAutoSetupComplete.cs` sẽ tự động tạo tất cả UI elements cho game của bạn!

### ⚡ Cách sử dụng nhanh:

#### Bước 1: Setup Script
```
1. Tạo empty GameObject trong scene
2. Đổi tên thành "UIAutoSetup"
3. Attach script UIAutoSetupComplete.cs
4. (Optional) Assign UI sprites từ Basic_GUI_Bundle vào inspector
```

#### Bước 2: Chạy Auto Setup
```
1. Select UIAutoSetup GameObject
2. Trong Inspector, click "Auto Setup All UI"
3. Chờ script tạo tất cả UI elements
4. Kiểm tra Console log để xem tiến trình
```

### 📋 Những gì script sẽ tạo:

#### ✅ Main Menu Panel:
- Game title "CITY DRIVER"
- Start Game button
- Settings button  
- Quit button
- Version text

#### ✅ Game HUD Panel:
- Score display (top left)
- Pause button (top right)
- Speedometer (bottom left) với needle
- Fuel panel với slider (bottom right)
- Low fuel warning

#### ✅ Pause Menu Panel:
- Semi-transparent background
- Resume button
- Settings button
- Main Menu button

#### ✅ Settings Panel:
- Master Volume slider
- Music Volume slider
- SFX Volume slider
- Back button

#### ✅ Game Over Panel:
- Game Over title
- Final Score display
- Restart button
- Main Menu button

#### ✅ Technical Setup:
- Canvas với optimal scaler settings (1280x720)
- Event System cho input
- Proper anchoring và responsive layout
- UIManager GameObject ready

### 🎨 Customization:

#### UI Assets (Optional):
```csharp
[Header("UI Assets")]
public Sprite buttonBackground;    // Assign từ ButtonsText/
public Sprite panelBackground;     // Assign từ BoxesBanners/  
public Sprite sliderBackground;    // Assign từ Sliders/
public Sprite sliderFill;         // Assign từ Sliders/
public Sprite sliderHandle;       // Assign từ Sliders/
```

#### Configuration:
```csharp
[Header("Auto Setup Configuration")]
public bool setupMainMenu = true;     // Tạo Main Menu
public bool setupGameHUD = true;      // Tạo Game HUD
public bool setupPauseMenu = true;    // Tạo Pause Menu
public bool setupSettings = true;     // Tạo Settings Menu
public bool setupGameOver = true;     // Tạo Game Over Screen
```

### 🔧 Sau khi chạy Auto Setup:

#### Bước 1: Attach Controller Scripts
```
1. UIManager GameObject → attach UIManager.cs
2. GameHUDPanel → attach GameHUD.cs (optional)
3. MainMenuPanel → attach MenuController.cs (optional)
4. PauseMenuPanel → attach PauseMenuController.cs (optional)
5. SettingsPanel → attach SettingsController.cs (optional)
```

#### Bước 2: Assign Panel References
```
1. Select UIManager GameObject
2. Trong UIManager script inspector:
   - Main Menu Panel: [Drag MainMenuPanel]
   - Game HUD Panel: [Drag GameHUDPanel]
   - Pause Menu Panel: [Drag PauseMenuPanel]
   - Settings Panel: [Drag SettingsPanel]
   - Game Over Panel: [Drag GameOverPanel]
```

#### Bước 3: Setup Button Events
```
1. Select các buttons (StartButton, PauseButton, etc.)
2. Trong Button component → On Click():
   - Add UIManager.StartGame(), UIManager.PauseGame(), etc.
```

#### Bước 4: Apply UI Assets
```
1. Select các Image components
2. Assign sprites từ Basic_GUI_Bundle:
   - Buttons → ButtonsText/ hoặc ButtonsIcons/
   - Panels → BoxesBanners/ 
   - Sliders → Sliders/
```

### 📱 Responsive Design:

Script tự động setup:
- ✅ Canvas Scaler optimized (1280x720)
- ✅ Anchor presets cho responsive layout
- ✅ Layout Groups cho auto-arrangement
- ✅ Proper text scaling
- ✅ Button size consistency

### 🎮 Input Integration:

Sau setup, add Input handling:
```csharp
// Trong Update() của UIManager hoặc GameManager:
if (Input.GetKeyDown(KeyCode.Escape))
{
    if (UIManager.Instance.IsPaused)
        UIManager.Instance.ResumeGame();
    else
        UIManager.Instance.PauseGame();
}
```

### 🔊 Audio Integration:

```csharp
// Setup AudioManager
1. Tạo AudioManager GameObject  
2. Attach AudioManager.cs script
3. Assign audio clips cho UI sounds
4. Connect với UIManager for sound effects
```

### ✅ Testing Checklist:

- [ ] All panels được tạo correctly
- [ ] Buttons có proper text và alignment
- [ ] Responsive trên different resolutions  
- [ ] Canvas scaling optimal
- [ ] No console errors
- [ ] UI assets assigned properly
- [ ] Controller scripts attached
- [ ] Button events working
- [ ] Scene transitions smooth

### 🚨 Troubleshooting:

#### UI quá nhỏ:
- Kiểm tra Canvas Scaler settings
- Reference Resolution nên là 1280x720
- Match Width Or Height = 0.0

#### Missing sprites:
- Import Basic_GUI_Bundle assets
- Assign vào script inspector before running
- Hoặc assign manually sau khi setup

#### Button events không hoạt động:
- Kiểm tra EventSystem exists
- Assign button onClick events manually
- Ensure UIManager script attached

---

## 🎉 Kết quả:

Sau khi chạy script, bạn sẽ có **complete UI system** với:
- ✅ Professional-looking menus
- ✅ Responsive design
- ✅ Proper navigation flow  
- ✅ Game HUD với speedometer
- ✅ Settings với volume controls
- ✅ Ready for customization

**Chỉ cần 1 click để có complete UI system! 🚀**