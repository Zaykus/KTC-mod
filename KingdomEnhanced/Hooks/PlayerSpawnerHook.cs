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

            // Ensure the ModMenu is present
            if (__instance.GetComponent<ModMenu>() == null)
            {
                __instance.gameObject.AddComponent<ModMenu>();
            }

            // ATTACH ALL FEATURES
            // This uses your SafeAdd to prevent the game from crashing if one feature fails
            SafeAdd<StaminaFeature>(__instance.gameObject);
            SafeAdd<EconomyFeature>(__instance.gameObject);
            SafeAdd<SpeedFeature>(__instance.gameObject);
            SafeAdd<BuildingFeature>(__instance.gameObject);
            SafeAdd<RecruitFeature>(__instance.gameObject);
            SafeAdd<CitizenFeature>(__instance.gameObject);
            SafeAdd<BankerFeature>(__instance.gameObject);
            SafeAdd<WalletFeature>(__instance.gameObject);
            SafeAdd<AccessibilityFeature>(__instance.gameObject);
            SafeAdd<WorldControlFeature>(__instance.gameObject);
            SafeAdd<UpgradeFeature>(__instance.gameObject);
            SafeAdd<PlayerSizeFeature>(__instance.gameObject);

            // NEW FEATURES
            SafeAdd<BetterCitizenHouseFeature>(__instance.gameObject);
            SafeAdd<DisplayTimesFeature>(__instance.gameObject);
            SafeAdd<CoinsStayDryFeature>(__instance.gameObject);
            SafeAdd<LargerCampsFeature>(__instance.gameObject);
            SafeAdd<BetterKnightFeature>(__instance.gameObject);
            SafeAdd<FasterWorkerCatchup>(__instance.gameObject);
        }

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