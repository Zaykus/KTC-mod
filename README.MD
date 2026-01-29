# 👑 Kingdom Enhanced (v1.0.0)

![Mod Version](https://img.shields.io/badge/version-1.0.0-green) ![Game Version](https://img.shields.io/badge/Kingdom%20Two%20Crowns-2.3.x-blue) ![Loader](https://img.shields.io/badge/BepInEx-IL2CPP_6.0+-orange)

**Kingdom Enhanced** is a powerful Quality of Life, Cheat, and Accessibility suite for *Kingdom Two Crowns*. It grants you full control over your army and economy while providing vital information hidden by the game, all wrapped in a native-style interface.

> **Tested On:** Norse Lands, Call of Olympus, Shogun, and European Biomes.
> **Compatible Versions:** 2.3.0, 2.3.1, 2.3.2 (Unity 6 Update).

![Mod Preview](modpic.png?raw=true)
*(A clean, non-intrusive HUD replacing the standard UI)*

---

## ✨ Features

### 🏰 Royal HUD (Heads-Up Display)
Stop guessing how much gold you have or when night will fall. The new HUD displays at the top-center:
*   **Precise Clock:** Digital 24-hour format (e.g., `18:45`). No more guessing if it's "Late Afternoon" or "Night".
*   **Treasury Counter:** Reads the internal memory of the Castle/Banker to show **exact gold stashed** (e.g., `Treasury: 120 Gold`).*(Beta feature)*
*   **Day Counter:** Displays the current day number clearly.

### ⚔️ Army & Management
*   **Global Recruit:** Instantly converts **ALL** Vagrants on the entire island into Peasants. No more running to camps!
*   **Supply Drop:** Instantly spawn **Bows** or **Hammers** at your feet to equip your new recruits.
*   **Hyper Builders:** (Toggle) Workers build and repair walls instantly.
*   **Larger Camps:** (Toggle) Vagrant camps hold more population and regenerate faster.

### 💰 Economy
*   **Instant Wealth:** Add **+10 Coins** or **+5 Gems** with a single click.
*   **Physical Overflow:** Coins drop physically into the world, meaning you can overfill your bag or create piles of gold for your coop partner.
*   **Infinite Stamina:** Your mount never tires. Gallop across the map endlessly.

### 👁️ Accessibility & Blind Support
A core pillar of this mod is making the game accessible. It includes a **Screen Reader / Log System** that outputs game state as text.
*   **Radar (F5):** Scans the area and reports the distance to Portals, Chests, and Vagrants.
*   **Compass (F6):** Tells you your exact X-coordinate and facing direction.
*   **Wallet Check (F7):** Reports exact Coin/Gem count.
*   **Time Check (F8):** Reports Day and Time (Morning/Night).
*   **Status Logs:** All actions (e.g., "Recruited 4 Beggars") are printed to the on-screen log for visibility.

---

## 📥 Installation (Important!)

This mod requires **IL2CPP** BepInEx. Do not use the standard Mono version.

1.  **Download BepInEx 6 (IL2CPP):**
    *   Go to the [BepInEx Releases](https://github.com/BepInEx/BepInEx/releases).
    *   Download the artifact labeled **`BepInEx_UnityIL2CPP_x64_...zip`**.
2.  **Install BepInEx:**
    *   Extract the zip content into your game folder:
        *   `\Steam\steamapps\common\Kingdom Two Crowns\`
    *   *Note: Run the game once now. It will take a few minutes to "Generate Interop Assemblies". This is normal.*
3.  **Install the Mod:**
    *   Download `KingdomEnhanced.dll` from the [Releases Tab](https://github.com/Zaykus/KTC-mod/releases).
    *   Place it in: `\Kingdom Two Crowns\BepInEx\plugins\`.
4.  **Launch:**
    *   Press **F1** in-game to open the menu.

---

## 🎮 Controls & Hotkeys

| Key | Function | Description |
| :--- | :--- | :--- |
| **F1** | **Toggle Menu** | Opens the Mod Menu and unlocks the mouse cursor. |
| **Shift** | **Sprint** | Hold while moving to move 2x faster. |
| **F5** | **Radar** | [Access] Logs nearby points of interest (Left/Right). |
| **F6** | **Compass** | [Access] Logs player position and direction. |
| **F7** | **Wallet** | [Access] Logs current currency count. |
| **F8** | **Clock** | [Access] Logs current Day and Time. |
| **F9** | **Mount** | [Access] Logs current Mount name and fatigue status. |
| **F10** | **Followers** | [Access] Logs hermit/dog companions. |

---

## 🏗️ Development & History

**Kingdom Enhanced** was built to solve the performance issues of older mods while adding modern features for Unity 6.

*   **v1.0.0 (Current):**
    *   Refactored codebase into 4 High-Performance Managers (`World`, `Army`, `Player`, `Building`).
    *   **Fixed Lag:** Removed `FindObjectsOfType` loops that caused stuttering in other mods. Now uses internal lists.
    *   **Fixed Treasury:** Uses Reflection to find the obfuscated `stashedCoins` field in the Banker class.
    *   **Fixed Compatibility:** Works with the new **Call of Olympus** and **Unity 6** engine updates.

*   **Upcoming (v1.2+):**
    *   Atmospheric Controls (Season/Weather locking) - *Currently disabled for stability.*
    *   Self-Repairing Walls - *Currently in testing.*

---

## 🤝 Credits & Special Thanks

*   **[Zaykus](https://github.com/Zaykus)**: Lead Developer, Accessibility Features, UI Design.
*   **[Abevol](https://github.com/abevol)**: Author of the `KingdomMod` framework.
    *   *This mod utilizes Abevol's `GameExtensions` and `TypeExtensions` for stable IL2CPP reflection.*
    *   *The logic for retrieving internal lists (`_enemies`, `_payables`) is adapted from his open-source work.*
*   **BepInEx Team**: For the IL2CPP modding tools.

---

## 📝 Feedback & Support

If a feature is not working for you (e.g., Treasury showing 0), please help us fix it!
1.  Open the Menu (**F1**).
2.  Click the **Feedback** tab.
3.  Mark which features are working `[ OK ]` or broken `[ X ]`.
4.  Click **Copy Report** and paste it in our [Issues Page](https://github.com/Zaykus/KTC-mod/issues).

---
*Disclaimer: This mod is for single-player fun. Please support Raw Fury by purchasing the game and DLCs.*