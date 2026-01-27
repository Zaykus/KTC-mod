using BepInEx.Configuration;

namespace KingdomEnhanced.Core
{
    public static class Settings
    {
        public static ConfigFile Config;

        // Visuals
        public static ConfigEntry<bool> ShowStaminaBar;
        public static ConfigEntry<int> BarStyle;
        public static ConfigEntry<int> BarPosition;
        public static ConfigEntry<float> ManualX;
        public static ConfigEntry<float> ManualY;

        // Accessibility
        public static ConfigEntry<bool> EnableAccessibility;

        // Gameplay
        public static ConfigEntry<float> SpeedMultiplier;
        public static ConfigEntry<bool> CheatsUnlocked;

        public static void Init(ConfigFile config)
        {
            Config = config;

            // Define the Save File structure
            ShowStaminaBar = config.Bind("1. Visuals", "ShowStaminaBar", true, "Show the stamina bar?");
            BarStyle = config.Bind("1. Visuals", "BarStyle", 0, "0=Classic, 1=RPG, 2=Retro, 3=Dual");
            BarPosition = config.Bind("1. Visuals", "BarPosition", 0, "0=Head, 1=Feet, 2=Bottom, 3=Left, 4=Right, 5=Manual");
            ManualX = config.Bind("1. Visuals", "ManualX", 500f, "Custom X Position");
            ManualY = config.Bind("1. Visuals", "ManualY", 500f, "Custom Y Position");

            EnableAccessibility = config.Bind("2. Accessibility", "EnableScreenReader", false, "Enable text logging for screen readers");

            SpeedMultiplier = config.Bind("3. Gameplay", "SpeedMultiplier", 1.5f, "Player movement speed");
            CheatsUnlocked = config.Bind("3. Gameplay", "CheatsUnlocked", false, "Have the cheat tabs been verified?");
        }
    }
}