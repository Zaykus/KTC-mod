using UnityEngine;
using KingdomEnhanced.UI;
using KingdomEnhanced.Systems;
using KingdomEnhanced.Utils; // New
using KingdomEnhanced.Systems.Accessibility; // New
using System.Collections.Generic;
using System.Reflection; 
using System.Text.RegularExpressions;

namespace KingdomEnhanced.Features
{
    public class AccessibilityFeature : MonoBehaviour
    {
        private Player _player;
        private MonoBehaviour _lastPayable = null;

        private RadarSystem _radarSystem; // New: Decoupled Radar

        void Start()
        {
            _player = GetComponent<Player>();
            _baseCampAnnounced = false;
            _radarSystem = new RadarSystem(_player); // Init Radar
        }

        // v1.5: Castle Proximity Logic
        private bool _wasInCastle = false;
        private float _castleCheckTimer = 0f;
        
        // v3.0: Base Camp Orientation
        private bool _baseCampAnnounced = false;
        
        // v1.5: Restoring missing fields
        private float _spamTimer = 0f;
        private string _lastSpokenMsg = "";
        private string _lastName = "";
        private int _lastPrice = -1;

        // Optimization: Throttling updates
        private float _lastPayableCheckTime = 0f;
        private const float PAYABLE_CHECK_INTERVAL = 0.15f; // Check every ~150ms instead of frame

        // Optimization: Cached Castle lists
        private Castle[] _cachedCastles;
        private float _castleCacheTimer = 0f;
        private const float CASTLE_CACHE_INTERVAL = 2.0f; // Refresh list every 2s

        // Cached Price PropertyInfo per type
        private static readonly Dictionary<System.Type, PropertyInfo> _pricePropertyCache = new();

        private static readonly Regex _endsWithDigitRegex = new Regex(@"\d$", RegexOptions.Compiled);
        private static readonly Regex _endsWithUpperRegex = new Regex(@"[A-Z]$", RegexOptions.Compiled);

        void Update()
        {
            if (!ModMenu.EnableAccessibility || _player == null) return;

            // Input Handling
            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    ReportDetailedInfo();
                else
                    _radarSystem.Pulse(); // New: Use RadarSystem
            }
            if (Input.GetKeyDown(KeyCode.F6)) CheckCompassAndSafety();
            if (Input.GetKeyDown(KeyCode.F7)) ReportWallet();
            if (Input.GetKeyDown(KeyCode.F8)) ReportWorld();
            if (Input.GetKeyDown(KeyCode.F9)) ReportMount();
            if (Input.GetKeyDown(KeyCode.F10)) ReportCompanions();
            
            // v3.0: Repeat Last / Message History
            if (Input.GetKeyDown(KeyCode.F11))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    TTSManager.ReadPreviousMessage();
                else
                    TTSManager.RepeatLast();
            }

            // v1.2.1: Debug Tool (Shift+F3)
            if (Input.GetKeyDown(KeyCode.F3) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                DumpPayableInfo();
            }

            // v3.1: Poll speech queue
            TTSManager.Update();

