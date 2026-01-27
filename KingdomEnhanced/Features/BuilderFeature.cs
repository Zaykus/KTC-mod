using HarmonyLib;
using Il2CppInterop.Runtime;
using KingdomEnhanced.Core;
using KingdomEnhanced.UI;
using UnityEngine;

namespace KingdomEnhanced.Features
{
    // Harmony patch that runs when workers are first spawned (Awake)
    [HarmonyPatch(typeof(Worker), "Awake")]
    public static class BuilderFeature
    {
        private const float RunMultiplier = 5.0f;
        private const float WorkMultiplier = 0.01f;

        [HarmonyPostfix]
        public static void Postfix(Worker __instance)
        {
            if (__instance == null) return;

            // Apply speedup to all new workers if HyperBuilders is enabled
            if (ModMenu.HyperBuilders)
            {
                __instance.runSpeed *= RunMultiplier;
                __instance.workTime *= WorkMultiplier;

                Plugin.Instance?.Log.LogInfo($"[Harmony] FasterWorker applied to new Worker: {__instance.name}");
            }
        }

        // Static method for manual application to existing workers
        public static void ApplyWorkerStats(Worker worker)
        {
            if (worker == null || !ModMenu.HyperBuilders) return;

            worker.runSpeed *= RunMultiplier;
            worker.workTime *= WorkMultiplier;

            Plugin.Instance?.Log.LogInfo($"Applied stats to Worker: {worker.name}");
        }

        public static void ForceUpdateAllWorkers()
        {
            var workers = Object.FindObjectsOfType<Worker>();
            if (workers == null || workers.Length == 0) return;

            foreach (var worker in workers)
            {
                // Set directly, not multiply
                worker.runSpeed = 5.0f;
                worker.workTime = 0.01f;
                Plugin.Instance?.Log.LogInfo($"Force updated Worker: {worker.name} - runSpeed: {worker.runSpeed}, workTime: {worker.workTime}");
            }
            Plugin.Instance?.Log.LogInfo($"Force updated {workers.Length} existing workers.");
        }
    }

    // MonoBehaviour that periodically applies speedup to existing workers
    public class FasterWorkerCatchup : MonoBehaviour
    {
        private float _updateTimer = 0f;
        private const float CheckInterval = 1f;

        void Update()
        {
            if (!ModMenu.HyperBuilders) return;

            _updateTimer += Time.deltaTime;
            if (_updateTimer < CheckInterval) return;
            _updateTimer = 0f;

            ApplyToExistingWorkers();
        }

        private void ApplyToExistingWorkers()
        {
            try
            {
                var workers = Object.FindObjectsOfType<Worker>();
                if (workers == null || workers.Length == 0) return;

                foreach (var worker in workers)
                {
                    if (worker == null) continue;

                    // Always apply the speedup - don't check if already set
                    // This ensures it works even if applied after the worker was created
                    worker.runSpeed = 5.0f;
                    worker.workTime = 0.01f;
                    
                    Plugin.Instance?.Log.LogDebug($"Applied FasterWorker to: {worker.name} - runSpeed: {worker.runSpeed}, workTime: {worker.workTime}");
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Instance?.Log.LogWarning($"FasterWorkerCatchup error: {ex.Message}");
            }
        }
    }
}