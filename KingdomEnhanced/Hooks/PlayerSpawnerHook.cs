using HarmonyLib;
using KingdomEnhanced.Features;
using KingdomEnhanced.UI;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace KingdomEnhanced.Hooks
{
    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    public static class PlayerSpawnerHook
    {
        private static bool _isInitialized = false;

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (__instance == null) return;

            if (_isInitialized) return;
            _isInitialized = true;

            var existing = Object.FindFirstObjectByType<WorldManager>();
            if (existing != null) return;

            KingdomEnhanced.Core.Plugin.Instance.LogSource.LogInfo("[KE] Player spawned. Attaching world managers...");

            var go = new UnityEngine.GameObject("KingdomEnhanced_WorldManagers");
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.hideFlags = UnityEngine.HideFlags.HideAndDontSave;

            SafeAdd<PlayerManager>(go);
            SafeAdd<WorldManager>(go);
            SafeAdd<ArmyManager>(go);
            SafeAdd<KingdomMonitor>(go);
        }

        private static void SafeAdd<T>(UnityEngine.GameObject go) where T : MonoBehaviour
        {
            try
            {
                go.AddComponent<T>();
                KingdomEnhanced.Core.Plugin.Instance?.LogSource.LogInfo($"[KE] Attached {typeof(T).Name}");
            }
            catch (Exception e)
            {
                KingdomEnhanced.Core.Plugin.Instance?.LogSource.LogWarning($"[KE] Failed to attach {typeof(T).Name}: {e.Message}");
            }
        }
    }
}
