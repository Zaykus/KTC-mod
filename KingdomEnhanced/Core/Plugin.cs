using System;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

#if IL2CPP
using BepInEx.Unity.IL2CPP;
using KingdomEnhanced.Shared.Attributes;
#endif

#if MONO
using BepInEx.Unity.Mono;
#endif

using KingdomEnhanced.Features;
using KingdomEnhanced.UI;
using KingdomEnhanced.Systems;
using KingdomEnhanced.Hooks;

namespace KingdomEnhanced.Core
{
    [BepInPlugin("kingdomenhanced", "Kingdom Enhanced", ModVersion.FULL)]
    public class Plugin :
#if IL2CPP
        BasePlugin
#else
        BaseUnityPlugin
#endif
    {
        public static Plugin Instance;

        public ManualLogSource LogSource
#if IL2CPP
            => Log;
#else
            => Logger;
#endif

#if IL2CPP
        public override void Load()
        {
            RegisterTypeInIl2Cpp.RegisterAssembly(Assembly.GetExecutingAssembly());

            Init();
        }
#else
        internal void Awake()
        {
            Init();
        }
#endif

        private void Init()
        {
            Instance = this;
            LogSource.LogInfo($"Kingdom Enhanced {ModVersion.DISPLAY} loaded!");
            Settings.Init(Config);
            StaminaBarHolder.Initialize();

            var go = new GameObject("KingdomEnhanced_UI");
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;
            go.AddComponent<ModMenu>();
            go.AddComponent<AccessibilityFeature>();

            var harmony = new Harmony("kingdomenhanced.harmony");
            harmony.PatchAll();
            UnitCachePatches.ApplyAll(harmony);
            LabPatches.ApplyAll(harmony);
            LogSource.LogInfo("Kingdom Enhanced ready. Waiting for player spawn...");
        }
    }
}
