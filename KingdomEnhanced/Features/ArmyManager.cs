using UnityEngine;
using System.Collections.Generic;
using KingdomEnhanced.UI;

namespace KingdomEnhanced.Features
{
    public class ArmyManager : MonoBehaviour
    {
        private float _campTimer = 0f;

        private float _builderTimer = 0f;

        void Update()
        {
            if (ModMenu.LargerCamps) {
                _campTimer += Time.deltaTime;
                if (_campTimer >= 5f) { _campTimer = 0f; BoostCamps(); }
            }
            
            // OPTIMIZED: Only scan for workers once per second, not every frame
            if (ModMenu.HyperBuilders) {
                _builderTimer += Time.deltaTime;
                if (_builderTimer >= 1f) {
                    _builderTimer = 0f;
                    ApplyBuilderBuffs();
                }
            }
        }

        private void ApplyBuilderBuffs() {
            var workers = FindObjectsOfType<Worker>();
            foreach (var w in workers) { 
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

        public static void RecruitBeggars() {
            var beggars = FindObjectsOfType<Beggar>();
            
            // v3.0: Use FindObjectsOfTypeAll instead of broken Resources.Load (IL2CPP fix)
            GameObject prefab = FindPeasantPrefab();
            if (prefab == null) {
                ModMenu.Speak("Error: Peasant prefab not found.");
                return;
            }
            
            int count = 0;
            foreach (var b in beggars) {
                if (b == null || !b.gameObject.activeInHierarchy) continue;
                Vector3 pos = b.transform.position; pos.z = 0f;
                b.gameObject.SetActive(false); 
                Destroy(b.gameObject);
                Instantiate(prefab, pos, Quaternion.identity);
                count++;
            }
            if (count > 0) ModMenu.Speak($"<color=lime>Recruited {count} Vagrants.</color>");
            else ModMenu.Speak("No Vagrants nearby.");
        }

        private static GameObject _peasantPrefabCache;
        
        private static GameObject FindPeasantPrefab() {
            if (_peasantPrefabCache != null) return _peasantPrefabCache;
            
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in all) {
                // Find the prefab (not a scene instance) named "Peasant"
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
                if (p.gameObject.activeInHierarchy && p.GetComponent<Worker>() == null && p.GetComponent<Archer>() == null) {
                    Instantiate(toolPrefab, p.transform.position + Vector3.up * 2, Quaternion.identity);
                    count++;
                }
            }
            if (count > 0) ModMenu.Speak($"<color=cyan>Supplied {count} {type} tools!</color>");
            else ModMenu.Speak("No unemployed Peasants found.");
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
    }
}