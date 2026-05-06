using System;
using UnityEngine;
using System.Collections.Generic;
using KingdomEnhanced.UI;
#if IL2CPP
using KingdomEnhanced.Shared.Attributes;
#endif

namespace KingdomEnhanced.Features
{
#if IL2CPP
    [RegisterTypeInIl2Cpp]
#endif
    public class ArmyManager : MonoBehaviour
    {
#if IL2CPP
        public ArmyManager(IntPtr ptr) : base(ptr) { }
#endif
        #region State
        private float _campTimer = 0f;
        private float _builderTimer = 0f;
        private float _unitBuffTimer = 0f;
        #endregion

        #region Standard Updates

        void Update()
        {
            UnitCacheManager.CheckColdBoot(); // Phase 2.5: Cold Boot Initialization

            _campTimer += Time.deltaTime;
            if (_campTimer >= 5f) { _campTimer = 0f; BoostCamps(); }
            
            _builderTimer += Time.deltaTime;
            if (_builderTimer >= 1f) {
                _builderTimer = 0f;
                ApplyBuilderBuffs();
            }

            _unitBuffTimer += Time.deltaTime;
            if (_unitBuffTimer >= 2f) {
                _unitBuffTimer = 0f;
                ApplyUnitBuffs();
            }
        }
        #endregion

        #region Unit Buff Hooks
        private void ApplyUnitBuffs() {
            foreach (var b in UnitCacheManager.Berserkers) {
                if (b != null && b.gameObject.activeInHierarchy) {
                    KingdomEnhanced.Hooks.LabPatches.BerserkerPostfix(b);
                }
            }
            
            foreach (var n in UnitCacheManager.Ninjas) {
                if (n != null && n.gameObject.activeInHierarchy) {
                    KingdomEnhanced.Hooks.LabPatches.NinjaPostfix(n);
                }
            }
            foreach (var k in UnitCacheManager.Knights) {
                if (k != null && k.gameObject.activeInHierarchy) {
                    var d = k.GetComponent<Damageable>();
                    if (d != null && d.initialHitPoints > 0) {
                        var md = ModData.GetOrAdd(k.gameObject);
                        if (md.knightBaseHp == 0) {
                            md.knightBaseHp = d.initialHitPoints;
                        }

                        if (ModMenu.BetterKnight) {
                            d.initialHitPoints = 50;
                            if (d.hitPoints < 50) d.hitPoints = 50; 
                        } else {
                            if (md.knightBaseHp > 0) {
                                d.initialHitPoints = md.knightBaseHp;
                                if (d.hitPoints > md.knightBaseHp) d.hitPoints = md.knightBaseHp; 
                            }
                        }
                    }
                }
            }
        }

        private void ApplyBuilderBuffs() {
            var workers = UnitCacheManager.Workers;
            var ballistas = UnitCacheManager.Ballistas;
            var catapults = UnitCacheManager.Catapults;

            foreach (var w in workers) {
                if (w == null) continue;
                var md = ModData.GetOrAdd(w.gameObject);
                
                if (md.workerBaseSpeed == 0f) {
                    md.workerBaseSpeed    = w.runSpeed;
                    md.workerBaseWorkTime = w.workTime;
                }

                // 1. Movement Speed
                if (ModMenu.HyperBuilders) {
                    w.runSpeed = md.workerBaseSpeed * ModMenu.BuilderSpeedMult;
                } else {
                    w.runSpeed = md.workerBaseSpeed;
                }

                // 2. Work Efficiency / Manning Weapons
                bool isBallista = false;
                bool isCatapult = false;
                bool isTower = false;

                if (w.transform.parent != null) {
                    string pName = w.transform.parent.name;
                    if (pName.IndexOf("ballista", StringComparison.OrdinalIgnoreCase) >= 0) isBallista = true;
                    else if (pName.IndexOf("catapult", StringComparison.OrdinalIgnoreCase) >= 0) isCatapult = true;
                    else if (pName.IndexOf("tower", StringComparison.OrdinalIgnoreCase) >= 0) isTower = true;
                }

                if (!isBallista && !isCatapult && !isTower) {
                    Vector3 pos = w.transform.position;
                    if (ballistas != null) {
                        foreach (var b in ballistas) {
                            if (b != null && Vector3.Distance(pos, b.transform.position) < 8.0f) {
                                isBallista = true; break;
                            }
                        }
                    }
                    if (!isBallista && catapults != null) {
                        foreach (var c in catapults) {
                            if (c != null && Vector3.Distance(pos, c.transform.position) < 8.0f) {
                                isCatapult = true; break;
                            }
                        }
                    }
                }

                if (isBallista || isCatapult || isTower) {
                    float timeMult = 1.0f;
                    if (isBallista && ModMenu.BallistaBoost) timeMult = ModMenu.BallistaReloadMult;
                    if (isCatapult && ModMenu.CatapultBoost) timeMult = ModMenu.CatapultReloadMult;
                    w.workTime = md.workerBaseWorkTime * timeMult;
                } else {
                    if (ModMenu.HyperBuilders) {
                        w.workTime = md.workerBaseWorkTime * ModMenu.BuilderEfficiencyMult;
                    } else {
                        w.workTime = md.workerBaseWorkTime;
                    }
                }
            }
        }

