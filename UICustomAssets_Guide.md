# UI Custom Assets Guide - CityDriver

## Tổng quan
UIQuickFix giờ đây hỗ trợ tùy chọn assets cho buttons, icons và UI elements. Bạn có thể:
- Sử dụng sprites tùy chỉnh cho buttons và panels
- Thêm icons cho các buttons
- Sử dụng fonts tùy chỉnh
- Tự động load assets từ Resources hoặc Basic GUI Bundle

## Cách sử dụng Assets

### 1. Qua Unity Inspector
1. Tạo một GameObject với UIQuickFix component
2. Kéo thả các sprites vào các trường asset tương ứng:
   - **Button Assets**: buttonSprite, buttonHoverSprite, buttonPressedSprite
   - **Panel Assets**: panelSprite
   - **Font**: customFont
   - **Button Icons**: playButtonIcon, pauseButtonIcon, settingsButtonIcon, etc.
   - **HUD Icons**: speedIcon, scoreIcon, fuelIcon

### 2. Qua Editor Window
1. Mở `Tools → UI Quick Fix`
2. Mở section "Custom Assets (Optional)"
3. Assign các sprites và fonts mong muốn
4. Click "Quick Fix Missing UI Elements"

### 3. Tự động Load Assets

#### Từ Resources Folder
Tạo cấu trúc thư mục trong `Assets/Resources/`:
```
Resources/
├── UI/
│   ├── button.png
│   ├── button_hover.png
│   ├── button_pressed.png
│   ├── panel.png
│   └── Icons/
│       ├── play_icon.png
│       ├── pause_icon.png
│       ├── settings_icon.png
│       ├── exit_icon.png
│       ├── resume_icon.png
│       ├── restart_icon.png
│       ├── menu_icon.png
│       ├── speed_icon.png
│       ├── score_icon.png
│       └── fuel_icon.png
└── Fonts/
    └── UIFont.ttf
```

#### Từ Basic GUI Bundle
UIQuickFix sẽ tự động tìm kiếm assets trong Basic_GUI_Bundle:
- Buttons: `Basic_GUI_Bundle/Buttons/button_01`
- Panels: `Basic_GUI_Bundle/Boxes/box_01`
- Icons: `Basic_GUI_Bundle/Icons/icon_XX`

## Asset Types và Quy tắc

### Button Assets
- **buttonSprite**: Trạng thái bình thường của button
- **buttonHoverSprite**: Khi hover mouse (tùy chọn)
- **buttonPressedSprite**: Khi nhấn button (tùy chọn)
- Nếu có sprite, sẽ dùng Sprite Swap transition
- Nếu không có sprite, sẽ dùng Color Tint transition

### Panel Assets
- **panelSprite**: Background cho các panels
- Nên sử dụng 9-slice sprites cho scaling tốt
- Sẽ tự động set type = Sliced

### Icon Assets
- Tất cả icons sẽ được tint theo textColor
- Kích thước tự động điều chỉnh theo kích thước button
- Position tự động để tạo space cho text

### Font Assets
- **customFont**: Font tùy chỉnh cho tất cả UI text
- Fallback về LegacyRuntime.ttf nếu không có

## Context Menu Commands

### Trong UIQuickFix Component:
- **Create Missing UI Elements**: Tạo UI với assets hiện tại
- **Load Assets from Resources**: Tự động load từ Resources folder
- **Load Assets from Basic GUI Bundle**: Load từ Basic GUI Bundle
- **Preview Asset Status**: Hiển thị trạng thái assets trong Console
- **Remove All Quick Fix UI**: Xóa tất cả UI đã tạo
- **Reset and Recreate UI**: Xóa và tạo lại UI

## Examples

### Ví dụ 1: Sử dụng Basic GUI Bundle
```csharp
// UIQuickFix sẽ tự động load assets từ Basic_GUI_Bundle khi Start()
// Không cần làm gì thêm, chỉ cần có Basic_GUI_Bundle trong Resources
```

### Ví dụ 2: Custom Icons
```csharp
UIQuickFix quickFix = FindObjectOfType<UIQuickFix>();
quickFix.playButtonIcon = yourPlayIcon;
quickFix.pauseButtonIcon = yourPauseIcon;
quickFix.CreateMissingUIElements();
```

### Ví dụ 3: Runtime Asset Loading
```csharp
UIQuickFix quickFix = GetComponent<UIQuickFix>();
quickFix.LoadAssetsFromResources();
quickFix.CreateMissingUIElements();
```

## Best Practices

### 1. Asset Organization
- Sử dụng naming conventions rõ ràng
- Nhóm assets theo function (buttons, icons, panels)
- Sử dụng 9-slice cho scalable UI elements

### 2. Performance
- Optimize sprites trước khi import
- Sử dụng sprite atlases cho nhiều small icons
- Load assets một lần và reuse

### 3. Consistency
- Sử dụng cùng style cho tất cả buttons
- Maintain consistent icon sizes
- Sử dụng cùng color palette

## Troubleshooting

### Assets không load được:
1. Check đường dẫn trong Resources folder
2. Đảm bảo sprites đã được import đúng
3. Sử dụng "Preview Asset Status" để debug

### UI không hiển thị đúng:
1. Check Canvas và CanvasScaler settings
2. Đảm bảo sprites có đúng import settings
9. Verify UI elements không bị overlap

### Performance issues:
1. Optimize sprite sizes
2. Sử dụng sprite compression
3. Consider object pooling cho dynamic UI

## Integration với UIManager

UIQuickFix tạo ra UI elements với naming conventions mà UIManager có thể tự động detect:
- All buttons được setup với proper names
- Cross-scene persistence được maintain
- Auto-detection system hoạt động seamlessly

Tất cả UI elements được tạo sẽ tự động tương thích với UIManager và SimpleUIManager.