            HandleHover();
            HandleCastleProximity();
            HandleBaseCampOrientation();
        }

        void HandleHover()
        {
            // v2.4: "Running Silence" Fix - Increased range to 12m
            var current = _player.selectedPayable as MonoBehaviour; 
            
            if (current == null)
            {
                current = GetClosestPayable();
            }
            if (current != null)
            {
                    // v2.2: Improved Cleanup
                    string rawName = PayableNameResolver.CleanName(current.name);
                    
                    // v2.2: Ignore Player self-hover immediately
                    if (current.GetComponent<Player>() != null) return;

                    // v2.3.1: Ignore current steed
                    if (_player.steed != null && current.gameObject == _player.steed.gameObject) return;

                    int price = 0; 
                    
                    // Reflection to get Price safely (cached per type)
                    var currentType = current.GetType();
                    if (!_pricePropertyCache.TryGetValue(currentType, out var priceProp))
                    {
                        priceProp = currentType.GetProperty("Price");
                        _pricePropertyCache[currentType] = priceProp;
                    }
                    if (priceProp != null) price = (int)priceProp.GetValue(current, null);
                    
                    // v2.2: Smart Tech Logic for Towers
                    string techWarning = "";
                    if (rawName.Contains("Watchtower") || rawName.Contains("Tower")) // Check if it's a tower
                    {
                         var tower = current.GetComponent<Tower>();
                         if (tower != null)
                         {
                             // Logic:
                             // Level 0 (Mound) -> 1 (Wood) -> 2 (Wood Max) -> 3 (Stone) -> 4 (Stone Max) -> 5 (Iron) ??
                             // Based on gameplay:
                             // If Level <= 2, we are in Wood age.
                             // If Level == 3, we need Stone to go to 4.
                             // If Level == 4+, we need Iron for higher?
                             
                             // Kingdom.StoneBuildingUnlocked / IronBuildingUnlocked
                             var k = Managers.Inst.kingdom;
                             if (k != null)
                             {
                                 if (tower.level >= 3 && !k.StoneBuildingUnlocked)
                                 {
                                     techWarning = "Need Stone Tech";
                                     price = 0; // Can't buy
                                 }
                                 else if (tower.level >= 5 && !k.IronBuildingUnlocked)
                                 {
                                     techWarning = "Need Iron Tech";
                                     price = 0;
                                 }
                             }
                         }
                    }

                    // Check change
                    // v2.2: Use techWarning in change detection
                    bool changed = (current != _lastPayable || rawName != _lastName || price != _lastPrice || techWarning != _lastSpokenMsg);

                    if (changed)
                    {
                        string action = "Build";
                        string currency = GetCurrencyName(current);
                        
                        // v1.2.1: Refined Actions
                        // v2.5: Use rawName for better matching after cleanup/mapping
                        if (rawName.Contains("Bank") || rawName.Contains("Chest")) action = "Deposit";
                        else if (rawName.Contains("Boat") || rawName.Contains("Ship")) action = "Repair Boat";
                        else if (rawName.Contains("Portal") || rawName.Contains("Border")) action = "Destroy Portal";
                        else if (rawName.Contains("Beggar") || rawName.Contains("Citizen") || rawName.Contains("Hermit")) action = "Hire";
                        else if (rawName.Contains("Shop") || rawName.Contains("Merchant")) action = "Buy";
                        else if (rawName.Contains("Teleporter")) action = "Teleport";
                        else if (rawName.Contains("Bell")) action = "Call";
                        else if (rawName.Contains("Gem Guard") || rawName.Contains("GemKeeper")) action = "Withdraw";
                        else if (rawName.Contains("Wreck") || rawName.Contains("Ruin")) action = "Repair";
                        else if (rawName.Contains("Tree") && !rawName.Contains("Close")) action = "Chop";
                        else if (rawName.Contains("Banner") || rawName.Contains("Bomb")) action = "Attack";
                        else if (rawName.Contains("Scythe")) action = "Buy Scythe";
                        
                        // Smart Heuristic v2
                        if (action == "Build")
                        {
                            if (_endsWithDigitRegex.IsMatch(rawName) || 
                                _endsWithUpperRegex.IsMatch(rawName))
                            {
                                action = "Upgrade";
                            }
                        }
                        
                        // Farm Upgrade
                        if (action == "Build" && current.name.Contains("Farm") && price >= 3) action = "Upgrade"; 
                        
                        // v2.4: Smart Castle & Wall Logic
                        if (current.name.Contains("Castle") || current.name.Contains("Town"))
                        {
                            var castle = current.GetComponent<Castle>();
                            if (castle != null)
                            {
                                // If level is 0/1/low it's a Camp, otherwise Castle upgrades
                                if ((int)castle.level == 0) action = "Build Camp";
                                else action = "Upgrade Castle";
                            }
                        }
                        
                        if (current.name.Contains("Wall"))
                        {
                            var wall = current.GetComponent<Wall>();
                            if (wall != null)
                            {
                                if (wall.level == 0 && price <= 1) action = "Build Wall";
                                else action = "Upgrade Wall";
                                
                                // v3.0: Blocked Upgrade Warning
                                var castle = FindClosestCastle();
                                if (castle != null && wall.level >= (int)castle.level)
                                {
                                    techWarning = "Upgrade blocked, upgrade castle first";
                                }
                            }
                            else if (price < 2) action = "Build";
                            else action = "Upgrade";
                        }
                        
                        // v3.0: Tree fallback — catch trees that slipped past the name check
                        if (action == "Build" && (rawName.Contains("Tree") || current.GetComponent<WorkableTree>() != null))
                        {
                            action = "Chop";
                        }
                        
                        // v2.3: Mount Logic
                        // If it's a mount (from mapping or component), action is "Switch" (or "Unlock" if expensive?)
                        if (rawName.Contains("Mount") || current.name.Contains("Steed") || current.name.Contains("Horse"))
                        {
                             action = "Switch";
                        }

                        // v2.2: Apply Tech Warning Override
                        string message;
                        if (!string.IsNullOrEmpty(techWarning))
                        {
                            message = $"{rawName}, {techWarning}";
                        }
                        else
                        {
                             // Special case: don't say price if it's 0 (unless specifically needed)
                            if (price > 0 || action == "Withdraw" || action == "Teleport")
                                message = $"{rawName}, {price} {currency}, {action}";
                            else
                                message = $"{rawName}, {action}";
                        }

                        // v2.3: Strict Anti-Spam
                        if (message != _lastSpokenMsg)
                        {
                             ModMenu.Speak(message, interrupt: false);
                             _lastSpokenMsg = message;
                             _spamTimer = Time.time; 
                        }
                        
                        _lastPayable = current;
                        _lastName = rawName;
                        _lastPrice = price;
                    }
            }
            else
            {
                 // v2.3: Reset state when no target found
                 if (_lastPayable != null)
                 {
                     _lastPayable = null;
                     _lastSpokenMsg = "";
                     _lastName = "";
                     _lastPrice = -1;
                 }
            }
        }

        // v2.4: Helper to find closest payable if game hasn't selected one
        // v2.4: Helper to find closest payable if game hasn't selected one
        MonoBehaviour GetClosestPayable()
        {
            // Optimization: Throttle checks
            if (Time.time < _lastPayableCheckTime + PAYABLE_CHECK_INTERVAL) return _lastPayable as MonoBehaviour; // Return last known good
            _lastPayableCheckTime = Time.time;

            if (Managers.Inst == null || Managers.Inst.payables == null) return null;
            
            float searchRange = 12.0f; // v2.4: Increased to 12m for running players
            float playerX = _player.transform.position.x;
            
            MonoBehaviour closest = null;
            float closestDist = float.MaxValue;

            foreach (var p in Managers.Inst.payables.AllPayables)
            {
                if (p == null) continue;
                var mb = p as MonoBehaviour;
                if (mb == null || !mb.gameObject.activeInHierarchy) continue;

                // v2.3.1: Ignore current mount
                if (_player.steed != null && mb.gameObject == _player.steed.gameObject) continue;

                float dist = Mathf.Abs(mb.transform.position.x - playerX);
                if (dist < searchRange && dist < closestDist)
                {
                    closest = mb;
                    closestDist = dist;
                }
            }
            return closest;
        }

        // v3.0: Find closest castle for upgrade-blocking checks and base camp orientation
        // v3.0: Find closest castle for upgrade-blocking checks and base camp orientation
        Castle FindClosestCastle()
        {
            // Optimization: Cached Castle Search (P1 Fix)
            if (_cachedCastles == null || Time.time > _castleCacheTimer + CASTLE_CACHE_INTERVAL) 
            {
                _cachedCastles = FindObjectsOfType<Castle>();
                _castleCacheTimer = Time.time;
            }

            if (_cachedCastles == null || _cachedCastles.Length == 0) return null;

            Castle closest = null;
            float closestDist = float.MaxValue;
            float playerX = _player.transform.position.x;

            foreach (var c in _cachedCastles)
            {
                if (c == null) continue;
                float dist = Mathf.Abs(c.transform.position.x - playerX);
                if (dist < closestDist)
                {
                    closest = c;
                    closestDist = dist;
                }
            }
            return closest;
        }

        // v3.0: Announce base camp direction when first arriving on an island
        void HandleBaseCampOrientation()
        {
            if (_baseCampAnnounced) return;
            
            // Wait a moment after loading before announcing
            if (Time.timeSinceLevelLoad < 3.0f) return;

            var castle = FindClosestCastle();
            if (castle != null)
            {
                float playerX = _player.transform.position.x;
                float castleX = castle.transform.position.x;
                string direction = castleX > playerX ? "right" : "left";
                ModMenu.Speak($"Base camp on the {direction}", interrupt: false);
                _baseCampAnnounced = true;
            }
        }

        void ReportDetailedInfo()
        {
            var current = _player.selectedPayable as MonoBehaviour ?? GetClosestPayable();
            if (current == null) {
                ModMenu.Speak("No object selected.");
                return;
            }

            string name = PayableNameResolver.CleanName(current.name);
            string currency = GetCurrencyName(current);
            int price = 0;
            
            // Get price
            var currentType = current.GetType();
            if (!_pricePropertyCache.TryGetValue(currentType, out var priceProp)) {
                priceProp = currentType.GetProperty("Price");
                _pricePropertyCache[currentType] = priceProp;
            }
            if (priceProp != null) price = (int)priceProp.GetValue(current, null);

            // Get level if possible
            string levelInfo = "";
            var wall = current.GetComponent<Wall>();
            if (wall != null) levelInfo = $", Level {wall.level}";
            
            var castle = current.GetComponent<Castle>();
            if (castle != null) levelInfo = $", Level {(int)castle.level}";

            var tower = current.GetComponent<Tower>();
            if (tower != null) 
            {
                name = "Watchtower"; // Override
                levelInfo = $", Level {tower.level}";
            }

            ModMenu.Speak($"{name}, {price} {currency}{levelInfo}");
        }

        // v1.2.1: Debug Reflection Dump
        void DumpPayableInfo()
        {
            var current = _player.selectedPayable as MonoBehaviour;
            if (current == null) current = GetClosestPayable();

            if (current == null)
            {
                ModMenu.Speak("No object found to inspect.");
                return;
            }

            ModMenu.Speak($"Inspecting: {current.name}");
            Debug.Log($"[DEBUG] Inspecting {current.name} ({current.GetType().Name})");

            var fields = current.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var f in fields)
            {
                Debug.Log($"   Field: {f.Name} = {f.GetValue(current)}");
            }
            
            var props = current.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var p in props)
            {
                try {
                    Debug.Log($"   Property: {p.Name} = {p.GetValue(current, null)}");
                } catch {}
            }
            
            ModMenu.Speak("Dumped fields to log file.");
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


        void CheckCompassAndSafety()
        {
            string dir = (_player.mover.GetDirection() == Side.Left) ? "Left" : "Right";
            
            // v3.1: Add threat status and time of day
            string threat = "Safe";
            string timeOfDay = "Day";
            string borderInfo = "";
            try
            {
                var enemyMgr = FindObjectOfType<EnemyManager>();
                if (enemyMgr != null && enemyMgr.IsDangerous)
                    threat = "DANGER";
                    
                var kingdom = FindObjectOfType<Kingdom>();
                if (kingdom != null && !kingdom.isDaytime)
                    timeOfDay = "Night";
                    
                // 9.6 Border Position
                var walls = FindObjectsOfType<Wall>();
                if (walls != null && walls.Length > 0)
                {
                    float min = float.MaxValue;
                    foreach(var w in walls) {
                        float d = Mathf.Abs(w.transform.position.x - _player.transform.position.x);
                        if (d < min) min = d;
                    }
                    if (min < float.MaxValue) borderInfo = $", {Mathf.RoundToInt(min)}m to Wall";
                }
            }
            catch { }
            
            ModMenu.Speak($"Facing {dir}, {timeOfDay}, {threat}{borderInfo}");
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
            if (_player.steed == null) return;
            string n = PayableNameResolver.CleanName(_player.steed.name);
            string status = _player.steed.IsTired ? "Tired" : "Ready";
            ModMenu.Speak($"{n}, {status}");
        }

        void ReportCompanions()
        {
            int archers = FindObjectsOfType<Archer>().Length;
            int workers = FindObjectsOfType<Worker>().Length;
            int peasants = FindObjectsOfType<Peasant>().Length;

            ModMenu.Speak($"{archers} Archers, {workers} Workers, {peasants} Peasants");
        }

        void OnDestroy()
        {
            _lastPayable = null;
            _lastSpokenMsg = "";
            _pricePropertyCache.Clear();
        }


        // v1.2.1: Reflection-based Currency Detection
        string GetCurrencyName(MonoBehaviour target)
        {
            // v2.5: Explicit overrides for known gem containers
            if (target.name.Contains("Gem Guard") || target.name.Contains("GemKeeper")) return "gems";

            try
            {
                // Try to find "currency" or "priceType" fields
                var type = target.GetType();
                
                // Common field names in Kingdom Assembly
                var fields = new[] { "currency", "priceType", "coinType", "paymentType" };
                
                foreach (var fieldName in fields)
                {
                    FieldInfo field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        object value = field.GetValue(target);
                        if (value != null)
                        {
                            string sVal = value.ToString().ToLower();
                            if (sVal.Contains("gem")) return "gems";
                            if (sVal.Contains("coin") || sVal.Contains("gold")) return "coins";
                        }
                    }
                    
                     // Also check Properties
                    PropertyInfo prop = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (prop != null)
                    {
                         object value = prop.GetValue(target, null);
                         if (value != null)
                         {
                             string sVal = value.ToString().ToLower();
                             if (sVal.Contains("gem")) return "gems";
                             if (sVal.Contains("coin") || sVal.Contains("gold")) return "coins";
                         }
                    }
                }
            }
            catch {}
            
            return "coins"; // Default
        }
    }
}