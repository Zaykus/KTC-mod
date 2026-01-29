# 👑 Kingdom Enhanced (v1.0.0)

![Mod Version](https://img.shields.io/badge/version-1.0.0-green)
![Game Version](https://img.shields.io/badge/Kingdom%20Two%20Crowns-2.3.x-blue)
![Loader](https://img.shields.io/badge/BepInEx-IL2CPP_6.0+-orange)

**Kingdom Enhanced** is a powerful Quality of Life, Cheat, and Accessibility suite for *Kingdom Two Crowns*.  
It grants you full control over your army and economy while providing vital information hidden by the game, all wrapped in a native-style interface.

> **Tested On:** Norse Lands, Call of Olympus, Shogun, and European Biomes  
> **Compatible Versions:** 2.3.0, 2.3.1, 2.3.2 (Unity 6 Update)

![Mod Preview](modpic.png?raw=true)  
*(A clean, non-intrusive HUD replacing the standard UI)*

---

## ✨ Features

### 🏰 Royal HUD (Heads-Up Display)
Stop guessing how much gold you have or when night will fall. The new HUD displays at the top-center:

- **Precise Clock:** Digital 24-hour format (e.g., `18:45`)
- **Treasury Counter:** Reads internal Castle/Banker memory to show **exact gold stashed**  
  *(Beta feature)*
- **Day Counter:** Displays the current day number clearly

---

### ⚔️ Army & Management
- **Global Recruit:** Instantly converts **ALL** Vagrants on the entire island into Peasants
- **Supply Drop:** Instantly spawn **Bows** or **Hammers** at your feet
- **Hyper Builders:** *(Toggle)* Workers build and repair walls instantly
- **Larger Camps:** *(Toggle)* Vagrant camps hold more population and regenerate faster

---

### 💰 Economy
- **Instant Wealth:** Add **+10 Coins** or **+5 Gems** with one click
- **Physical Overflow:** Coins drop physically into the world
- **Infinite Stamina:** Your mount never tires

---

### 👁️ Accessibility & Blind Support
A core pillar of this mod is accessibility. It includes a **Screen Reader / Log System** that outputs game state as text.

- **Radar (F5):** Reports distance to Portals, Chests, and Vagrants
- **Compass (F6):** Reports exact X-coordinate and facing direction
- **Wallet Check (F7):** Reports exact Coin/Gem count
- **Time Check (F8):** Reports Day and Time (Morning/Night)
- **Status Logs:** All actions are printed to the on-screen log

---

## 📥 Installation (Important!)

> **TL;DR:**  
> Download **KTC-mod-AIO-v1.0.0.zip**, extract its contents directly into your game folder:  
> `\Steam\steamapps\common\Kingdom Two Crowns\`  
> That’s it — no extra steps needed.

This mod requires **IL2CPP BepInEx**.  
❌ Do **NOT** use the Mono version.

---

### Manual Installation (Advanced / Dev)

1. **Download BepInEx 6 (IL2CPP)**
   - Go to the [BepInEx Releases](https://github.com/BepInEx/BepInEx/releases)
   - Download **`BepInEx_UnityIL2CPP_x64_*.zip`**

2. **Install BepInEx**
   - Extract into:
     ```
     \Steam\steamapps\common\Kingdom Two Crowns\
     ```
   - Run the game **once**
   - ⚠️ First launch may take several minutes while *Interop Assemblies* generate

3. **Install the Mod**
   - Download `KingdomEnhanced.dll` from the  
     [Releases Tab](https://github.com/Zaykus/KTC-mod/releases)
   - Place it in:
     ```
     \Kingdom Two Crowns\BepInEx\plugins\
     ```

4. **Launch the Game**
   - Press **F1** in-game to open the mod menu

---

## 🎮 Controls & Hotkeys

| Key | Function | Description |
|-----|---------|-------------|
| **F1** | Toggle Menu | Opens the mod menu and unlocks cursor |
| **Shift** | Sprint | Move 2× faster |
| **F5** | Radar | Logs nearby POIs |
| **F6** | Compass | Logs position and direction |
| **F7** | Wallet | Logs currency |
| **F8** | Clock | Logs day & time |
| **F9** | Mount | Logs mount status |
| **F10** | Followers | Logs companions |

---

## 🏗️ Development & History

**Kingdom Enhanced** was built to solve performance issues of older mods while adding modern Unity 6 support.

### v1.0.0 (Current)
- Refactored into 4 high-performance managers:
  - `World`, `Army`, `Player`, `Building`
- **Fixed Lag:** Removed `FindObjectsOfType` loops
- **Fixed Treasury:** Reflection-based access to obfuscated `stashedCoins`
- **Fixed Compatibility:** Works with **Call of Olympus** and **Unity 6**

### Upcoming (v1.2+)
- Atmospheric Controls *(disabled for stability)*
- Self-Repairing Walls *(in testing)*

---

## 🤝 Credits & Special Thanks

- **[Zaykus](https://github.com/Zaykus)** — Lead Developer, Accessibility, UI
- **[Abevol](https://github.com/abevol)** — `KingdomMod` framework
  - Uses `GameExtensions` and `TypeExtensions`
  - Internal list logic adapted from open-source work
- **BepInEx Team** — IL2CPP tooling

---

## 📝 Feedback & Support

If something doesn’t work (e.g. Treasury shows `0`):

1. Press **F1**
2. Open **Feedback**
3. Mark features `[ OK ]` or `[ X ]`
4. Click **Copy Report**
5. Paste it on the  
   [Issues Page](https://github.com/Zaykus/KTC-mod/issues)

---

*Disclaimer: This mod is for single-player fun.  
Please support Raw Fury by purchasing the game and DLCs.*
