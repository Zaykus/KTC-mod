using HarmonyLib;
using KingdomEnhanced.Features;
using KingdomEnhanced.UI;
using UnityEngine;
using System;

namespace KingdomEnhanced.Hooks
{
    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    public static class PlayerSpawnerHook
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (__instance == null) return;
            GameObject go = __instance.gameObject;

            KingdomEnhanced.Core.Plugin.Instance.Log.LogInfo("Player Spawned. Attaching Managers...");

            // 1. Core UI & Accessibility
            SafeAdd<ModMenu>(go);
            SafeAdd<AccessibilityFeature>(go);

            // 2. The Big 4 Managers
            SafeAdd<PlayerManager>(go);
            SafeAdd<WorldManager>(go);
            SafeAdd<ArmyManager>(go);
            // BuildingManager removed — was empty stub
            
            // 3. Gameplay Enhancements
            SafeAdd<KingdomMonitor>(go);
            SafeAdd<CoinBuoyancy>(go);
        }

        // Helper to safely add components without crashing
        private static void SafeAdd<T>(GameObject go) where T : MonoBehaviour
        {
            try
            {
                if (go.GetComponent<T>() == null)
                    go.AddComponent<T>();
            }
            catch (Exception e)
            {
                KingdomEnhanced.Core.Plugin.Instance?.Log.LogWarning($"Failed to attach {typeof(T).Name}: {e.Message}");
            }
        }
    }
}