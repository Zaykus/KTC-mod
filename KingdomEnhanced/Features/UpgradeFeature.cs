using UnityEngine;
using HarmonyLib;
using KingdomEnhanced.Core;
using Coatsink.Common;
using System;
public enum GamePrefabID
{
    Citizen_House = 0,
    Workshop = 1,
    Tower_Baker = 2,
    Tower0 = 3,
    Tower1 = 4,
    Tower2 = 5,
    Tower3 = 6
    // Add other prefab IDs as needed
}
namespace KingdomEnhanced.Features
{
    public class UpgradeFeature : MonoBehaviour
    {
        public static bool IsEnabled = true; // Make this a toggle later if you want
        private bool _hasRun = false;

        void Update()
        {
            // Only run once
            if (!IsEnabled || _hasRun) return;

            // Wait until the game's prefabs are loaded
            if (Managers.Inst != null && Managers.Inst.prefabs != null)
            {
                AdjustCosts();
                _hasRun = true;
            }
        }

        void AdjustCosts()
        {
            var prefabs = Managers.Inst.prefabs;

            // Using the exact IDs from your screenshot!
            ModifyPrice(prefabs, GamePrefabID.Citizen_House, 3);
            ModifyPrice(prefabs, GamePrefabID.Workshop, 3);
            ModifyPrice(prefabs, GamePrefabID.Tower_Baker, 2);
            ModifyPrice(prefabs, GamePrefabID.Tower0, 2);
            ModifyPrice(prefabs, GamePrefabID.Tower1, 4);
            ModifyPrice(prefabs, GamePrefabID.Tower2, 5);
            ModifyPrice(prefabs, GamePrefabID.Tower3, 8);

            Plugin.Instance.Log.LogInfo("BetterPayableUpgrade: Building costs have been reduced.");
        }

        // Removed an accidental stub that used `object` parameter types which prevented Il2Cpp registration.

        void ModifyPrice(PrefabManager pm, GamePrefabID id, int newPrice)
        {
            var go = pm.GetPrefabById((int)id);
            if (go == null) return;

            var payable = go.GetComponent<Payable>();
            if (payable != null)
            {
                payable.Price = newPrice;
            }
        }
    }

    // --- The Coin Shrinker (Abevol's Original Patch) ---
    [HarmonyPatch(typeof(CurrencyBag), "Init")]
    public static class CoinShrinkPatch
    {
        [HarmonyPrefix]
        public static void Prefix(CurrencyBag __instance)
        {
            if (Managers.Inst == null || Managers.Inst.currency == null) return;

            foreach (var type in CurrencyManager.AllCurrencyTypes)
            {
                if (Managers.Inst.currency.TryGetData(type, out var config))
                {
                    if (config.BagPrefab != null)
                    {
                        config.BagPrefab.transform.localScale = new Vector3(0.668f, 0.668f, 0.668f);
                    }
                }
            }
        }
    }

    // Add this enum at the top of the file or in a shared location if it already exists elsewhere
    
}