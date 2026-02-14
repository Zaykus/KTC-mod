# 👑 Kingdom Enhanced (v1.5)

![Mod Version](https://img.shields.io/badge/version-1.5-green)
![Game Version](https://img.shields.io/badge/Kingdom%20Two%20Crowns-2.3.x-blue)
![Loader](https://img.shields.io/badge/BepInEx-IL2CPP_6.0+-orange)

**Kingdom Enhanced** is a powerful Quality of Life, Cheat, and Accessibility suite for *Kingdom Two Crowns*.  
It grants you full control over your army and economy while providing vital information hidden by the game, all wrapped in a native-style interface.

> **Tested On:** Norse Lands, Call of Olympus, Shogun, and European Biomes  
> **Compatible Versions:** 2.3.0+ (Unity 6 Update)

![Mod Preview](modpic.png?raw=true)  
*(A clean, non-intrusive HUD replacing the standard UI)*

---

## ✨ Features

### 🏰 Kingdom Monitor & HUD (New in v1.5)
*   **Kingdom Monitor (F4):** A comprehensive dashboard showing stats, census, and economy.
*   **Stamina Bar:** Visual indicator for mount energy with multiple styles (Classic, RPG, Retro).
*   **Resizable Menus:** Drag the bottom-right corner (↘) of F1 and F4 menus to resize them.
*   **Smart HUD:** The top bar automatically hides when menus are open to prevent overlap.
*   **Precise Clock:** Digital 24-hour format (e.g., `18:45`).
*   **Day Counter:** Displays current day number.
*   **Coin Counter:** Shows inventory count + Stashed gold (Beta).

### 👁️ Advanced Accessibility
A complete screen-reader / TTS experience for blind or visually impaired players.
*   **Smart Interactions:** The mod speaks intelligent action labels:
    *   *"Deposit"* (Bank/Chest), *"Pay"* (Merchant/Boat), *"Hire"* (Beggar), *"Repair"* (Ruin).
    *   *"Build Wall"* vs *"Upgrade Wall"* (Tier detection).
*   **Narrator Queue Mode (v1.5):** Choose between **Interrupt** (immediate) or **Queue** (finish sentence) modes.
*   **Castle Announcer:** Alerts you when entering/leaving the safety of your castle.
*   **Tree Notification:** Correctly identifies and announces individual trees in a forest.
*   **Full Reporting (F5-F10):** Radar, Compass, Wallet, Time.
    *   **F5 (Radar):** Targeted scan for Portals, Chests, and Vagrants.
    *   **F6 (Compass):** Exact coordinates and facing.
    *   **F7 (Wallet):** "35 Gold, 5 Gems".
    *   **F8 (Time):** "Day 12, Night".

### ⚔️ Army & Management
*   **Global Recruit:** Instantly converts **ALL** Vagrants on the island.
*   **Supply Drop:** Spawn Bows or Hammers at your feet.
*   **Hyper Builders:** Workers build and repair instantly.
*   **Larger Camps:** Vagrant camps hold more population.

### 💰 Economy & Cheats
*   **Instant Wealth:** Add +10 Coins / +5 Gems.
*   **Physical Overflow:** Coins drop physically if wallet is full.
*   **Infinite Stamina:** Mount never tires.
*   **Invincibility:** (Optional) God mode.

---

## 📥 Installation

### Which file should I download?

| File Name | Usage | Description |
| :--- | :--- | :--- |
| **`KTC-mod-AIOv1.5.0.zip`** | **✅ Recommended** | **All-In-One**. Contains the Mod, BepInEx loader, and all required DLLs. Best for new users. |
| **`KTC-mod-1.5.0 (Mod Only).zip`** | **Standard** | Just the plugin DLL + BepInEx. Use this **don't** need TTS. |
| **`KTC-mod-1.5.0 (Mod + Speech).zip`**| **Accessibility** | Plugin DLL + `System.Speech.dll`. Required if you need **Blind/TTS support** but installed BepInEx manually. |

---

### 🐧 Linux / Steam Deck / Wine Support
This mod **DOES** work on Linux/Steam Deck, but with caveats:
1.  **Do NOT use the AIO zip**: The included `System.Speech.dll` often crashes Wine/Proton.
2.  **Download "Mod Only"**: Use `KTC-mod-1.5.0 (Mod Only).zip`.
3.  **Launch Arguments**: If the mod doesn't load, add this to Steam Launch Options:
    `--doorstop-enable true --doorstop-target "BepInEx\core\BepInEx.IL2CPP.dll"`
4.  **No Text-to-Speech**: The accessibility features rely on Windows SAPI and will not work on Linux.

---

### Method 1: All-In-One (Recommended)
The easiest way to get started.
1.  Download **`KTC-mod-AIOv1.5.0.zip`**.
2.  Extract the **contents** directly into your game folder:
    *   `\Steam\steamapps\common\Kingdom Two Crowns\`
3.  Run the game! Press **F1** to open the menu.

### Method 2: Manual Update
Use this if you are updating an existing installation.
1.  Download the **Mod Only** (or **Mod + Speech**) zip.
2.  Drag `KingdomEnhanced.dll` (and `System.Speech.dll` if needed) into:
    *   `\Kingdom Two Crowns\BepInEx\plugins\`
3.  Replace existing files.

---

## 🎮 Controls

| Key | Function | Description |
|-----|---------|-------------|
| **F1** | **Menu Toggle** | Opens Mod Menu (Resizable) & Unlocks Cursor |
| **Shift** | **Sprint** | Move 2x Faster |
| **F4** | **Kingdom Monitor** | Stats Dashboard (Resizable) |
| **F5** | Radar | Scan for POIs (Portals/Cheats) |
| **F6** | Compass | Location & Facing |
| **F7** | Wallet | Coin/Gem Count |
| **F8** | Time | Day/Night Status |
| **F9** | Mount | Stamina Status |
| **F10**| Followers | (Coming Soon) |

---

## 🏗️ Version History

### v1.5 - The Polish Update
*   **New Features:** Kingdom Monitor (F4), Mount Stamina Bar.
*   **Performance:** Major CPU optimizations for Radar and Object Detection.
*   **UI:** Resizable menus (F1/F4) and smart HUD hiding.
*   **Accessibility:** Narrator Queue Mode, improved compatibility.
*   **Fixes:** Tree notification silencing, HUD overlap issues.

### v1.2 - Accessibility Update
*   **Smart Actions:** Context-aware labels (Deposit, Pay, Hire).
*   **Refined TTS:** Improved formatting and name cleaning.
*   **Fixes:** Menu freeze, repetitive logs, build crashes.

### v1.0 - Initial Release
*   Core Features: HUD, Cheats, Basic Accessibility.

---

## 🤝 Credits

*   **[Zaykus](https://github.com/Zaykus)** — Lead Developer
*   **[Abevol](https://github.com/abevol)** — KingdomMod Framework & IL2CPP Tools
*   **BepInEx Team** — Mod Loader

---

## 📝 Feedback
Found a bug?
1. Press **F1** -> **Feedback** Tab.
2. Click **Copy Report**.
3. Paste it in [Issues](https://github.com/Zaykus/KTC-mod/issues).

*Disclaimer: This mod is for single-player use. Please support Raw Fury!*
