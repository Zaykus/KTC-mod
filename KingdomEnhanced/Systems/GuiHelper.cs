using UnityEngine;

namespace KingdomEnhanced.Systems
{
    public static class GuiHelper
    {
        private static Texture2D _whiteTex;
        private static GUIStyle _style;

        private static void Init()
        {
            if (_whiteTex == null)
            {
                _whiteTex = new Texture2D(1, 1);
                _whiteTex.SetPixel(0, 0, Color.white);
                _whiteTex.Apply();
            }
            if (_style == null)
            {
                _style = new GUIStyle();
                _style.normal.background = _whiteTex;
            }
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
        {
            Init();
            float w = end.x - start.x;
            if (w < 1) w = 1;

            // Draw a box to simulate a line, just like the original mod
            Rect rect = new Rect(start.x, start.y - (width / 2), w, width);

            Color old = GUI.color;
            GUI.color = color;
            GUI.Box(rect, "", _style);
            GUI.color = old;
        }
    }
}