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
    public class WorldManager : MonoBehaviour
    {
        private GUIStyle _timeStyle;
        private GUIStyle _bankStyle;
        
        private float _statusTimer = 0f;
        private float _radarTimer = 0f;
        private bool _wasDay = true;
        private bool _wasBloodMoon = false;
        private float _lastAttackAlert = 0f;

        // Reflection caches
        private FieldInfo _bloodMoonField;
        private FieldInfo _stashedCoinsField;
        private FieldInfo _enemiesListField;

        void Start()
        {
            DiscoverFields();
        }

        private void DiscoverFields()
        {
            try {
                // Find Blood Moon Field
                _bloodMoonField = AccessTools.Field(typeof(Director), "_isBloodMoonToday") ?? 
                                 AccessTools.Field(typeof(Director), "isBloodMoonToday");

                // Find Treasury Field
                var bFields = typeof(Banker).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                foreach (var f in bFields) {
                    if (f.Name.ToLower().Contains("stash") && !f.Name.ToLower().Contains("height")) {
                        _stashedCoinsField = f;
                        break;
                    }
                }

                // Find Enemy List for Radar
                _enemiesListField = AccessTools.Field(typeof(EnemyManager), "_enemies");
            } catch { }
        }

        void OnGUI()
        {
            if (!ModMenu.DisplayTimes || Managers.Inst == null || Managers.Inst.director == null) return;

            InitStyles();
            DrawSimpleHUD();
        }

        void InitStyles()
        {
            if (_timeStyle == null) {
                _timeStyle = new GUIStyle(GUI.skin.label) { 
                    normal = { textColor = new Color(1f, 0.9f, 0.5f) }, 
                    fontSize = 16, 
                    fontStyle = FontStyle.Bold, 
                    alignment = TextAnchor.MiddleCenter 
                };
                
                _bankStyle = new GUIStyle(GUI.skin.label) { 
                    normal = { textColor = new Color(1f, 0.8f, 0f) }, 
                    fontSize = 20, 
                    fontStyle = FontStyle.Bold, 
                    alignment = TextAnchor.MiddleCenter 
                };
            }
        }

        void DrawSimpleHUD()
        {
            // Position at top center
            float hudWidth = 300f;
            float hudX = (Screen.width / 2) - (hudWidth / 2);
            float hudY = 20f;

            var d = Managers.Inst.director;
            
            // 1. Time Calculation
            float rawTime = d.currentTime; 
            float totalHours = (rawTime % 24f);
            int hours = Mathf.FloorToInt(totalHours);
            int minutes = Mathf.FloorToInt((totalHours % 1f) * 60f);
            string clock = string.Format("{0:00}:{1:00}", hours, minutes);
            string timeStr = d.IsDaytime ? "Day" : "Night";

            // 2. Treasury Count
            int treasury = GetStashedCoins();

            // 3. Draw Text with Shadows
            DrawShadowedLabel(new Rect(hudX, hudY, hudWidth, 25), $"DAY {d.CurrentIslandDays} | {timeStr} ({clock})", _timeStyle);
            DrawShadowedLabel(new Rect(hudX, hudY + 22, hudWidth, 30), $"Treasury: {treasury} Gold", _bankStyle);
        }

        private void DrawShadowedLabel(Rect r, string t, GUIStyle s)
        {
            Color oldColor = GUI.color;
            GUI.color = new Color(0, 0, 0, 0.8f); // Shadow color
            GUI.Label(new Rect(r.x + 2, r.y + 2, r.width, r.height), t, s);
            GUI.color = oldColor; // Restore color
            GUI.Label(r, t, s);
        }

        private int GetStashedCoins()
        {
            try {
                var banker = UnityEngine.Object.FindObjectOfType<Banker>();
                if (banker != null && _stashedCoinsField != null) {
                    return Convert.ToInt32(_stashedCoinsField.GetValue(banker));
                }
            } catch { }
            return 0;
        }

        void Update()
        {
            if (Managers.Inst == null || Managers.Inst.director == null) return;
            
            float dt = Time.deltaTime;
            _statusTimer += dt;
            _radarTimer += dt;

            // Status Check (2s)
            if (_statusTimer >= 2.0f) {
                _statusTimer = 0f;
                CheckGameStatus();
            }

            // Radar Check (4s)
            if (_radarTimer >= 4.0f) {
                _radarTimer = 0f;
                CheckForGreedAttack();
            }
        }

        private void CheckForGreedAttack()
        {
            // Only alert at night
            if (Managers.Inst.director.IsDaytime) return;

            try {
                // Use reflection to check enemy count without scanning GameObject tags
                var em = UnityEngine.Object.FindObjectOfType<EnemyManager>();
                if (em != null && _enemiesListField != null) {
                    var listObj = _enemiesListField.GetValue(em);
                    if (listObj != null) {
                        var countProp = listObj.GetType().GetProperty("Count");
                        int count = (int)countProp.GetValue(listObj);
                        
                        // If lots of enemies and alert hasn't fired recently
                        if (count > 15 && Time.time > _lastAttackAlert + 60f) {
                            ModMenu.Speak("<color=red><b>⚠️ SIEGE DETECTED!</b></color>");
                            _lastAttackAlert = Time.time;
                        }
                    }
                }
            } catch { }
        }

        private void CheckGameStatus()
        {
            var d = Managers.Inst.director;
            
            // Dawn/Dusk
            if (d.IsDaytime && !_wasDay) { 
                ModMenu.Speak("<color=orange>The sun rises.</color>"); 
                _wasDay = true; 
            }
            else if (!d.IsDaytime && _wasDay) { 
                ModMenu.Speak("<color=lightblue>Night approaches.</color>"); 
                _wasDay = false; 
            }

            // Blood Moon
            if (_bloodMoonField != null) {
                try {
                    bool isBlood = (bool)_bloodMoonField.GetValue(d);
                    if (isBlood && !_wasBloodMoon) {
                        ModMenu.Speak("<color=red><b>🌑 BLOOD MOON!</b></color>");
                        _wasBloodMoon = true;
                    } else if (!isBlood) _wasBloodMoon = false;
                } catch { }
            }
        }
    }
}