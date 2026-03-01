using HarmonyLib;
using KingdomEnhanced.Systems;
using KingdomEnhanced.UI;
using UnityEngine;

namespace KingdomEnhanced.Hooks
{
    [HarmonyPatch(typeof(UIMainMap), "SelectLand")]
    public static class MapHooks
    {
        [HarmonyPostfix]
        public static void Postfix(int index)
        {
            if (ModMenu.EnableAccessibility)
                TTSManager.Speak($"Island {index + 1}", interrupt: true);
        }
    }

    [HarmonyPatch(typeof(UIMainMapLand), "SelectButton")]
    public static class MapLandHoverHook
    {
        [HarmonyPostfix]
        public static void Postfix(UIMainMapLand __instance)
        {
            if (ModMenu.EnableAccessibility)
                TTSManager.Speak(__instance.name, interrupt: true);
        }
    }
}
