# Cross-Scene UI Management System

## Vấn đề đã giải quyết
- UIManager singleton persist qua các scene nhưng mất references đến UI panels
- Pause functionality không hoạt động khi chuyển từ StartMenu sang RacingGameplay
- UI elements không được assign tự động khi load scene mới

## Giải pháp
### 1. Auto-Detection System
UIManager giờ tự động tìm và assign UI elements khi scene mới được load:
- Sử dụng `SceneManager.sceneLoaded` event
- Tự động tìm UI elements bằng tên GameObject
- Refresh button listeners để tránh duplicate

### 2. Naming Convention System
- Tất cả UI elements phải có tên chính xác để auto-detection hoạt động
- Sử dụng `UIElementNamer` component để đảm bảo tên đúng

## Cách sử dụng

### Bước 1: Setup UIManager
1. Đặt UIManager trong StartMenu scene
2. UIManager sẽ tự động persist qua tất cả scenes
3. Không cần tạo UIManager mới trong scene khác

### Bước 2: Naming UI Elements
**Main Menu Elements:**
- MainMenuPanel
- PlayButton
- SettingsButton
- ExitButton

**Game HUD Elements:**
- GameHUDPanel
- SpeedText
- ScoreText
- HighScoreText
- FuelFill
- PauseButton

**Pause Menu Elements:**
- PauseMenuPanel
- ResumeButton
- RestartButton
- MainMenuFromPauseButton

**Settings Panel Elements:**
- SettingsPanel
- MasterVolumeSlider
- MusicVolumeSlider
- SFXVolumeSlider
- SettingsCloseButton

**Game Over Panel Elements:**
- GameOverPanel
- FinalScoreText
- PlayAgainButton
- MainMenuFromGameOverButton

### Bước 3: Sử dụng UIElementNamer
1. Add `UIElementNamer` component vào UI elements
2. Chọn đúng `ElementType` trong dropdown
3. Element sẽ tự động rename khi Start()
4. Hoặc click "Rename Element" trong context menu

### Bước 4: Testing
1. Tạo UI trong RacingGameplay scene với tên đúng
2. Chạy game từ StartMenu scene
3. Pause functionality sẽ hoạt động bình thường

## Debug Tools

### UIManager Editor
- **Refresh UI References**: Manually refresh trong Play Mode
- **Add UIElementNamer to Selected**: Add component vào selected objects
- **Rename All UI Elements**: Rename tất cả elements có UIElementNamer
- **Missing References Check**: Hiển thị số lượng references bị thiếu

### Console Logs
- UIManager sẽ log khi scene load và refresh references
- UIElementNamer sẽ log khi rename elements

## Troubleshooting

### Pause không hoạt động
1. Kiểm tra tên PauseButton, PauseMenuPanel có đúng không
2. Xem console log xem UIManager có tìm thấy elements không
3. Dùng "Refresh UI References" button trong UIManager Inspector

### UI elements không được tìm thấy
1. Đảm bảo tên GameObject chính xác (case-sensitive)
2. Elements phải là direct child của Canvas hoặc có thể tìm thấy bằng GameObject.Find()
3. Thử manually assign trong Inspector rồi test

### Button không respond
1. Kiểm tra EventSystem có trong scene không
2. Button Interactable phải = true
3. Raycast Target của Image phải = true

## Performance Notes
- Auto-detection chỉ chạy khi scene load (không impact runtime)
- GameObject.Find() chỉ chạy 1 lần per scene load
- Button listeners được clear và re-setup để tránh memory leak

## Examples

### Correct GameObject Hierarchy:
```
Canvas
├── MainMenuPanel
│   ├── PlayButton
│   ├── SettingsButton
│   └── ExitButton
├── GameHUDPanel
│   ├── SpeedText
│   ├── ScoreText
│   └── PauseButton
└── PauseMenuPanel
    ├── ResumeButton
    ├── RestartButton
    └── MainMenuFromPauseButton
```

### UIElementNamer Setup:
1. Select PlayButton
2. Add UIElementNamer component
3. Set ElementType = PlayButton
4. Element sẽ auto-rename thành "PlayButton"