# Project Architecture

## Overview

Kingdom Enhanced is a BepInEx IL2CPP mod for Kingdom Two Crowns, providing quality-of-life improvements, cheat capabilities, and accessibility features.

## Tech Stack

- **Runtime**: .NET 6.0 (net6.0-windows)
- **Mod Framework**: BepInEx IL2CPP 6.0+
- **Hooking**: Harmony
- **UI**: Unity IMGUI (OnGUI)
- **Game Engine**: Unity IL2CPP (Unity 6)

## Project Layers

```
KingdomEnhanced/
├── Core/           # Entry point and configuration
│   ├── Plugin.cs       # BepInEx plugin entry
│   ├── Settings.cs     # Config management (BepInEx ConfigFile)
│   └── ModVersion.cs   # Version constants
├── Hooks/          # Harmony patches (game method hooks)
│   ├── AbilityHooks.cs
│   ├── CurrencyHooks.cs
│   ├── CurrencyWaterHooks.cs
│   ├── LabPatches.cs
│   ├── MapHooks.cs
│   └── PlayerSpawnerHook.cs
├── Features/       # Feature modules (MonoBehaviours)
│   ├── AccessibilityFeature.cs  # Accessibility assistance
│   ├── ArmyManager.cs           # Army management
│   ├── KingdomMonitor.cs        # Live stats dashboard
│   ├── PlayerManager.cs         # Player modifications
│   ├── WorldManager.cs          # World modifications
│   ├── HardModeFeature.cs       # Hard mode
│   └── ...
├── Systems/        # Standalone subsystems
│   ├── StaminaBarHolder.cs  # Stamina bar overlay
│   ├── TTSManager.cs        # Text-to-speech
│   └── Accessibility/
│       └── RadarSystem.cs   # Radar scanning
├── UI/             # User interface
│   ├── ModMenu.cs           # Main menu (hub)
│   └── ModMenuFeatures.cs   # Feature metadata registry
├── Shared/         # Shared utilities
│   ├── GameExtensions.cs    # Game object query helpers
│   ├── GuiHelper.cs         # UI utility methods
│   ├── ObjectExtensions.cs  # Object extensions
│   └── ...
└── Utils/          # Utility classes
    └── PayableNameResolver.cs  # Payable object naming
```

## Data Flow

```
BepInEx ConfigFile (Settings.cs)
        │
        ▼
  ModMenu.cs (runtime state)
        │
        ├──▶ Features/*.cs    (read ModMenu static state)
        ├──▶ Hooks/*.cs       (read Settings config)
        ├──▶ Systems/*.cs     (independent subsystems)
        └──▶ UI/ModMenu.cs    (IMGUI rendering)
```

## Key Class Dependencies

- `Plugin.cs` → Registers all `MonoBehaviour` types with IL2CPP, initializes Harmony
- `ModMenu.cs` → Central hub: manages all static state, renders UI, coordinates features
- `Settings.cs` → Single config entry point: all persistent settings via BepInEx ConfigFile
- `ArmyManager.cs` / `WorldManager.cs` / `PlayerManager.cs` → Feature execution layer

## Adding a New Feature

1. Create a new `MonoBehaviour` class under `Features/`
2. Add `ConfigEntry` fields in `Settings.cs` if config is needed
3. Register UI controls in `ModMenuFeatures.Build()`
4. Register the type in `Plugin.cs`: `ClassInjector.RegisterTypeInIl2Cpp<T>()`
5. Create Harmony patches if required
