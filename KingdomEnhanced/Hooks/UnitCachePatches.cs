using HarmonyLib;
using KingdomEnhanced.Features;

namespace KingdomEnhanced.Hooks
{
    public static class UnitCachePatches
    {
        public static void ApplyAll(Harmony harmony)
        {
            PatchAwake(harmony, typeof(Archer));
            PatchAwake(harmony, typeof(Worker));
            PatchAwake(harmony, typeof(Knight));
            PatchAwake(harmony, typeof(Ninja));
            PatchAwake(harmony, typeof(Berserker));
            PatchAwake(harmony, typeof(Castle));
            PatchAwake(harmony, typeof(BeggarCamp));
            PatchAwake(harmony, typeof(Enemy));
            PatchAwake(harmony, typeof(Peasant));
            PatchAwake(harmony, typeof(Farmer));
            PatchAwake(harmony, typeof(Pikeman));
            PatchAwake(harmony, typeof(Beggar));
            PatchAwake(harmony, typeof(Ballista));
            PatchAwake(harmony, typeof(Catapult));
            PatchAwake(harmony, typeof(Wall));
            PatchAwake(harmony, typeof(Portal));
        }

        private static void PatchAwake(Harmony harmony, System.Type type)
        {
            try
            {
                var method = AccessTools.Method(type, "Awake");
                if (method != null)
                {
                    harmony.Patch(method, postfix: new HarmonyMethod(typeof(UnitCachePatches), nameof(AwakePostfix)));
                }
            }
            catch (System.Exception ex)
            {
                KingdomEnhanced.Core.Plugin.Instance.LogSource.LogWarning($"[UnitCache] Failed to patch Awake on {type.Name}: {ex.Message}");
            }
        }

        public static void AwakePostfix(UnityEngine.MonoBehaviour __instance)
        {
            if (__instance != null)
            {
                UnitCacheRegistrar.EnsureAttached(__instance.gameObject);
            }
        }
    }
}
