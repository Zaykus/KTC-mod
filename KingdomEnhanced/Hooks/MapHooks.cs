using HarmonyLib;
using KingdomEnhanced.Systems;
using KingdomEnhanced.UI; // Needed for ModMenu
using UnityEngine;

namespace KingdomEnhanced.Hooks
{
    [HarmonyPatch(typeof(UIMainMap), "SelectLand")]
    public static class MapHooks
    {
        public static void Postfix(int index)
        {
            if (ModMenu.EnableAccessibility)
            {
                TTSManager.Speak($"Island {index + 1}", interrupt: true);
            }
            }
        }


    [HarmonyPatch(typeof(UIMainMapLand), "SelectButton")]
    public static class MapLandHoverHook
    {
        public static void Postfix(UIMainMapLand __instance)
        {
            if (ModMenu.EnableAccessibility)
            {
                // Announce element name (e.g. "Island 1", "Island 2")
                TTSManager.Speak(__instance.name, interrupt: true);
            }
        }
    }
}
