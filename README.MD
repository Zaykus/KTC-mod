# Kingdom Enhanced (v2.1.0)

![Mod Version](https://img.shields.io/badge/version-2.1.0-green)
![Game Version](https://img.shields.io/badge/Kingdom%20Two%20Crowns-2.4.x-blue)
![Loader](https://img.shields.io/badge/BepInEx-6_(Mono_+_IL2CPP)-orange)

**Kingdom Enhanced** is a comprehensive Quality of Life, Cheat, and Accessibility modification suite for *Kingdom Two Crowns*.  
It grants players granular control over their kingdom’s army, economy, and world systems, while exposing critical gameplay data through a modern, native-style interface.

---

## Universal Compatibility Update (v2.1.0)

This update focuses on **eliminating compatibility limitations** and ensuring the mod works across all environments.

### 🌐 Dual Backend Support (Mono + IL2CPP)
- Fully supports both **Mono** and **IL2CPP**
- Compatible with all current game distributions
- Internally restructured for seamless backend handling

### 💻 Cross-Platform Support
Pre-configured builds are now available for:
- Windows (**x64 / x86**)
- Linux (**x64 / x86**)

### 📦 Ready-to-Play Packages
- Bundled with **BepInEx 6 (latest)**
- Includes required system libraries (e.g. TTS support on Windows)
- No manual configuration required

> **Install process:** Download → Extract → Launch

---

## ❓ Which Version Should I Download?

### Step 1 — Identify Backend
Check your game folder:

| If you see | Download |
|------------|----------|
| `KingdomTwoCrowns_Data/MonoBleedingEdge` | ✅ Mono version |
| `KingdomTwoCrowns_Data/il2cpp_data` | ✅ IL2CPP version |

### Step 2 — Identify Architecture

| Condition | Download |
|----------|----------|
| `UnityCrashHandler32.exe` exists | ✅ x86 |
| Not present | ✅ x64 |

---

<div align="center">
  <a href="#mod-overview--features"><b>Mod Overview</b></a> &nbsp;&bull;&nbsp;
  <a href="#installation-guide"><b>Installation</b></a> &nbsp;&bull;&nbsp;
  <a href="#controls-reference"><b>Controls</b></a> &nbsp;&bull;&nbsp;
  <a href="#credits--feedback"><b>Credits</b></a>
</div>

> **Tested Biomes:** Norse Lands, Call of Olympus, Shogun, European  
> **Compatible Versions:** 2.3.0+ (Unity 6)

---

##  Mod Overview & Features

###  Kingdom Monitor & HUD
| Sub-System | Description |
| :--- | :--- |
| **Kingdom Monitor (F3)** | Real-time dashboard for kingdom stats, population (including vagrants), and economy systems |
| **Stamina Bar** | Visual mount stamina with multiple styles (Classic, RPG, Retro) |
| **Smart HUD (F4)** | Auto-hide or manual toggle to prevent UI overlap |

---

## 🎛️ Mod Menu Suites (F1)

### MAIN  
*Core System Controls & Readouts*

- HUD visibility and layout control  
- Accessibility & TTS configuration (Queue / Interrupt modes)  
- Travel speed multiplier  

---

### CHEATS  
*Gameplay Overrides & System Control*

- **Character**
  - Invincibility  
  - Infinite stamina  
  - Artemis Bow (damage, spread, multi-shot)

- **Economy**
  - Add coins/gems or fill wallet  
  - Income and drop rate scaling  

- **Military**
  - Recruit all vagrants  
  - Spawn tools  
  - Kill all enemies  
  - Destroy all portals  
  - Max army instantly  

- **Spawner**
  - Units: Vagrants, Archers, Builders, Farmers, etc.  
  - Hermits: Bakery, Ballista, Stable, etc.  

---

### LAB (Experimental)  
*Advanced World & Engine Manipulation*

- Weather & season control  
- Blood Moon disable  
- Invincible/self-repairing walls  
- Rapid housing  
- Instant day/night skip  

- **Unit Enhancements**
  - Elite Knights  
  - Archer fire rate boost  
  - Berserker rage mode  
  - Ninja speed boost  

- **World Controls**
  - Tree regrowth  
  - Farm output  
  - Tower/Ballista rates  
  - Animal spawn rates  

---

### HARD (Custom Difficulty)  
*Experimental Director Overrides*

- Presets: Nightmare, Relentless, Oblivion, No Escape  

- Manual Controls:
  - Crown stealing toggle  
  - Wave size multiplier  
  - Enemy speed  
  - Portal spawn rate  
  - Greed Queen HP  
  - Director threat scaling  

> ⚠️ Experimental — behavior may vary

---

## 👁️ Advanced Accessibility

A full **Text-to-Speech (TTS)** system designed for blind and visually impaired players.

### Features
- Smart contextual interaction narration  
- Queue vs Interrupt speech modes  
- Environmental announcements (village, castle, etc.)  
- Tree detection to prevent critical mistakes  

### Hotkeys
- **F5** — Radar (Portals, Chests, Vagrants)  
- **F6** — Compass (Position & Direction)  
- **F7** — Wallet  
- **F8** — Time  
- **F9** — Mount  
- **F10** — Followers (WIP)  

---

## Performance & Stability

- Reduced CPU usage (radar & detection systems)  
- Optimized update cycles and caching  
- Improved exception handling  

> Result: smoother gameplay and cleaner logs

---

## 📥 Installation Guide

### Method 1 — Recommended (AIO)

1. Download the correct **v2.1.0 package**
2. Extract into:
```
Kingdom Two Crowns/
```
3. Launch the game
4. Press **F1**

> ⚠️ Internet required on first launch only

---

### Method 2 — Manual (Advanced)

1. Download `KingdomEnhanced.dll`  
2. Place into:
```
BepInEx/plugins/
```
3. Overwrite existing files  

---

## 🐧 Linux / Steam Deck / Wine

- Use builds **without TTS** if needed  
- `System.Speech.dll` is Windows-only  
- TTS will not function under Proton/Wine  

---

## 🎮 Controls Reference

| Key | Function |
|-----|--------|
| F1 | Mod Menu |
| F3 | Kingdom Monitor |
| F4 | HUD Toggle |
| F5 | Radar |
| F6 | Compass |
| F7 | Wallet |
| F8 | Time |
| F9 | Mount |
| F10 | Followers |

---

## 🤝 Credits & Feedback

- **Zaykus** — Lead Developer  
- **Abevol** — Framework, Mono/IL2CPP compatibility  
- **BepInEx Team** — Modding framework  

### Contributing
See `CONTRIBUTING.md` for setup and guidelines.

### Bug Reports
1. Press **F1 → Report**
2. Copy report
3. Submit via GitHub Issues

---

## ⚠️ Disclaimer

- Single-player focused  
- No telemetry or data collection  
- Use at your own risk  

---

## Final Note

**Kingdom Enhanced v2.1.0** removes backend and platform limitations entirely.

No compatibility issues.  
No manual setup complexity.  

**Just install and play.**
