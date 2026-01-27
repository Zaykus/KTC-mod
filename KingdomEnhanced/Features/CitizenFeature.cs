using UnityEngine;
using KingdomEnhanced.UI;
using System.Collections.Generic;
using System;

namespace KingdomEnhanced.Features
{
    public class CitizenFeature : MonoBehaviour
    {
        private static GameObject _bowPrefab;
        private static GameObject _hammerPrefab;

        public static void PromoteAll(string jobType)
        {
            // 1. Find Unemployed Peasants
            var allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            List<GameObject> peasants = new List<GameObject>();

            foreach (var obj in allObjects)
            {
                // Unemployed are named "Peasant"
                if (obj.activeInHierarchy && obj.name.Contains("Peasant"))
                {
                    peasants.Add(obj);
                }
            }

            if (peasants.Count == 0)
            {
                ModMenu.Speak("No unemployed peasants found.");
                return;
            }

            // 2. Find the CORRECT Tool Prefab
            GameObject toolToSpawn = null;
            
            if (jobType == "Archer")
            {
                if (_bowPrefab == null) _bowPrefab = FindToolPrefab("ToolBow");
                toolToSpawn = _bowPrefab;
            }
            else if (jobType == "Builder")
            {
                if (_hammerPrefab == null) _hammerPrefab = FindToolPrefab("ToolHammer");
                toolToSpawn = _hammerPrefab;
            }

            if (toolToSpawn == null)
            {
                ModMenu.Speak($"Error: Could not find '{jobType}' tool. Check logs.");
                return;
            }

            // 3. Drop Tools
            int count = 0;
            foreach (var p in peasants)
            {
                if (p == null) continue;

                // Drop slightly above the peasant so physics takes over
                Vector3 dropPos = p.transform.position;
                dropPos.y += 3.0f; 

                UnityEngine.Object.Instantiate(toolToSpawn, dropPos, Quaternion.identity);
                count++;
            }

            ModMenu.Speak($"Dropped {count} {jobType} tools!");
        }

        private static GameObject FindToolPrefab(string exactName)
        {
            var allGOs = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (var go in allGOs)
            {
                // 1. Must be a Prefab (not in scene)
                if (go.scene.name != null) continue;

                // 2. Must NOT be a Shop
                if (go.name.Contains("Shop") || go.name.Contains("Giver")) continue;

                // 3. Name Match (Look for "ToolBow" or "ToolHammer")
                if (go.name.Contains(exactName))
                {
                    KingdomEnhanced.Core.Plugin.Instance.Log.LogInfo($"Found Tool: {go.name}");
                    return go;
                }
            }
            return null;
        }
    }
}