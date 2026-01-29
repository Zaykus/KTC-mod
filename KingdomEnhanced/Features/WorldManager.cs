using UnityEngine;
using KingdomEnhanced.UI;
using System.Reflection;
using HarmonyLib;
using System;
using System.Collections.Generic;
using KingdomMod;
using Il2CppInterop.Runtime;

namespace KingdomEnhanced.Features
{
    /// <summary>
    /// Manages world information display and game status monitoring
    /// </summary>
    public class WorldManager : MonoBehaviour
    {
        #region Styles
        private GUIStyle _timeStyle;
        private GUIStyle _mapLabelStyle;
        private GUIStyle _minimapStyle;
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
        private FieldInfo _enemiesListField;
        private bool _fieldsDiscovered = false;
        #endregion

        #region Map Overlay
        private bool _showMinimap = false;
        private Dictionary<string, Vector2> _mapMarkers = new Dictionary<string, Vector2>();
        #endregion

        #region Unity Lifecycle
        void Start()
        {
            DiscoverFields();
            InitializeMapMarkers();
        }

        void Update()
        {
            if (!IsManagersValid()) return;
            
            UpdateTimers(Time.deltaTime);
            HandleInput();
        }

        void OnGUI()
        {
            if (!ModMenu.DisplayTimes || !IsManagersValid()) return;

            InitializeStyles();
            DrawHUD();
            
            if (_showMinimap)
            {
                DrawMinimap();
            }
        }
        #endregion

        #region Initialization
        private void DiscoverFields()
        {
            if (_fieldsDiscovered) return;

            try
            {
                // Find Blood Moon Field with fallback
                _bloodMoonField = AccessTools.Field(typeof(Director), "_isBloodMoonToday") 
                               ?? AccessTools.Field(typeof(Director), "isBloodMoonToday");

                if (_bloodMoonField == null)
                {
                    LogWarning("Blood Moon field not found");
                }

                // Find Enemy List for Radar
                _enemiesListField = AccessTools.Field(typeof(EnemyManager), "_enemies");

                if (_enemiesListField == null)
                {
                    LogWarning("Enemy list field not found");
                }

                _fieldsDiscovered = true;
                LogInfo("Fields discovered successfully");
            }
            catch (Exception ex)
            {
                LogError($"Error discovering fields: {ex.Message}");
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

            if (_mapLabelStyle == null)
            {
                _mapLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    normal = { textColor = new Color(0.8f, 1f, 0.8f) },
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft
                };
            }

            if (_minimapStyle == null)
            {
                _minimapStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = { textColor = Color.white }
                };
            }
        }

        private void InitializeMapMarkers()
        {
            // Initialize with common map markers
            _mapMarkers.Clear();
            _mapMarkers.Add("Portal", Vector2.zero);
            _mapMarkers.Add("Camp", Vector2.zero);
        }
        #endregion

        #region HUD Drawing
        private void DrawHUD()
        {
            const float hudWidth = 280f;
            float hudX = (Screen.width / 2) - (hudWidth / 2);
            float hudY = 20f;

            var director = Managers.Inst.director;
            
            // Time information
            string timeDisplay = FormatTimeDisplay(director);
            DrawShadowedLabel(new Rect(hudX, hudY, hudWidth, 25), timeDisplay, _timeStyle);
            
            // Optional: Draw map toggle hint
            if (_showMinimap)
            {
                DrawShadowedLabel(
                    new Rect(hudX, hudY + 25, hudWidth, 20), 
                    "Press M to hide map", 
                    _mapLabelStyle
                );
            }
        }

        private string FormatTimeDisplay(Director director)
        {
            if (director == null) return "ERROR";

            float rawTime = director.currentTime;
            float totalHours = rawTime % 24f;
            int hours = Mathf.FloorToInt(totalHours);
            int minutes = Mathf.FloorToInt((totalHours % 1f) * 60f);
            
            string clock = string.Format("{0:00}:{1:00}", hours, minutes);
            string timeStr = director.IsDaytime ? "Day" : "Night";
            
            return $"DAY {director.CurrentIslandDays} | {timeStr} ({clock})";
        }