        private void BoostCamps() {
            var camps = UnitCacheManager.BeggarCamps;
            bool larger = ModMenu.LargerCamps;
            foreach (var camp in camps) {
                if (camp != null) { 
                    camp.maxBeggars = larger ? 10 : 2; 
                    camp.spawnInterval = larger ? 20f : 120f; 
                }
            }
        }
        #endregion

        #region Actions & Cheats

        public static void ClearCoins()
        {
            var coins = UnityEngine.Object.FindObjectsByType<DroppableCurrency>(FindObjectsSortMode.None);
            int count = 0;
            foreach (var c in coins)
            {
                if (c != null && c.gameObject.activeInHierarchy)
                {
                    UnityEngine.Object.Destroy(c.gameObject);
                    count++;
                }
            }
            ModMenu.Speak($"Cleared {count} coins from the ground!", ModMenu.C_ON);
        }

        public static void RecruitBeggars() {
            var beggars = UnitCacheManager.Beggars;
            var player = Managers.Inst?.kingdom?.GetPlayer(0);

            if (player == null || player.wallet == null) {
                ModMenu.Speak("Error: Player wallet not found.");
                return;
            }

            int maxPerUse = DifficultyRules.GetMaxRecruitPerUse();
            if (!DifficultyRules.IsHardModeActive() && ModMenu.RecruitCap > 0) maxPerUse = ModMenu.RecruitCap;
            int count = 0;

            foreach (var b in beggars) {
                if (count >= maxPerUse) break;
                if (b == null || !b.gameObject.activeInHierarchy) continue;
                Vector3 pos = b.transform.position;
                pos.y += 1.5f;

                player.wallet.AddCurrency(CurrencyType.Coins, 1);
                var dropped = player.wallet.DropCurrency(CurrencyType.Coins, Vector2.zero, PickUpPolicy.Anybody, (GameObject)null, false, false, false);

                if (dropped != null) {
                    dropped.transform.position = pos;
                }

                count++;
            }

            if (count > 0) {
                string cap = (maxPerUse < int.MaxValue && count >= maxPerUse) ? $" (limit: {maxPerUse}/use in {HardModeFeature.GetActivePreset()})" : "";
                ModMenu.Speak($"<color=lime>Recruited {count} Vagrants.</color>{cap}");
            } else {
                ModMenu.Speak("No Vagrants nearby.");
            }
        }
        #endregion

        #region Prefab Resolution

        private static GameObject _peasantPrefabCache;
        
        private static GameObject FindPeasantPrefab() {
            if (_peasantPrefabCache != null) return _peasantPrefabCache;
            
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in all) {
                if (go.scene.name == null && go.name == "Peasant" && go.GetComponent<Peasant>() != null) {
                    _peasantPrefabCache = go;
                    return go;
                }
            }
            return null;
        }

