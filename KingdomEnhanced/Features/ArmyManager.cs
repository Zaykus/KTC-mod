using UnityEngine;
using KingdomEnhanced.UI;
using System.Collections.Generic;
using System.Linq;

namespace KingdomEnhanced.Features
{
    public class ArmyManager : MonoBehaviour
    {
        private float _campTimer = 0f;
        private float _workerTimer = 0f;

        // Cache the prefabs so we don't search every time
        private static GameObject _bowPrefab;
        private static GameObject _hammerPrefab;

        void Update()
        {
            float dt = Time.deltaTime;
            _campTimer += dt;
            _workerTimer += dt;

            // 1. LARGER CAMPS & KNIGHTS (Every 5 seconds)
            if (_campTimer >= 5f)
            {
                _campTimer = 0f;
                if (ModMenu.LargerCamps) BoostCamps();
                if (ModMenu.BetterKnight) BoostKnights();
            }

            // 2. WORKER CATCHUP (Every 1 second)
            if (_workerTimer >= 1f)
            {
                _workerTimer = 0f;
                if (ModMenu.HyperBuilders) ApplyWorkerStats();
            }
        }

        private void BoostCamps()
        {
            var camps = FindObjectsOfType<BeggarCamp>();
            foreach (var c in camps)
            {
                if (c == null) continue;
                c.maxBeggars = 10;
                c.spawnInterval = 20f; // Very fast spawn
            }
        }

        private void BoostKnights()
        {
            var knights = FindObjectsOfType<Knight>();
            foreach (var k in knights)
            {
                if (k == null) continue;
                k.SendMessage("SetHealth", 50f, SendMessageOptions.DontRequireReceiver);
                k.SendMessage("BoostDamage", 3.0f, SendMessageOptions.DontRequireReceiver);
            }
        }

        private void ApplyWorkerStats()
        {
            var workers = FindObjectsOfType<Worker>();
            foreach (var w in workers)
            {
                w.runSpeed = 8.0f; // Even faster
                w.workTime = 0.001f;
            }
        }

        // --- SPAWNING LOGIC ---

        public static void RecruitBeggars()
        {
            var beggars = FindObjectsOfType<Beggar>();
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Characters/Peasant");
            Transform layer = GameObject.FindGameObjectWithTag("GameLayer")?.transform;
            
            if (!prefab) { ModMenu.Speak("Error: Peasant prefab not found."); return; }

            int count = 0;
            foreach (var b in beggars)
            {
                if (b == null || !b.gameObject.activeInHierarchy) continue;
                
                // IMPORTANT: Reset Z to 0 so they are on the walking path
                Vector3 pos = b.transform.position;
                pos.z = 0f; 
                
                b.gameObject.SetActive(false);
                Destroy(b.gameObject);
                
                var p = Instantiate(prefab, pos, Quaternion.identity);
                if (layer) p.transform.SetParent(layer);
                count++;
            }
            ModMenu.Speak($"Recruited {count} beggars!");
        }

        public static void DropTools(string type)
        {
            // 1. Find the Prefab
            GameObject toolToSpawn = null;

            if (type == "Archer")
            {
                if (_bowPrefab == null) _bowPrefab = FindToolPrefab("ToolBow");
                toolToSpawn = _bowPrefab;
            }
            else if (type == "Builder")
            {
                if (_hammerPrefab == null) _hammerPrefab = FindToolPrefab("ToolHammer");
                toolToSpawn = _hammerPrefab;
            }

            if (toolToSpawn == null)
            {
                ModMenu.Speak($"Error: Could not find '{type}' prefab in memory.");
                return;
            }

            // 2. Find Peasants to drop tools on
            var allObjects = FindObjectsOfType<GameObject>();
            int count = 0;
            
            foreach (var obj in allObjects)
            {
                // Look for unemployed peasants
                if (obj.activeInHierarchy && obj.name.Contains("Peasant") && !obj.name.Contains("House"))
                {
                    // Drop slightly above them
                    Vector3 dropPos = obj.transform.position;
                    dropPos.y += 2.5f; 
                    dropPos.z = 0f; // Critical fix

                    Instantiate(toolToSpawn, dropPos, Quaternion.identity);
                    count++;
                }
            }

            if (count == 0) ModMenu.Speak("No unemployed peasants found.");
            else ModMenu.Speak($"Dropped {count} {type} tools!");
        }

        // Deep search for tool prefabs (Original Logic Restored)
        private static GameObject FindToolPrefab(string exactName)
        {
            var allGOs = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in allGOs)
            {
                if (go.scene.name != null) continue; // Skip items in the scene, we want prefabs
                if (go.name.Contains("Shop") || go.name.Contains("Giver")) continue;

                if (go.name.Contains(exactName))
                {
                    return go;
                }
            }
            return null;
        }
    }
}