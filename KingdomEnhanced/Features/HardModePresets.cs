using System.Collections.Generic;
using UnityEngine;

namespace KingdomEnhanced.Features
{
    public enum HardModePreset
    {
        None = 0,
        Nightmare = 100,
        Relentless = 101,
        Oblivion = 102,
        NoEscape = 103
    }

    public static class HardModePresets
    {
        public static readonly Dictionary<HardModePreset, string> PresetNames = new Dictionary<HardModePreset, string>
        {
            { HardModePreset.Nightmare, "Nightmare" },
            { HardModePreset.Relentless, "Relentless" },
            { HardModePreset.Oblivion, "Oblivion" },
            { HardModePreset.NoEscape, "No Escape" }
        };

        public static readonly Dictionary<HardModePreset, string> PresetDescriptions = new Dictionary<HardModePreset, string>
        {
            { HardModePreset.Nightmare, "Massive waves of enemies. Prepare your defenses." },
            { HardModePreset.Relentless, "Nights are longer. Enemies never retreat." },
            { HardModePreset.Oblivion, "The Blood Moon rises often. Retaliation is swift." },
            { HardModePreset.NoEscape, "Enemies are stronger, faster, and deadlier." }
        };

        public static DifficultyData CreateDifficultyData(HardModePreset preset)
        {
            var data = new DifficultyData();

            data.difficultyName = PresetNames[preset];
            data.difficultyLevel = (DifficultyData.DifficultyLevel)(int)preset;

            data.difficultyMultiplier = 1.0f;
            data.difficultyMultiplierBosses = 1.0f;
            data.retaliationMultiplier = 1.0f;

            switch (preset)
            {
                case HardModePreset.Nightmare:
                    break;
                case HardModePreset.Oblivion:
                    data.difficultyMultiplier = 5.0f;
                    data.retaliationMultiplier = 2.0f;
                    break;
                case HardModePreset.NoEscape:
                    data.difficultyMultiplierBosses = 2.0f;
                    break;
            }

            return data;
        }
    }
}
