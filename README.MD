# 👑 Kingdom Enhanced (v2.0)

![Mod Version](https://img.shields.io/badge/version-2.0-green)
![Game Version](https://img.shields.io/badge/Kingdom%20Two%20Crowns-2.3.x-blue)
![Loader](https://img.shields.io/badge/BepInEx-IL2CPP_6.0+-orange)

**Kingdom Enhanced** is a powerful Quality of Life, Cheat, and Accessibility suite for *Kingdom Two Crowns*.  
It grants you full control over your army and economy while providing vital information hidden by the game, all wrapped in a native-style interface.

> **Tested On:** Norse Lands, Call of Olympus, Shogun, and European Biomes  
> **Compatible Versions:** 2.3.0+ (Unity 6 Update)

![Mod Preview](modpic.png?raw=true)  
*(A clean, card-based Royal Dark Console replacing the standard UI)*

---

## ✨ Features

### 🏰 Kingdom Monitor & HUD
*   **Kingdom Monitor (F3):** A comprehensive dashboard showing stats, census, and economy.
*   **Stamina Bar:** Visual indicator for mount energy with multiple styles (Classic, RPG, Retro).
*   **Smart HUD (F4):** The top bar automatically hides when menus are open to prevent overlap. You can toggle the in-game HUD manually by pressing F4.
*   **Vagrant Counter:** Built-in integration for counting vagrants directly into the UI.

### 🎛️ Mod Menu (New in v2.0)
The fully rebuilt Mod Menu uses a horizontal tab navigation system:

Here's the detailed breakdown of the suites:
*   **MAIN:** Manage HUD settings, accessibility, TTS announcers, and monarch travel speed.
*   **CHEATS:** Toggle Invincibility, Infinite Mount Stamina, Infinite Stone tools (custom Artemis Bow modifiers). Economy injects (add coins/gems instantly), military control (instant recruit, kill enemies, destroy portals), and fully operational entity spawners for all units and hermits.
*   **LAB (Experimental):** Control world mechanics (Lock Summer, Clear Weather, Disable Blood Moons), empower your military (Elite Knights, Archer Fire Rate Boosts), enable self-repairing invincible walls, instantly skip day/night, and modify steed damage/speeds!
*   **HARD:** Experimental variables to modify wave difficulties (multiplier sliders for Wave Size, Enemy Speed, Portal Rate, Director Threat, and Queen HP).

### 👁️ Advanced Accessibility
A complete screen-reader / TTS experience for blind or visually impaired players.
*   **Smart Interactions:** The mod speaks intelligent action labels:
    *   *"Deposit"* (Bank/Chest), *"Pay"* (Merchant/Boat), *"Hire"* (Beggar), *"Repair"* (Ruin).
    *   *"Build Wall"* vs *"Upgrade Wall"* (Tier detection).
*   **Narrator Queue Mode:** Choose between **Interrupt** (immediate) or **Queue** (finish sentence) modes.
*   **Announcers:** Alerts you when entering/leaving the safety of your castle or a peasant village.
*   **Tree Notification:** Correctly identifies and announces individual trees in a forest so you don't accidentally cut village trees.
*   **Full Reporting (F5-F10):** Radar, Compass, Wallet, Time.
    *   **F5 (Radar):** Targeted scan for Portals, Chests, and Vagrants.
    *   **F6 (Compass):** Exact coordinates and facing.
    *   **F7 (Wallet):** "35 Gold, 5 Gems".
    *   **F8 (Time):** "Day 12, Night".

---

## 📥 Installation

### Which file should I download?

| File Name | Usage | Description |
| :--- | :--- | :--- |
| **`KTC-mod-AIOv2.0.0.zip`** | **✅ Recommended** | **All-In-One**. Contains the Mod, BepInEx loader, and all required DLLs. Best for new users. |
| **`KTC-mod-without-Speech-v2.0.0.zip`** | **Linux / No-TTS** | AIO pack **without** System.Speech.dll. Best for Linux/Steam Deck or if you don't need TTS. |
| **`KingdomEnhanced.dll`** | **Update / Expert** | The mod plugin itself. Download this to update your existing version. |
| **`System.Speech.dll`**| **Optional** | Required **only** if you need Text-to-Speech (TTS) and aren't using the AIO zip. |

---

### 🐧 Linux / Steam Deck / Wine Support
This mod **DOES** work on Linux/Steam Deck, but with caveats:
1.  **Do NOT use the AIO zip**: The included `System.Speech.dll` often crashes Wine/Proton.
2.  **Download "No Speech" Version**: Use `KTC-mod-without-Speech-v2.0.0.zip`.
3.  **Launch Arguments**: If the mod doesn't load, add this to Steam Launch Options:
    `--doorstop-enable true --doorstop-target "BepInEx\core\BepInEx.IL2CPP.dll"`
4.  **No Text-to-Speech**: The accessibility features rely on Windows SAPI and will not work on Linux.

---

### Method 1: All-In-One (Recommended)
The easiest way to get started.
1.  Download **`KTC-mod-AIOv2.0.0.zip`**.
2.  Extract the **contents** directly into your game folder:
    *   `\Steam\steamapps\common\Kingdom Two Crowns\`
3.  Run the game! Press **F1** to open the menu.

### Method 2: Manual Update
Use this if you are updating an existing installation.
1.  Download **`KingdomEnhanced.dll`** (and `System.Speech.dll` if needed).
2.  Drag `KingdomEnhanced.dll` into:
    *   `\Kingdom Two Crowns\BepInEx\plugins\`
3.  Replace existing files.

---

## 🎮 Controls

| Key | Function | Description |
|-----|---------|-------------|
| **F1** | **Menu Toggle** | Opens Mod Menu (Resizable) & Unlocks Cursor |
| **Shift** | **Sprint** | Move 2x Faster |
| **F3** | **Kingdom Monitor** | Stats Dashboard |
| **F4** | **HUD Toggle** | Show/Hide Game HUD |
| **F5** | Radar | Scan for POIs (Portals/Cheats) |
| **F6** | Compass | Location & Facing |
| **F7** | Wallet | Coin/Gem Count |
| **F8** | Time | Day/Night Status |
| **F9** | Mount | Stamina Status |
| **F10**| Followers | (Coming Soon) |

---

## 🏗️ Version History

### v2.0.0 - A LOT OF FIXES AND FEATURES AND MORE
*   **UI:** Implemented new card-based custom Mod Menu UI with a horizontal tab navigation feature.
*   **CHEATS & LABS:** Added Custom Mod Spawners, Artemis Bow Modifiers, World Mechanic adjustments (Weather, Time, Walls), and comprehensive Army boosts.
*   **Accessibility:** Upgraded Radar, entering/leaving village TTS announcers, and village tree detection.
*   **Fixes:** Resolved core upgrade logic bugs, fixed wallet cost notifications, and patched deprecated function hooks.

<video src="vid3.mp4?raw=true" autoplay loop muted playsinline></video>

### v1.5.0 - The Polish Update
*   **New Features:** Kingdom Monitor (F3), Mount Stamina Bar.
*   **Performance:** Major CPU optimizations for Radar and Object Detection.
*   **Accessibility:** Narrator Queue Mode, improved compatibility.

---

## 🤝 Credits

*   **[Zaykus](https://github.com/Zaykus)** — Lead Developer, Menu UI, Everything else.
*   **[Abevol](https://github.com/abevol)** — Start Point & IL2CPP Tools
*   **BepInEx Team** — Mod Loader

---

## 📝 Feedback
Found a bug?
1. Press **F1** -> **Report** Tab.
2. Click **Copy Report**.
3. Paste it in [Issues](https://github.com/Zaykus/KTC-mod/issues).

*Disclaimer: This mod is for single-player use. Please support Raw Fury!*
