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

            if (Input.GetKeyDown(KeyCode.F5)) PulseRadar();
            if (Input.GetKeyDown(KeyCode.F6)) CheckCompassAndSafety();
            if (Input.GetKeyDown(KeyCode.F7)) ReportWallet();
            if (Input.GetKeyDown(KeyCode.F8)) ReportWorld();
            if (Input.GetKeyDown(KeyCode.F9)) ReportMount();
            if (Input.GetKeyDown(KeyCode.F10)) ReportCompanions();

            var current = _player.selectedPayable;
            if (current != _lastPayable)
            {
                if (current != null)
                {
                    string rawName = current.name.Replace("(Clone)", "").Replace("_", " ").Trim();
                    int price = 0; try { price = current.Price; } catch { }
                    Speak($"Target: {rawName}. Cost: {price} coins.");
                }
                _lastPayable = current;
            }
        }

        void Speak(string message)
        {
            ModMenu.LastAccessMessage = message;
            ModMenu.MessageTimer = 8.0f;
            KingdomEnhanced.Core.Plugin.Instance.Log.LogMessage(message);
            // Call the static method in ModMenu
            ModMenu.Speak(message);
        }

        // --- IMPROVED RADAR: No more duplicates ---
        void PulseRadar()
        {
            float range = 50f;
            var allObjects = Object.FindObjectsOfType<GameObject>();

            // We use Dictionaries to only store the CLOSEST of each type in each direction
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
                { // Right
                    if (!closestRight.ContainsKey(type) || dist < closestRight[type]) closestRight[type] = dist;
                }
                else
                { // Left
                    float absDist = Mathf.Abs(dist);
                    if (!closestLeft.ContainsKey(type) || absDist < closestLeft[type]) closestLeft[type] = absDist;
                }
            }

            List<string> results = new List<string>();
            foreach (var kvp in closestLeft) results.Add($"{kvp.Key} {Mathf.RoundToInt(kvp.Value)}m Left");
            foreach (var kvp in closestRight) results.Add($"{kvp.Key} {Mathf.RoundToInt(kvp.Value)}m Right");

            string msg = results.Count > 0 ? "Radar: " + string.Join(", ", results) : "Radar: Area clear.";
            Speak(msg);
        }

        void CheckCompassAndSafety()
        {
            string dir = (_player.mover.GetDirection() == Side.Left) ? "Facing Left (Dock)." : "Facing Right (Cliff).";
            string safety = "Outside walls. Danger!";
            var walls = Object.FindObjectsOfType<Wall>();
            float minW = float.MaxValue, maxW = float.MinValue;
            bool foundWalls = false;
            foreach (var w in walls)
            {
                if (w.name.Contains("Wreck")) continue;
                float wx = w.transform.position.x;
                if (wx < minW) minW = wx; if (wx > maxW) maxW = wx;
                foundWalls = true;
            }
            if (foundWalls && _player.transform.position.x > minW && _player.transform.position.x < maxW)
                safety = "Inside walls. Safe.";
            Speak($"{dir} {safety}");
        }


        void ReportWallet()
        {
            int coins = _player.wallet.GetCurrency(CurrencyType.Coins);
            int gems = _player.wallet.GetCurrency(CurrencyType.Gems);
            string fullness = (coins + gems) > 30 ? "Bag nearly full." : "Bag is light.";
            Speak($"Wallet: {coins} coins, {gems} gems. {fullness}");
        }

        void ReportWorld()
        {
            var d = Managers.Inst.director;
            string time = d.IsDaytime ? "Daytime" : "Nighttime";
            Speak($"Day {d.CurrentIslandDays}. {time}.");
        }

        void ReportMount()
        {
            string n = _player.steed.name.Replace("(Clone)", "").Replace("P1", "").Trim();
            string status = _player.steed.IsTired ? "Horse exhausted." : "Horse ready.";
            Speak($"Mount: {n}. {status}");
        }

        void ReportCompanions()
        {
            string companions = "No followers.";
            if (_player.steed != null && _player.steed.Passenger != null && _player.steed.Passenger.gameObject.activeInHierarchy)
            {
                string pName = _player.steed.Passenger.name.ToLower();
                if (!pName.Contains("seat") && !pName.Contains("passenger"))
                    companions = $"Carrying {_player.steed.Passenger.name.Replace("(Clone)", "")} Hermit.";
            }
            var dog = Object.FindObjectOfType<Dog>();
            if (dog != null && dog.isActiveAndEnabled && dog.ShouldFollow())
            {
                if (companions.Contains("No")) companions = "Dog is following.";
                else companions += " Dog is also with you.";
            }
            Speak(companions);
        }
    }
}