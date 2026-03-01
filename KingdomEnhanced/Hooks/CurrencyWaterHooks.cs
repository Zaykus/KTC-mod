using HarmonyLib;
using KingdomEnhanced.UI; 

namespace KingdomEnhanced.Hooks
{
    // Targets the specific class and method in the Kingdom Two Crowns assembly
    [HarmonyPatch(typeof(CurrencyManagerExt), "CanDropInWater")]
    public class CoinBuoyancyPatch
    {
        // The Postfix runs immediately after the original CanDropInWater method finishes.
        // By passing 'ref bool __result', we can read and change what the game returns.
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            // Check your custom UI toggle
            if (ModMenu.CoinsStayDry)
            {
                // Overrides the game's decision, forcing it to keep the coin dry
                __result = false; 
            }
        }
    }
}