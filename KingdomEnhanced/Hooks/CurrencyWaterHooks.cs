using HarmonyLib;
using KingdomEnhanced.UI; 

namespace KingdomEnhanced.Hooks
{
    
    [HarmonyPatch(typeof(CurrencyManagerExt), "CanDropInWater")]
    public class CoinBuoyancyPatch
    {
        
        
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            
            if (ModMenu.CoinsStayDry)
            {
                
                __result = false; 
            }
        }
    }
}