using UnityEngine;
using KingdomEnhanced.UI;
using System.Reflection;
using HarmonyLib;
using System;
using System.Collections.Generic;
using KingdomMod;

namespace KingdomEnhanced.Features
{
    /// <summary>
    /// Manages world information display and game status monitoring
    /// </summary>
    public class WorldManager : MonoBehaviour
    {
        #region GUI Styles
        private GUIStyle _timeStyle;
        private GUIStyle _coinStyle;
        #endregion

        #region Timers
        private float _statusTimer = 0f;
        private float _radarTimer = 0f;
        private float _lastAttackAlert = 0f;
        
        private const float STATUS_CHECK_INTERVAL = 2.0f;
        private const float RADAR_CHECK_INTERVAL = 4.0f;
        private const float ATTACK_ALERT_COOLDOWN = 60f;
        #endregion

        #region State Tracking
        private bool _wasDay = true;
        private bool _wasBloodMoon = false;
        #endregion

        #region Reflection Caches
        private FieldInfo _bloodMoonField;
        private PropertyInfo _bloodMoonProperty;
        private FieldInfo _enemiesListField;
        private bool _fieldsDiscovered = false;
        #endregion

        #region Unity Lifecycle
        void Start()
        {
            DiscoverFields();
            Debug.Log("[WorldManager] Started - HUD Display Mode");
        }

        void Update()
        {
            if (!IsManagersValid()) return;
            UpdateTimers(Time.deltaTime);
        }

        void OnGUI()
        {
            if (!ModMenu.DisplayTimes || !IsManagersValid()) return;
            InitializeStyles();
            DrawHUD();
        }
        #endregion

        #region Initialization
        private void DiscoverFields()
        {
            if (_fieldsDiscovered) return;

            try
            {
                // Try multiple approaches to find Blood Moon indicator
                var directorType = typeof(Director);
                
                // Approach 1: Search for bool fields with "blood" or "moon" in name
                var allFields = directorType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in allFields)
                {
                    string fieldName = field.Name.ToLower();
                    if (field.FieldType == typeof(bool) && (fieldName.Contains("blood") || fieldName.Contains("moon")))
                    {
                        _bloodMoonField = field;
                        Debug.Log($"[WorldManager] Found blood moon field: {field.Name}");
                        break;
                    }
                }

                // Approach 2: Try properties
                if (_bloodMoonField == null)
                {
                    var allProps = directorType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var prop in allProps)
                    {
                        string propName = prop.Name.ToLower();
                        if (prop.PropertyType == typeof(bool) && (propName.Contains("blood") || propName.Contains("moon")))
                        {
                            _bloodMoonProperty = prop;
                            Debug.Log($"[WorldManager] Found blood moon property: {prop.Name}");
                            break;
                        }
                    }
                }

                // Try to find Enemy List for radar
                var enemyType = typeof(EnemyManager);
                var enemyFields = enemyType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                foreach (var field in enemyFields)
                {
                    string fieldName = field.Name.ToLower();
                    if (fieldName.Contains("enem") || fieldName.Contains("list"))
                    {
                        _enemiesListField = field;
                        Debug.Log($"[WorldManager] Found enemy list field: {field.Name}");
                        break;
                    }
                }

