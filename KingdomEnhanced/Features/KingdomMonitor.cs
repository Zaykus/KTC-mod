using System;
using System.Linq;
using UnityEngine;
using KingdomEnhanced.Core;
using KingdomEnhanced.UI;
#if IL2CPP
using KingdomEnhanced.Shared.Attributes;
#endif

namespace KingdomEnhanced.Features
{
#if IL2CPP
    [RegisterTypeInIl2Cpp]
#endif
    public class KingdomMonitor : MonoBehaviour
    {
#if IL2CPP
        public KingdomMonitor(IntPtr ptr) : base(ptr) { }
#endif
        private Kingdom _kingdom;
        private EnemyManager _enemyManager;
        
        public static KingdomMonitor Instance { get; private set; }
        
        private bool _isVisible = true;
        public bool IsVisible => _isVisible;
        private Rect _windowRect = new Rect(10, 10, 250, 350);
        private bool _isResizing = false;

        
        public enum MonitorStyle { Classic, Neon, Light, Ghost }
        private MonitorStyle _currentStyle = MonitorStyle.Classic;
        private readonly string[] _styleNames = { "Classic", "Neon", "Light", "Ghost" };
        private GUIStyle _styleBtn;
        private Texture2D _btnTex;
        private bool _stylesBuilt;

        
        private int _archerCount;
        private int _workerCount;
        private int _peasantCount;
        private int _knightCount;
        private int _enemyCount;
        private int _vagrantCount;
        private int _farmerCount;
        private int _pikemanCount;
        private int _coinsCount;
        private int _gemsCount;
        private float _nextCensusTime;
        private int _censusStep = 0;

        
        private StyleColors[] _stylePalette;

        private struct StyleColors
        {
            public Color bgBottom;
            public Color bgTop;
            public Color header;
            public Color body;
            public Color footer;
            public Color btnBg;
            public Color btnText;
            public Color frameColor;
            public string safeHex;
            public string dangerHex;
            public float baseAlpha;
            public int frameThickness;

            public StyleColors(Color bb, Color bt, Color h, Color b, Color f, Color bbgn, Color btnT, Color fc, string s, string d, float a, int ft)
            {
                bgBottom = bb; bgTop = bt; header = h; body = b; footer = f;
                btnBg = bbgn; btnText = btnT; frameColor = fc;
                safeHex = s; dangerHex = d; baseAlpha = a; frameThickness = ft;
            }
        }

        private void Start()
        {
            Instance = this;
            _kingdom = FindFirstObjectByType<Kingdom>();
            _enemyManager = FindFirstObjectByType<EnemyManager>();
            
            if (_kingdom == null) Plugin.Instance.LogSource.LogWarning("KingdomMonitor: Kingdom not found.");
            if (_enemyManager == null) Plugin.Instance.LogSource.LogWarning("KingdomMonitor: EnemyManager not found.");
            
            if (_kingdom != null && _kingdom.playerOne != null)
                _isVisible = false;

            
            _stylePalette = new StyleColors[]
            {
                
                new StyleColors(
                    new Color(0.12f, 0.12f, 0.12f, 1.0f),
                    new Color(0.18f, 0.18f, 0.18f, 1.0f),
                    new Color(0.95f, 0.90f, 0.70f),
                    new Color(0.85f, 0.85f, 0.85f),
                    new Color(0.60f, 0.60f, 0.60f),
                    new Color(0.25f, 0.20f, 0.15f, 0.9f),
                    new Color(0.95f, 0.90f, 0.70f),
                    new Color(0.75f, 0.65f, 0.45f, 1.0f),
                    "#44ff44", "#ff4444", 0.95f, 3
                ),
                
                new StyleColors(
                    new Color(0.05f, 0.0f, 0.15f, 1.0f),
                    new Color(0.10f, 0.0f, 0.25f, 1.0f),
                    new Color(0.0f, 1.0f, 1.0f),
                    new Color(0.0f, 1.0f, 0.5f),
                    new Color(1.0f, 0.0f, 1.0f),
                    new Color(0.0f, 0.3f, 0.4f, 0.9f),
                    new Color(0.0f, 1.0f, 1.0f),
                    new Color(0.0f, 1.0f, 1.0f, 1.0f),
                    "#00ff88", "#ff0088", 0.90f, 2
                ),
                
                new StyleColors(
                    new Color(0.92f, 0.92f, 0.88f, 1.0f),
                    new Color(0.98f, 0.98f, 0.95f, 1.0f),
                    new Color(0.15f, 0.10f, 0.05f),
                    new Color(0.25f, 0.20f, 0.15f),
                    new Color(0.45f, 0.40f, 0.35f),
                    new Color(0.75f, 0.70f, 0.65f, 0.9f),
                    new Color(0.15f, 0.10f, 0.05f),
                    new Color(0.35f, 0.30f, 0.25f, 1.0f),
                    "#006600", "#990000", 0.95f, 3
                ),
                
                new StyleColors(
                    new Color(0.00f, 0.00f, 0.00f, 0.00f),
                    new Color(0.00f, 0.00f, 0.00f, 0.00f),
                    new Color(1.00f, 1.00f, 0.80f),
                    new Color(0.95f, 0.95f, 0.95f),
                    new Color(0.85f, 0.85f, 0.85f),
                    new Color(0.00f, 0.00f, 0.00f, 0.7f),
                    new Color(1.00f, 1.00f, 0.80f),
                    new Color(1.0f, 1.0f, 1.0f, 0.5f),
                    "#7CFC00", "#FF4500", 0.00f, 2
                )
            };
        }

