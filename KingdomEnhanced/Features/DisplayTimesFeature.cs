using UnityEngine;
using KingdomEnhanced.UI;
using KingdomEnhanced.Core;

namespace KingdomEnhanced.Features
{
    public class DisplayTimesFeature : MonoBehaviour
    {
        // Simple style for the text
        private GUIStyle _style;

        void OnGUI()
        {
            if (!ModMenu.DisplayTimes || Managers.Inst == null || Managers.Inst.director == null) return;

            if (_style == null)
            {
                _style = new GUIStyle();
                _style.normal.textColor = Color.white;
                _style.fontSize = 20;
                _style.fontStyle = FontStyle.Bold;
                // Add a black outline effect by drawing multiple times slightly offset
            }

            var d = Managers.Inst.director;

            // Calculate Day and Time
            int day = d.CurrentIslandDays;
            float time = d.currentTime; // usually 0.0 to 1.0 (0=dawn, 0.5=dusk)

            // Format time nicely
            string timeStr = "";
            if (time < 0.25f) timeStr = "Morning";
            else if (time < 0.5f) timeStr = "Afternoon";
            else if (time < 0.75f) timeStr = "Evening";
            else timeStr = "Night";

            string text = $"Day: {day} | {timeStr} ({time:F2})";

            // Draw shadow for readability
            GUI.color = Color.black;
            GUI.Label(new Rect(Screen.width - 248, 22, 250, 30), text, _style);

            // Draw text
            GUI.color = Color.yellow;
            GUI.Label(new Rect(Screen.width - 250, 20, 250, 30), text, _style);
        }
    }
}