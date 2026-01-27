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
    [BepInPlugin("kingdomenhanced", "Kingdom Enhanced", "2.0.0")] // Bumped version to 2.0
    public class Plugin : BasePlugin
    {
        public static Plugin Instance;

        public override void Load()
        {
            Instance = this;
            Log.LogInfo("Initializing Kingdom Enhanced 2.0 (Manager Edition)...");

            // 1. SETTINGS
            Settings.Init(Config);

            // 2. REGISTER TYPES (Crucial: Updated to match new Managers)
            // We register the Managers so IL2CPP knows they exist when we AddComponent()
            ClassInjector.RegisterTypeInIl2Cpp<PlayerManager>();
            ClassInjector.RegisterTypeInIl2Cpp<WorldManager>();
            ClassInjector.RegisterTypeInIl2Cpp<ArmyManager>();
            ClassInjector.RegisterTypeInIl2Cpp<BuildingManager>();
            
            // Register UI & Systems
            ClassInjector.RegisterTypeInIl2Cpp<ModMenu>();
            ClassInjector.RegisterTypeInIl2Cpp<StaminaBarHolder>();
            ClassInjector.RegisterTypeInIl2Cpp<AccessibilityFeature>(); // Keeping this separate is fine

            // 3. UI INITIALIZATION
            StaminaBarHolder.Initialize();

            // 4. APPLY HARMONY PATCHES
            var harmony = new Harmony("kingdomenhanced.harmony");
            harmony.PatchAll();

            Log.LogInfo("Kingdom Enhanced initialized. Waiting for Player Spawn...");
        }
    }
}