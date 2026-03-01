using System;

namespace KingdomEnhanced.Features
{
    public static class HardModeFeature
    {
        public static HardModePreset GetActivePreset()
        {
            if (CampaignSaveData.current != null)
            {
                var diff = CampaignSaveData.current.difficultyLevel;
                var preset = (HardModePreset)(int)diff;
                if (Enum.IsDefined(typeof(HardModePreset), preset))
                {
                    return preset;
                }
            }
            return HardModePreset.None;
        }
    }
}
