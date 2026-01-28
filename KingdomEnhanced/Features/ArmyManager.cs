using UnityEngine;
using KingdomEnhanced.UI;
using System.Collections.Generic;

namespace KingdomEnhanced.Features
{
    public class ArmyManager : MonoBehaviour
    {
        private float _campTimer = 0f;
        private static GameObject _bowPrefab;
        private static GameObject _hammerPrefab;

        void Update()
        {
            if (ModMenu.LargerCamps)
            {
                _campTimer += Time.deltaTime;
                if (_campTimer >= 5f)
                {
                    _campTimer = 0f;
                    BoostCamps();
                }
            }

            if (ModMenu.HyperBuilders)
            {
                var workers = FindObjectsOfType<Worker>();
                foreach (var w in workers)
                {
                    w.runSpeed = 8.0f;
                    w.workTime = 0.001f;
                }
            }
        }

        private void BoostCamps()
        {
            var camps = FindObjectsOfType<BeggarCamp>();
            foreach (var c in camps)
            {
                if (c == null) continue;
                c.maxBeggars = 10;
                c.spawnInterval = 20f;
            }
        }

        public static void RecruitBeggars()
        {
            var beggars = FindObjectsOfType<Beggar>();
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Characters/Peasant");
            Transform layer = GameObject.FindGameObjectWithTag("GameLayer")?.transform;
            
            if (!prefab) { ModMenu.Speak("<color=red>Error: Peasant blueprint missing.</color>"); return; }

            int count = 0;
            foreach (var b in beggars)
            {
                if (b == null || !b.gameObject.activeInHierarchy) continue;
                
                Vector3 pos = b.transform.position;
                pos.z = 0f; 
                b.gameObject.SetActive(false);
                Destroy(b.gameObject);
                
                var p = Instantiate(prefab, pos, Quaternion.identity);
                if (layer) p.transform.SetParent(layer);
                count++;
            }

            if (count > 0)
                ModMenu.Speak($"<color=lime>Recruited {count} Vagrants.</color>");
            else
                ModMenu.Speak("No Vagrants found to recruit.");
        }

        public static void DropTools(string type)
        {
            GameObject toolToSpawn = (type == "Archer") ? GetBow() : GetHammer();

            if (toolToSpawn == null) {
                ModMenu.Speak($"<color=red>Error: {type} tool not found.</color>");
                return;
            }

            var peasants = FindObjectsOfType<Peasant>();
            int count = 0;
            
            foreach (var p in peasants)
            {
                // Check if they are actually unemployed (no other job components)
                if (p.gameObject.activeInHierarchy && 
                    p.GetComponent<Worker>() == null && 
                    p.GetComponent<Archer>() == null)
                {
                    Vector3 dropPos = p.transform.position;
                    dropPos.y += 2.5f; 
                    dropPos.z = 0f;
                    Instantiate(toolToSpawn, dropPos, Quaternion.identity);
                    count++;
                }
            }

            if (count > 0)
                ModMenu.Speak($"<color=cyan>Supplied {count} {type} tools!</color>");
            else
                ModMenu.Speak("No unemployed Peasants found.");
        }

        private static GameObject GetBow() {
            if (_bowPrefab == null) _bowPrefab = FindTool("ToolBow");
            return _bowPrefab;
        }

        private static GameObject GetHammer() {
            if (_hammerPrefab == null) _hammerPrefab = FindTool("ToolHammer");
            return _hammerPrefab;
        }

        private static GameObject FindTool(string name) {
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in all) {
                if (go.scene.name == null && go.name.Contains(name) && !go.name.Contains("Shop"))
                    return go;
            }
            return null;
        }
    }
}