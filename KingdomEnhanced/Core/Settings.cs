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

            ShowStaminaBar = config.Bind("1. Visuals", "ShowStaminaBar", true, "Show the energy bar");
            BarStyle = config.Bind("1. Visuals", "BarStyle", 0, "Visual style index");
            BarPosition = config.Bind("1. Visuals", "BarPosition", 0, "Position index");
            ManualX = config.Bind("1. Visuals", "ManualX", 500f, "Custom X");
            ManualY = config.Bind("1. Visuals", "ManualY", 500f, "Custom Y");
            DisplayTimes = config.Bind("1. Visuals", "DisplayTimes", true, "Show day and time");

            EnableAccessibility = config.Bind("2. Accessibility", "EnableScreenReader", false, "Enable vocal feedback");

            SpeedMultiplier = config.Bind("3. Gameplay", "SpeedMultiplier", 1.5f, "Travel speed");
            CheatsUnlocked = config.Bind("3. Gameplay", "CheatsUnlocked", false, "Unlock menu");
            InfiniteStamina = config.Bind("3. Gameplay", "InfiniteStamina", true, "Unlimited energy");
            InvincibleWalls = config.Bind("3. Gameplay", "InvincibleWalls", false, "Self-repairing walls");
            HyperBuilders = config.Bind("3. Gameplay", "HyperBuilders", false, "Instant build");
            BetterKnight = config.Bind("3. Gameplay", "BetterKnight", false, "Elite Knights");
            LargerCamps = config.Bind("3. Gameplay", "LargerCamps", false, "Large vagrant camps");
            BetterCitizenHouses = config.Bind("3. Gameplay", "BetterCitizenHouses", false, "Rapid housing");
            EnableSizeHack = config.Bind("3. Gameplay", "EnableSizeHack", false, "Size modification");
            TargetSize = config.Bind("3. Gameplay", "TargetSize", 1.0f, "Size scale");

            LockSummer = config.Bind("4. World", "LockSummer", false, "Persistent Summer");
            ClearWeather = config.Bind("4. World", "ClearWeather", false, "Clear skies");
            NoBloodMoons = config.Bind("4. World", "NoBloodMoons", false, "No red moons");
            CoinsStayDry = config.Bind("4. World", "CoinsStayDry", false, "Floating coins");
        }
    }
}