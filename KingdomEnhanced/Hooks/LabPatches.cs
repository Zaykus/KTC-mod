using HarmonyLib;
using KingdomEnhanced.UI;
using UnityEngine;
using System;
using System.Reflection;
using KingdomEnhanced.Features;
using Rulers;

namespace KingdomEnhanced.Hooks
{
    public static class LabPatches
    {
        #region Registration

        public static void ApplyAll(Harmony harmony)
        {
            TryPatch(harmony, "Season Lock",    typeof(SeasonalEventManager), "CheckSeasonalEvents",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(SeasonPrefix)));

            TryPatch(harmony, "No Blood Moons", typeof(EnemyManager), "StartRedMoon",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(BloodMoonPrefix)));


            TryPatch(harmony, "Archer Fire",    typeof(Archer),     "Awake",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(ArcherAwakePostfix)));
            TryPatch(harmony, "Archer Update",  typeof(Archer),     "Update",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(ArcherUpdatePostfix)));
            TryPatch(harmony, "Berserker Rage", typeof(Berserker),  "Awake",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(BerserkerAwakePostfix)));
            TryPatch(harmony, "Ninja Speed",    typeof(Ninja),      "Awake",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(NinjaAwakePostfix)));

            TryPatch(harmony, "Farm Output",    typeof(Farmland),   "Awake",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(FarmAwakePostfix)));
            TryPatch(harmony, "Farm Output Apply", typeof(Farmland), "OnEnable",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(FarmOutputPostfix)));

            TryPatch(harmony, "Ballista Boost", typeof(Bolt),       "Launch",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(BallistaLaunchPrefix)));

            TryPatch(harmony, "Ballista Reload", typeof(Ballista),   "Update",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(BallistaUpdatePostfix)));

            TryPatch(harmony, "Catapult Reload", typeof(Catapult),   "Update",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(CatapultUpdatePostfix)));

            // Targeted declared method on base class to avoid Harmony warning, 
            // but we filter for Catapult-launched projectiles in the prefix.
            TryPatch(harmony, "Launchable Boost", typeof(Launchable), "Launch",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(LaunchableLaunchPrefix)));

            TryPatch(harmony, "Enemy Speed Apply", typeof(Mover), "Update",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(MoverUpdatePostfix)));

            TryPatch(harmony, "Wave Size",      typeof(EnemyWaveSpawner), "SpawnEnemy",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(SpawnEnemyPrefix)));

            TryPatch(harmony, "Portal Rate",    typeof(Portal),     "Awake",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(PortalAwakePostfix)));
            TryPatch(harmony, "Portal Apply",   typeof(Portal),     "Update",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(PortalApplyRate)));

            TryPatch(harmony, "No Crown Steal", typeof(CrownStealer), "PickupLoot",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(NoCrownStealPrefix)));

            TryPatch(harmony, "Greed Queen HP", typeof(GreedQueen), "Awake",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(GreedQueenHPPostfix)));

            TryPatch(harmony, "Instant Day Skip", typeof(TimeStopper), "OnEnable",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(TimeStopperPrefix)));

            TryPatch(harmony, "No Tool Cooldowns - Gate", typeof(ItemBasedRulerAbility), "CanActivate",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(RulerCanActivatePrefix)));

            TryPatch(harmony, "No Tool Cooldowns - Reset", typeof(ItemBasedRulerAbility), "Activate",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(RulerActivatePostfix)));

            TryPatch(harmony, "No Tool Cooldowns - Item", typeof(ItemOfPower), "TriggerItemAbility",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(ItemTriggerPostfix)));

            TryPatch(harmony, "Artemis Bow Apply", typeof(ArtemisBow), "TriggerItemAbility",
                prefix: new HarmonyMethod(typeof(LabPatches), nameof(ArtemisBowTriggerPrefix)));
        }

        private static void TryPatch(Harmony harmony, string name, Type type, string method,
            Type[] parameters = null, HarmonyMethod prefix = null, HarmonyMethod postfix = null)
        {
            try
            {
                var target = parameters != null
                    ? AccessTools.Method(type, method, parameters)
                    : AccessTools.Method(type, method);

                if (target == null)
                {
                    Core.Plugin.Instance.LogSource.LogWarning($"[Lab] '{name}' skipped — {type.Name}.{method} not found");
                    return;
                }
                harmony.Patch(target, prefix: prefix, postfix: postfix);
                Core.Plugin.Instance.LogSource.LogInfo($"[Lab] '{name}' patch applied");
            }
            catch (Exception ex)
            {
                Core.Plugin.Instance.LogSource.LogWarning($"[Lab] '{name}' patch failed: {ex}");
            }
        }

        #endregion

        #region World Patches

        public static bool SeasonPrefix() => !ModMenu.LockSummer;

        public static bool BloodMoonPrefix() => !ModMenu.NoBloodMoons;

        public static bool TimeStopperPrefix(TimeStopper __instance)
        {
            if (ModMenu.InstantDaySkip)
            {
                __instance.enabled = false; 
                return false; 
            }
            return true;
        }

        public static bool RulerCanActivatePrefix(ItemBasedRulerAbility __instance, ref bool __result)
        {
            if (!ModMenu.NoToolCooldowns) return true;
            __result = true;
            return false;
        }

        public static void RulerActivatePostfix(ItemBasedRulerAbility __instance)
        {
            if (!ModMenu.NoToolCooldowns) return;
            __instance.nextActivationTime = 0f;
            __instance.cooldown = 0f;
            var currentItem = __instance.CurrentItem;
            if (currentItem != null)
                currentItem._nextActivationTime = 0f;
        }

        public static void ItemTriggerPostfix(ItemOfPower __instance)
        {
            if (ModMenu.NoToolCooldowns && __instance != null)
                __instance._nextActivationTime = 0f;
        }

        public static void ArtemisBowTriggerPrefix(ArtemisBow __instance)
        {
            if (__instance == null) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            if (!md.isInitialized)
            {
                md.artemisBaseArrows = __instance._arrowsToFire;
                md.artemisBaseRange  = __instance._abilityRange;
                md.artemisBaseDamage = __instance._damagePerArrow;
                md.isInitialized = true;
            }
            __instance._arrowsToFire   = Mathf.RoundToInt(ModMenu.ArtemisArrowCount);
            __instance._abilityRange   = md.artemisBaseRange  * ModMenu.ArtemisRangeMult;
            __instance._damagePerArrow = Mathf.RoundToInt(md.artemisBaseDamage * ModMenu.ArtemisArrowDamageMult);
        }

        #endregion

        #region Unit Patches

        public static void ArcherAwakePostfix(Archer __instance)
        {
            if (__instance == null) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            if (md.baseFireRate == 0f) md.baseFireRate = __instance.shootCooldownTime;
        }

        public static void ArcherUpdatePostfix(Archer __instance)
        {
            if (__instance == null || (!ModMenu.ArcherFireBoost && !ModMenu.TowerFireBoost)) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            if (md.baseFireRate == 0f) return;
            float mult = 1.0f;
            if (ModMenu.ArcherFireBoost)  mult *= 2.0f;
            if (ModMenu.TowerFireBoost && __instance.GetComponentInParent<Tower>() != null) mult *= 2.0f;
            __instance.shootCooldownTime = md.baseFireRate / mult;
        }

        public static void BerserkerAwakePostfix(Berserker __instance)
        {
            if (__instance == null) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            if (md.baseSpeed == 0f) md.baseSpeed = __instance.runSpeed;
        }

        public static void BerserkerPostfix(Berserker __instance)
        {
            if (__instance == null) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            if (md.baseSpeed == 0f) return;
            
            if (ModMenu.BerserkerRage) {
                __instance.runSpeed  = md.baseSpeed * 2.5f;
                __instance.rageTimer = float.MaxValue;
            } else {
                __instance.runSpeed  = md.baseSpeed;
            }
        }

        public static void NinjaAwakePostfix(Ninja __instance)
        {
            if (__instance == null) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            if (md.baseSpeed == 0f) md.baseSpeed = __instance.runSpeed;
        }

        public static void NinjaPostfix(Ninja __instance)
        {
            if (__instance == null) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            if (md.baseSpeed == 0f) return;
            __instance.runSpeed = ModMenu.NinjaSpeedBoost ? md.baseSpeed * 2.0f : md.baseSpeed;
        }

        #endregion

        #region Building Patches

        public static void FarmAwakePostfix(Farmland __instance)
        {
            if (__instance == null) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            if (md.baseCoinYield == 0f) md.baseCoinYield = __instance.coinYield;
        }

        public static void FarmOutputPostfix(Farmland __instance)
        {
            if (__instance == null) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            if (md.baseCoinYield == 0f) return;
            __instance.coinYield = ModMenu.FarmOutputBoost
                ? Mathf.RoundToInt(md.baseCoinYield * 2f)
                : (int)md.baseCoinYield;
        }

        public static void BallistaLaunchPrefix(Bolt __instance)
        {
            if (__instance == null || __instance._boltData == null) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            
            if (!md.isInitialized)
            {
                md.baseDamage = __instance._boltData._damage;
                md.baseForce  = __instance._boltData._shootForce;
                md.isInitialized = true;
            }
            
            if (!ModMenu.BallistaBoost)
            {
                __instance._boltData._damage = md.baseDamage;
                __instance._boltData._shootForce = md.baseForce;
                return;
            }

            __instance._boltData._damage     = md.baseDamage * 2;
            __instance._boltData._shootForce = md.baseForce  * ModMenu.BallistaFlightMult;
        }

        public static void BallistaUpdatePostfix(Ballista __instance)
        {
            if (!ModMenu.BallistaBoost || ModMenu.BallistaReloadMult <= 1.0f) return;
            // State 0 is Reloading in Ballista
            if (__instance._state == Ballista.State.Reloading && __instance._currentWork < __instance.reloadWork)
            {
                var md = ModData.GetOrAdd(__instance.gameObject);
                float extraWork = Time.deltaTime * (ModMenu.BallistaReloadMult - 1.0f) * 5f; // Scale up for feel
                md.ballistaFractionalWork += extraWork;

                if (md.ballistaFractionalWork >= 1.0f)
                {
                    int add = (int)md.ballistaFractionalWork;
                    __instance._currentWork += add;
                    md.ballistaFractionalWork -= add;
                }
            }
        }

        public static void CatapultUpdatePostfix(Catapult __instance)
        {
            if (!ModMenu.CatapultBoost || ModMenu.CatapultReloadMult <= 1.0f) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            
            // IL2CPP dummy assemblies often hide these as private fields or properties
            var crankField = typeof(Catapult).GetField("crankRate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var crankFormationField = typeof(Catapult).GetField("crankRateFormation", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (crankField == null) return;

            if (md.baseCrankRate == 0f)
            {
                md.baseCrankRate = (float)crankField.GetValue(__instance);
                if (crankFormationField != null)
                    md.baseCrankRateFormation = (float)crankFormationField.GetValue(__instance);
            }

            crankField.SetValue(__instance, md.baseCrankRate * ModMenu.CatapultReloadMult);
            if (crankFormationField != null)
                crankFormationField.SetValue(__instance, md.baseCrankRateFormation * ModMenu.CatapultReloadMult);
        }

        public static void LaunchableLaunchPrefix(Launchable __instance, ref Vector2 __0, GameObject __1)
        {
            if (!ModMenu.CatapultBoost || __1 == null) return;
            
            // Check if the launcher is a Catapult
            if (__1.GetComponent<Catapult>() != null)
            {
                __0 *= ModMenu.CatapultFlightMult;
            }
        }

        #endregion

        private static System.Reflection.FieldInfo _moverSpeedField = null;

        private static System.Reflection.FieldInfo GetMoverSpeedField()
        {
            if (_moverSpeedField != null) return _moverSpeedField;
            foreach (var f in typeof(Mover).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            {
                if (f.FieldType == typeof(float) && (f.Name.ToLower().Contains("speed") || f.Name.ToLower().Contains("max")))
                {
                    _moverSpeedField = f;
                    break;
                }
            }
            return _moverSpeedField;
        }

        public static void MoverUpdatePostfix(Mover __instance)
        {
            if (__instance == null) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            var f = GetMoverSpeedField();
            if (f == null) return;

            if (!md.isInitialized)
            {
                md.moverBaseSpeed = (float)f.GetValue(__instance);
                md.isInitialized = true;
            }
            
            f.SetValue(__instance, md.moverBaseSpeed * ModMenu.EnemySpeedMult);
        }

        private static bool _inWaveSpawnExtra = false;

        public static void SpawnEnemyPrefix(EnemyWaveSpawner __instance, EnemyBlueprint __0)
        {
            if (ModMenu.WaveSizeMult <= 1.0f || _inWaveSpawnExtra) return;

            int extra = Mathf.Max(0, Mathf.RoundToInt(ModMenu.WaveSizeMult) - 1);
            if (extra == 0) return;

            _inWaveSpawnExtra = true;
            for (int i = 0; i < extra; i++)
                __instance.SpawnEnemy(__0);
            _inWaveSpawnExtra = false;
        }

        public static void PortalAwakePostfix(Portal __instance)
        {
            if (__instance == null) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            if (md.baseSpawnInterval == 0f) md.baseSpawnInterval = __instance._spawnInterval;
        }

        public static void PortalApplyRate(Portal __instance)
        {
            if (__instance == null) return;
            var md = ModData.GetOrAdd(__instance.gameObject);
            if (md.baseSpawnInterval == 0f) return;
            __instance._spawnInterval = md.baseSpawnInterval / ModMenu.PortalSpawnRate;
        }

        public static bool NoCrownStealPrefix()
        {
            if (DifficultyRules.IsHardModeActive()) return true;
            return !ModMenu.NoCrownStealing;
        }

        public static void GreedQueenHPPostfix(GreedQueen __instance)
        {
            if (__instance == null || ModMenu.GreedQueenHPScale == 1.0f) return;
            var dmg = __instance.GetComponent<Damageable>();
            if (dmg != null && dmg.initialHitPoints > 0)
            {
                dmg.initialHitPoints = Mathf.RoundToInt(dmg.initialHitPoints * ModMenu.GreedQueenHPScale);
                dmg.hitPoints = dmg.initialHitPoints;
            }
        }
    
    }
}