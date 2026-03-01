using UnityEngine;
using System.Collections.Generic;
using KingdomEnhanced.UI;

namespace KingdomEnhanced.Features
{
    public class ArmyManager : MonoBehaviour
    {
        #region State
        private float _campTimer = 0f;
        private float _builderTimer = 0f;
        private float _unitBuffTimer = 0f;
        #endregion

        #region Standard Updates

        void Update()
        {
            if (ModMenu.LargerCamps) {
                _campTimer += Time.deltaTime;
                if (_campTimer >= 5f) { _campTimer = 0f; BoostCamps(); }
            }
            
            if (ModMenu.HyperBuilders) {
                _builderTimer += Time.deltaTime;
                if (_builderTimer >= 1f) {
                    _builderTimer = 0f;
                    ApplyBuilderBuffs();
                }
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
            if (ModMenu.BerserkerRage) {
                var berserkers = FindObjectsOfType<Berserker>();
                foreach (var b in berserkers) {
                    if (b != null && b.gameObject.activeInHierarchy) {
                        KingdomEnhanced.Hooks.LabPatches.BerserkerPostfix(b);
                    }
                }
            }
            if (ModMenu.NinjaSpeedBoost) {
                var ninjas = FindObjectsOfType<Ninja>();
                foreach (var n in ninjas) {
                    if (n != null && n.gameObject.activeInHierarchy) {
                        KingdomEnhanced.Hooks.LabPatches.NinjaPostfix(n);
                    }
                }
            }
            if (ModMenu.BetterKnight) {
                var knights = FindObjectsOfType<Knight>();
                foreach (var k in knights) {
                    if (k != null && k.gameObject.activeInHierarchy) {
                        var d = k.GetComponent<Damageable>();
                        if (d != null && d.initialHitPoints > 0) {
                            d.initialHitPoints = 50; 
                            d.hitPoints = 50;
                        }
                    }
                }
            }
        }

        private void ApplyBuilderBuffs() {
            if (!DifficultyRules.CanUseInstantConstruction()) {
                float sp = DifficultyRules.GetBuilderSpeedMultiplier();
                float wt = DifficultyRules.GetBuilderWorkTime();
                var workers = FindObjectsOfType<Worker>();
                foreach (var w in workers) {
                    if (w != null) {
                        w.runSpeed = 4.0f * sp;
                        w.workTime = wt;
                    }
                }
                return;
            }
            var allWorkers = FindObjectsOfType<Worker>();
            foreach (var w in allWorkers) {
                if (w != null) {
                    w.runSpeed = 8.0f;
                    w.workTime = 0.001f;
                }
            }
        }

        private void BoostCamps() {
            var camps = Object.FindObjectsOfType<BeggarCamp>();
            foreach (var camp in camps) {
                if (camp != null) { 
                    camp.maxBeggars = 10; 
                    camp.spawnInterval = 20f; 
                }
            }
        }
        #endregion

        #region Actions & Cheats

        public static void RecruitBeggars() {
            var beggars = Object.FindObjectsOfType<Beggar>();
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

            var peasants = FindObjectsOfType<Peasant>();
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
            var enemies = FindObjectsOfType<Enemy>();
            int count = 0;
            foreach (var e in enemies)
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
            var portals = FindObjectsOfType<Portal>();
            int count = 0;
            foreach (var p in portals)
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
            
            int max = ModMenu.RecruitCap > 0 ? ModMenu.RecruitCap : 50;
            var current = FindObjectsOfType<Peasant>().Length + FindObjectsOfType<Archer>().Length + FindObjectsOfType<Worker>().Length + FindObjectsOfType<Knight>().Length;
            int toSpawn = max - current;
            
            if (toSpawn <= 0)
            {
                ModMenu.Speak("Army is already at cap.", ModMenu.C_LOCK);
                return;
            }

            Vector3 pos = player.transform.position;
            for (int i = 0; i < toSpawn; i++)
            {
                UnityEngine.Object.Instantiate((UnityEngine.Object)prefab, pos + new Vector3(Random.Range(-4f, 4f), 0, 0), Quaternion.identity);
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
            var activeHermits = UnityEngine.Object.FindObjectsOfType<Hermit>();

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
        #endregion
    }
}