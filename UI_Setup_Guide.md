# 🎮 CityDriver UI System Setup Guide

## 📋 Danh sách Scripts đã tạo:

### Core System
- ✅ `UIManager.cs` - Quản lý tất cả UI panels và transitions
- ✅ `AudioManager.cs` - Quản lý âm thanh và music
- ✅ `UIInputHandler.cs` - Xử lý input cho UI

### UI Controllers
- ✅ `MenuController.cs` - Main menu controller
- ✅ `GameHUD.cs` - In-game HUD với speedometer, score, fuel
- ✅ `PauseMenuController.cs` - Pause menu system
- ✅ `SettingsController.cs` - Settings menu với audio/graphics options

## 🎯 Setup Instructions trong Unity:

### 1. **Tạo UI Canvas Structure**

#### Main Canvas (Screen Space - Overlay):
```
Canvas
├── MainMenuPanel
│   ├── Background (sử dụng Banner từ Basic_GUI_Bundle)
│   ├── StartButton (sử dụng ButtonsText assets)
│   ├── SettingsButton
│   └── QuitButton
├── GameHUDPanel
│   ├── SpeedText
│   ├── ScoreText
│   ├── FuelSlider (sử dụng Sliders assets)
│   └── PauseButton (sử dụng ButtonsIcons)
├── PauseMenuPanel
│   ├── Background (với blur effect)
│   ├── ResumeButton
│   ├── SettingsButton
│   └── MainMenuButton
├── SettingsPanel
│   ├── TabButtons (Audio, Graphics, Controls)
│   ├── AudioPanel
│   │   ├── MasterVolumeSlider
│   │   ├── MusicVolumeSlider
│   │   └── SFXVolumeSlider
│   ├── GraphicsPanel
│   │   ├── QualityDropdown
│   │   ├── FullscreenToggle
│   │   └── ResolutionDropdown
│   └── BackButton
└── GameOverPanel
    ├── FinalScoreText
    ├── RestartButton
    └── MainMenuButton
```

### 2. **Setup GameObjects với Scripts**

#### UIManager GameObject:
1. Tạo empty GameObject tên "UIManager"
2. Attach script `UIManager.cs`
3. Assign tất cả UI panels vào inspector
4. Set DontDestroyOnLoad

#### AudioManager GameObject:
1. Tạo empty GameObject tên "AudioManager"
2. Attach script `AudioManager.cs`
3. Thêm 3 AudioSource components (Music, SFX, Voice)
4. Import audio clips và assign vào inspector

#### Input Handler:
1. Attach `UIInputHandler.cs` vào UIManager GameObject
2. Assign InputActionAsset reference

### 3. **UI Assets Integration**

#### Sử dụng Basic_GUI_Bundle:
- **Buttons**: `ButtonsText/` và `ButtonsIcons/` cho các nút
- **Backgrounds**: `BoxesBanners/` cho panel backgrounds
- **Sliders**: `Sliders/` cho volume controls và fuel bar
- **Icons**: `Icons/` cho các biểu tượng UI

#### Button Setup Example:
1. Tạo Button từ UI -> Button
2. Replace background Image với assets từ `ButtonsText/`
3. Set Image Type = Sliced cho scaling
4. Configure colors: Normal, Highlighted, Pressed, Disabled

### 4. **Scene Setup**

#### StartMenu Scene:
1. Tạo Canvas với MainMenuPanel active
2. Attach `MenuController.cs` vào một GameObject
3. Link buttons với UIManager

#### RacingGameplay Scene:
1. Tạo Canvas với GameHUDPanel active
2. Attach `GameHUD.cs` vào HUD GameObject
3. Setup speedometer với needle rotation
4. Configure fuel warning system

### 5. **Input System Integration**

#### Thêm Pause Action vào InputSystem_Actions.inputactions:
```json
{
  "name": "Pause",
  "type": "Button",
  "id": "new-guid-here",
  "expectedControlType": "Button",
  "bindings": [
    {
      "path": "<Keyboard>/escape",
      "groups": "Keyboard&Mouse"
    },
    {
      "path": "<Gamepad>/start",
      "groups": "Gamepad"
    }
  ]
}
```

### 6. **Animation Setup (Optional)**

#### UI Animations:
1. Tạo Animator Controllers cho UI panels
2. Setup transitions: Show, Hide, Pulse effects
3. Assign Animators vào panel GameObjects

#### Button Hover Effects:
- Scale animation khi hover
- Color transitions
- Sound effects

### 7. **Audio Setup**

#### Audio Clips cần thiết:
- **UI Sounds**: button click, hover, transition
- **Game Music**: menu theme, gameplay theme
- **SFX**: car engine, horn, score pickup

#### Audio Mixer (Optional):
1. Tạo Audio Mixer Asset
2. Setup groups: Master, Music, SFX
3. Configure volume controls trong Settings

## 🔧 Testing Checklist:

