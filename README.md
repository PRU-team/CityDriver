# 🚗 CityDriver - Unity 2D Endless Runner Game

![Unity Version](https://img.shields.io/badge/Unity-2025.1+-blue.svg)
![Input System](https://img.shields.io/badge/Input%20System-New-green.svg)
![C#](https://img.shields.io/badge/C%23-Script-purple.svg)

## 📝 Mô tả dự án

**CityDriver** là một game 2D endless runner được phát triển trên Unity, nơi người chơi điều khiển một chiếc xe trong thành phố với những thách thức và collectibles không ngừng nghỉ.

### 🎯 Tính năng chính

- **🚗 Điều khiển xe**: Di chuyển trái/phải, tăng/giảm tốc độ
- **❤️ Hệ thống sức khỏe**: 3 trái tim, mất tim khi va chạm với ch장애vật
- **🪙 Thu thập coin**: Nhặt coin để tăng điểm số
- **🛡️ Shield system**: Tạm thời bảo vệ khỏi va chạm
- **⏸️ Pause menu**: Tạm dừng game với đầy đủ UI controls
- **📊 Score system**: Theo dõi điểm số và tốc độ thời gian thực
- **🎵 Audio management**: Âm thanh và nhạc nền
- **📱 UI hoàn chỉnh**: Main menu, game HUD, settings, game over

## 🎮 Cách chơi

### Điều khiển cơ bản
- **A / ←**: Di chuyển trái
- **D / →**: Di chuyển phải  
- **W**: Tăng tốc độ game
- **S**: Giảm tốc độ game
- **ESC**: Pause/Resume game

### Gameplay
1. **Tránh chướng ngại vật**: Va chạm sẽ làm mất 1 trái tim ❤️
2. **Thu thập coin**: Nhặt coin vàng để tăng điểm số 🪙
3. **Sử dụng shield**: Items đặc biệt tạo lá chắn bảo vệ 🛡️
4. **Sinh tồn**: Cố gắng sống sót càng lâu càng tốt!

## 🎯 Cheat Codes

### Kích hoạt God Mode
```
Nhập liên tiếp: W-W-S-S
```
**Hiệu ứng**: 
- 🛡️ Miễn dịch hoàn toàn với mọi va chạm
- ♾️ Không mất trái tim khi đâm vào chướng ngại vật
- 🌟 Vẫn có thể thu thập coins bình thường

### Developer Controls (Debug Mode)
- **F1**: Toggle debug info
- **F2**: Add 1 heart
- **F3**: Add 100 coins
- **F4**: Activate shield (5 seconds)

## 🏗️ Cấu trúc dự án

### 📁 Thư mục chính

```
Assets/
├── Scenes/
│   ├── StartMenu.unity          # Main menu scene
│   └── RacingGameplay.unity     # Gameplay scene chính
├── Script/
│   ├── Managers/                # Game management systems
│   │   ├── GameManager.cs       # Core game logic
│   │   ├── UIManager.cs         # UI management
│   │   ├── AudioManager.cs      # Audio system
│   │   └── CameraManager.cs     # Camera controls
│   ├── Player/
│   │   └── CarController.cs     # Player movement
│   ├── Obstacle/
│   │   └── Obstacle.cs          # Obstacle behaviors
│   ├── Item/
│   │   └── Item.cs              # Collectible items
│   ├── Spawner/
│   │   └── RoadObjectSpawner.cs # Object spawning system
│   └── Tilemap/
│       └── TileMapScrolling.cs  # Background scrolling
├── Prefab/                      # Game prefabs
├── UI/                          # UI assets và components
├── Material/                    # Materials và textures
└── Characters/                  # Character sprites
```

### 🎛️ Core Systems

#### GameManager
- **Singleton pattern**: Quản lý trạng thái game toàn cục
- **Health system**: Quản lý trái tim và game over
- **Score tracking**: Tính điểm dựa trên thời gian và tốc độ
- **Shield mechanics**: Hệ thống bảo vệ tạm thời
- **Cheat detection**: Nhận dạng cheat codes

#### UIManager  
- **DontDestroyOnLoad**: Persistent UI across scenes
- **Panel management**: Main menu, pause, settings, game over
- **Auto-reference**: Tự động tìm và gán UI elements
- **Input System**: Hỗ trợ ESC để pause/resume

#### CarController
- **Input System**: Sử dụng Unity's New Input System
- **Smooth movement**: Di chuyển mượt mà với giới hạn
- **Speed control**: Điều chỉnh tốc độ game real-time

## 🔧 Technical Features

### Input System
- ✅ **Unity Input System Package**: Modern input handling
- ✅ **Keyboard support**: WASD + Arrow keys
- ✅ **Conditional compilation**: Hỗ trợ multiple input backends

### Architecture Patterns
- 🏗️ **Singleton Pattern**: GameManager, UIManager
- 🔄 **Observer Pattern**: UI updates from game events
- 📦 **Component-based**: Modular script organization

### Performance Optimization
- ⚡ **Object Pooling**: Efficient obstacle/item spawning
- 🎯 **Auto-reference**: Tự động tìm references để giảm setup manual
- 🔄 **DontDestroyOnLoad**: Tránh recreate managers

## 🛠️ Development Tools

### Editor Scripts
```
Assets/Script/Editor/
├── UIManagerEditor.cs           # Custom UI Manager inspector
├── CameraSystemSetup.cs         # Camera auto-setup
├── SceneTransitionTester.cs     # Scene testing tools
└── PauseMenuPanelFix.cs         # UI debugging tools
```

### Debugging Features
- 🔍 **Comprehensive logging**: Debug.Log trong tất cả systems
- 🎛️ **Inspector tweaking**: Public fields để tuning real-time
- 🧪 **Scene transition testing**: Tools để test chuyển scene
- 🎯 **Auto-fix tools**: Scripts tự động sửa common issues

## 📋 Setup Instructions

### Prerequisites
- **Unity 2025.1+** 
- **Input System Package** (đã được import)
- **TextMesh Pro** (đã được setup)

### Cài đặt
1. Clone repository:
   ```bash
   git clone https://github.com/PRU-team/CityDriver.git
   ```

2. Mở project trong Unity Hub

3. Chọn scene **StartMenu** để bắt đầu

4. Press **Play** để test game!

### Build Settings
- **Platform**: PC, Mac & Linux Standalone
- **Scenes**: 
  - `Assets/Scenes/StartMenu.unity` (Index: 0)
  - `Assets/Scenes/RacingGameplay.unity` (Index: 1)

## 🎨 Art Assets

### Sprites và Textures
- **Car sprites**: Player character variations
- **Obstacle sprites**: Various urban obstacles  
- **Background tiles**: Seamless city environment
- **UI elements**: Complete UI kit với buttons, panels
- **Collectibles**: Coins, hearts, shields

### Audio Assets
- **Background music**: Looping city ambience
- **SFX**: Collision, collect, button sounds
- **Engine sounds**: Car movement audio

## 🐛 Known Issues & Solutions

### Input System Conflicts
**Problem**: `InvalidOperationException` khi sử dụng legacy Input
**Solution**: ✅ Đã fix - chỉ sử dụng Input System package

### UI References Missing  
**Problem**: UI elements không được assign
**Solution**: ✅ Auto-reference system trong UIManager

### Camera Conflicts
**Problem**: Multiple cameras gây rendering issues
**Solution**: ✅ CameraManager với proper singleton pattern

## 🚀 Future Enhancements

### Planned Features
- [ ] **Power-ups**: Speed boost, magnet, double coins
- [ ] **Multiple car types**: Different stats và abilities  
- [ ] **Daily challenges**: Specific objectives
- [ ] **Leaderboard**: High score tracking
- [ ] **Mobile support**: Touch controls
- [ ] **Particle effects**: Enhanced visual feedback

### Technical Improvements
- [ ] **Save system**: Progress persistence
- [ ] **Settings menu**: Volume, graphics options
- [ ] **Analytics**: Player behavior tracking
- [ ] **Localization**: Multi-language support

## 👥 Team

**PRU-team** - Unity Game Development

## 📄 License

This project is developed for educational purposes.

---

## 🎮 Quick Start Guide

1. **Run the game**: Open Unity → Select StartMenu scene → Press Play
2. **Start playing**: Click "Play" button in main menu
3. **Basic controls**: Use WASD or Arrow keys to move
4. **Pause anytime**: Press ESC during gameplay
5. **Try cheat codes**: Input W-W-S-S for God Mode! 😈

**Have fun playing CityDriver! 🚗💨**