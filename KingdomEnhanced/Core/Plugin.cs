using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using KingdomEnhanced.Features;
using KingdomEnhanced.UI;
using KingdomEnhanced.Systems;
using KingdomEnhanced.Hooks;
using UnityEngine;

namespace KingdomEnhanced.Core
{
    [BepInPlugin("kingdomenhanced", "Kingdom Enhanced", ModVersion.FULL)] 
    public class Plugin : BasePlugin
    {
        public static Plugin Instance;
        public override void Load()
        {
            Instance = this;
            Log.LogInfo($"Kingdom Enhanced {ModVersion.DISPLAY} loaded!");
            Settings.Init(Config);
            ClassInjector.RegisterTypeInIl2Cpp<PlayerManager>();
            ClassInjector.RegisterTypeInIl2Cpp<WorldManager>();
            ClassInjector.RegisterTypeInIl2Cpp<ArmyManager>();
            ClassInjector.RegisterTypeInIl2Cpp<ModMenu>();
            ClassInjector.RegisterTypeInIl2Cpp<AccessibilityFeature>();
            ClassInjector.RegisterTypeInIl2Cpp<KingdomMonitor>();
            ClassInjector.RegisterTypeInIl2Cpp<HardModeBuffData>();
            StaminaBarHolder.Initialize();

            var go = new GameObject("KingdomEnhanced_UI");
            Object.DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;
            go.AddComponent<ModMenu>();
            go.AddComponent<AccessibilityFeature>();

            var harmony = new Harmony("kingdomenhanced.harmony");
            harmony.PatchAll(); 
            LabPatches.ApplyAll(harmony);
            Log.LogInfo("Kingdom Enhanced ready. Waiting for player spawn...");
        }
    }
}