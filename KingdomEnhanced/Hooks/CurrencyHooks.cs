using HarmonyLib;
using UnityEngine;
using Coatsink.Common;
using System.Reflection;
using KingdomEnhanced.Core;
using KingdomEnhanced.UI;

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
                var field = typeof(CurrencyBag).GetField("OVERFLOW_LIMIT", BindingFlags.NonPublic | BindingFlags.Static);
                if (field != null) field.SetValue(null, 9999);

                if (Managers.Inst != null && Managers.Inst.currency != null)
                {
                    foreach (var type in CurrencyManager.AllCurrencyTypes)
                    {
                        if (Managers.Inst.currency.TryGetData(type, out var config))
                        {
                            if (config.BagPrefab != null)
                            {
                                config.BagPrefab.transform.localScale = new Vector3(0.668f, 0.668f, 0.668f);
                                
                                var swap = BiomeData.GetPrefabSwap(config.BagPrefab);
                                if (swap != null)
                                    swap.transform.localScale = new Vector3(0.668f, 0.668f, 0.668f);
                            }

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
                Plugin.Instance.Log.LogWarning($"CurrencyHook Error: {ex.Message}");
            }
        }
    }

    [HarmonyPatch(typeof(Wallet), "AddCurrency", typeof(CurrencyType), typeof(int))]
    public static class AddCurrencyPatch
    {
        [HarmonyPrefix]
        public static void Prefix(ref int amountToAdd, CurrencyType currencyType)
        {
            if (currencyType == CurrencyType.Coins && ModMenu.CoinIncomeMult != 1.0f && ModMenu.CoinIncomeMult > 0f)
            {
                amountToAdd = Mathf.RoundToInt(amountToAdd * ModMenu.CoinIncomeMult);
            }
        }
    }
}