using UnityEngine;
using KingdomEnhanced.UI;
using System.Reflection;
using HarmonyLib;

namespace KingdomEnhanced.Features
{
    public class WorldManager : MonoBehaviour
    {
        private GUIStyle _timeStyle;
        private float _statusTimer = 0f;
        
        // State tracking to prevent notification spam
        private bool _wasDay = true;
        private bool _wasBloodMoon = false;
        private float _lastAttackAlert = 0f;

        // Reflection caches
        private FieldInfo _bloodMoonField;

        void Start()
        {
            // Cache the private field for performance
            _bloodMoonField = AccessTools.Field(typeof(Director), "_isBloodMoonToday");
        }

        void OnGUI()
        {
            if (!ModMenu.DisplayTimes || Managers.Inst == null || Managers.Inst.director == null) return;

            if (_timeStyle == null) {
                _timeStyle = new GUIStyle();
                _timeStyle.normal.textColor = Color.yellow;
                _timeStyle.fontSize = 20;
                _timeStyle.fontStyle = FontStyle.Bold;
            }

            var d = Managers.Inst.director;
            int day = d.CurrentIslandDays;
            float time = d.currentTime; 

            string timeStr = time < 0.25f ? "Morning" : time < 0.5f ? "Afternoon" : time < 0.75f ? "Evening" : "Night";
            string text = $"Day {day} | {timeStr} ({time:F2})";

            GUI.color = Color.black;
            GUI.Label(new Rect(Screen.width - 248, 22, 250, 30), text, _timeStyle);
            GUI.color = Color.yellow;
            GUI.Label(new Rect(Screen.width - 250, 20, 250, 30), text, _timeStyle);
        }

        void Update()
        {
            if (Managers.Inst == null || Managers.Inst.director == null) return;
            
            _statusTimer += Time.deltaTime;
            if (_statusTimer < 2.0f) return; // Only check every 2 seconds for performance
            _statusTimer = 0f;

            CheckGameStatus();
        }

        private void CheckGameStatus()
        {
            var d = Managers.Inst.director;

            // 1. Dawn/Dusk Detection
            if (d.IsDaytime && !_wasDay)
            {
                ModMenu.Speak("<color=orange>The sun rises. A new day begins.</color>");
                _wasDay = true;
            }
            else if (!d.IsDaytime && _wasDay)
            {
                ModMenu.Speak("<color=lightblue>Shadows lengthen. Retreat to the walls!</color>");
                _wasDay = false;
            }

            // 2. Blood Moon Detection (Using Reflection to avoid compile error)
            bool isBlood = false;
            if (_bloodMoonField != null)
            {
                try { isBlood = (bool)_bloodMoonField.GetValue(d); } catch { }
            }

            if (isBlood && !_wasBloodMoon)
            {
                ModMenu.Speak("<color=red>THE BLOOD MOON RISES! Prepare for the swarm!</color>");
                _wasBloodMoon = true;
            }
            else if (!isBlood) _wasBloodMoon = false;

            // 3. Enemy Radar (Greed Attack Alert)
            // Using generic GameObject search to avoid missing 'Greed' type error
            if (!d.IsDaytime && Time.time > _lastAttackAlert + 45f) 
            {
                int greedCount = 0;
                var allGOS = GameObject.FindObjectsOfType<GameObject>();
                foreach (var go in allGOS)
                {
                    if (go.name.Contains("Greed") || go.name.Contains("Enemy"))
                    {
                        greedCount++;
                        if (greedCount > 5) break;
                    }
                }

                if (greedCount > 5)
                {
                    ModMenu.Speak("<color=red>Urgent: Enemy movement detected outside the walls!</color>");
                    _lastAttackAlert = Time.time;
                }
            }
        }
    }
}