using BepInEx.Configuration;
using UnityEngine;

namespace KingdomEnhanced.Core
{
    public static class Settings
    {
        public static ConfigFile Config;

        
        public static ConfigEntry<bool> UseBetaUI;
        public static ConfigEntry<bool> ShowStaminaBar;
        public static ConfigEntry<bool> DisplayTimes;
        public static ConfigEntry<bool> ShowGreedCounter;
        
        
        public static ConfigEntry<bool> EnableAccessibility;
        public static ConfigEntry<bool> EnableTTS;
        public static ConfigEntry<bool> NarratorQueueMode;
        public static ConfigEntry<bool> SimplifyNames;
        public static ConfigEntry<bool> EnableCastleAnnouncer;
        public static ConfigEntry<bool> DebugZones;

        
        public static ConfigEntry<bool> CheatsUnlocked;
        public static ConfigEntry<float> SpeedMultiplier;
        public static ConfigEntry<bool> InfiniteStamina;
        public static ConfigEntry<bool> InvincibleWalls;
        public static ConfigEntry<bool> NoToolCooldowns;
        public static ConfigEntry<float> ArtemisArrowCount;
        public static ConfigEntry<float> ArtemisRangeMult;
        public static ConfigEntry<float> ArtemisArrowDamageMult;
        public static ConfigEntry<float> CoinIncomeMult;
        public static ConfigEntry<float> BagDropMult;
        public static ConfigEntry<float> SpawnUnitCount;
        
        
        public static ConfigEntry<bool> HyperBuilders;
        public static ConfigEntry<bool> LargerCamps;
        public static ConfigEntry<bool> BetterKnight;
        public static ConfigEntry<bool> BetterCitizenHouses;
        public static ConfigEntry<bool> LockSummer;
        public static ConfigEntry<bool> ClearWeather;
        public static ConfigEntry<bool> NoBloodMoons;
        public static ConfigEntry<bool> CoinsStayDry;
        public static ConfigEntry<bool> ArcherFireBoost;
        public static ConfigEntry<bool> BerserkerRage;
        public static ConfigEntry<bool> NinjaSpeedBoost;
        public static ConfigEntry<int> RecruitCapOverride;
        public static ConfigEntry<float> TreeRegrowthMult;
        public static ConfigEntry<bool> AnimalSpawnBoost;
        public static ConfigEntry<bool> InstantDaySkip;
        public static ConfigEntry<bool> FarmOutputBoost;
        public static ConfigEntry<bool> TowerFireBoost;
        public static ConfigEntry<bool> BallistaBoost;
        public static ConfigEntry<float> BallistaReloadMult;
        public static ConfigEntry<float> BallistaFlightMult;
        public static ConfigEntry<bool> CatapultBoost;
        public static ConfigEntry<float> CatapultReloadMult;
        public static ConfigEntry<float> CatapultFlightMult;
        public static ConfigEntry<bool> InstantCastle;

        public static ConfigEntry<float> BuilderSpeedMult;
        public static ConfigEntry<float> BuilderWorkMult;

        
        public static ConfigEntry<bool> EnableSizeHack;
        public static ConfigEntry<float> TargetSize;

        
        public static ConfigEntry<float> SteedSpeedMult;
        public static ConfigEntry<bool> ChargeDmgBoost;
        public static ConfigEntry<float> BuffAuraDuration;

        
        public static ConfigEntry<float> WaveSizeMult;
        public static ConfigEntry<float> EnemySpeedMult;
        public static ConfigEntry<float> PortalSpawnRate;
        public static ConfigEntry<bool> NoCrownStealing;
        public static ConfigEntry<float> GreedQueenHPScale;
        public static ConfigEntry<float> DirectorThreatMult;