        public static void DropTools(string type) {
            string toolName = (type == "Archer") ? "ToolBow" : "ToolHammer";
            var toolPrefab = FindTool(toolName);
            if (toolPrefab == null) return;

            var peasants = UnitCacheManager.Peasants;
            int count = 0;
            foreach (var p in peasants) {
                if (p != null && p.gameObject.activeInHierarchy) {
                    UnityEngine.Object.Instantiate((UnityEngine.Object)toolPrefab, p.transform.position + Vector3.up * 2, Quaternion.identity);
                    count++;
                }
            }
            if (count > 0) ModMenu.Speak($"<color=cyan>Supplied {count} {type} tools!</color>");
            else ModMenu.Speak("No unemployed Peasants found.");
        }

        private static readonly Dictionary<string, GameObject> _unitCache = new();

        public static GameObject FindPrefab(string name) {
             if (_unitCache.TryGetValue(name, out var cached) && cached != null) return cached;
             
             if (name == "Peasant") return FindPeasantPrefab();

             var all = Resources.FindObjectsOfTypeAll<GameObject>();
             foreach (var go in all) {
                if (go.scene.name == null) {
                    if (name == "Coin" && go.name.StartsWith("Coin") && go.GetComponent<DroppableCurrency>() != null) {
                        _unitCache[name] = go;
                        return go;
                    }
                    if (go.name == name) {
                        _unitCache[name] = go;
                        return go;
                    }
                }
             }
             return null;
        }

        private static readonly Dictionary<string, GameObject> _toolCache = new();

