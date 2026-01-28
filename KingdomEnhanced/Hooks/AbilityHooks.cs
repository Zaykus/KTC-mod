using HarmonyLib;
using KingdomEnhanced.Core;
using UnityEngine;

namespace KingdomEnhanced.Hooks
{
    public static class AbilityHooks
    {
        // 1. BUFF HEL'S HEAD
        [HarmonyPatch(typeof(HelsHead), "TriggerItemAbility")]
        public static class HelsHeadPatch
        {
            [HarmonyPrefix]
            public static void Prefix(HelsHead __instance)
            {
                __instance._vanguardsToSpawn = 12;
                __instance._archersToSpawn = 12;
            }
        }

        // 2. BUFF THOR'S HAMMER
        [HarmonyPatch(typeof(ThorItem), "TriggerItemAbility")]
        public static class ThorHammerPatch
        {
            [HarmonyPrefix]
            public static void Prefix(ThorItem __instance)
            {
                // Fix: Removed 'f' suffix. 100 works for both int and float.
                __instance._lightningStrikeDamage = 100;
                __instance._lightningStrikeDamageRange = 15; // Removed 'f' just in case
                __instance._numberOfLightningStrikes = 8;
                
                // This is definitely an int (frames)
                __instance._delayBeforeStrikes = 2;   
            }
        }
    }
}