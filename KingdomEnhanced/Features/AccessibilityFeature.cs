using System;
using UnityEngine;
using KingdomEnhanced.UI;
using KingdomEnhanced.Systems;
using KingdomEnhanced.Utils; 
using KingdomEnhanced.Systems.Accessibility; 
using System.Collections.Generic;
using System.Text.RegularExpressions;
#if IL2CPP
using KingdomEnhanced.Shared.Attributes;
#endif

namespace KingdomEnhanced.Features
{
#if IL2CPP
    [RegisterTypeInIl2Cpp]
#endif
    public class AccessibilityFeature : MonoBehaviour
    {
#if IL2CPP
        public AccessibilityFeature(IntPtr ptr) : base(ptr) { }
#endif
        private Player _player;
        private MonoBehaviour _lastPayable = null;

        private RadarSystem _radarSystem; 

        void Start()
        {
            _baseCampAnnounced = false;
        }

        private bool _wasInCastle = false;
        
        private bool _baseCampAnnounced = false;
        private bool _wasInVillage = false; 
        
        private float _spamTimer = 0f;
        private string _lastSpokenMsg = "";
        private string _lastName = "";
        private int _lastPrice = -1;


        private float _lastPayableCheckTime = 0f;
        private const float PAYABLE_CHECK_INTERVAL = 0.15f; 

        private Castle[] _cachedCastles;
        private float _castleCacheTimer = 0f;
        private const float CASTLE_CACHE_INTERVAL = 2.0f; 

        private static readonly Regex _endsWithDigitRegex = new Regex(@"\d$", RegexOptions.Compiled);
        private static readonly Regex _endsWithUpperRegex = new Regex(@"[A-Z]$", RegexOptions.Compiled);

        void Update()
        {
            if (!ModMenu.EnableAccessibility) return;

            if (_player == null)
            {
                _player = FindFirstObjectByType<Player>();
                if (_player != null) _radarSystem = new RadarSystem(_player);
            }
            if (_player == null) return;

            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    AccessibilityReportHandler.ReportDetailedInfo(_player);
                else
                    _radarSystem.Pulse(); 
            }
            if (Input.GetKeyDown(KeyCode.F6)) AccessibilityReportHandler.CheckCompassAndSafety(_player);
            if (Input.GetKeyDown(KeyCode.F7)) AccessibilityReportHandler.ReportWallet(_player);
            if (Input.GetKeyDown(KeyCode.F8)) AccessibilityReportHandler.ReportWorld();
            if (Input.GetKeyDown(KeyCode.F9)) AccessibilityReportHandler.ReportMount(_player);
            if (Input.GetKeyDown(KeyCode.F10)) AccessibilityReportHandler.ReportCompanions();
            
            if (Input.GetKeyDown(KeyCode.F11))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    TTSManager.ReadPreviousMessage();
                else
                    TTSManager.RepeatLast();
            }

            if (Input.GetKeyDown(KeyCode.F3) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                AccessibilityReportHandler.DumpPayableInfo(_player);
            }

            TTSManager.Update();

