using System;
using HarmonyLib;
using KingdomEnhanced.Core;
using UnityEngine;

namespace KingdomEnhanced.Features
{
    public class DifficultyIntegration
    {
        [HarmonyPatch(typeof(BiomeHolder), "Awake")]
        public static class BiomeHolder_Awake_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(BiomeHolder __instance)
            {
                try
                {
                    Plugin.Instance.LogSource.LogInfo("BiomeHolder Awake: injecting custom difficulty levels...");

                    var difficulties = __instance.difficultyLevels;

                    foreach (HardModePreset preset in Enum.GetValues(typeof(HardModePreset)))
                    {
                        if (preset == HardModePreset.None) continue;

                        var level = (DifficultyData.DifficultyLevel)(int)preset;

                        if (!difficulties.ContainsKey(level))
                        {
                            var data = HardModePresets.CreateDifficultyData(preset);
                            difficulties.Add(level, data);
                            Plugin.Instance.LogSource.LogInfo($"Injected Difficulty: {data.difficultyName} (ID: {(int)level})");
                        }
                    }
                }
                catch (Exception e)
                {
                    Plugin.Instance.LogSource.LogError($"Failed to inject difficulty levels: {e}");
                }
            }
        }
    }
}