        private static GameObject FindTool(string name) {
            if (_toolCache.TryGetValue(name, out var cached) && cached != null)
                return cached;

            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in all) {
                if (go.scene.name == null && go.name.Contains(name) && !go.name.Contains("Shop"))
                {
                    _toolCache[name] = go;
                    return go;
                }
            }
            return null;
        }
        #endregion

        #region Advanced Actions

        public static void KillAllEnemies()
        {
            var enemies = UnitCacheManager.Enemies;
            int count = 0;
            
            // Convert to list before destroying to avoid modifying collection while iterating
            var toDestroy = new List<Enemy>(enemies);
            foreach (var e in toDestroy)
            {
                if (e != null && e.gameObject.activeInHierarchy)
                {
                    e.Despawn(1f, false);
                    count++;
                }
            }
            if (count > 0) ModMenu.Speak($"Killed {count} enemies!", ModMenu.C_ON);
            else ModMenu.Speak("No enemies found.", ModMenu.C_LOCK);
        }

        public static void DestroyAllPortals()
        {
            var portals = UnitCacheManager.Portals;
            int count = 0;
            
            // Convert to list before destroying
            var toDestroy = new List<Portal>(portals);
            foreach (var p in toDestroy)
            {
                if (p != null && p.gameObject.activeInHierarchy)
                {
                    p.Crumble(-1);
                    count++;
                }
            }
            if (count > 0) ModMenu.Speak($"Destroyed {count} portals!", ModMenu.C_ON);
            else ModMenu.Speak("No portals found.", ModMenu.C_LOCK);
        }

        public static void SpawnMaxArmy()
        {
            var prefab = FindPrefab("Peasant");
            if (prefab == null) return;
            var player = Managers.Inst?.kingdom?.GetPlayer(0);
            if (player == null) return;
            
            // Phase 3 Bug Fix: Base the spawn logic purely on the max limit 
            // without capping it negatively against the entire kingdom population.
            int max = ModMenu.RecruitCap > 0 ? ModMenu.RecruitCap : 50;
            int currentPeasants = UnitCacheManager.Peasants.Count;
            
            int toSpawn = max - currentPeasants;
            
            if (toSpawn <= 0)
            {
                ModMenu.Speak($"Army cheat cap reached ({max} peasants).", ModMenu.C_LOCK);
                return;
            }

            Vector3 pos = player.transform.position;
            for (int i = 0; i < toSpawn; i++)
            {
                UnityEngine.Object.Instantiate((UnityEngine.Object)prefab, pos + new Vector3(UnityEngine.Random.Range(-4f, 4f), 0, 0), Quaternion.identity);
            }
            ModMenu.Speak($"Spawned {toSpawn} peasants!", ModMenu.C_ON);
        }

        public static void SpawnUnit(string unitName, int amount)
        {
            if (Managers.Inst != null && Managers.Inst.game != null)
            {
                var currentBiome = BiomeHolder.Inst.BiomeIndex;
                if (unitName == "Ninja" && currentBiome != (int)BiomeHolder.Biomes.Shogun)
                {
                    ModMenu.Speak("Cannot spawn Ninja outside Shogun biome!", ModMenu.C_DANGER_BG);
                    return;
                }
                if (unitName == "Berserker" && currentBiome != (int)BiomeHolder.Biomes.Norselands)
                {
                    ModMenu.Speak("Cannot spawn Berserker outside Norselands biome!", ModMenu.C_DANGER_BG);
                    return;
                }
            }

            var prefab = FindPrefab(unitName);
            if (prefab == null)
            {
                ModMenu.Speak($"Prefab not found: {unitName}", ModMenu.C_LOCK);
                return;
            }

            var player = Managers.Inst?.kingdom?.GetPlayer(0);
            if (player == null) return;

            Vector3 pos = player.transform.position;
            for (int i = 0; i < amount; i++)
            {
                UnityEngine.Object.Instantiate(prefab, pos + new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, 0), Quaternion.identity);
            }
            ModMenu.Speak($"Spawned {amount} {unitName}(s)!", ModMenu.C_ON);
        }

        public static void SpawnHermit(string hermitName)
        {
            var activeHermits = UnityEngine.Object.FindObjectsByType<Hermit>(FindObjectsSortMode.None);

            Hermit.HermitType typeWanted = Hermit.HermitType.Baker;
            switch(hermitName.ToLower())
            {
                case "bakery": typeWanted = Hermit.HermitType.Baker; break;
                case "ballista": typeWanted = Hermit.HermitType.Ballista; break;
                case "fire": typeWanted = Hermit.HermitType.Fire; break;
                case "horn": typeWanted = Hermit.HermitType.Horn; break;
                case "stable": typeWanted = Hermit.HermitType.Horse; break;
                case "warrior": typeWanted = Hermit.HermitType.Knight; break;
                case "berserker": typeWanted = Hermit.HermitType.Persephone; break;
            }

            foreach(var h in activeHermits)
            {
                if (h != null && h.gameObject.activeInHierarchy && h.Type == typeWanted)
                {
                    ModMenu.Speak($"You already have the {hermitName} hermit!", ModMenu.C_LOCK);
                    return;
                }
            }

            Hermit prefab = null;
            var hermitsPrefabs = Managers.Inst.holder.hermits;
            for (int i = 0; i < hermitsPrefabs.Length; i++)
            {
                if (hermitsPrefabs[i] != null && hermitsPrefabs[i].Type == typeWanted)
                {
                    prefab = hermitsPrefabs[i];
                    break;
                }
            }

            if (prefab == null)
            {
                ModMenu.Speak($"Could not find prefab for {hermitName} hermit.", ModMenu.C_LOCK);
                return;
            }

            var player = Managers.Inst?.kingdom?.GetPlayer(0);
            if (player == null) return;

            UnityEngine.Object.Instantiate(prefab.gameObject, player.transform.position + new Vector3(2f, 0, 0), Quaternion.identity);
            ModMenu.Speak($"Spawned {hermitName} hermit!", ModMenu.C_ON);
        }

        public static void SpawnEnemy(EnemyType type, int amount)
        {
            var em = Managers.Inst?.enemies;
            if (em == null) return;

            var prefab = em.GetPrefab(type);
            if (prefab == null)
            {
                ModMenu.Speak($"Enemy prefab not found for type: {type}", ModMenu.C_LOCK);
                return;
            }

            var player = Managers.Inst?.kingdom?.GetPlayer(0);
            if (player == null) return;

            float faceDir = (player.transform.localScale.x < 0) ? -1f : 1f;
            Vector3 pos = player.transform.position + new Vector3(10f * faceDir, 0, 0);
            for (int i = 0; i < amount; i++)
            {
                UnityEngine.Object.Instantiate(prefab.gameObject, pos + new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, 0), Quaternion.identity);
            }
            ModMenu.Speak($"Spawned {amount} {type}(s)!", ModMenu.C_ON);
        }
        #endregion
    }
}