        private void OnGUI()
        {
            if (!_isVisible) return;
            BuildStyles();

            StyleColors colors = _stylePalette[(int)_currentStyle];
            
            
            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1f, 1f, 1f, colors.baseAlpha);
            
            
            GUIStyle windowStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10 + colors.frameThickness, 10 + colors.frameThickness, 10 + colors.frameThickness, 10 + colors.frameThickness),
                border = new RectOffset(colors.frameThickness, colors.frameThickness, colors.frameThickness, colors.frameThickness)
            };
            
            _windowRect = GUI.Window(999, _windowRect, (GUI.WindowFunction)DrawWindow, "Kingdom Monitor", windowStyle);
            
            GUI.backgroundColor = prevBg;
        }

        private void BuildStyles()
        {
            if (_stylesBuilt) return;
            _stylesBuilt = true;

            StyleColors colors = _stylePalette[(int)_currentStyle];

            
            _btnTex = new Texture2D(1, 1);
            _btnTex.SetPixel(0, 0, colors.btnBg);
            _btnTex.Apply();

            _styleBtn = new GUIStyle(GUI.skin.button)
            {
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { background = _btnTex, textColor = colors.btnText },
                padding = new RectOffset(4, 4, 2, 2),
                fixedWidth = 60,
                fixedHeight = 18
            };
        }

        private void DrawWindow(int windowID)
        {
            StyleColors colors = _stylePalette[(int)_currentStyle];

            GUILayout.BeginVertical();
            GUILayout.Space(15); 

            
            string cycle = "Unknown";
            int day = 0;
            if (Managers.Inst != null && Managers.Inst.director != null)
            {
                day = Managers.Inst.director.CurrentIslandDays;
            }
            if (_kingdom != null)
            {
                cycle = _kingdom.isDaytime ? "Day" : "Night";
            }
            
            
            GUI.contentColor = colors.header;
            GUILayout.Label($"<b>Day {day} ({cycle})</b>");
            GUI.contentColor = Color.white;

            
            if (_enemyManager != null)
            {
                bool danger = _enemyManager.IsDangerous;
                string status = danger 
                    ? $"<color={colors.dangerHex}>DANGER</color>" 
                    : $"<color={colors.safeHex}>SAFE</color>";
                GUI.contentColor = colors.body;
                GUILayout.Label($"Threat: {status}");
                GUILayout.Label($"Greed: {_enemyCount}");
                GUI.contentColor = Color.white;
            }
            
            
            if (_kingdom != null && _kingdom.playerOne != null && _kingdom.playerOne.wallet != null)
            {
                int coins = _kingdom.playerOne.wallet.GetCurrency(CurrencyType.Coins);
                int gems = _kingdom.playerOne.wallet.GetCurrency(CurrencyType.Gems);
                GUI.contentColor = colors.body;
                GUILayout.Label($"Wallet: {coins} Coins, {gems} Gems");
                GUI.contentColor = Color.white;
            }

            GUILayout.Space(5);

            
            GUI.contentColor = colors.header;
            GUILayout.Label($"<b>Population</b>");
            GUI.contentColor = colors.body;
            
            if (_archerCount > 0) GUILayout.Label($"Archers: {_archerCount}");
            if (_workerCount > 0) GUILayout.Label($"Workers: {_workerCount}");
            if (_peasantCount > 0) GUILayout.Label($"Peasants: {_peasantCount}");
            if (_farmerCount > 0) GUILayout.Label($"Farmers: {_farmerCount}");
            if (_pikemanCount > 0) GUILayout.Label($"Pikemen: {_pikemanCount}");
            if (_knightCount > 0) GUILayout.Label($"Knights: {_knightCount}");
            if (_vagrantCount > 0) GUILayout.Label($"Vagrants: {_vagrantCount}");
            
            GUI.contentColor = Color.white;

            GUILayout.FlexibleSpace();

            
            GUILayout.Space(18); 

            
            var handleSize = 20f;
            var resizeRect = new Rect(_windowRect.width - handleSize, _windowRect.height - handleSize, handleSize, handleSize);
            GUI.color = colors.footer;
            GUI.Box(resizeRect, "[➘]");
            GUI.color = Color.white;

            Event e = Event.current;
            if (e.type == EventType.MouseDown && resizeRect.Contains(e.mousePosition))
            {
                _isResizing = true;
                e.Use();
            }
            else if (e.type == EventType.MouseUp)
            {
                _isResizing = false;
            }
            
            if (_isResizing && e.type == EventType.MouseDrag)
            {
                _windowRect.width = Mathf.Max(200, _windowRect.width + e.delta.x);
                _windowRect.height = Mathf.Max(200, _windowRect.height + e.delta.y);
            }

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            GUILayout.EndVertical();
        }

        
        public void Show() => _isVisible = true;
        public void Hide() => _isVisible = false;
        public void Toggle() => _isVisible = !_isVisible;
        public void NextStyle()
        {
            _currentStyle = (MonitorStyle)(((int)_currentStyle + 1) % _stylePalette.Length);
            _stylesBuilt = false;
        }

        private void Update()
        {
            if (!_isVisible) return;

            
            if (Time.time > _nextCensusTime)
            {
                _nextCensusTime = Time.time + 0.1f;
                
                try 
                {
                    switch (_censusStep)
                    {
                        case 0: _archerCount = FindObjectsByType<Archer>(FindObjectsSortMode.None).Length; break;
                        case 1: _workerCount = FindObjectsByType<Worker>(FindObjectsSortMode.None).Length; break;
                        case 2: _peasantCount = FindObjectsByType<Peasant>(FindObjectsSortMode.None).Length; break;
                        case 3: _knightCount = FindObjectsByType<Knight>(FindObjectsSortMode.None).Length; break;
                        case 4: _vagrantCount = FindObjectsByType<Beggar>(FindObjectsSortMode.None).Length; break;
                        case 5: _farmerCount = FindObjectsByType<Farmer>(FindObjectsSortMode.None).Length; break;
                        case 6: _pikemanCount = FindObjectsByType<Pikeman>(FindObjectsSortMode.None).Length; break;
                        case 7:
                            var em = _enemyManager != null ? _enemyManager : FindFirstObjectByType<EnemyManager>();
                            _enemyCount = (em != null && em.AllEnemies != null) ? em.AllEnemies.Count : 0;
                            break;
                        case 8:
                            var player = Managers.Inst?.kingdom?.GetPlayer(0);
                            if (player != null && player.wallet != null)
                            {
                                _coinsCount = player.wallet.Coins;
                                _gemsCount = player.wallet.Gems;
                            }
                            break;
                    }
                } 
                catch { }

                _censusStep++;
                if (_censusStep > 8) _censusStep = 0;
            }
        }
    }
}