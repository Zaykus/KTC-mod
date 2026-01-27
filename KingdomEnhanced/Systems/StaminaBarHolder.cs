using UnityEngine;
using KingdomEnhanced.Core;
using KingdomEnhanced.UI;
using Il2CppInterop.Runtime.Injection;
using System;

namespace KingdomEnhanced.Systems
{
    public class StaminaBarHolder : MonoBehaviour
    {
        public static StaminaBarHolder Instance { get; private set; }

        public bool enableStaminaBar = true;
        public int visualStyle = 0;
        public int positionMode = 0; // 0=Head, 1=Feet, 2=Bottom, 3=Left, 4=Right, 5=Manual

        // Manual Position Coordinates
        public float manualX = 500;
        public float manualY = 500;

        private static GUIStyle _boxStyle;
        private static GUIStyle _textStyle;
        private static Texture2D _whiteTex;

        public static void Initialize()
        {
            if (Instance != null) return;
            ClassInjector.RegisterTypeInIl2Cpp<StaminaBarHolder>();
            GameObject obj = new GameObject("KingdomEnhanced_StaminaBar");
            DontDestroyOnLoad(obj);
            obj.hideFlags = HideFlags.HideAndDontSave;
            Instance = obj.AddComponent<StaminaBarHolder>();
        }

        private void OnGUI()
        {
            if (_whiteTex == null)
            {
                _whiteTex = new Texture2D(1, 1);
                _whiteTex.SetPixel(0, 0, Color.white);
                _whiteTex.Apply();
                _boxStyle = new GUIStyle() { normal = { background = _whiteTex } };
                _textStyle = new GUIStyle() { normal = { textColor = Color.white }, fontSize = 11, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, richText = true };
            }

            if (!enableStaminaBar || !ModMenu.ShowStaminaBar || !IsPlaying()) return;

            DrawStaminaBar(0);
        }

        private void DrawStaminaBar(int playerId)
        {
            var player = Managers.Inst.kingdom.GetPlayer(playerId);
            if (player == null || player.steed == null) return;
            var steed = player.steed;

            // --- 1. DATA ---
            float pct = 0f;
            Color barColor = Color.cyan;
            string status = "";

            if (steed.IsTired)
            {
                float rem = steed._tiredTimer - Time.time;
                pct = Mathf.Clamp01(1.0f - (rem / (steed.tiredDuration > 0 ? steed.tiredDuration : 10f)));
                barColor = Color.red; status = "TIRED";
            }
            else
            {
                float cur = steed.Stamina;
                pct = Mathf.Clamp01(cur > 1.0f ? cur / 100f : cur);
                if (steed.WellFedTimer > 0) { barColor = Color.yellow; status = "BUFF"; pct = 1f; }
                else { barColor = new Color(0.2f, 0.9f, 1f); status = $"{Mathf.Round(pct * 100)}%"; }
            }

            // --- 2. POSITION ---
            float x = 0, y = 0;
            if (positionMode <= 1)
            { // WORLD SPACE (Head/Feet)
                Transform t = steed._mover != null ? steed._mover.transform : steed.transform;
                Vector3 worldPos = t.position;
                worldPos.y += (positionMode == 0 ? 2.5f : 0.5f);
                Camera cam = Managers.Inst.game?._mainCameraComponent ?? Camera.main;
                if (cam == null) return;
                Vector3 sPos = cam.WorldToScreenPoint(worldPos);
                if (sPos.z < 0) return;
                x = sPos.x; y = Screen.height - sPos.y;
            }
            else
            { // HUD SPACE
                if (positionMode == 2) { x = Screen.width / 2; y = Screen.height - 80; }
                else if (positionMode == 3) { x = 80; y = 80; }
                else if (positionMode == 4) { x = Screen.width - 80; y = 80; }
                else if (positionMode == 5) { x = manualX; y = manualY; } // MANUAL
            }

            // --- 3. DRAW ---
            GUI.depth = -2000;
            switch (visualStyle)
            {
                case 0: DrawClassic(x, y, pct, barColor); break;
                case 1: DrawRPG(x, y, pct, barColor, status); break;
                case 2: DrawRetro(x, y, pct, barColor); break;
                case 3: DrawDual(x, y, pct, barColor, steed); break;
            }
        }

        void DrawClassic(float x, float y, float pct, Color c)
        {
            DrawColoredBox(new Rect(x - 31, y - 1, 62, 10), Color.black);
            DrawColoredBox(new Rect(x - 30, y, 60 * pct, 8), c);
        }
        void DrawRPG(float x, float y, float pct, Color c, string txt)
        {
            DrawColoredBox(new Rect(x - 26, y - 1, 52, 8), Color.black);
            DrawColoredBox(new Rect(x - 25, y, 50 * pct, 6), c);
            GUI.Label(new Rect(x - 25, y + 8, 50, 20), txt, _textStyle);
        }
        void DrawRetro(float x, float y, float pct, Color c)
        {
            int blocks = Mathf.CeilToInt(pct * 10);
            for (int i = 0; i < 10; i++)
            {
                Rect r = new Rect(x - 30 + (i * 6), y, 5, 6);
                DrawColoredBox(new Rect(r.x - 1, r.y - 1, 7, 8), Color.black);
                DrawColoredBox(r, i < blocks ? c : new Color(0.2f, 0.2f, 0.2f));
            }
        }
        void DrawDual(float x, float y, float pct, Color c, Steed s)
        {
            DrawClassic(x, y, pct, c);
            if (s.IsTired)
            {
                float raw = Mathf.Clamp01(s.Stamina > 1f ? s.Stamina / 100f : s.Stamina);
                DrawColoredBox(new Rect(x - 31, y + 10, 62, 4), Color.black);
                DrawColoredBox(new Rect(x - 30, y + 11, 60 * raw, 2), Color.gray);
            }
        }

        private void DrawColoredBox(Rect r, Color c) { GUI.color = c; GUI.Box(r, "", _boxStyle); GUI.color = Color.white; }
        private static bool IsPlaying() { return Managers.Inst?.game?.state.ToString().Contains("Playing") ?? false; }
    }
}