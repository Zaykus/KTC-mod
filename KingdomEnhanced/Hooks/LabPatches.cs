using HarmonyLib;
using KingdomEnhanced.UI;
using UnityEngine;
using System;
using System.Reflection;

namespace KingdomEnhanced.Hooks
{
    /// <summary>
    /// Contains all Lab feature patches. Applied manually (not via PatchAll)
    /// so that missing methods don't crash the entire mod.
    /// v3.1: Fixed IL2CPP method names (SlowUpdate, ReceiveDamage, StartRedMoon, CheckSeasonalEvents)
    /// </summary>
    public static class LabPatches
    {
        public static void ApplyAll(Harmony harmony)
        {
            // v3.1: Correct IL2CPP method names
            TryPatch(harmony, "Season Lock", typeof(SeasonalEventManager), "CheckSeasonalEvents",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(SeasonPrefix)));

            TryPatch(harmony, "Clear Weather", typeof(Precipitation), "SlowUpdate",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(WeatherPrefix)));

            TryPatch(harmony, "No Blood Moons", typeof(EnemyManager), "StartRedMoon",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(BloodMoonPrefix)));

            TryPatch(harmony, "Wall Repair", typeof(Damageable), "ReceiveDamage",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(WallPrefix)));
        }

        private static void TryPatch(Harmony harmony, string name, Type type, string method,
            HarmonyMethod prefix = null, HarmonyMethod postfix = null)
        {
            try
            {
                var target = AccessTools.Method(type, method);
                if (target == null)
                {
                    Core.Plugin.Instance.Log.LogWarning($"[Lab] '{name}' skipped — {type.Name}.{method} not found");
                    return;
                }
                harmony.Patch(target, prefix: prefix, postfix: postfix);
                Core.Plugin.Instance.Log.LogInfo($"[Lab] '{name}' patch applied successfully");
            }
            catch (Exception ex)
            {
                Core.Plugin.Instance.Log.LogWarning($"[Lab] '{name}' patch failed: {ex.Message}");
            }
        }

        // --- Patch Methods ---

        public static bool SeasonPrefix()
        {
            return !ModMenu.LockSummer;
        }

        public static bool WeatherPrefix(Precipitation __instance)
        {
            if (!ModMenu.ClearWeather) return true;
            var renderers = __instance.GetComponentsInChildren<Renderer>();
            if (renderers != null)
            {
                foreach (var r in renderers)
                {
                    if (r.enabled) r.enabled = false;
                }
            }
            return false;
        }

        public static bool BloodMoonPrefix()
        {
            return !ModMenu.NoBloodMoons;
        }

        public static bool WallPrefix(Damageable __instance)
        {
            if (!ModMenu.InvincibleWalls) return true;
            var wall = __instance.GetComponent<Wall>();
            if (wall == null) return true;
            // Skip damage calculation for walls
            return false;
        }
    }
}