            HandleHover();
            HandleCastleProximity();
            HandleBaseCampOrientation();
        }

        void HandleHover()
        {
            var current = _player.selectedPayable as MonoBehaviour; 
            if (current == null) current = GetClosestPayable();

            if (current != null)
            {
                var payable = current.GetComponent<Payable>();
                if (payable == null) return; 

                
                if (current.GetComponent<Player>() != null || current.gameObject == _player.gameObject) return;
                
                if (_player.steed != null && current.gameObject == _player.steed.gameObject) return;

                string rawName = PayableNameResolver.CleanName(current.name);
                
                
                int price = payable.Price;
                string currency = (payable.Currency == CurrencyType.Gems) ? "Gems" : "Coins";



                
                if (string.IsNullOrEmpty(rawName))
                {
                    rawName = current.name.Replace("(Clone)", "").Trim();
                }

                
                if (current.GetComponent<Boat>() != null || current.name.ToLower().Contains("boat")) 
                {
                    rawName = "Boat";
                }
                else if (current.name.ToLower().Contains("wreck"))
                {
                    rawName = "Shipwreck";
                }
                else if (current.name.ToLower().Contains("wharf"))
                {
                    rawName = "Wharf";
                }

                
                string techWarning = "";
                if (payable.IsLocked(_player, out LockIndicator.LockReason reason))
                {
                    
                    switch (reason)
                    {
                        case LockIndicator.LockReason.StoneTechRequired: techWarning = "Need Stone Tech"; break;
                        case LockIndicator.LockReason.IronTechRequired: techWarning = "Need Iron Tech"; break;
                        case LockIndicator.LockReason.HermitLocked: techWarning = "Locked by Hermit"; break;
                        case LockIndicator.LockReason.NoUpgrade: techWarning = "Fully Upgraded"; break; 
                        case LockIndicator.LockReason.Base: techWarning = "Base upgrade required"; break;
                        default: techWarning = "Locked"; break; 
                    }
                    if (reason == LockIndicator.LockReason.NotLocked) techWarning = ""; 
                }

                
                bool isProtecting = false;
                if (rawName.Contains("Tree") && (isProtecting = IsTreeProtectingVillage(current.transform.position.x)))
                {
                    techWarning = "WARNING: Destroys Village";
                }

                
                string action = "Build";
                
                
                if (rawName == "Boat" || rawName.Contains("Boat") || rawName.Contains("Ship"))
                {
                     if (price <= 3) action = "Add Parts";
                     else if (price >= 10) action = "Sail";
                     else action = "Repair Hull";
                     
                     if (rawName.Contains("Wreck") || rawName.Contains("Ruin")) action = "Repair Hull";
                }
                
                if (rawName.Contains("Statue") || rawName.Contains("Idol"))
                {
                    action = (currency == "Gems") ? "Pay" : "Activate";
                }
                
                if (rawName.Contains("Bank") || rawName.Contains("Chest")) action = "Deposit";
                else if (rawName.Contains("Portal") || rawName.Contains("Border")) action = "Destroy Portal";
                else if (rawName.Contains("Beggar") || rawName.Contains("Citizen") || rawName.Contains("Hermit")) action = "Hire";
                else if (rawName.Contains("Shop") || rawName.Contains("Merchant")) action = (rawName.Contains("Merchant")) ? "Invest" : "Buy";
                else if (rawName.Contains("Teleporter")) action = "Teleport";
                else if (rawName.Contains("Bell")) action = "Call";
                else if (rawName.Contains("Gem Guard") || rawName.Contains("GemKeeper")) action = "Withdraw";
                
                else if (rawName.Contains("Tree") && !rawName.Contains("Close")) action = "Chop";
                else if (rawName.Contains("Mount") || rawName.Contains("Chimera") || current.name.Contains("Steed") || current.name.Contains("Horse")) action = "Switch";
                else if (rawName.Contains("Banner")) action = "Expedition";
                
                
                if (action == "Build")
                {
                    
                    
                    if (_endsWithDigitRegex.IsMatch(rawName) || _endsWithUpperRegex.IsMatch(rawName)) action = "Upgrade";
                    
                    var wall = current.GetComponent<Wall>();
                    if (wall != null && wall.level > 0) action = "Upgrade Wall";
                    
                    var castle = current.GetComponent<Castle>();
                    
                    if (castle != null)
                    {
                         action = (castle.level == 0) ? "Build" : "Upgrade";
                    }
                    
                    var farm = current.GetComponent<Farmhouse>();
                    if (farm != null && price >= 3) action = "Upgrade Farm"; 
                }

                
                string message = $"{rawName}";
                
                if (!string.IsNullOrEmpty(techWarning))
                {
                    message += $", {techWarning}";
                }
                else
                {
                    
                    if (price > 0 || action == "Withdraw" || action == "Deposit") 
                        message += $", {price} {currency}, {action}";
                    else
                        message += $", {action}";
                }

                
                bool changed = (current != _lastPayable || message != _lastSpokenMsg);
                if (changed)
                {
                    ModMenu.Speak(message, interrupt: false);
                    _lastSpokenMsg = message;
                    _spamTimer = Time.time;
                }

                _lastPayable = current;
                _lastName = rawName;
                _lastPrice = price;
            }
            else
            {
                 if (_lastPayable != null)
                 {
                     _lastPayable = null;
                     _lastSpokenMsg = "";
                     _lastName = "";
                     _lastPrice = -1;
                 }
            }
        }

        MonoBehaviour GetClosestPayable()
        {
            if (Time.time < _lastPayableCheckTime + PAYABLE_CHECK_INTERVAL) return _lastPayable as MonoBehaviour; 
            _lastPayableCheckTime = Time.time;

            if (Managers.Inst == null || Managers.Inst.payables == null) return null;
            
            float searchRange = 18.0f; 
            float playerX = _player.transform.position.x;
            
            MonoBehaviour closest = null;
            float closestDist = float.MaxValue;

            foreach (var p in Managers.Inst.payables.AllPayables)
            {
                if (p == null) continue;
                var mb = p as MonoBehaviour;
                if (mb == null || !mb.gameObject.activeInHierarchy) continue;

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

        Castle FindClosestCastle()
        {
            if (_cachedCastles == null || Time.time > _castleCacheTimer + CASTLE_CACHE_INTERVAL) 
            {
                _cachedCastles = FindObjectsByType<Castle>(FindObjectsSortMode.None);
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

        void HandleBaseCampOrientation()
        {
            if (_baseCampAnnounced) return;
            
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

        private float _zoneUpdateTimer = 0f;
        private float _castleMinX = 0f;
        private float _castleMaxX = 0f;
        private List<Vector2> _campIntervals = new List<Vector2>();
        
        private struct TriggerZone {
            public Rect Box;
            public Color Color;
            public string Label;
        }
        private List<TriggerZone> _debugZones = new List<TriggerZone>();
        
        private float _announcerCooldown = 0f;

        void UpdateZones()
        {
            if (_player == null) return;
            
            _debugZones.Clear();
            float y = _player.transform.position.y;
            float h = 4.0f; 
            float wBox = 0.5f;

            
            float minWallX = float.MaxValue, maxWallX = float.MinValue;
            var walls = FindObjectsByType<Wall>(FindObjectsSortMode.None);
            if (walls != null && walls.Length > 0)
            {
                foreach (var w in walls)
                {
                    if (w.transform.position.x < minWallX) minWallX = w.transform.position.x;
                    if (w.transform.position.x > maxWallX) maxWallX = w.transform.position.x;
                }
            }
            else
            {
                var k = Managers.Inst?.kingdom;
                if (k != null) { minWallX = k.transform.position.x - 20f; maxWallX = k.transform.position.x + 20f; }
            }

            _castleMinX = minWallX;
            _castleMaxX = maxWallX;

            if (minWallX != float.MaxValue)
            {
                _debugZones.Add(new TriggerZone { Box = new Rect(minWallX, y - 2f, 0.1f, h), Color = Color.cyan, Label = "Last Wall" });
                _debugZones.Add(new TriggerZone { Box = new Rect(maxWallX, y - 2f, 0.1f, h), Color = Color.cyan, Label = "Last Wall" });
                
                _debugZones.Add(new TriggerZone { Box = new Rect(minWallX - wBox, y - 1f, wBox, h - 1f), Color = Color.red, Label = "Entering/Leaving Castle (Trigger)" });
                _debugZones.Add(new TriggerZone { Box = new Rect(maxWallX,        y - 1f, wBox, h - 1f), Color = Color.red, Label = "Entering/Leaving Castle (Trigger)" });
            }

            
            _campIntervals.Clear();
            var camps = GameObject.FindObjectsByType<BeggarCamp>(FindObjectsSortMode.None);
            if (camps != null)
            {
                foreach (var camp in camps)
                {
                    float cx = camp.transform.position.x;
                    float treeL1 = cx - 15f; 
                    float treeR1 = cx + 15f;
                    float treeL_N = cx - 30f;
                    float treeR_N = cx + 30f;

                    if (Managers.Inst != null && Managers.Inst.payables != null)
                    {
                        float minL = float.MaxValue, minR = float.MaxValue;
                        float maxL = float.MinValue, maxR = float.MinValue;

                        foreach (var p in Managers.Inst.payables.AllPayables)
                        {
                            var mb = p as MonoBehaviour;
                            if (mb == null || !mb.name.Contains("Tree") || mb.name.Contains("Close")) continue;
                            float tx = mb.transform.position.x;

                            if (tx < cx && (cx - tx) < minL) { minL = cx - tx; treeL1 = tx; }
                            if (tx > cx && (tx - cx) < minR) { minR = tx - cx; treeR1 = tx; }
                            
                            if (tx < cx && (cx - tx) < 100f && (cx - tx) > maxL) { maxL = cx - tx; treeL_N = tx; }
                            if (tx > cx && (tx - cx) < 100f && (tx - cx) > maxR) { maxR = tx - cx; treeR_N = tx; }
                        }
                    }

                    _campIntervals.Add(new Vector2(treeL1, treeR1));

                    _debugZones.Add(new TriggerZone { Box = new Rect(treeL1, y - 2f, 0.1f, h), Color = Color.cyan, Label = "Inner Tree Line" });
                    _debugZones.Add(new TriggerZone { Box = new Rect(treeR1, y - 2f, 0.1f, h), Color = Color.cyan, Label = "Inner Tree Line" });

                    _debugZones.Add(new TriggerZone { Box = new Rect(treeL1 - wBox, y - 1f, wBox, h - 1f), Color = Color.red, Label = "Entering/Leaving Camp (Trigger)" });
                    _debugZones.Add(new TriggerZone { Box = new Rect(treeR1,        y - 1f, wBox, h - 1f), Color = Color.red, Label = "Entering/Leaving Camp (Trigger)" });

                    _debugZones.Add(new TriggerZone { Box = new Rect(treeL_N, y - 0.5f, Mathf.Max(0.1f, treeL1 - treeL_N - wBox), h - 2f), Color = new Color(0.3f, 0.3f, 0.3f, 0.6f), Label = "Exclusion Zone (No triggers)" });
                    _debugZones.Add(new TriggerZone { Box = new Rect(treeR1 + wBox, y - 0.5f, Mathf.Max(0.1f, treeR_N - treeR1 - wBox), h - 2f), Color = new Color(0.3f, 0.3f, 0.3f, 0.6f), Label = "Exclusion Zone (No triggers)" });
                }
            }
        }

        void HandleCastleProximity()
        {
            if (_announcerCooldown > 0f) _announcerCooldown -= Time.deltaTime;

            _zoneUpdateTimer -= Time.deltaTime;
            if (_zoneUpdateTimer <= 0f)
            {
                UpdateZones();
                _zoneUpdateTimer = 2.0f;
            }

            if (_player == null) return;
            float playerX = _player.transform.position.x;
            
            if (ModMenu.EnableCastleAnnouncer)
            {
                bool insideCastle = playerX >= _castleMinX && playerX <= _castleMaxX;
                if (insideCastle != _wasInCastle)
                {
                    _wasInCastle = insideCastle;
                    if (_announcerCooldown <= 0f)
                    {
                        ModMenu.Speak(insideCastle ? "Entering Castle" : "Leaving Castle");
                        _announcerCooldown = 0.5f;
                    }
                }
            }

            bool insideAnyCamp = false;
            foreach (var interval in _campIntervals)
            {
                if (playerX >= interval.x && playerX <= interval.y)
                {
                    insideAnyCamp = true;
                    break;
                }
            }

            if (insideAnyCamp != _wasInVillage)
            {
                _wasInVillage = insideAnyCamp;
                if (_announcerCooldown <= 0f)
                {
                    ModMenu.Speak(insideAnyCamp ? "Entering Camp" : "Leaving Camp");
                    _announcerCooldown = 0.5f;
                }
            }
        }

        void OnGUI()
        {
            if (!ModMenu.DebugZones) return;

            Camera cam = Camera.main;
            if (cam == null)
                cam = UnityEngine.Object.FindFirstObjectByType<Camera>();

            if (cam == null) return;

            foreach (var zone in _debugZones)
            {
                
                Vector3 screenPointBL = cam.WorldToScreenPoint(new Vector3(zone.Box.xMin, zone.Box.yMin, 0));
                Vector3 screenPointTR = cam.WorldToScreenPoint(new Vector3(zone.Box.xMax, zone.Box.yMax, 0));
                
                
                float width = Mathf.Abs(screenPointTR.x - screenPointBL.x);
                float height = Mathf.Abs(screenPointTR.y - screenPointBL.y);
                
                Rect screenRect = new Rect(Mathf.Min(screenPointBL.x, screenPointTR.x), Screen.height - Mathf.Max(screenPointBL.y, screenPointTR.y), width, height);
                
                
                Color prevC = GUI.color;
                Color prevBg = GUI.backgroundColor;
                GUI.backgroundColor = zone.Color;
                GUI.color = Color.white;
                GUI.Box(screenRect, GUIContent.none, GUI.skin.box);
                
                
                GUIStyle style = new GUIStyle(GUI.skin.label) { 
                    fontSize = 11, alignment = TextAnchor.MiddleCenter, 
                    normal = { textColor = Color.white }, fontStyle = FontStyle.Bold 
                };
                
                
                GUI.color = Color.black;
                GUI.Label(new Rect(screenRect.x - 49, screenRect.yMax + 1, width + 100, 20), zone.Label, style);
                
                
                GUI.color = zone.Color;
                GUI.Label(new Rect(screenRect.x - 50, screenRect.yMax, width + 100, 20), zone.Label, style);
                
                GUI.backgroundColor = prevBg;
                GUI.color = prevC;
            }
        }

        private BeggarCamp FindClosestCamp()
        {
            
            var camps = GameObject.FindObjectsByType<BeggarCamp>(FindObjectsSortMode.None);
            if (camps == null || camps.Length == 0) return null;
            
            BeggarCamp closest = null;
            float minDst = float.MaxValue;
            foreach (var c in camps)
            {
                float d = Mathf.Abs(c.transform.position.x - _player.transform.position.x);
                if (d < minDst) { minDst = d; closest = c; }
            }
            return closest;
        }

        private bool IsTreeProtectingVillage(float treeX)
        {
            
            
            foreach (var interval in _campIntervals)
            {
                
                if (Mathf.Abs(treeX - interval.x) < 0.1f || Mathf.Abs(treeX - interval.y) < 0.1f)
                {
                    return true; 
                }
            }
            return false;
        }


        void OnDestroy()
        {
            _lastPayable = null;
            _lastSpokenMsg = "";
        }
    }
}
