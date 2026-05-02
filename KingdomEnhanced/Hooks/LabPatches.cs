using HarmonyLib;
using KingdomEnhanced.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
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
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(BallistaLaunchPostfix)));

            TryPatch(harmony, "Enemy Speed Cache", typeof(Mover), "OnEnable",
                postfix: new HarmonyMethod(typeof(LabPatches), nameof(MoverEnablePostfix)));
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
                Core.Plugin.Instance.LogSource.LogWarning($"[Lab] '{name}' patch failed: {ex.Message}");
            }
        }

        #endregion

        #region Base Value Caches

        private static readonly Dictionary<int, float> _archerBaseFireRate  = new();
        private static readonly Dictionary<int, float> _berserkerBaseSpeed  = new();
        private static readonly Dictionary<int, float> _ninjaBaseSpeed      = new();
        private static readonly Dictionary<int, float> _farmBaseCoinYield   = new();
        private static readonly Dictionary<int, int>   _boltBaseDamage      = new();
        private static readonly Dictionary<int, float> _boltBaseForce       = new();
        private static readonly Dictionary<int, float> _enemyBaseSpeed      = new();
        private static readonly Dictionary<int, float> _portalBaseInterval  = new();
        private static readonly HashSet<int>           _enemyMoverIds       = new();
        private static readonly Dictionary<int, int>   _artemisBaseArrows   = new();
        private static readonly Dictionary<int, float> _artemisBaseRange    = new();
        private static readonly Dictionary<int, int>   _artemisBaseDamage   = new();

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
            int id = __instance.GetInstanceID();
            if (!_artemisBaseArrows.ContainsKey(id))
            {
                _artemisBaseArrows[id] = __instance._arrowsToFire;
                _artemisBaseRange[id]  = __instance._abilityRange;
                _artemisBaseDamage[id] = __instance._damagePerArrow;
            }
            __instance._arrowsToFire   = Mathf.RoundToInt(ModMenu.ArtemisArrowCount);
            __instance._abilityRange   = _artemisBaseRange[id]  * ModMenu.ArtemisRangeMult;
            __instance._damagePerArrow = Mathf.RoundToInt(_artemisBaseDamage[id] * ModMenu.ArtemisArrowDamageMult);
        }

        #endregion

        #region Unit Patches

        public static void ArcherAwakePostfix(Archer __instance)
        {
            if (__instance == null) return;
            int id = __instance.GetInstanceID();
            if (!_archerBaseFireRate.ContainsKey(id))
                _archerBaseFireRate[id] = __instance.shootCooldownTime;
        }

        public static void ArcherUpdatePostfix(Archer __instance)
        {
            if (__instance == null || (!ModMenu.ArcherFireBoost && !ModMenu.TowerFireBoost)) return;
            int id = __instance.GetInstanceID();
            if (!_archerBaseFireRate.TryGetValue(id, out float baseRate)) return;
            float mult = 1.0f;
            if (ModMenu.ArcherFireBoost)  mult *= 2.0f;
            if (ModMenu.TowerFireBoost && __instance.GetComponentInParent<Tower>() != null) mult *= 2.0f;
            __instance.shootCooldownTime = baseRate / mult;
        }

        public static void BerserkerAwakePostfix(Berserker __instance)
        {
            if (__instance == null) return;
            int id = __instance.GetInstanceID();
            if (!_berserkerBaseSpeed.ContainsKey(id))
                _berserkerBaseSpeed[id] = __instance.runSpeed;
        }

        public static void BerserkerPostfix(Berserker __instance)
        {
            if (__instance == null || !ModMenu.BerserkerRage) return;
            int id = __instance.GetInstanceID();
            if (!_berserkerBaseSpeed.TryGetValue(id, out float baseSpeed)) return;
            __instance.runSpeed  = baseSpeed * 2.5f;
            __instance.rageTimer = float.MaxValue;
        }

        public static void NinjaAwakePostfix(Ninja __instance)
        {
            if (__instance == null) return;
            int id = __instance.GetInstanceID();
            if (!_ninjaBaseSpeed.ContainsKey(id))
                _ninjaBaseSpeed[id] = __instance.runSpeed;
        }

        public static void NinjaPostfix(Ninja __instance)
        {
            if (__instance == null || !ModMenu.NinjaSpeedBoost) return;
            int id = __instance.GetInstanceID();
            if (!_ninjaBaseSpeed.TryGetValue(id, out float baseSpeed)) return;
            __instance.runSpeed = baseSpeed * 2.0f;
        }

        #endregion

        #region Building Patches

        public static void FarmAwakePostfix(Farmland __instance)
        {
            if (__instance == null) return;
            int id = __instance.GetInstanceID();
            if (!_farmBaseCoinYield.ContainsKey(id))
                _farmBaseCoinYield[id] = __instance.coinYield;
        }

        public static void FarmOutputPostfix(Farmland __instance)
        {
            if (__instance == null) return;
            int id = __instance.GetInstanceID();
            if (!_farmBaseCoinYield.TryGetValue(id, out float baseYield)) return;
            __instance.coinYield = ModMenu.FarmOutputBoost
                ? Mathf.RoundToInt(baseYield * 2f)
                : (int)baseYield;
        }

        public static void BallistaLaunchPostfix(Bolt __instance)
        {
            if (__instance == null || !ModMenu.BallistaBoost) return;
            if (__instance._boltData == null) return;
            int id = __instance.GetInstanceID();
            if (!_boltBaseDamage.ContainsKey(id))
            {
                _boltBaseDamage[id] = __instance._boltData._damage;
                _boltBaseForce[id]  = __instance._boltData._shootForce;
            }
            __instance._boltData._damage     = _boltBaseDamage[id] * 2;
            __instance._boltData._shootForce = _boltBaseForce[id]  * 2f;
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

        public static void MoverEnablePostfix(Mover __instance)
        {
            if (__instance == null || __instance.GetComponent<Enemy>() == null) return;
            var f = GetMoverSpeedField();
            if (f == null) return;
            int id = __instance.GetInstanceID();
            _enemyMoverIds.Add(id);
            if (!_enemyBaseSpeed.ContainsKey(id))
                _enemyBaseSpeed[id] = (float)f.GetValue(__instance);
        }

        public static void MoverUpdatePostfix(Mover __instance)
        {
            if (__instance == null || ModMenu.EnemySpeedMult == 1.0f) return;
            int id = __instance.GetInstanceID();
            if (!_enemyMoverIds.Contains(id)) return;
            var f = GetMoverSpeedField();
            if (f == null) return;
            if (!_enemyBaseSpeed.TryGetValue(id, out float baseSpeed)) return;
            f.SetValue(__instance, baseSpeed * ModMenu.EnemySpeedMult);
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
            int id = __instance.GetInstanceID();
            if (!_portalBaseInterval.ContainsKey(id))
                _portalBaseInterval[id] = __instance._spawnInterval;
        }

        public static void PortalApplyRate(Portal __instance)
        {
            if (__instance == null || ModMenu.PortalSpawnRate == 1.0f) return;
            int id = __instance.GetInstanceID();
            if (!_portalBaseInterval.TryGetValue(id, out float baseInterval)) return;
            __instance._spawnInterval = baseInterval / ModMenu.PortalSpawnRate;
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