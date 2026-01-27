using UnityEngine;
using KingdomEnhanced.UI;
using System.Collections.Generic;

namespace KingdomEnhanced.Features
{
    public class AccessibilityFeature : MonoBehaviour
    {
        private Player _player;
        private MonoBehaviour _lastPayable = null;

        void Start() => _player = GetComponent<Player>();

        void Update()
        {
            if (!ModMenu.EnableAccessibility || _player == null) return;

            // Input Handling
            if (Input.GetKeyDown(KeyCode.F5)) PulseRadar();
            if (Input.GetKeyDown(KeyCode.F6)) CheckCompassAndSafety();
            if (Input.GetKeyDown(KeyCode.F7)) ReportWallet();
            if (Input.GetKeyDown(KeyCode.F8)) ReportWorld();
            if (Input.GetKeyDown(KeyCode.F9)) ReportMount();
            if (Input.GetKeyDown(KeyCode.F10)) ReportCompanions();

            // Hover Object Reporting
            var current = _player.selectedPayable;
            if (current != _lastPayable)
            {
                if (current != null)
                {
                    string rawName = current.name.Replace("(Clone)", "").Replace("_", " ").Trim();
                    int price = 0; try { price = current.Price; } catch { }
                    ModMenu.Speak($"Target: {rawName}. Cost: {price} coins.");
                }
                _lastPayable = current;
            }
        }

        // --- RADAR LOGIC ---
        void PulseRadar()
        {
            float range = 50f;
            var allObjects = FindObjectsOfType<GameObject>(); // Scan nearby

            Dictionary<string, float> closestLeft = new Dictionary<string, float>();
            Dictionary<string, float> closestRight = new Dictionary<string, float>();

            foreach (var obj in allObjects)
            {
                if (obj == null || !obj.activeInHierarchy) continue;
                float dist = obj.transform.position.x - _player.transform.position.x;
                if (Mathf.Abs(dist) > range || Mathf.Abs(dist) < 3) continue;

                string n = obj.name.ToLower();
                string type = "";

                if (n.Contains("beggar")) type = "Beggar";
                else if (n.Contains("chest")) type = "Chest";
                else if (n.Contains("portal")) type = "Portal";
                else if (n.Contains("merchant")) type = "Merchant";
                else if (n.Contains("statue")) type = "Statue";

                if (type == "") continue;

                if (dist > 0)
                {
                    if (!closestRight.ContainsKey(type) || dist < closestRight[type]) closestRight[type] = dist;
                }
                else
                {
                    float absDist = Mathf.Abs(dist);
                    if (!closestLeft.ContainsKey(type) || absDist < closestLeft[type]) closestLeft[type] = absDist;
                }
            }

            List<string> results = new List<string>();
            foreach (var kvp in closestLeft) results.Add($"{kvp.Key} {Mathf.RoundToInt(kvp.Value)}m Left");
            foreach (var kvp in closestRight) results.Add($"{kvp.Key} {Mathf.RoundToInt(kvp.Value)}m Right");

            ModMenu.Speak(results.Count > 0 ? "Radar: " + string.Join(", ", results) : "Radar: Area clear.");
        }

        void CheckCompassAndSafety()
        {
            string dir = (_player.mover.GetDirection() == Side.Left) ? "Facing Left." : "Facing Right.";
            ModMenu.Speak($"{dir} Position: {Mathf.RoundToInt(_player.transform.position.x)}");
        }

        void ReportWallet()
        {
            int coins = _player.wallet.GetCurrency(CurrencyType.Coins);
            int gems = _player.wallet.GetCurrency(CurrencyType.Gems);
            ModMenu.Speak($"Wallet: {coins} coins, {gems} gems.");
        }

        void ReportWorld()
        {
            var d = Managers.Inst.director;
            string time = d.IsDaytime ? "Daytime" : "Nighttime";
            ModMenu.Speak($"Day {d.CurrentIslandDays}. {time}.");
        }

        void ReportMount()
        {
            string n = _player.steed.name.Replace("(Clone)", "").Trim();
            string status = _player.steed.IsTired ? "Exhausted" : "Ready";
            ModMenu.Speak($"Mount: {n}. {status}");
        }

        void ReportCompanions()
        {
            ModMenu.Speak("Companions check not implemented in simplified mode.");
        }
    }
}