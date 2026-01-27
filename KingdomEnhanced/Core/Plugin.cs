using BepInEx;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using KingdomEnhanced.Features;
using KingdomEnhanced.UI;
using KingdomEnhanced.Systems;

namespace KingdomEnhanced.Core
{
    [BepInPlugin("kingdomenhanced", "Kingdom Enhanced", "1.0.0")]
    public class Plugin : BasePlugin
    {
        public static Plugin Instance;

        public override void Load()
        {
            Instance = this;
            Log.LogInfo("Initializing Kingdom Enhanced...");

            // 1. SETTINGS
            Settings.Init(Config);

            // 2. REGISTER TYPES FOR IL2CPP
            // This tells the game engine that these custom classes exist
            ClassInjector.RegisterTypeInIl2Cpp<StaminaFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<EconomyFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<SpeedFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<BuildingFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<RecruitFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<CitizenFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<BankerFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<WalletFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<AccessibilityFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<WorldControlFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<UpgradeFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerSizeFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<LargerCampsFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<BetterKnightFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<BetterCitizenHouseFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<DisplayTimesFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<CoinsStayDryFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<FasterWorkerCatchup>();
            ClassInjector.RegisterTypeInIl2Cpp<ModMenu>();
            ClassInjector.RegisterTypeInIl2Cpp<StaminaBarHolder>();

            // 3. UI INITIALIZATION
            StaminaBarHolder.Initialize();

            // 4. APPLY HARMONY PATCHES
            var harmony = new Harmony("kingdomenhanced.harmony");
            harmony.PatchAll();

            Log.LogInfo("Kingdom Enhanced initialized successfully! Features will attach when Player spawns.");
        }
    }
}