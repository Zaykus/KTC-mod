# Kingdom Enhanced (v2.0)

![Mod Version](https://img.shields.io/badge/version-2.0-green)
![Game Version](https://img.shields.io/badge/Kingdom%20Two%20Crowns-2.3.x-blue)
![Loader](https://img.shields.io/badge/BepInEx-IL2CPP_6.0+-orange)

**Kingdom Enhanced** is a comprehensive Quality of Life, Cheat, and Accessibility modification suite for *Kingdom Two Crowns*.  
It grants players complete granular control over their kingdom's army, economy, and world states, while exposing vital hidden statistics through a modern, native-style interface. 

<div align="center">
  <a href="#mod-overview--features"><b>Mod Overview</b></a> &nbsp;&bull;&nbsp;
  <a href="#installation-guide"><b>Installation</b></a> &nbsp;&bull;&nbsp;
  <a href="#controls-reference"><b>Controls</b></a> &nbsp;&bull;&nbsp;
  <a href="#credits--feedback"><b>Credits</b></a>
</div>

> **Tested Biomes:** Norse Lands, Call of Olympus, Shogun, and European  
> **Compatible Versions:** 2.3.0+ (Unity 6 Update)
> **2.0.0 Update** has added many features, but there are also many errors here... I'm sorry about that.

<video src="vid3.mp4?raw=true" autoplay loop muted playsinline></video>
*(A clean, card-based setup designed within the Royal Dark Console aesthetic replacing standard UI)*

---

## 🌟 Mod Overview & Features

### 🏰 Kingdom Monitor & HUD
| Sub-System | Description |
| :--- | :--- |
| **Kingdom Monitor (F3)** | A comprehensive live dashboard indicating real-time metrics for realm stats, active census (including vagrants), and underlying economy logic. |
| **Stamina Bar** | A visual indicator explicitly tracking your mount's energy. Supports multiple UI overlay styles (Classic, RPG, Retro). |
| **Smart HUD (F4)** | The native top bar automatically hides when menus are open to prevent UI overlap. Alternatively, you can toggle the entire game HUD manually via F4. |

### 🎛️ The Mod Menu Suites (F1)
The fully rebuilt custom Mod Menu implements a native horizontal layout separated into core control suites:

<br>

#### ⚙️ [ MAIN ]
> *Core System Controls & Active Readouts*

*   **HUD Control**
    Toggle primary HUD visibility, select visual overlay stylings, and adjust interface element offsets.
*   **Accessibility & Text-to-Speech**
    Configure the built-in narrator options, manage "Queue" versus "Interrupt" speech rules, and toggle smart interaction naming for complex structures.
*   **Travel Speed Configuration**
    Dynamically scale the base speed multiplier for the Monarch to drastically adjust movement speed across biomes.

---

#### ⚡ [ CHEATS ]
> *Modify Core Gameplay Boundaries & System Resource Logic*

*   **Character Modifiers**
    *   `Invincibility` : Shield the Monarch from incoming damage.
    *   `Infinite Stamina` : Prevent steeds from ever tiring or pausing.
    *   `Artemis Bow` : A custom bow configuration tool. Overwrite standard arrow behavior to shoot multiple projectiles, scale the spread range distance, and universally multiply damage output.
*   **Economy Modifiers**
    Instantly inject +10/+50 Coins or +5 Gems into your wallet, or trigger an absolute "Fill to Max". Utilize override sliders to scale passive coin income logic and adjust global physical bag drop rates.
*   **Military Commands**
    *   Instantly recruit all active vagrants island-wide.
    *   Force-spawn Archer Bows or Builder Hammers directly at your coordinates.
    *   Instantly execute every active Greed enemy on the level.
    *   Systematically destroy all active dimensional portals.
    *   Instantly queue and max out your local army capacity.
*   **Entity Spawner Engine**
    Explicitly target and instance any unit on your cursor location.
    *   *Includes:* Vagrants, Villagers, Archers, Builders, Farmers, Pikemen, Ninjas, Berserkers, Knights.
    *   *Hermits:* Bakery, Ballista, Berserker, Fire, Horn, Stable, Warrior.

---

#### 🧪 [ LAB (Experimental) ]
> *Deep Engine Manipulation for Weather, Time, and Mechanics*

*   **World Mechanics overrides**
    *   `Lock Season` : Freeze the active logic state rigidly to Summer.
    *   `Clear Weather` : Disable ongoing rain, snow, and ambient storm phenomena.
    *   `Buoyant Coins` : Override physics protocols so coins float harmlessly on water surfaces.
    *   `No Blood Moons` : Permanently blacklist the Blood Moon timeline triggers.
    *   `Invincible Walls` : Enable a self-repairing subroutine for all barricades.
    *   `Rapid Housing` : Accelerate the timeframe for citizen move-in logic.
*   **Time Control Systems**
    Execute immediate sequence skips traversing from Day to Night instantly preventing manual wait times.
*   **Unit Enhancers**
    Configure active tactical buffs (Elite Knights, heightened Archer rate-of-fire, permanent Berserker rage mode, and enhanced Ninja speed curves).
*   **World & Mount Data Control**
    Unlock sliders to manipulate the background math for tree regrowth timing, farm production outputs, defense tower fire rates, ballista impact parameters, and underlying animal spawn intervals.

---

#### 💀 [ HARD (Custom Difficulties) ]
> *Experimental Overrides for the game's director AI. Stability is not guaranteed.*

*   **Director Presets**
    *   `[ Nightmare ]` : Massive waves of enemies. Prepare your defenses.
    *   `[ Relentless ]` : Nights are longer. Enemies never retreat.
    *   `[ Oblivion ]`  : The Blood Moon rises often. Retaliation is swift.
    *   `[ No Escape ]` : Enemies are stronger, faster, and deadlier.
*   **Manual Overrides**
    *   `Crown Stealing` : Explicitly toggle Greed crown-stripping capabilities.
    *   `Wave Multipliers` : Multiply the exact volume of units generated per wave.
    *   `Enemy Speed` : Adjust the base movement speed of the Greed across the map.
    *   `Portal Spawn Rate` : Manipulate the dynamic spawning thresholds for dimensional threats.
    *   `Greed Queen HP` : Multiply the base health pool of the Greed Queen.
    *   `Director Threat` : Aggressively ramp the Director Threat scaling multipliers for extreme testing.

---

### 👁️ Advanced Accessibility
A thorough Text-To-Speech (TTS) experience tailored specifically for blind or visually impaired players to navigate and thrive independently.

- **Smart Interactions:** The mod speaks intelligent contextual labels based on tier scaling (*"Deposit"*, *"Pay"*, *"Repair"*, *"Build Wall"* vs *"Upgrade Wall"*).
- **Narrator Queue Mode:** Choose whether the system prioritizes "Interrupt" for fast-paced reporting or "Queue" to safely finish auditory sentences.
- **Environmental Announcers:** Automatic alerts pinging when the player breaches the safety threshold of the castle or physically enters/leaves a local peasant village.
- **Tree Identification:** Detects and separates general forestry from critical village trees to prevent accidental village destruction.

**Full Reporting Hotkeys:**
- **F5 (Radar):** Targeted distance scanning isolating Portals, Chests, and Vagrants.
- **F6 (Compass):** Exact map coordinates and precise directional facing.
- **F7 (Wallet):** Live purse counts (e.g. *"35 Gold, 5 Gems"*).
- **F8 (Time):** Current temporal cycle (e.g. *"Day 12, Night"*).

---

## 📥 Installation Guide

### Which file should I download?

| File Name | Usage | Description |
| :--- | :--- | :--- |
| **`KTC-mod-AIOv2.0.0.zip`** | **Recommended** | **All-In-One**. Contains the Mod, BepInEx loader, and all required IL2CPP DLLs. Start here. |
| **`KTC-mod-without-Speech-v2.0.0.zip`** | **Linux / No-TTS** | AIO pack without System.Speech.dll. Best for Linux/Steam Deck. |
| **`KingdomEnhanced.dll`** | **Update / Expert** | The isolated mod plugin. Download only if you are already configured and updating. |
| **`System.Speech.dll`**| **Optional** | Required only if you need Text-to-Speech (TTS) manually and aren't using the AIO zip. |

---

### Linux / Steam Deck / Wine Support
This mod natively handles Linux/Steam Deck parameters, given you follow proper load techniques:
1.  **Skip the AIO zip**: The included `System.Speech.dll` architecture generally crashes Wine/Proton instances.
2.  **Download "No Speech" Version**: Use `KTC-mod-without-Speech-v2.0.0.zip`.
3.  **Launch Arguments**: If the mod doesn't attach immediately, update your Steam Launch Options with:
    `--doorstop-enable true --doorstop-target "BepInEx\core\BepInEx.IL2CPP.dll"`
4.  **No Text-to-Speech**: Note that all visual/audio accessibility hooks rely strictly on Windows SAPI. TTS functionality will not execute on Linux.

---

### Method 1: All-In-One (Recommended)
1.  Download **`KTC-mod-AIOv2.0.0.zip`**.
2.  Extract the entire directory into your core game folder:
    *   `\Steam\steamapps\common\Kingdom Two Crowns\`
3.  Run the game! Press **F1** to initialize the control panel.

### Method 2: Manual Update
1.  Download **`KingdomEnhanced.dll`**.
2.  Move `KingdomEnhanced.dll` into:
    *   `\Kingdom Two Crowns\BepInEx\plugins\`
3.  Overwrite the existing file and deploy.

---

## 🎮 Controls Reference

| Key | Function | Description |
|-----|---------|-------------|
| **F1** | Menu Toggle | Opens Mod Menu (Resizable) & Unlocks Cursor |
| **Shift** | Sprint | Move 2x Faster |
| **F3** | Kingdom Monitor| Live Stats Dashboard |
| **F4** | HUD Toggle | Show/Hide Base Game HUD |
| **F5** | Radar | Scan for POIs (Portals/Vagrants/Chests) |
| **F6** | Compass | Absolute Location & Facing direction |
| **F7** | Wallet | Announce Active Coin/Gem Count |
| **F8** | Time | Announce Day/Night Status |
| **F9** | Mount | Announce Stamina Status |
| **F10**| Followers | (Coming Soon) |

---

## 🤝 Credits & Feedback

*   **[Zaykus](https://github.com/Zaykus)** — Lead Developer, Menu UI, Everything else.
*   **[Abevol](https://github.com/abevol)** — Start Point & IL2CPP Tools
*   **BepInEx Team** — Mod Loader Framework

Found a bug or visual error?
1. Press **F1** -> **Report** Tab.
2. Click **Copy Report**.
3. Submit the details on the [Issues Tracker](https://github.com/Zaykus/KTC-mod/issues).

*Disclaimer: This mod is strictly designed for single-player integrity testing and enjoyment. Please support Raw Fury!*
