using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using KingdomEnhanced.Features;
using KingdomEnhanced.UI;
using KingdomEnhanced.Systems;
using KingdomEnhanced.Hooks;

namespace KingdomEnhanced.Core
{
    [BepInPlugin("kingdomenhanced", "Kingdom Enhanced", ModVersion.FULL)] // Bumped version to 2.0
    public class Plugin : BasePlugin
    {
        public static Plugin Instance;

        public override void Load()
        {
            Instance = this;
            Log.LogInfo($"Kingdom Enhanced {ModVersion.DISPLAY} loaded!");

            // 1. SETTINGS
            Settings.Init(Config);

            // 2. REGISTER TYPES (Crucial: Updated to match new Managers)
            // We register the Managers so IL2CPP knows they exist when we AddComponent()
            ClassInjector.RegisterTypeInIl2Cpp<PlayerManager>();
            ClassInjector.RegisterTypeInIl2Cpp<WorldManager>();
            ClassInjector.RegisterTypeInIl2Cpp<ArmyManager>();
            // BuildingManager removed — was empty stub
            
            // Register UI & Systems
            ClassInjector.RegisterTypeInIl2Cpp<ModMenu>();
            // StaminaBarHolder registers itself inside Initialize() below
            ClassInjector.RegisterTypeInIl2Cpp<AccessibilityFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<KingdomMonitor>();
            ClassInjector.RegisterTypeInIl2Cpp<CoinBuoyancy>();

            // 3. UI INITIALIZATION
            StaminaBarHolder.Initialize();

            // 4. APPLY HARMONY PATCHES
            var harmony = new Harmony("kingdomenhanced.harmony");
            harmony.PatchAll(); // Core patches (CurrencyHooks, PlayerSpawnerHook, etc.)

            // 5. APPLY LAB PATCHES (safe — each one wrapped in try-catch)
            LabPatches.ApplyAll(harmony);

            Log.LogInfo("Kingdom Enhanced initialized. Waiting for Player Spawn...");
        }
    }
}