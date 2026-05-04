namespace KingdomEnhanced.Features
{
    public static class DifficultyRules
    {
        public static bool IsHardModeActive()
        {
            return HardModeFeature.GetActivePreset() != HardModePreset.None;
        }

        public static bool CanAddCoins()
        {
            return true;
        }

        public static bool CanAddGems()
        {
            return true;
        }

        public static bool CanUseInvincibleWalls()
        {
            return true;
        }

        public static bool CanUseNoBloodMoons()
        {
            return true;
        }


        public static bool CanUseInstantConstruction()
        {
            return KingdomEnhanced.UI.ModMenu.HyperBuilders && !IsHardModeActive();
        }


        public static float GetBuilderSpeedMultiplier()
        {
            return 1.0f;
        }

        public static float GetBuilderWorkTime()
        {
            var preset = HardModeFeature.GetActivePreset();
            switch (preset)
            {
                case HardModePreset.NoEscape:   return 1.0f;
                case HardModePreset.Nightmare:  return 0.3f;
                case HardModePreset.Relentless: return 0.001f;
                default: return 0.001f;
            }
        }

        public static int GetMaxRecruitPerUse()
        {
            var preset = HardModeFeature.GetActivePreset();
            switch (preset)
            {
                case HardModePreset.NoEscape:  return 5;
                case HardModePreset.Nightmare: return 10;
                default: return int.MaxValue;
            }
        }

        public static string ActivePresetName
        {
            get
            {
                switch (HardModeFeature.GetActivePreset())
                {
                    case HardModePreset.Nightmare:  return "NIGHTMARE";
                    case HardModePreset.Relentless: return "RELENTLESS";
                    case HardModePreset.Oblivion:   return "OBLIVION";
                    case HardModePreset.NoEscape:   return "NO ESCAPE";
                    default: return null;
                }
            }
        }

        public static int ActiveRestrictionCount
        {
            get
            {
                if (!IsHardModeActive()) return 0;
                int n = 4;
                if (GetMaxRecruitPerUse() < int.MaxValue) n++;
                return n;
            }
        }

        public static string GetDifficultyBannerLabel()
        {
            string name = ActivePresetName;
            if (name == null) return null;
            return $"[!] {name} -- {ActiveRestrictionCount} Restrictions Active";
        }
    }
}
