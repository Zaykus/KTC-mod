using HarmonyLib;
using UnityEngine;
using Coatsink.Common;
using System.Reflection;
using KingdomEnhanced.Core;

namespace KingdomEnhanced.Hooks
{
    // This patch runs when the Currency Bag initializes.
    // It shrinks the Bag AND the Coin Prefabs so physics works correctly.
    [HarmonyPatch(typeof(CurrencyBag), "Init")]
    public static class CurrencyHooks
    {
        private static bool _hasAppliedSettings = false;

        [HarmonyPrefix]
        public static void Prefix(CurrencyBag __instance)
        {
            if (_hasAppliedSettings) return; // Only run once to save performance

            try
            {
                Plugin.Instance.Log.LogInfo("[CurrencyHooks] Applying Wallet & Coin modifications...");

                // 1. Hack the Overflow Limit (Allow 9999 coins)
                // We use Reflection because OVERFLOW_LIMIT is a private static field
                var field = typeof(CurrencyBag).GetField("OVERFLOW_LIMIT", BindingFlags.NonPublic | BindingFlags.Static);
                if (field != null)
                {
                    field.SetValue(null, 9999);
                    Plugin.Instance.Log.LogInfo("[CurrencyHooks] OVERFLOW_LIMIT set to 9999.");
                }

                // 2. Modify Sizes (The Abevol Method)
                if (Managers.Inst != null && Managers.Inst.currency != null)
                {
                    foreach (var type in CurrencyManager.AllCurrencyTypes)
                    {
                        if (Managers.Inst.currency.TryGetData(type, out var config))
                        {
                            // A. Shrink the Bag (Visuals)
                            if (config.BagPrefab != null)
                            {
                                // 0.668f is the magic number from Abevol's mod
                                config.BagPrefab.transform.localScale = new Vector3(0.668f, 0.668f, 0.668f);
                            }

                            // B. Shrink the Coin/Gem Object itself! (Physics)
                            // This is the CRITICAL part. If the coin is smaller, more fit in the bag.
                            if (config.Prefab != null)
                            {
                                if (type == CurrencyType.Coins)
                                {
                                    // Make coins 35% the size of normal
                                    config.Prefab.transform.localScale = new Vector3(0.35f, 0.35f, 1f);
                                    Plugin.Instance.Log.LogInfo("[CurrencyHooks] Coin Prefab shrunk to 0.35f");
                                }
                                else if (type == CurrencyType.Gems)
                                {
                                    config.Prefab.transform.localScale = new Vector3(0.35f, 0.35f, 1f);
                                }
                            }
                        }
                    }
                    _hasAppliedSettings = true;
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Instance.Log.LogError($"[CurrencyHooks] Error: {ex.Message}");
            }
        }
    }
}