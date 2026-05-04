using System;
using System.Collections.Generic;
using KingdomEnhanced.Features;

namespace KingdomEnhanced.UI
{
    /// <summary>
    /// Builds metadata for all features registered in the ModMenu.
    /// Separated from ModMenu for maintainability and team collaboration.
    /// </summary>
    public static class ModMenuFeatures
    {
        public static FeatureMeta Toggle(string id, string label, TabCategory cat, string section, string desc,
            Func<bool> get, Action<bool> set, Func<bool> isLocked = null,
            Func<string> lockReason = null, Func<bool> hasConflict = null)
        {
            return new FeatureMeta
            {
                Id = id, Label = label, Section = section, Category = cat, Description = desc,
                GetValue = get, SetValue = set, OnAction = null,
                IsLocked = isLocked, GetLockReason = lockReason, HasConflict = hasConflict
            };
        }

        public static FeatureMeta Button(string id, string label, TabCategory cat, string section, string desc,
            Action act, Func<bool> isLocked = null, Func<string> lockReason = null)
        {
            return new FeatureMeta
            {
                Id = id, Label = label, Section = section, Category = cat, Description = desc,
                GetValue = null, SetValue = null, OnAction = act,
                IsLocked = isLocked, GetLockReason = lockReason, HasConflict = null
            };
        }

        public static FeatureMeta Slider(string id, string label, TabCategory cat, string section, string desc,
            Func<float> get, Action<float> set, float min, float max)
        {
            return new FeatureMeta
            {
                Id = id, Label = label, Section = section, Category = cat, Description = desc,
                GetFloatValue = get, SetFloatValue = set, MinVal = min, MaxVal = max,
                IsLocked = null, GetLockReason = null, HasConflict = null
            };
        }

