using UnityEngine;

namespace KingdomEnhanced.Shared
{
    public static class GuiHelper
    {
        public const int GL_TRIANGLES = 4;

        private static Material _lineMaterial;
        private static GUIStyle _sliderRangeStyle;

        private static Material LineMaterial
        {
            get
            {
                if (_lineMaterial != null) return _lineMaterial;
                var shader = Shader.Find("UI/Default");
                if (shader == null) return null;
                _lineMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
                _lineMaterial.SetInt("_SrcBlend",  (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                _lineMaterial.SetInt("_DstBlend",  (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                _lineMaterial.SetInt("_ZWrite", 0);
                _lineMaterial.SetInt("_Cull",   (int)UnityEngine.Rendering.CullMode.Off);
                _lineMaterial.enableInstancing = false;
                return _lineMaterial;
            }
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color color, float thickness)
        {
            if (Event.current.type != EventType.Repaint) return;
            if (LineMaterial == null) return;

            LineMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            GL.Begin(GL_TRIANGLES);
            GL.Color(color);

            Vector2 dir    = (end - start).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x);
            Vector2 offset = normal * (thickness * 0.5f);

            GL.Vertex3(start.x + offset.x, start.y + offset.y, 0);
            GL.Vertex3(start.x - offset.x, start.y - offset.y, 0);
            GL.Vertex3(end.x   - offset.x, end.y   - offset.y, 0);
            GL.Vertex3(start.x + offset.x, start.y + offset.y, 0);
            GL.Vertex3(end.x   - offset.x, end.y   - offset.y, 0);
            GL.Vertex3(end.x   + offset.x, end.y   + offset.y, 0);

            GL.End();
            GL.PopMatrix();
        }

        public static void DrawSection(string title, GUIStyle style)
        {
            GUILayout.Space(18f);
            GUILayout.Label(title, style);
            GUILayout.Space(6f);
        }

        public static void DrawSlider(string label, ref float value, float min, float max, float defaultValue,
            GUIStyle labelStyle, GUIStyle dimStyle)
        {
            EnsureRangeStyle(dimStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, labelStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("↺ Reset", _sliderRangeStyle, GUILayout.Width(52))) value = defaultValue;
            GUILayout.Label(value.ToString("F1") + "x", dimStyle, GUILayout.Width(42));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(min.ToString("F1"), _sliderRangeStyle, GUILayout.Width(30));
            value = GUILayout.HorizontalSlider(value, min, max);
            GUILayout.Label(max.ToString("F1"), _sliderRangeStyle, GUILayout.Width(30));
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
        }

        public static void DrawIntSlider(string label, ref int value, int min, int max, int defaultValue,
            GUIStyle labelStyle, GUIStyle dimStyle)
        {
            EnsureRangeStyle(dimStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, labelStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("↺ Reset", _sliderRangeStyle, GUILayout.Width(52))) value = defaultValue;
            GUILayout.Label(value <= 0 ? "Default" : value.ToString(), dimStyle, GUILayout.Width(52));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(min.ToString(), _sliderRangeStyle, GUILayout.Width(30));
            float f = GUILayout.HorizontalSlider(value, min, max);
            value = Mathf.RoundToInt(f);
            GUILayout.Label(max.ToString(), _sliderRangeStyle, GUILayout.Width(30));
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
        }

        private static void EnsureRangeStyle(GUIStyle baseStyle)
        {
            if (_sliderRangeStyle != null) return;
            _sliderRangeStyle = new GUIStyle(baseStyle) { fontSize = 10 };
        }
    }
}