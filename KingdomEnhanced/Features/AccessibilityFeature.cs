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

        // v1.5: Castle Proximity Logic
        private bool _wasInCastle = false;
        private float _castleCheckTimer = 0f;
        
        // v1.5: Restoring missing fields
        private float _spamTimer = 0f;
        private string _lastSpokenMsg = "";
        private string _lastName = "";
        private int _lastPrice = -1;

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

            HandleHover();
            HandleCastleProximity();
        }

        void HandleHover()
        {
            var current = _player.selectedPayable;
            
            if (current != null)
            {
                string rawName = CleanName(current.name);
                int price = 0; try { price = current.Price; } catch { }
                
                if (current != _lastPayable || rawName != _lastName || price != _lastPrice)
                {
                    string action = "Build";
                    string currency = "coins"; // Default currency
                    
                    // 1. Custom Actions (Bank, Boat, etc.)
                    if (current.name.Contains("Bank") || current.name.Contains("Chest")) action = "Deposit";
                    else if (current.name.Contains("Boat") || current.name.Contains("Ship") || current.name.Contains("Portal")) action = "Pay";
                    else if (current.name.Contains("Beggar") || current.name.Contains("Citizen")) action = "Hire";
                    else if (current.name.Contains("Wreck") || current.name.Contains("Ruin")) action = "Repair";
                    
                    // 2. Smart Heuristic v2 (Only if still "Build")
                    // Check Raw Name for "Level", "Tier", or trailing numbers/letters (e.g. "Tower2", "TowerA")
                    // If found, it's definitely an Upgrade.
                    if (action == "Build" && (System.Text.RegularExpressions.Regex.IsMatch(current.name, @"\d$") || 
                        System.Text.RegularExpressions.Regex.IsMatch(current.name, @"[A-Z]$")))
                    {
                        action = "Upgrade";
                    }
                    
                    // 3. Specific Known Upgrades
                    if (action == "Build" && current.name.Contains("Farm") && price >= 3) action = "Upgrade"; 
                    
                    // 4. Wall Logic: Level 1 is Build, Level 2+ is Upgrade
                    if (current.name.Contains("Wall"))
                    {
                        if (price < 2) action = "Build";
                        else action = "Upgrade";
                    }

                    // 4. Format: Name, Price, Action
                    string message = $"{rawName}, {price} {currency}, {action}";
                    
                    ModMenu.Speak(message);
                    
                    _lastPayable = current;
                    _lastName = rawName;
                    _lastPrice = price;
                }
            }
            else
            {
                 _lastPayable = null;
                 if (_spamTimer <= -1f) _lastSpokenMsg = ""; 
            }
            
            _spamTimer -= Time.deltaTime;
        }

        void HandleCastleProximity()
        {
            if (!ModMenu.EnableCastleAnnouncer) return;

            _castleCheckTimer -= Time.deltaTime;
            if (_castleCheckTimer > 0) return;
            _castleCheckTimer = 1.0f; // Check every second

            var kingdom = Managers.Inst?.kingdom;
            if (kingdom != null)
            {
                // Kingdom object is usually the anchor for the town center
                float center = kingdom.transform.position.x;
                float playerX = _player.transform.position.x;
                float dist = playerX - center;
                bool inside = Mathf.Abs(dist) < 25.0f; // Approximate castle width

                if (inside && !_wasInCastle)
                {
                    ModMenu.Speak("Inside Castle");
                    _wasInCastle = true;
                }
                else if (!inside && _wasInCastle)
                {
                     string dir = dist < 0 ? "Left" : "Right";
                     ModMenu.Speak($"Leaving Castle {dir}");
                     _wasInCastle = false;
                }
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
            string n = CleanName(_player.steed.name);
            string status = _player.steed.IsTired ? "Tired" : "Ready";
            ModMenu.Speak($"{n}, {status}");
        }

        void ReportCompanions()
        {
            ModMenu.Speak("Companions check not implemented in simplified mode.");
        }

        string CleanName(string original)
        {
            if (string.IsNullOrEmpty(original)) return "";

            // 1. Basic Unity Cleanup
            string s = original.Replace("(Clone)", "").Replace("_", " ");

            // 2. Remove Internal IDs/Numbers
            s = System.Text.RegularExpressions.Regex.Replace(s, @"[\d-]", "");

            // 3. Strict Mode: Remove single trailing letters
            s = System.Text.RegularExpressions.Regex.Replace(s, @"\s?[A-Z]$", "");

            // 4. Split CamelCase
            s = System.Text.RegularExpressions.Regex.Replace(s, "([a-z])([A-Z])", "$1 $2");

            // v1.5: Simplify Names Toggle
            if (ModMenu.SimplifyNames)
            {
                // Regex for case-insensitive removal of biome terms
                // (?i) enables case insensitivity
                s = System.Text.RegularExpressions.Regex.Replace(s, @"(?i)(bamboo|iron|stone|dead|lands|scaffold|wreck|grove)", "");

                // Fix double spaces created by removal
                s = System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ");
            }

            return s.Trim();
        }
    }
}