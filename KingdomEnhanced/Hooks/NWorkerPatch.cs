using HarmonyLib;

namespace KingdomEnhanced.Hooks
{
    [HarmonyPatch(typeof(Worker), "Awake")]
    public static class NWorkerPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Worker __instance)
        {
            // This triggers the logic whenever a Worker spawns/initializes
            KingdomEnhanced.Features.BuilderFeature.ApplyWorkerStats(__instance);
        }
    }
}