                _fieldsDiscovered = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WorldManager] Error discovering fields: {ex.Message}");
            }
        }

        private void InitializeStyles()
        {
            if (_timeStyle == null)
            {
                _timeStyle = new GUIStyle(GUI.skin.label)
                {
                    normal = { textColor = new Color(1f, 0.9f, 0.5f) },
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (_coinStyle == null)
            {
                _coinStyle = new GUIStyle(GUI.skin.label)
                {
                    normal = { textColor = new Color(1f, 0.85f, 0.2f) },
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }
        }
        #endregion

        #region HUD Drawing
        private void DrawHUD()
        {
            const float hudWidth = 320f;
            float hudX = (Screen.width / 2) - (hudWidth / 2);
            float hudY = 20f;

            var director = Managers.Inst.director;
            
            // Time Display
            string timeDisplay = FormatTimeDisplay(director);
            DrawShadowedLabel(new Rect(hudX, hudY, hudWidth, 25), timeDisplay, _timeStyle);
            
            // Coin Count Display
            int bagCoins = GetPlayerCoins();
            if (bagCoins >= 0)
            {
                DrawShadowedLabel(
                    new Rect(hudX, hudY + 25, hudWidth, 22), 
                    $"💰 Bag: {bagCoins} Coins", 
                    _coinStyle
                );
            }
        }

        private string FormatTimeDisplay(Director director)
        {
            if (director == null) return "ERROR";

            try
            {
                float rawTime = director.currentTime;
                float totalHours = rawTime % 24f;
                int hours = Mathf.FloorToInt(totalHours);
                int minutes = Mathf.FloorToInt((totalHours % 1f) * 60f);
                
                string clock = string.Format("{0:00}:{1:00}", hours, minutes);
                string timeStr = director.IsDaytime ? "Day" : "Night";
                
                return $"DAY {director.CurrentIslandDays} | {timeStr} ({clock})";
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WorldManager] Error formatting time: {ex.Message}");
                return "TIME ERROR";
            }
        }

        private void DrawShadowedLabel(Rect rect, string text, GUIStyle style)
        {
            Color originalColor = GUI.color;
            
            // Shadow
            GUI.color = new Color(0, 0, 0, 0.8f);
            GUI.Label(new Rect(rect.x + 2, rect.y + 2, rect.width, rect.height), text, style);
            
            // Main text
            GUI.color = originalColor;
            GUI.Label(rect, text, style);
        }
        #endregion

        #region Coin Counting
        private int GetPlayerCoins()
        {
            try
            {
                var player = Managers.Inst?.kingdom?.GetPlayer(0);
                if (player == null)
                {
                    return -1;
                }

                // Try to get wallet
                var wallet = player.wallet;
                if (wallet == null)
                {
                    return -1;
                }

                // Method 1: Try GetCurrency method
                try
                {
                    int coins = wallet.GetCurrency(CurrencyType.Coins);
                    return coins;
                }
                catch
                {
                    // Method 2: Try reflection to find coins field
                    var walletType = wallet.GetType();
                    var fields = walletType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    foreach (var field in fields)
                    {
                        string fieldName = field.Name.ToLower();
                        if ((fieldName.Contains("coin") && !fieldName.Contains("gem")) || 
                            fieldName == "_coins" || 
                            fieldName == "coins")
                        {
                            if (field.FieldType == typeof(int))
                            {
                                return (int)field.GetValue(wallet);
                            }
                        }
                    }

                    // Method 3: Try properties
                    var properties = walletType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var prop in properties)
                    {
                        string propName = prop.Name.ToLower();
                        if ((propName.Contains("coin") && !propName.Contains("gem")) && prop.PropertyType == typeof(int))
                        {
                            return (int)prop.GetValue(wallet);
                        }
                    }
                }

                return -1;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WorldManager] Error getting player coins: {ex.Message}");
                return -1;
            }
        }
        #endregion

        #region Update Logic
        private void UpdateTimers(float deltaTime)
        {
            _statusTimer += deltaTime;
            _radarTimer += deltaTime;

            if (_statusTimer >= STATUS_CHECK_INTERVAL)
            {
                _statusTimer = 0f;
                CheckGameStatus();
            }

            if (_radarTimer >= RADAR_CHECK_INTERVAL)
            {
                _radarTimer = 0f;
                CheckForGreedAttack();
            }
        }
        #endregion

        #region Game Status Monitoring
        private void CheckGameStatus()
        {
            var director = Managers.Inst.director;
            if (director == null) return;
            
            CheckDayNightTransition(director);
            CheckBloodMoon(director);
        }

        private void CheckDayNightTransition(Director director)
        {
            try
            {
                if (director.IsDaytime && !_wasDay)
                {
                    ModMenu.Speak("<color=orange>☀ The sun rises.</color>");
                    _wasDay = true;
                }
                else if (!director.IsDaytime && _wasDay)
                {
                    ModMenu.Speak("<color=lightblue>🌙 Night approaches.</color>");
                    _wasDay = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WorldManager] Error checking day/night: {ex.Message}");
            }
        }

        private void CheckBloodMoon(Director director)
        {
            try
            {
                bool isBloodMoon = false;

                // Try field first
                if (_bloodMoonField != null)
                {
                    isBloodMoon = (bool)_bloodMoonField.GetValue(director);
                }
                // Try property
                else if (_bloodMoonProperty != null)
                {
                    isBloodMoon = (bool)_bloodMoonProperty.GetValue(director);
                }
                else
                {
                    return; // No blood moon detection available
                }
                
                if (isBloodMoon && !_wasBloodMoon)
                {
                    ModMenu.Speak("<color=red><b>🌑 BLOOD MOON!</b></color>");
                    _wasBloodMoon = true;
                }
                else if (!isBloodMoon && _wasBloodMoon)
                {
                    _wasBloodMoon = false;
                }
            }
            catch (Exception ex)
            {
                // Silent fail - blood moon detection is optional
            }
        }

        private void CheckForGreedAttack()
        {
            // Only check at night
            if (Managers.Inst.director.IsDaytime) return;

            try
            {
                var enemyManager = UnityEngine.Object.FindObjectOfType<EnemyManager>();
                if (enemyManager == null || _enemiesListField == null) return;

                int enemyCount = GetEnemyCount(enemyManager);
                
                if (ShouldTriggerSiegeAlert(enemyCount))
                {
                    ModMenu.Speak("<color=red><b>⚠️ SIEGE DETECTED!</b></color>");
                    _lastAttackAlert = Time.time;
                }
            }
            catch (Exception ex)
            {
                // Silent fail - greed detection is optional
            }
        }

        private int GetEnemyCount(EnemyManager enemyManager)
        {
            try
            {
                var enemyList = _enemiesListField.GetValue(enemyManager);
                if (enemyList == null) return 0;

                // Try to get count from list
                var countProperty = enemyList.GetType().GetProperty("Count");
                if (countProperty != null)
                {
                    return (int)countProperty.GetValue(enemyList);
                }

                var countField = enemyList.GetType().GetField("Count");
                if (countField != null)
                {
                    return (int)countField.GetValue(enemyList);
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private bool ShouldTriggerSiegeAlert(int enemyCount)
        {
            const int SIEGE_THRESHOLD = 15;
            return enemyCount > SIEGE_THRESHOLD && 
                   Time.time > _lastAttackAlert + ATTACK_ALERT_COOLDOWN;
        }
        #endregion

        #region Helper Methods
        private bool IsManagersValid()
        {
            return Managers.Inst != null && Managers.Inst.director != null;
        }
        #endregion
    }
}