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
        public static ConfigEntry<bool> DisplayTimes;

        // Accessibility
        public static ConfigEntry<bool> EnableAccessibility;

        // Gameplay / Cheats
        public static ConfigEntry<float> SpeedMultiplier;
        public static ConfigEntry<bool> CheatsUnlocked;
        public static ConfigEntry<bool> InfiniteStamina;
        public static ConfigEntry<bool> InvincibleWalls;
        public static ConfigEntry<bool> HyperBuilders;
        public static ConfigEntry<bool> BetterKnight;
        public static ConfigEntry<bool> LargerCamps;
        public static ConfigEntry<bool> BetterCitizenHouses;
        public static ConfigEntry<bool> EnableSizeHack;
        public static ConfigEntry<float> TargetSize;

        // World
        public static ConfigEntry<bool> LockSummer;
        public static ConfigEntry<bool> ClearWeather;
        public static ConfigEntry<bool> NoBloodMoons;
        public static ConfigEntry<bool> CoinsStayDry;

        public static void Init(ConfigFile config)
        {
            Config = config;

            // 1. Visuals
            ShowStaminaBar = config.Bind("1. Visuals", "ShowStaminaBar", true, "Show the stamina bar?");
            BarStyle = config.Bind("1. Visuals", "BarStyle", 0, "0=Classic, 1=RPG, 2=Retro, 3=Dual");
            BarPosition = config.Bind("1. Visuals", "BarPosition", 0, "0=Head, 1=Feet, 2=Bottom, 3=Left, 4=Right, 5=Manual");
            ManualX = config.Bind("1. Visuals", "ManualX", 500f, "Custom X Position");
            ManualY = config.Bind("1. Visuals", "ManualY", 500f, "Custom Y Position");
            DisplayTimes = config.Bind("1. Visuals", "DisplayTimes", false, "Show Day and Time?");

            // 2. Accessibility
            EnableAccessibility = config.Bind("2. Accessibility", "EnableScreenReader", false, "Enable text logging");

            // 3. Gameplay
            SpeedMultiplier = config.Bind("3. Gameplay", "SpeedMultiplier", 1.5f, "Player movement speed");
            CheatsUnlocked = config.Bind("3. Gameplay", "CheatsUnlocked", false, "Cheats enabled?");
            InfiniteStamina = config.Bind("3. Gameplay", "InfiniteStamina", true, "Unlimited Horse Stamina");
            InvincibleWalls = config.Bind("3. Gameplay", "InvincibleWalls", false, "Walls heal automatically");
            HyperBuilders = config.Bind("3. Gameplay", "HyperBuilders", false, "Fast building speed");
            BetterKnight = config.Bind("3. Gameplay", "BetterKnight", false, "Stronger Knights");
            LargerCamps = config.Bind("3. Gameplay", "LargerCamps", false, "Larger Beggar Camps");
            BetterCitizenHouses = config.Bind("3. Gameplay", "BetterCitizenHouses", false, "Houses auto-spawn");
            EnableSizeHack = config.Bind("3. Gameplay", "EnableSizeHack", false, "Change player size");
            TargetSize = config.Bind("3. Gameplay", "TargetSize", 1.0f, "Player Size Multiplier");

            // 4. World
            LockSummer = config.Bind("4. World", "LockSummer", false, "Lock season to Summer");
            ClearWeather = config.Bind("4. World", "ClearWeather", false, "Force clear weather");
            NoBloodMoons = config.Bind("4. World", "NoBloodMoons", false, "Disable Blood Moon");
            CoinsStayDry = config.Bind("4. World", "CoinsStayDry", false, "Prevent coins from sinking");
        }
    }
}