        public static void Init(ConfigFile config)
        {
            Config = config;

            
            UseBetaUI         = Config.Bind("1. Visuals", "UseBetaUI", false, "Use the new Beta UI style");
            ShowStaminaBar    = Config.Bind("1. Visuals", "ShowStaminaBar", false, "Show the energy/stamina bar");
            DisplayTimes      = Config.Bind("1. Visuals", "DisplayTimes", false, "Show day/night and coins HUD");
            ShowGreedCounter  = Config.Bind("1. Visuals", "ShowGreedCounter", false, "Show greed counter HUD");

            
            EnableAccessibility   = Config.Bind("2. Accessibility", "EnableAccessibility", true, "Enable logic for hover tracking");
            EnableTTS             = Config.Bind("2. Accessibility", "EnableTTS", true, "Enable text-to-speech output");
            NarratorQueueMode     = Config.Bind("2. Accessibility", "NarratorQueueMode", false, "Queue messages instead of interrupting");
            SimplifyNames         = Config.Bind("2. Accessibility", "SimplifyNames", true, "Simplify object names for TTS");
            EnableCastleAnnouncer = Config.Bind("2. Accessibility", "CastleAnnouncer", false, "Announce entering/leaving castle");
            DebugZones            = Config.Bind("2. Accessibility", "DebugZones", false, "Show visual debug boxes for announcer zones");

            
            CheatsUnlocked    = Config.Bind("3. Cheats", "CheatsUnlocked", false, "Unlock the cheat menu");
            SpeedMultiplier   = Config.Bind("3. Cheats", "SpeedMultiplier", 1.0f, "Travel speed multiplier");
            InfiniteStamina   = Config.Bind("3. Cheats", "InfiniteStamina", false, "Infinite mount stamina");
            InvincibleWalls   = Config.Bind("3. Cheats", "InvincibleWalls", false, "Walls repair instantly");
            NoToolCooldowns      = Config.Bind("3. Cheats", "NoToolCooldowns", false, "Disable item ability cooldowns");
            ArtemisArrowCount    = Config.Bind("3. Cheats", "ArtemisArrowCount", 6f, "Artemis Bow arrows fired per cast");
            ArtemisRangeMult     = Config.Bind("3. Cheats", "ArtemisRangeMult", 1.0f, "Artemis Bow ability range multiplier");
            ArtemisArrowDamageMult = Config.Bind("3. Cheats", "ArtemisArrowDamageMult", 1.0f, "Artemis Bow arrow damage multiplier");
            CoinIncomeMult    = Config.Bind("3. Cheats", "CoinIncomeMult", 1.0f, "Coin income multiplier (0.25-4.0)");
            BagDropMult       = Config.Bind("3. Cheats", "BagDropMult", 1.0f, "Bag drop multiplier (1-10)");
            SpawnUnitCount    = Config.Bind("3. Cheats", "SpawnUnitCount", 1.0f, "Number of units to spawn");

            
            HyperBuilders     = Config.Bind("4. Development", "HyperBuilders", false, "Instant construction");
            LargerCamps       = Config.Bind("4. Development", "LargerCamps", false, "Expand vagrant camps");
            BetterKnight      = Config.Bind("4. Development", "BetterKnight", false, "Elite knights buffs");
            BetterCitizenHouses = Config.Bind("4. Development", "BetterCitizenHouses", false, "Rapid housing");
            LockSummer        = Config.Bind("4. Development", "LockSummer", false, "Lock season to summer");
            ClearWeather      = Config.Bind("4. Development", "ClearWeather", false, "Force clear weather");
            NoBloodMoons      = Config.Bind("4. Development", "NoBloodMoons", false, "Disable blood moon events");
            CoinsStayDry      = Config.Bind("4. Development", "CoinsStayDry", false, "Coins don't sink in water");
            
            ArcherFireBoost    = Config.Bind("4. Development", "ArcherFireBoost", false, "Archer fire rate boost x2");
            BerserkerRage      = Config.Bind("4. Development", "BerserkerRage", false, "Berserker rage mode (earlier)");
            NinjaSpeedBoost    = Config.Bind("4. Development", "NinjaSpeedBoost", false, "Ninja speed boost x2");
            RecruitCapOverride = Config.Bind("4. Development", "RecruitCapOverride", 0, "Recruit cap override (0 = default)");
            TreeRegrowthMult   = Config.Bind("4. Development", "TreeRegrowthMult", 1.0f, "Tree regrowth speed multiplier");
            AnimalSpawnBoost   = Config.Bind("4. Development", "AnimalSpawnBoost", false, "Animal spawn boost");
            InstantDaySkip     = Config.Bind("4. Development", "InstantDaySkip", false, "Instant day skip");
            FarmOutputBoost    = Config.Bind("4. Development", "FarmOutputBoost", false, "Farm output boost x2");
            TowerFireBoost     = Config.Bind("4. Development", "TowerFireBoost", false, "Tower fire rate boost x2");
            BallistaBoost      = Config.Bind("4. Development", "BallistaBoost", false, "Ballista power boost x2");
            BallistaReloadMult = Config.Bind("4. Development", "BallistaReloadMult", 1.0f, "Ballista reload speed multiplier");
            BallistaFlightMult = Config.Bind("4. Development", "BallistaFlightMult", 1.0f, "Ballista projectile speed multiplier");
            CatapultBoost      = Config.Bind("4. Development", "CatapultBoost", false, "Catapult reload speed boost x3");
            CatapultReloadMult = Config.Bind("4. Development", "CatapultReloadMult", 1.0f, "Catapult reload speed multiplier");
            CatapultFlightMult = Config.Bind("4. Development", "CatapultFlightMult", 1.0f, "Catapult projectile speed multiplier");
            InstantCastle      = Config.Bind("4. Development", "InstantCastle", false, "Instant castle upgrade completion");

            BuilderSpeedMult   = Config.Bind("4. Development", "BuilderSpeedMult", 1.0f, "Builder movement speed multiplier");
            BuilderWorkMult    = Config.Bind("4. Development", "BuilderWorkMult", 1.0f, "Builder efficiency multiplier (lower is faster)");

            
            EnableSizeHack    = Config.Bind("5. Player Hack", "EnableSizeHack", false, "Enable player scaling");
            TargetSize        = Config.Bind("5. Player Hack", "TargetSize", 1.0f, "Player scale multiplier");

            
            SteedSpeedMult    = Config.Bind("6. Steed", "SteedSpeedMult", 1.0f, "Steed run speed scale");
            ChargeDmgBoost    = Config.Bind("6. Steed", "ChargeDmgBoost", false, "Steed charge damage x2");
            BuffAuraDuration  = Config.Bind("6. Steed", "BuffAuraDuration", 1.0f, "Steed buff aura duration multiplier");

            
            WaveSizeMult       = Config.Bind("7. Enemies", "WaveSizeMult", 1.0f, "Wave size multiplier");
            EnemySpeedMult     = Config.Bind("7. Enemies", "EnemySpeedMult", 1.0f, "Enemy speed multiplier");
            PortalSpawnRate    = Config.Bind("7. Enemies", "PortalSpawnRate", 1.0f, "Portal spawn rate multiplier");
            NoCrownStealing    = Config.Bind("7. Enemies", "NoCrownStealing", false, "Disable crown stealing");
            GreedQueenHPScale  = Config.Bind("7. Enemies", "GreedQueenHPScale", 1.0f, "Greed Queen HP scale");
            DirectorThreatMult = Config.Bind("7. Enemies", "DirectorThreatMult", 1.0f, "Director threat ramp multiplier");
        }
    }
}
