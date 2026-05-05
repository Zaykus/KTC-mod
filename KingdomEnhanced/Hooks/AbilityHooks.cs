using HarmonyLib;
using KingdomEnhanced.Core;
using KingdomEnhanced.UI;
using UnityEngine;

namespace KingdomEnhanced.Hooks
{
    public static class AbilityHooks
    {
        [HarmonyPatch(typeof(HelsHead), "TriggerItemAbility")]
        public static class HelsHeadPatch
        {
            [HarmonyPrefix]
            public static void Prefix(HelsHead __instance)
            {
                if (!ModMenu.CheatsUnlocked) return;
                __instance._vanguardsToSpawn = 12;
                __instance._archersToSpawn = 12;
            }
        }

        [HarmonyPatch(typeof(ThorItem), "TriggerItemAbility")]
        public static class ThorHammerPatch
        {
            [HarmonyPrefix]
            public static void Prefix(ThorItem __instance)
            {
                if (!ModMenu.CheatsUnlocked) return;
                __instance._lightningStrikeDamage = 100;
                __instance._lightningStrikeDamageRange = 15;
                __instance._numberOfLightningStrikes = 8;
                __instance._delayBeforeStrikes = 2;
            }
        }

        [HarmonyPatch(typeof(Steed), "Update")]
        public static class SteedSpeedPatch
        {
            public static readonly System.Collections.Generic.Dictionary<int, float> _runCache = new();
            public static readonly System.Collections.Generic.Dictionary<int, float> _walkCache = new();
            
            [HarmonyPostfix]
            public static void Postfix(Steed __instance)
            {
                if (__instance == null) return;

                float travelMult = ModMenu.SpeedMultiplier;
                int id = __instance.GetInstanceID();
                
                if (!_runCache.ContainsKey(id))
                {
                    if (__instance.runSpeed <= 0.1f) goto stamina;
                    _runCache[id] = __instance.runSpeed;
                    _walkCache[id] = __instance.walkSpeed;
                }
                
                __instance.runSpeed = _runCache[id] * travelMult;
                __instance.walkSpeed = _walkCache[id] * travelMult;

                stamina:
                if (ModMenu.InfiniteStamina && __instance.Rider != null)
                {
                    __instance.Stamina = 100f;
                    __instance._tiredTimer = -1f;
                }
            }

            public static void ClearCache(int id)
            {
                _runCache.Remove(id);
                _walkCache.Remove(id);
            }
        }

        [HarmonyPatch(typeof(Player), "Ride")]
        public static class PlayerRidePatch
        {
            [HarmonyPrefix]
            public static void Prefix(Steed steed)
            {
                if (steed != null)
                {
                    SteedSpeedPatch.ClearCache(steed.GetInstanceID());
                }
            }
        }


    }
}