using UnityEngine;
using KingdomEnhanced.UI;
using System.Reflection;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace KingdomEnhanced.Features
{
    public class WorldManager : MonoBehaviour
    {
        private GUIStyle _timeStyle;
        private GUIStyle _coinStyle;

        private float _statusTimer = 0f;
        private float _radarTimer = 0f;
        private float _lastAttackAlert = 0f;
        
        private const float STATUS_CHECK_INTERVAL = 2.0f;
        private const float RADAR_CHECK_INTERVAL = 4.0f;
        private const float ATTACK_ALERT_COOLDOWN = 60f;

        private bool _wasDay = true;




        private FieldInfo _enemiesListField;
        private bool _fieldsDiscovered = false;

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
            if (KingdomMonitor.Instance != null && KingdomMonitor.Instance.IsVisible) return;

            if (!ModMenu.DisplayTimes || !IsManagersValid()) return;
            InitializeStyles();
            DrawHUD();
        }

        private void DiscoverFields()
        {
            if (_fieldsDiscovered) return;

            try
            {
                /* Blood Moon reflection removed as unused */

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

        private void DrawHUD()
        {
            try
            {
                const float hudWidth = 320f;
                float hudX = (Screen.width / 2) - (hudWidth / 2);
                float hudY = 20f;

                var director = Managers.Inst?.director;
                if (director == null) return;

                string timeDisplay = FormatTimeDisplay(director);
                DrawShadowedLabel(new Rect(hudX, hudY, hudWidth, 25), timeDisplay, _timeStyle);

                var stats = GetPlayerWalletStats();
                if (stats.Coins >= 0)
                {
                    DrawShadowedLabel(
                        new Rect(hudX, hudY + 25, hudWidth, 22),
                        $" Coins: {stats.Coins}   Gems: {stats.Gems}",
                        _coinStyle
                    );
                }
            }
            catch { }
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
            
            GUI.color = new Color(0, 0, 0, 0.8f);
            GUI.Label(new Rect(rect.x + 2, rect.y + 2, rect.width, rect.height), text, style);
            
            GUI.color = originalColor;
            GUI.Label(rect, text, style);
        }

        private (int Coins, int Gems) GetPlayerWalletStats()
        {
            try
            {
                var player = Managers.Inst?.kingdom?.GetPlayer(0);
                if (player == null) return (-1, 0);

                var wallet = player.wallet;
                if (wallet == null) return (-1, 0);

                try
                {
                    int coins = wallet.GetCurrency(CurrencyType.Coins);
                    int gems = wallet.GetCurrency(CurrencyType.Gems);
                    return (coins, gems);
                }
                catch
                {
                    int c = -1;
                    int g = 0;
                    var walletType = wallet.GetType();
                    
                    // Fallback reflection for Coins
                    var cFields = walletType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var field in cFields)
                    {
                        string fn = field.Name.ToLower();
                        if (fn == "_coins" || fn == "coins" || (fn.Contains("coin") && !fn.Contains("gem")))
                        {
                            if (field.FieldType == typeof(int)) c = (int)field.GetValue(wallet);
                        }
                        if (fn == "_gems" || fn == "gems" || (fn.Contains("gem") && !fn.Contains("coin")))
                        {
                            if (field.FieldType == typeof(int)) g = (int)field.GetValue(wallet);
                        }
                    }
                    return (c, g);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WorldManager] Error getting wallet stats: {ex.Message}");
                return (-1, 0);
            }
        }

        private void UpdateTimers(float deltaTime)
        {
            _statusTimer += deltaTime;
            _radarTimer += deltaTime;

            if (_statusTimer >= STATUS_CHECK_INTERVAL)
            {
                _statusTimer = 0f;
                CheckGameStatus();
                if (ModMenu.InvincibleWalls) RepairWalls();
                if (ModMenu.ClearWeather) ClearWeather();
                ApplyPortalRates();
            }

            if (_radarTimer >= RADAR_CHECK_INTERVAL)
            {
                _radarTimer = 0f;
                CheckForGreedAttack();
            }
        }

        private void RepairWalls()
        {
            var walls = FindObjectsOfType<Wall>();
            foreach (var w in walls) {
                if (w == null || !w.gameObject.activeInHierarchy) continue;
                var d = w.GetComponent<Damageable>();
                if (d != null && d.initialHitPoints > 0) d.hitPoints = d.initialHitPoints;
            }
        }

        private void ClearWeather()
        {
            var pre = FindObjectsOfType<Precipitation>();
            foreach (var p in pre) {
                if (p != null && p.gameObject.activeSelf) p.gameObject.SetActive(false);
            }
        }

        private void ApplyPortalRates()
        {
            if (ModMenu.PortalSpawnRate == 1.0f) return;
            var portals = FindObjectsOfType<Portal>();
            foreach (var p in portals) {
                if (p != null) KingdomEnhanced.Hooks.LabPatches.PortalApplyRate(p);
            }
        }

        private void CheckGameStatus()
        {
            var director = Managers.Inst.director;
            if (director == null) return;
            
            CheckDayNightTransition(director);
        }

        public static void SkipDaytime()
        {
            var director = Managers.Inst?.director;
            if (director == null || !director.IsDaytime) return;

            float currentHour = director.currentTime % 24f;
            float endHour = director.dayEnd;
            float diff = endHour - currentHour;
            if (diff <= 0f) diff += 24f;

            // Add a tiny buffer to ensure the threshold is crossed
            director.AdvanceTime(diff + 0.1f);
            ModMenu.Speak("<color=lightblue> Fast-forwarded to Nightfall!</color>");
        }

        public static void SkipNighttime()
        {
            var director = Managers.Inst?.director;
            if (director == null || director.IsDaytime) return;

            float currentHour = director.currentTime % 24f;
            float startHour = director.dayStart;
            float diff = startHour - currentHour;
            if (diff <= 0f) diff += 24f;

            director.AdvanceTime(diff + 0.1f);
            ModMenu.Speak("<color=orange> Fast-forwarded to Dawn!</color>");
        }

        private void CheckDayNightTransition(Director director)
        {
            try
            {
                if (director.IsDaytime && !_wasDay)
                {
                    ModMenu.Speak("<color=orange> O The sun rises.</color>");
                    _wasDay = true;
                }
                else if (!director.IsDaytime && _wasDay)
                {
                    ModMenu.Speak("<color=lightblue> * Stars appear.</color>");
                    _wasDay = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WorldManager] Error checking day/night: {ex.Message}");
            }
        }

        
        private void CheckForGreedAttack()
        {
            try
            {
                if (Managers.Inst?.director == null || Managers.Inst.director.IsDaytime) return;

                var enemyManager = UnityEngine.Object.FindObjectOfType<EnemyManager>();
                if (enemyManager == null || _enemiesListField == null) return;

                int enemyCount = GetEnemyCount(enemyManager);

                if (ShouldTriggerSiegeAlert(enemyCount))
                {
                    ModMenu.Speak("<color=red><b>⚠️ SIEGE DETECTED!</b></color>");
                    _lastAttackAlert = Time.time;
                }
            }
            catch { }
        }

        private int GetEnemyCount(EnemyManager enemyManager)
        {
            try
            {
                var enemyList = _enemiesListField.GetValue(enemyManager);
                if (enemyList == null) return 0;

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

        private bool IsManagersValid()
        {
            try { return Managers.Inst != null && Managers.Inst.director != null; }
            catch { return false; }
        }
    }
}