- [ ] Main Menu buttons hoạt động
- [ ] Scene transitions smooth
- [ ] Game HUD hiển thị đúng speed/score
- [ ] Pause menu xuất hiện khi nhấn ESC
- [ ] Settings menu điều chỉnh được volume
- [ ] Audio Manager phát music/SFX
- [ ] UI responsive trên các resolution
- [ ] Input System hoạt động với keyboard/gamepad

## 🚨 TROUBLESHOOTING - UI Size Issues:

### Vấn đề: StartMenu quá nhỏ, Canvas quá to

#### Giải pháp 1: Kiểm tra Game View Resolution
```
1. Trong Unity, click tab "Game"
2. Dropdown ở góc trên trái Game view
3. Chọn resolution phù hợp (1920x1080 hoặc Free Aspect)
4. Kiểm tra "Low Resolution Aspect Ratios" nếu có
```

#### Giải pháp 2: Điều chỉnh Canvas Scaler
```
Canvas Scaler settings khuyến nghị:
- UI Scale Mode: Scale With Screen Size
- Reference Resolution: 1280x720 (thay vì 1920x1080)
- Screen Match Mode: Match Width Or Height  
- Match: 0.0 (ưu tiên Width scaling)
```

#### Giải pháp 3: Kiểm tra RectTransform
```
MainMenuPanel RectTransform:
- Anchor Presets: Click "Alt + Stretch" (fill cả Canvas)
- Left: 0, Top: 0, Right: 0, Bottom: 0
- Scale: (1, 1, 1)
```

#### Giải pháp 4: Font Size và Button Size
```
GameTitle Text:
- Font Size: 48 (thay vì 72 nếu vẫn nhỏ)
- RectTransform Width: 600, Height: 100

Buttons:
- Preferred Height: 60 (thay vì 80)
- Font Size: 24 (thay vì 32)
```

#### Giải pháp 5: Sử dụng UIScaleFixer Script (Khuyên dùng)
```
1. Attach script UIScaleFixer.cs vào Canvas GameObject
2. Trong Inspector:
   - Auto Fix On Start: ✓
   - Reference Resolution: 1280x720
   - Match Width Or Height: 0.0
3. Click "Fix Canvas Scaling" button trong Inspector
4. Hoặc click "Auto Detect Best Settings" để tự động
```

#### Giải pháp 6: Manual Canvas Settings
```
Canvas Component:
- Render Mode: Screen Space - Overlay
- Pixel Perfect: ✓ (checked)
- Sort Order: 0

Canvas Scaler Component:
- UI Scale Mode: Scale With Screen Size
- Reference Resolution: 1280x720 (KHÔNG phải 1920x1080)
- Screen Match Mode: Match Width Or Height
- Match: 0.0 (ưu tiên Width)
```

#### Kiểm tra Game View
```
1. Window → General → Game
2. Aspect ratio dropdown → Free Aspect
3. Hoặc chọn 16:9 (1920x1080)
4. Scale slider ở 1x
```

## 🛠️ Quick Debug Steps:

### Bước 1: Kiểm tra Canvas Scale Factor
```
1. Select Canvas trong Hierarchy
2. Trong Inspector, xem Canvas component
3. Scale Factor phải là 1.0 hoặc gần 1.0
4. Nếu quá nhỏ (< 0.5) → giảm Reference Resolution
5. Nếu quá lớn (> 2.0) → tăng Reference Resolution
```

### Bước 2: Test với UIScaleFixer
```
1. Attach UIScaleFixer.cs vào Canvas
2. Click "Debug Canvas Info" → xem Console log
3. Click "Auto Detect Best Settings"
4. Click "Fix All UI Elements Size"
```

### Bước 3: Common Fix Values
```
Cho màn hình 1920x1080:
- Reference Resolution: 1280x720
- Match: 0.0

Cho màn hình 1366x768:
- Reference Resolution: 1024x576  
- Match: 0.0

Cho màn hình Ultrawide:
- Reference Resolution: 1920x1080
- Match: 0.5
```

## 🚀 Next Steps:

1. **Polish Visual**: Thêm animations, particle effects
2. **Mobile Support**: Touch controls, responsive UI
3. **Localization**: Multi-language support
4. **Accessibility**: Colorblind support, font scaling
5. **Analytics**: Track user interactions

## 📁 File Organization:

```
Assets/Script/
├── Managers/
│   ├── UIManager.cs
│   ├── AudioManager.cs
│   └── GameManager.cs (đã có)
└── UI/
    ├── MenuController.cs
    ├── GameHUD.cs
    ├── PauseMenuController.cs
    ├── SettingsController.cs
    └── UIInputHandler.cs
```

## 🎨 UI Design Tips:

1. **Consistency**: Sử dụng cùng font, colors, spacing
2. **Hierarchy**: Size và contrast để tạo visual hierarchy
3. **Feedback**: Visual/audio feedback cho mọi interaction
4. **Accessibility**: Readable fonts, sufficient contrast
5. **Performance**: Pool UI elements, optimize graphics

---

**Chúc bạn setup thành công! 🎉**