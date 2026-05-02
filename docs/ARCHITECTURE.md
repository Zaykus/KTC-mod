# Project Architecture

## Overview

Kingdom Enhanced is a BepInEx mod for Kingdom Two Crowns, providing quality-of-life improvements, cheat capabilities, and accessibility features. It supports both **IL2CPP** and **Mono** game versions from a single codebase via conditional compilation.

## Tech Stack

- **Runtime**: .NET 6.0 (IL2CPP) / netstandard2.1 (Mono)
- **Mod Framework**: BepInEx 6.0+ (IL2CPP + Mono)
- **Hooking**: Harmony
- **UI**: Unity IMGUI (OnGUI)
- **Game Engine**: Unity IL2CPP / Mono (Unity 6)

## Build System

```
Directory.Build.props          ← MSBuild auto-imported: BepInEx paths per OS
KingdomEnhanced.csproj         ← Three configurations + conditional DLL refs
deps/KTC-ModDevLibs/           ← Git submodule: IL2CPP & Mono dev libraries
```

| Configuration | TFM | Defines | DLL Source |
|:---|:---|:---|:---|
| `Debug` | `net6.0` | `IL2CPP, BIE, BIE6` | `BIE6_IL2CPP/` |
| `BIE6_IL2CPP` | `net6.0` | `IL2CPP, BIE, BIE6` | `BIE6_IL2CPP/` |
| `BIE6_Mono` | `netstandard2.1` | `MONO, BIE, BIE6` | `BIE6_Mono/` |

For details on the IL2CPP/Mono compatibility architecture, see [IL2CPP-MONO-COMPAT.md](IL2CPP-MONO-COMPAT.md).

## Project Layers

```
KingdomEnhanced/
├── Core/              # Entry point and configuration
│   ├── Plugin.cs           # BepInEx dual-base-class entry (BasePlugin / BaseUnityPlugin)
│   ├── Settings.cs         # Persistent config via BepInEx ConfigFile
│   └── ModVersion.cs       # Version constants
├── Hooks/             # Harmony patches (game method interception)
│   ├── AbilityHooks.cs     # Item ability cooldowns / Artemis Bow
│   ├── CurrencyHooks.cs    # Coin/gem income and bag drop multipliers
│   ├── CurrencyWaterHooks.cs
│   ├── LabPatches.cs       # Experimental feature patches
│   ├── MapHooks.cs
│   └── PlayerSpawnerHook.cs # Attaches Feature managers on player spawn
├── Features/          # Gameplay feature modules (MonoBehaviours)
│   ├── AccessibilityFeature.cs      # Hover narration, keyboard reports (F5-F10)
│   ├── AccessibilityReportHandler.cs # F5-F10 report logic (separated from above)
│   ├── ArmyManager.cs               # Unit spawning, recruitment, military commands
│   ├── AutoPayHandler.cs            # Automatic coin payment helper
│   ├── DifficultyIntegration.cs     # Custom difficulty injection into BiomeSelect
│   ├── DifficultyRules.cs           # Hard-mode restriction rules
│   ├── DifficultyUIPatch.cs         # UI patches for custom difficulty selectors
│   ├── HardModeBuffData.cs          # Hard-mode enemy buff state
│   ├── HardModeFeature.cs           # Hard-mode feature controller
│   ├── HardModePresets.cs           # Difficulty preset definitions
│   ├── KingdomMonitor.cs            # Real-time kingdom stats dashboard (F3)
│   ├── PlayerManager.cs             # Player speed, size, invincibility
│   └── WorldManager.cs              # Season, weather, time, structure overrides
├── Systems/           # Standalone subsystems
│   ├── StaminaBarHolder.cs   # Mount stamina visual overlay
│   ├── TTSManager.cs         # Windows text-to-speech engine
│   └── Accessibility/
│       └── RadarSystem.cs    # POI distance scanning (portals, chests, vagrants)
├── UI/                # User interface
│   ├── ModMenu.cs            # Main IMGUI menu hub (F1) — lifecycle + rendering
│   └── ModMenuFeatures.cs    # All feature metadata registrations (BuildFeatureMetadata)
├── Shared/            # Cross-cutting utilities and compat layer
│   ├── Attributes/
│   │   └── RegisterTypeInIl2Cpp.cs  # [RegisterTypeInIl2Cpp] — auto IL2CPP type registration
│   ├── CachePrefabID.cs
│   ├── GameExtensions.cs     # Game object queries, Color.WithAlpha
│   ├── GameObjectDetails.cs  # Deep game-object introspection
│   ├── GamePrefabID.cs       # Prefab ID enum (76 values)
│   ├── GuiHelper.cs          # IMGUI drawing helpers
│   ├── NullableAttributes.cs # Polyfill for netstandard2.1 nullable types
│   ├── ObjectExtensions.cs   # Reflection helpers (GetField/SetField/CallMethod)
│   └── TypeExtensions.cs
└── Utils/             # Utility classes
    └── PayableNameResolver.cs # Smart naming for in-game interactable objects
```

## Data Flow

```
BepInEx ConfigFile (Settings.cs)
        │  LoadFromSettings() / SaveToSettings()
        ▼
  ModMenu.cs (persistent + runtime state)
        │
        ├──▶ ModMenuFeatures.cs  (feature metadata registry)
        ├──▶ Features/*.cs       (read ModMenu static state)
        ├──▶ Hooks/*.cs          (read Settings config at patch time)
        ├──▶ Systems/*.cs        (independent subsystems)
        └──▶ UI/ModMenu.cs       (IMGUI rendering)
```

## Key Class Dependencies

- `Plugin.cs` → **Dual base-class**: `#if IL2CPP: BasePlugin / #else: BaseUnityPlugin`. Calls `RegisterTypeInIl2Cpp.RegisterAssembly()` on IL2CPP. Initializes `ModMenu` + `AccessibilityFeature` as GameObjects, sets up Harmony.
- `ModMenu.cs` → Central hub: manages all runtime state, renders IMGUI, coordinates features via `ModMenuFeatures.Build()`.
- `Settings.cs` → Single config entry point: all persistent settings via BepInEx `ConfigFile`. Bidirectionally synced with `ModMenu` static state.
- `RegisterTypeInIl2Cpp` → IL2CPP-only attribute. Scans assembly for annotated `MonoBehaviour` classes and auto-registers them with `ClassInjector`.
- `ArmyManager.cs` / `WorldManager.cs` / `PlayerManager.cs` → Feature execution layer — attached to a `DontDestroyOnLoad` GameObject by `PlayerSpawnerHook`.

## Adding a New Feature

1. Create a `MonoBehaviour` class under `Features/`
2. Add `[RegisterTypeInIl2Cpp]` attribute and `IntPtr` constructor (wrapped in `#if IL2CPP`):
   ```csharp
   #if IL2CPP
   [RegisterTypeInIl2Cpp]
   #endif
   public class MyFeature : MonoBehaviour
   {
   #if IL2CPP
       public MyFeature(IntPtr ptr) : base(ptr) { }
   #endif
   }
   ```
3. Add `ConfigEntry` fields in `Settings.cs` if persistence is needed
4. Register UI controls in `ModMenuFeatures.Build()`
5. If the feature needs to be present from game start, attach it in `PlayerSpawnerHook.SafeAdd<T>()`
6. Create Harmony patches if game method interception is required
