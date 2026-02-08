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
            // Hover Object Reporting
            var current = _player.selectedPayable;
            
            // Fix Spam: Check if object actually changed significantly (Name or Price), not just Unity Instance
            if (current != null)
            {
                string rawName = current.name.Replace("(Clone)", "").Replace("_", " ").Trim();
                int price = 0; try { price = current.Price; } catch { }
                
                // Only speak if the target changed or we haven't spoken in a while
                if (current != _lastPayable || _spamTimer <= 0f)
                {
                    // Extra check: Don't repeat the EXACT same message within 2 seconds
                    string message = $"{rawName}, {price} coins.";
                    if (message != _lastSpokenMsg || _spamTimer <= -2f) 
                    {
                        ModMenu.Speak(message);
                        _lastSpokenMsg = message;
                        _spamTimer = 1.0f; // 1 second cooldown
                    }
                }
                _lastPayable = current;
            }
            else
            {
                 _lastPayable = null;
                 // Reset message if we look away, so we can look back and hear it again
                 if (_spamTimer <= -1f) _lastSpokenMsg = ""; 
            }
            
            _spamTimer -= Time.deltaTime;
        }

        private float _spamTimer = 0f;
        private string _lastSpokenMsg = "";

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

            ModMenu.Speak(results.Count > 0 ? string.Join(", ", results) : "Clear");
        }

        void CheckCompassAndSafety()
        {
            string dir = (_player.mover.GetDirection() == Side.Left) ? "Left" : "Right";
            ModMenu.Speak($"{dir}, {Mathf.RoundToInt(_player.transform.position.x)}");
        }

        void ReportWallet()
        {
            int coins = _player.wallet.GetCurrency(CurrencyType.Coins);
            int gems = _player.wallet.GetCurrency(CurrencyType.Gems);
            // Concise: "35 Gold, 5 Gems"
            ModMenu.Speak($"{coins} Gold, {gems} Gems");
        }

        void ReportWorld()
        {
            var d = Managers.Inst.director;
            string time = d.IsDaytime ? "Day" : "Night";
            ModMenu.Speak($"Day {d.CurrentIslandDays}, {time}");
        }

        void ReportMount()
        {
            string n = _player.steed.name.Replace("(Clone)", "").Trim();
            string status = _player.steed.IsTired ? "Tired" : "Ready";
            ModMenu.Speak($"{n}, {status}");
        }

        void ReportCompanions()
        {
            ModMenu.Speak("Companions check not implemented in simplified mode.");
        }
    }
}