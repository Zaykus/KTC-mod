using UnityEngine;
using KingdomEnhanced.UI;
using KingdomEnhanced.Core;

namespace KingdomEnhanced.Features
{
    public class WorldManager : MonoBehaviour
    {
        private float _slowTimer = 0f;
        private GUIStyle _timeStyle;

        void Update()
        {
            // Optimization: Only run expensive world scans once per second
            _slowTimer += Time.deltaTime;
            if (_slowTimer < 1.0f) return;
            _slowTimer = 0f;

            ManageWorldState();
            if (ModMenu.CoinsStayDry) ProtectCoins();
        }

        private void ManageWorldState()
        {
            if (Managers.Inst == null || Managers.Inst.director == null) return;
            var d = Managers.Inst.director;

            if (ModMenu.LockSummer) d.SendMessage("SetSeason", 1, SendMessageOptions.DontRequireReceiver);
            if (ModMenu.ClearWeather) d.SendMessage("SetWeather", 0, SendMessageOptions.DontRequireReceiver);
            if (ModMenu.NoBloodMoons) 
            {
                d.SendMessage("SetIsBloodMoonToday", false, SendMessageOptions.DontRequireReceiver);
                d.SendMessage("StopRedMoon", SendMessageOptions.DontRequireReceiver);
            }
        }

        private void ProtectCoins()
        {
            // Optimized: Find Objects is heavy, but running it once per second is acceptable.
            // Future upgrade: Hook into Coin.Start() to track them in a list instead.
            var allObjs = FindObjectsOfType<GameObject>();
            foreach (var obj in allObjs)
            {
                if (!obj.activeInHierarchy) continue;
                string n = obj.name.ToLower();
                if (n.Contains("coin") || n.Contains("gem"))
                {
                    obj.SendMessage("PreventWaterLoss", SendMessageOptions.DontRequireReceiver);
                    if (obj.transform.position.y < -0.1f)
                    {
                        Vector3 fix = obj.transform.position;
                        fix.y = 0.5f;
                        obj.transform.position = fix;
                    }
                }
            }
        }

        void OnGUI()
        {
            if (!ModMenu.DisplayTimes || Managers.Inst?.director == null) return;

            if (_timeStyle == null)
            {
                _timeStyle = new GUIStyle();
                _timeStyle.normal.textColor = Color.yellow;
                _timeStyle.fontSize = 20;
                _timeStyle.fontStyle = FontStyle.Bold;
            }

            var d = Managers.Inst.director;
            string timeStr = d.currentTime < 0.5f ? "Day" : "Night";
            string text = $"Day: {d.CurrentIslandDays} | {timeStr} ({d.currentTime:F2})";

            // Shadow
            GUI.color = Color.black;
            GUI.Label(new Rect(Screen.width - 248, 22, 250, 30), text, _timeStyle);
            // Text
            GUI.color = Color.yellow;
            GUI.Label(new Rect(Screen.width - 250, 20, 250, 30), text, _timeStyle);
        }
    }
}