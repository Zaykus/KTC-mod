using UnityEngine;
using System.Collections.Generic;
using KingdomEnhanced.UI;
using KingdomEnhanced.Utils;

namespace KingdomEnhanced.Systems.Accessibility
{
    public class RadarSystem
    {
        private float _range = 60f;
        private Player _player;

        public RadarSystem(Player player)
        {
            _player = player;
        }

        public void Pulse()
        {
            if (_player == null) return;
            
            float playerX = _player.transform.position.x;
            Dictionary<string, float> closestLeft = new Dictionary<string, float>();
            Dictionary<string, float> closestRight = new Dictionary<string, float>();

            // 1. Scan Payables
            var payables = Managers.Inst?.payables;
            if (payables != null)
            {
                foreach (var p in payables.AllPayables)
                {
                    if (p == null) continue;
                    var mb = p as MonoBehaviour;
                    if (mb == null || !mb.gameObject.activeInHierarchy) continue;

                    ProcessRadarTarget(mb, playerX, _range, closestLeft, closestRight);
                }
            }

            // 2. Scan Shops - simplified approach (relying on PayableShop being a Payable usually)
            // If specific non-payable shops exist, we'd add logic here. 
            // Original code used FindObjectsOfType which is slow, but acceptable for manual trigger.
            // We'll stick to Payables for now as most interactables are Payables.
            
            List<string> results = new List<string>();
            foreach (var kvp in closestLeft) results.Add($"{kvp.Key} {Mathf.RoundToInt(kvp.Value)}m Left");
            foreach (var kvp in closestRight) results.Add($"{kvp.Key} {Mathf.RoundToInt(kvp.Value)}m Right");

            ModMenu.Speak(results.Count > 0 ? string.Join(", ", results) : "No targets found");
        }

        private void ProcessRadarTarget(MonoBehaviour mb, float playerX, float range, Dictionary<string, float> left, Dictionary<string, float> right)
        {
            float dist = mb.transform.position.x - playerX;
            if (Mathf.Abs(dist) > range || Mathf.Abs(dist) < 3) return;

            string n = mb.name.ToLower();
            string type = "";

            // v2.4: ShopTag Support
            var shopTag = mb.GetComponent<ShopTag>();
            if (shopTag != null)
            {
                type = PayableNameResolver.GetShopTypeName(shopTag.type);
            }
            else
            {
                // Fallback to name parsing
                if (n.Contains("beggar")) type = "Beggar";
                else if (n.Contains("chest")) type = "Chest";
                else if (n.Contains("portal")) type = "Portal";
                else if (n.Contains("merchant")) type = "Merchant";
                else if (n.Contains("statue")) type = "Statue";
                else if (n.Contains("dog")) type = "Dog";
                else if (n.Contains("hermit")) type = "Hermit";
            }

            if (type == "") return;

            float absDist = Mathf.Abs(dist);
            var dict = dist > 0 ? right : left;
            
            if (!dict.ContainsKey(type) || absDist < dict[type]) 
                dict[type] = absDist;
        }
    }
}