        private void DrawShadowedLabel(Rect rect, string text, GUIStyle style)
        {
            Color originalColor = GUI.color;
            
            // Draw shadow
            GUI.color = new Color(0, 0, 0, 0.8f);
            GUI.Label(new Rect(rect.x + 2, rect.y + 2, rect.width, rect.height), text, style);
            
            // Draw main text
            GUI.color = originalColor;
            GUI.Label(rect, text, style);
        }
        #endregion

        #region Minimap System
        private void DrawMinimap()
        {
            const float mapWidth = 300f;
            const float mapHeight = 200f;
            float mapX = Screen.width - mapWidth - 20f;
            float mapY = 20f;

            Rect mapRect = new Rect(mapX, mapY, mapWidth, mapHeight);
            
            // Draw map background
            DrawMapBackground(mapRect);
            
            // Draw player position
            DrawPlayerMarker(mapRect);
            
            // Draw markers
            DrawMapMarkers(mapRect);
        }

        private void DrawMapBackground(Rect rect)
        {
            Color originalColor = GUI.color;
            GUI.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);
            GUI.Box(rect, "Map Overlay", _minimapStyle);
            GUI.color = originalColor;
        }

        private void DrawPlayerMarker(Rect mapRect)
        {
            try
            {
                var player = Managers.Inst?.kingdom?.GetPlayer(0);
                if (player != null)
                {
                    // Center position for player
                    float centerX = mapRect.x + mapRect.width / 2;
                    float centerY = mapRect.y + mapRect.height / 2;
                    
                    Color originalColor = GUI.color;
                    GUI.color = Color.yellow;
                    GUI.Box(new Rect(centerX - 3, centerY - 3, 6, 6), "");
                    GUI.color = originalColor;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error drawing player marker: {ex.Message}");
            }
        }

        private void DrawMapMarkers(Rect mapRect)
        {
            // Placeholder for map markers
            // In a full implementation, this would draw discovered locations, portals, etc.
            Color originalColor = GUI.color;
            GUI.color = new Color(0.5f, 1f, 0.5f);
            
            float labelY = mapRect.y + 30;
            foreach (var marker in _mapMarkers)
            {
                GUI.Label(new Rect(mapRect.x + 10, labelY, mapRect.width - 20, 20), 
                         $"• {marker.Key}", _mapLabelStyle);
                labelY += 20;
            }
            
            GUI.color = originalColor;
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

        private void HandleInput()
        {
            // Toggle minimap with M key
            if (Input.GetKeyDown(KeyCode.M))
            {
                _showMinimap = !_showMinimap;
                ModMenu.Speak(_showMinimap ? "Map Overlay Enabled" : "Map Overlay Disabled");
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

        private void CheckBloodMoon(Director director)
        {
            if (_bloodMoonField == null) return;

            try
            {
                bool isBloodMoon = (bool)_bloodMoonField.GetValue(director);
                
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
                LogError($"Error checking blood moon: {ex.Message}");
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
                LogError($"Error checking for greed attack: {ex.Message}");
            }
        }

        private int GetEnemyCount(EnemyManager enemyManager)
        {
            try
            {
                var enemyList = _enemiesListField.GetValue(enemyManager);
                if (enemyList == null) return 0;

                var countProperty = enemyList.GetType().GetProperty("Count");
                if (countProperty == null) return 0;

                return (int)countProperty.GetValue(enemyList);
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

        private void LogInfo(string message)
        {
            // Logging removed to avoid Plugin dependency
            // Use ModMenu.Speak for important messages if needed
        }

        private void LogWarning(string message)
        {
            // Logging removed to avoid Plugin dependency
            Debug.LogWarning($"[WorldManager] {message}");
        }

        private void LogError(string message)
        {
            // Logging removed to avoid Plugin dependency
            Debug.LogError($"[WorldManager] {message}");
        }
        #endregion
    }
}