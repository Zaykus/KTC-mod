using HarmonyLib;
using UnityEngine;
using Coatsink.Common;
using System.Reflection;
using KingdomEnhanced.Core;

namespace KingdomEnhanced.Hooks
{
    [HarmonyPatch(typeof(CurrencyBag), "Init")]
    public static class CurrencyHooks
    {
        [HarmonyPrefix]
        public static void Prefix(CurrencyBag __instance)
        {
            try
            {
                // 1. Hack Overflow Limit
                var field = typeof(CurrencyBag).GetField("OVERFLOW_LIMIT", BindingFlags.NonPublic | BindingFlags.Static);
                if (field != null) field.SetValue(null, 9999);

                // 2. Shrink Bag & Coins (Robust Loop)
                if (Managers.Inst != null && Managers.Inst.currency != null)
                {
                    foreach (var type in CurrencyManager.AllCurrencyTypes)
                    {
                        if (Managers.Inst.currency.TryGetData(type, out var config))
                        {
                            // Shrink Main Bag Prefab
                            if (config.BagPrefab != null)
                            {
                                config.BagPrefab.transform.localScale = new Vector3(0.668f, 0.668f, 0.668f);
                                
                                // IMPORTANT: Handle Biome Swaps (Dead Lands, Shogun, etc.)
                                // This ensures the bag stays small even if the theme changes
                                var swap = BiomeData.GetPrefabSwap(config.BagPrefab);
                                if (swap != null)
                                    swap.transform.localScale = new Vector3(0.668f, 0.668f, 0.668f);
                            }

                            // Shrink Coins (Physics)
                            if (config.Prefab != null)
                            {
                                if (type == CurrencyType.Coins || type == CurrencyType.Gems)
                                {
                                    config.Prefab.transform.localScale = new Vector3(0.35f, 0.35f, 1f);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Silent catch to prevent crash
                Plugin.Instance.Log.LogWarning($"CurrencyHook Error: {ex.Message}");
            }
        }
    }
}