        public static FeatureMeta[] Build()
        {
            var list = new List<FeatureMeta>();

            // ==================== MAIN ====================
            list.Add(Toggle("show_stamina", "Energy Bar", TabCategory.Main, "HUD",
                "Shows or hides the stamina bar on the HUD.",
                () => ModMenu.ShowStaminaBar, v => ModMenu.ShowStaminaBar = v));
            list.Add(Button("stamina_style", "Cycle Energy Bar Style", TabCategory.Main, "HUD",
                "Changes the visual style of the Energy Bar.",
                () => ModMenu.CycleStaminaBarStyle(),
                () => !ModMenu.ShowStaminaBar, () => "Requires Energy Bar"));
            list.Add(Button("stamina_pos", "Cycle Energy Bar Position", TabCategory.Main, "HUD",
                "Changes the position of the Energy Bar on screen.",
                () => ModMenu.CycleStaminaBarPosition(),
                () => !ModMenu.ShowStaminaBar, () => "Requires Energy Bar"));
            list.Add(Toggle("display_times", "HUD Display", TabCategory.Main, "HUD",
                "Toggles the entire in-game HUD overlay.",
                () => ModMenu.DisplayTimes, v => ModMenu.DisplayTimes = v));
            list.Add(Button("monitor_style", "Cycle Monitor Style", TabCategory.Main, "HUD",
                "Changes the visual style of the Kingdom Monitor panel.",
                () => KingdomMonitor.Instance?.NextStyle(),
                () => KingdomMonitor.Instance == null || !KingdomMonitor.Instance.IsVisible,
                () => "Requires Monitor"));

            list.Add(Toggle("enable_accessibility", "Accessibility & Radar", TabCategory.Main, "Accessibility",
                "Enables world tracking, radar pings, and proximity alerts.",
                () => ModMenu.EnableAccessibility, v => ModMenu.EnableAccessibility = v));
            list.Add(Toggle("enable_tts", "Narrator (TTS)", TabCategory.Main, "Accessibility",
                "Reads menu interactions aloud using the system TTS engine.",
                () => ModMenu.EnableTTS, v => ModMenu.EnableTTS = v));
            list.Add(Toggle("simplify_names", "Simplify Names", TabCategory.Main, "Accessibility",
                "Replaces payable object names with shorter labels.",
                () => ModMenu.SimplifyNames, v => ModMenu.SimplifyNames = v));
            list.Add(Toggle("castle_announcer", "Castle Announcer", TabCategory.Main, "Accessibility",
                "Announces castle events via TTS.",
                () => ModMenu.EnableCastleAnnouncer, v => ModMenu.EnableCastleAnnouncer = v));

            list.Add(Slider("speed_mult", "Travel Speed", TabCategory.Main, "Movement",
                "Multiplies the monarch's base movement speed.",
                () => ModMenu.SpeedMultiplier, v => ModMenu.SpeedMultiplier = v, 0.5f, 10.0f));

            // ==================== CHEATS ====================
            list.Add(Toggle("infinite_stamina", "Infinite Mount Stamina", TabCategory.Cheats, "Invincibility",
                "Prevents mount stamina from depleting.",
                () => ModMenu.InfiniteStamina, v => ModMenu.InfiniteStamina = v,
                () => !ModMenu.CheatsUnlocked));

            list.Add(Toggle("no_tool_cooldowns", "No Tool Cooldowns", TabCategory.Cheats, "Infinite Stone",
                "Removes the cooldown/timeout of all Hermit tools (Horn of Healing, Athena's Shield, Hermes' Staff, etc).",
                () => ModMenu.NoToolCooldowns, v => ModMenu.NoToolCooldowns = v,
                () => !ModMenu.CheatsUnlocked));
            list.Add(Slider("artemis_arrows", "Artemis Arrow Count", TabCategory.Cheats, "Infinite Stone",
                "How many arrows fall per cast of the Artemis Bow.",
                () => ModMenu.ArtemisArrowCount, v => ModMenu.ArtemisArrowCount = v, 1f, 50f));
            list.Add(Slider("artemis_range", "Artemis Range", TabCategory.Cheats, "Infinite Stone",
                "Multiplies the range across which arrows are spread.",
                () => ModMenu.ArtemisRangeMult, v => ModMenu.ArtemisRangeMult = v, 0.5f, 5.0f));
            list.Add(Slider("artemis_damage", "Artemis Arrow Damage", TabCategory.Cheats, "Infinite Stone",
                "Multiplies damage dealt per arrow.",
                () => ModMenu.ArtemisArrowDamageMult, v => ModMenu.ArtemisArrowDamageMult = v, 0.5f, 5.0f));

            list.Add(Button("add_10_coins", "+10 Coins", TabCategory.Cheats, "Economy",
                "Adds 10 coins instantly.",
                () => ModMenu.GiveCurrency(10, false),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("add_50_coins", "+50 Coins", TabCategory.Cheats, "Economy",
                "Adds 50 coins instantly.",
                () => ModMenu.GiveCurrency(50, false),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("add_5_gems", "+5 Gems", TabCategory.Cheats, "Economy",
                "Instant gem grant.",
                () => ModMenu.GiveCurrency(5, true),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("fill_wallet", "Fill Wallet to Max", TabCategory.Cheats, "Economy",
                "Fills coins and gems to capacity.",
                () => ModMenu.FillWallet(),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Slider("coin_income", "Coin Income", TabCategory.Cheats, "Economy",
                "Multiplies passive coin income rate.",
                () => ModMenu.CoinIncomeMult, v => ModMenu.CoinIncomeMult = v, 0.5f, 4.0f));
            list.Add(Slider("bag_drop", "Bag Drop Rate", TabCategory.Cheats, "Economy",
                "Multiplies bag drop rate.",
                () => ModMenu.BagDropMult, v => ModMenu.BagDropMult = v, 0.5f, 4.0f));

            list.Add(Button("recruit_beggars", "Recruit All Beggars", TabCategory.Cheats, "Military",
                "Forces all vagrants to immediately pick up tools and join.",
                () => ArmyManager.RecruitBeggars(),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("drop_archer", "Drop Archer Bow", TabCategory.Cheats, "Military",
                "Spawns an archer bow pickup at the monarch's position.",
                () => ArmyManager.DropTools("Archer"),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("drop_builder", "Drop Builder Hammer", TabCategory.Cheats, "Military",
                "Spawns a builder hammer pickup at the monarch's position.",
                () => ArmyManager.DropTools("Builder"),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));

            list.Add(Button("kill_enemies", "Kill All Enemies", TabCategory.Cheats, "Military",
                "Instantly destroys all active Greed units.",
                () => ArmyManager.KillAllEnemies(),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("destroy_portals", "Destroy All Portals", TabCategory.Cheats, "Military",
                "Closes all active portals.",
                () => ArmyManager.DestroyAllPortals(),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("max_army", "Spawn Max Army", TabCategory.Cheats, "Military",
                "Fills all available archer/knight slots instantly.",
                () => ArmyManager.SpawnMaxArmy(),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));

            list.Add(Slider("spawn_unit_count", "Unit Spawn Amount", TabCategory.Cheats, "Unit Spawner",
                "Select amount of units to spawn (1-50).",
                () => ModMenu.SpawnUnitCount, v => ModMenu.SpawnUnitCount = (int)v, 1f, 50f));
            list.Add(Button("spawn_u_vagrant", "Spawn Vagrants", TabCategory.Cheats, "Unit Spawner",
                "Spawns Vagrants (Baggers).",
                () => ArmyManager.SpawnUnit("Beggar", (int)ModMenu.SpawnUnitCount),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_u_villager", "Spawn Villagers", TabCategory.Cheats, "Unit Spawner",
                "Spawns Villagers.",
                () => ArmyManager.SpawnUnit("Peasant", (int)ModMenu.SpawnUnitCount),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_u_archer", "Spawn Archers", TabCategory.Cheats, "Unit Spawner",
                "Spawns Archers.",
                () => ArmyManager.SpawnUnit("Archer", (int)ModMenu.SpawnUnitCount),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_u_builder", "Spawn Builders", TabCategory.Cheats, "Unit Spawner",
                "Spawns Builders.",
                () => ArmyManager.SpawnUnit("Worker", (int)ModMenu.SpawnUnitCount),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_u_farmer", "Spawn Farmers", TabCategory.Cheats, "Unit Spawner",
                "Spawns Farmers.",
                () => ArmyManager.SpawnUnit("Farmer", (int)ModMenu.SpawnUnitCount),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_u_pikeman", "Spawn Pikemen", TabCategory.Cheats, "Unit Spawner",
                "Spawns Pikemen.",
                () => ArmyManager.SpawnUnit("Pikeman", (int)ModMenu.SpawnUnitCount),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_u_ninja", "Spawn Ninjas", TabCategory.Cheats, "Unit Spawner",
                "Spawns Ninjas (Shogun only).",
                () => ArmyManager.SpawnUnit("Ninja", (int)ModMenu.SpawnUnitCount),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_u_berserker", "Spawn Berserkers", TabCategory.Cheats, "Unit Spawner",
                "Spawns Berserkers (Norse only).",
                () => ArmyManager.SpawnUnit("Berserker", (int)ModMenu.SpawnUnitCount),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_u_knight", "Spawn Knights", TabCategory.Cheats, "Unit Spawner",
                "Spawns Knights.",
                () => ArmyManager.SpawnUnit("Knight", (int)ModMenu.SpawnUnitCount),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));

            list.Add(Button("spawn_h_bakery", "Spawn Bakery Hermit", TabCategory.Cheats, "Hermit Spawner",
                "Spawns the Bakery Hermit.",
                () => ArmyManager.SpawnHermit("Bakery"),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_h_ballista", "Spawn Ballista Hermit", TabCategory.Cheats, "Hermit Spawner",
                "Spawns the Ballista Hermit.",
                () => ArmyManager.SpawnHermit("Ballista"),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_h_berserker", "Spawn Berserker Hermit", TabCategory.Cheats, "Hermit Spawner",
                "Spawns the Berserker Hermit (Norse only).",
                () => ArmyManager.SpawnHermit("Berserker"),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_h_fire", "Spawn Fire Hermit", TabCategory.Cheats, "Hermit Spawner",
                "Spawns the Fire Hermit.",
                () => ArmyManager.SpawnHermit("Fire"),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_h_horn", "Spawn Horn Hermit", TabCategory.Cheats, "Hermit Spawner",
                "Spawns the Horn Hermit.",
                () => ArmyManager.SpawnHermit("Horn"),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_h_stable", "Spawn Stable Hermit", TabCategory.Cheats, "Hermit Spawner",
                "Spawns the Stable Hermit.",
                () => ArmyManager.SpawnHermit("Stable"),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));
            list.Add(Button("spawn_h_warrior", "Spawn Warrior Hermit", TabCategory.Cheats, "Hermit Spawner",
                "Spawns the Warrior Hermit.",
                () => ArmyManager.SpawnHermit("Warrior"),
                () => !ModMenu.CheatsUnlocked, () => "Locked"));

            list.Add(Toggle("hyper_builders", "Instant Construction", TabCategory.Cheats, "Builders",
                "Buildings complete in one frame.",
                () => ModMenu.HyperBuilders, v => ModMenu.HyperBuilders = v,
                () => !ModMenu.CheatsUnlocked, null,
                () => ModMenu.HyperBuilders && ModMenu.LargerCamps));
            list.Add(Toggle("larger_camps", "Expand Vagrant Camps", TabCategory.Cheats, "Builders",
                "Increases the maximum vagrant camp population.",
                () => ModMenu.LargerCamps, v => ModMenu.LargerCamps = v,
                () => !ModMenu.CheatsUnlocked, null,
                () => ModMenu.HyperBuilders && ModMenu.LargerCamps));

            // ==================== LAB ====================
            list.Add(Toggle("lock_summer", "Lock Summer Season", TabCategory.Lab, "World",
                "Prevents the season from advancing past summer.",
                () => ModMenu.LockSummer, v => ModMenu.LockSummer = v));
            list.Add(Toggle("clear_weather", "Clear Weather", TabCategory.Lab, "World",
                "Disables rain and snow effects.",
                () => ModMenu.ClearWeather, v => ModMenu.ClearWeather = v,
                null, null, () => ModMenu.LockSummer && ModMenu.ClearWeather));
            list.Add(Toggle("coins_stay_dry", "Buoyant Currency", TabCategory.Lab, "World",
                "Coins dropped in water float instead of sinking.",
                () => ModMenu.CoinsStayDry, v => ModMenu.CoinsStayDry = v));
            list.Add(Toggle("no_blood_moons", "Disable Blood Moons", TabCategory.Lab, "World",
                "Prevents blood moon wave events.",
                () => ModMenu.NoBloodMoons, v => ModMenu.NoBloodMoons = v));

            list.Add(Toggle("invincible_walls", "Self-Repairing Walls", TabCategory.Lab, "Structures",
                "Damaged walls automatically restore over time.",
                () => ModMenu.InvincibleWalls, v => ModMenu.InvincibleWalls = v));
            list.Add(Toggle("better_citizen_houses", "Rapid Citizen Housing", TabCategory.Lab, "Structures",
                "Citizens move into houses faster.",
                () => ModMenu.BetterCitizenHouses, v => ModMenu.BetterCitizenHouses = v));
            list.Add(Toggle("better_knight", "Elite Knights", TabCategory.Lab, "Combat",
                "Increases knight combat effectiveness.",
                () => ModMenu.BetterKnight, v => ModMenu.BetterKnight = v));

            list.Add(Toggle("archer_fire_boost", "Archer Fire Rate Boost", TabCategory.Lab, "Units",
                "Archers shoot significantly faster.",
                () => ModMenu.ArcherFireBoost, v => ModMenu.ArcherFireBoost = v));
            list.Add(Toggle("berserker_rage", "Berserker Rage Mode", TabCategory.Lab, "Units",
                "Berserkers enter rage state permanently.",
                () => ModMenu.BerserkerRage, v => ModMenu.BerserkerRage = v));
            list.Add(Toggle("ninja_speed_boost", "Ninja Speed Boost", TabCategory.Lab, "Units",
                "Ninjas move faster.",
                () => ModMenu.NinjaSpeedBoost, v => ModMenu.NinjaSpeedBoost = v));

            list.Add(Slider("recruit_cap", "Recruit Cap", TabCategory.Lab, "Lab Rules",
                "Overrides max recruitable units. 0 for default.",
                () => ModMenu.RecruitCap, v => ModMenu.RecruitCap = (int)v, 0, 50));
            list.Add(Slider("tree_regrow", "Tree Regrowth", TabCategory.Lab, "World",
                "Multiplies tree regrowth speed.",
                () => ModMenu.TreeRegrowthMult, v => ModMenu.TreeRegrowthMult = v, 0.1f, 5.0f));

            list.Add(Toggle("farm_output", "Farm Output Boost", TabCategory.Lab, "World",
                "Increases farm production yield.",
                () => ModMenu.FarmOutputBoost, v => ModMenu.FarmOutputBoost = v));
            list.Add(Toggle("tower_fire", "Tower Fire Boost", TabCategory.Lab, "World",
                "Increases tower fire rate.",
                () => ModMenu.TowerFireBoost, v => ModMenu.TowerFireBoost = v));
            list.Add(Toggle("ballista_boost", "Ballista Boost", TabCategory.Lab, "World",
                "Increases ballista damage/speed.",
                () => ModMenu.BallistaBoost, v => ModMenu.BallistaBoost = v));
            list.Add(Toggle("instant_castle", "Instant Castle Upgrade", TabCategory.Lab, "World",
                "Castle upgrades finish immediately.",
                () => ModMenu.InstantCastle, v => ModMenu.InstantCastle = v));
            list.Add(Toggle("instant_day_skip", "Instant Day Skip", TabCategory.Lab, "World",
                "Skips the day/night transition delays.",
                () => ModMenu.InstantDaySkip, v => ModMenu.InstantDaySkip = v));
            list.Add(Toggle("animal_spawn", "Animal Spawn Boost", TabCategory.Lab, "World",
                "Increases animal spawn rates.",
                () => ModMenu.AnimalSpawnBoost, v => ModMenu.AnimalSpawnBoost = v));

            list.Add(Slider("steed_speed", "Steed Speed", TabCategory.Lab, "Steed",
                "Multiplies steed movement speed.",
                () => ModMenu.SteedSpeedMult, v => ModMenu.SteedSpeedMult = v, 0.5f, 3.0f));
            list.Add(Toggle("charge_dmg", "Charge Damage Boost", TabCategory.Lab, "Steed",
                "Increases steed charge damage.",
                () => ModMenu.ChargeDmgBoost, v => ModMenu.ChargeDmgBoost = v));
            list.Add(Slider("buff_aura", "Buff Aura Duration", TabCategory.Lab, "Steed",
                "Extends duration of buff auras.",
                () => ModMenu.BuffAuraDuration, v => ModMenu.BuffAuraDuration = v, 1.0f, 10.0f));

            // ==================== HARD ====================
            list.Add(Toggle("no_crown_stealing", "No Crown Stealing", TabCategory.Hard, "Wave Control",
                "Greed units cannot steal the crown.",
                () => ModMenu.NoCrownStealing, v => ModMenu.NoCrownStealing = v,
                () => DifficultyRules.IsHardModeActive(), () => "Hard Mode"));

            list.Add(Slider("wave_size", "Wave Size", TabCategory.Hard, "Wave Sliders",
                "Multiplies the number of enemies per wave.",
                () => ModMenu.WaveSizeMult, v => ModMenu.WaveSizeMult = v, 0.1f, 5.0f));
            list.Add(Slider("enemy_speed", "Enemy Speed", TabCategory.Hard, "Wave Sliders",
                "Multiplies Greed unit movement speed.",
                () => ModMenu.EnemySpeedMult, v => ModMenu.EnemySpeedMult = v, 0.5f, 3.0f));
            list.Add(Slider("portal_rate", "Portal Spawn Rate", TabCategory.Hard, "Wave Sliders",
                "Multiplies portal spawn rate.",
                () => ModMenu.PortalSpawnRate, v => ModMenu.PortalSpawnRate = v, 0.1f, 5.0f));
            list.Add(Slider("queen_hp", "Greed Queen HP", TabCategory.Hard, "Wave Sliders",
                "Multiplies the Greed Queen's max health.",
                () => ModMenu.GreedQueenHPScale, v => ModMenu.GreedQueenHPScale = v, 0.5f, 5.0f));
            list.Add(Slider("threat", "Director Threat", TabCategory.Hard, "Wave Sliders",
                "Multiplies the Director's threat scaling.",
                () => ModMenu.DirectorThreatMult, v => ModMenu.DirectorThreatMult = v, 0.1f, 5.0f));

            return list.ToArray();
        }
    }
}
