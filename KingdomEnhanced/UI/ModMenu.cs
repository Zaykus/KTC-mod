using UnityEngine;
using System.Collections.Generic;
using System.Text;
using KingdomEnhanced.Features;
using KingdomEnhanced.Systems;
using KingdomEnhanced.Core;

namespace KingdomEnhanced.UI
{
    public class ModMenu : MonoBehaviour
    {
        // --- ALL SETTINGS (Saveable & Uncut) ---
        public static bool InvincibleWalls, ShowStaminaBar, EnableAccessibility, LockSummer, ClearWeather, NoBloodMoons, CheatsUnlocked, HyperBuilders, BetterCitizenHouses, DisplayTimes, CoinsStayDry, LargerCamps, BetterKnight, InfiniteStamina, EnableSizeHack;
        public static float TargetSize, SpeedMultiplier;

        // --- NOTIFICATION SYSTEM ---
        public class Notification {
            public string Message; public float ExpiryTime; public float Alpha = 1.0f;
            public Notification(string msg, float duration) { Message = msg; ExpiryTime = Time.time + duration; }
        }
        private static List<Notification> _notifications = new List<Notification>();
        private const float NotificationDuration = 5.0f;

        // --- FEEDBACK SYSTEM ---
        private Dictionary<string, int> _featureStatus = new Dictionary<string, int>() {
            {"Movement Controls", 0}, {"Stamina Bar", 0}, {"Celestial Scroll Map", 0},
            {"Treasury Counter", 0}, {"Infinite Stamina", 0}, {"Economy Actions", 0},
            {"Military Actions", 0}, {"Hyper Builders", 0}, {"Larger Camps", 0}, 
            {"Player Scaling", 0}, {"Greed Radar", 0}
        };

        public static string LastAccessMessage = ""; 
        public static float MessageTimer = 0f;

        private bool _isVisible = false;
        private int _currentTab = 0;
        private Rect _windowRect = new Rect(30, 30, 420, 680); 
        private Vector2 _scrollPosition;
        
        private Texture2D _whiteTex;
        private GUIStyle _headerStyle, _cursorStyle, _creditStyle, _statusBtnStyle, _notifStyle;

        void Start()
        {
            _whiteTex = new Texture2D(1, 1);
            _whiteTex.SetPixel(0, 0, Color.white);
            _whiteTex.Apply();
            LoadFromSettings();
        }

        public static void Speak(string msg) 
        { 
            _notifications.Insert(0, new Notification(msg, NotificationDuration));
            if (_notifications.Count > 8) _notifications.RemoveAt(_notifications.Count - 1);
            LastAccessMessage = msg; MessageTimer = 5.0f; 
            if (Plugin.Instance != null) Plugin.Instance.Log.LogMessage(msg);
        }

        void InitStyles()
        {
            if (_headerStyle == null) {
                _headerStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 15 };
                _headerStyle.normal.textColor = new Color(1f, 0.75f, 0f);
            }
            if (_cursorStyle == null) {
                _cursorStyle = new GUIStyle(GUI.skin.label) { fontSize = 30, fontStyle = FontStyle.Bold };
                _cursorStyle.normal.textColor = Color.white;
            }
            if (_creditStyle == null) {
                _creditStyle = new GUIStyle(GUI.skin.label) { fontSize = 11, alignment = TextAnchor.LowerRight };
                _creditStyle.normal.textColor = Color.gray;
            }
            if (_statusBtnStyle == null) {
                _statusBtnStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold };
            }
            if (_notifStyle == null) {
                _notifStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, richText = true, alignment = TextAnchor.MiddleLeft };
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) { _isVisible = !_isVisible; Speak(_isVisible ? "Menu Visible" : "Menu Hidden"); }
            if (MessageTimer > 0) MessageTimer -= Time.deltaTime;
            _notifications.RemoveAll(n => Time.time > n.ExpiryTime + 1.0f);
        }

        void OnGUI()
        {
            InitStyles();
            DrawNotificationLog();
            if (!_isVisible) return;
            GUI.backgroundColor = new Color(0.08f, 0.08f, 0.08f, 0.96f);
            _windowRect = GUI.Window(0, _windowRect, (GUI.WindowFunction)DrawWindow, "<b>KINGDOM ENHANCED v1.5.0</b>");
            DrawHandCursor();
        }

        void DrawNotificationLog()
        {
            float startY = Screen.height - 100f;
            float startX = 20f;
            float spacing = 25f;
            for (int i = 0; i < _notifications.Count; i++) {
                var n = _notifications[i];
                float timeLeft = n.ExpiryTime - Time.time;
                n.Alpha = (timeLeft < 1.0f) ? Mathf.Max(0, timeLeft) : 1.0f;
                if (n.Alpha <= 0) continue;
                GUI.color = new Color(0, 0, 0, n.Alpha * 0.6f);
                GUI.Box(new Rect(startX - 5, startY - (i * spacing), 320, 22), "");
                GUI.color = new Color(1, 1, 1, n.Alpha);
                GUI.Label(new Rect(startX, startY - (i * spacing), 400, 25), n.Message, _notifStyle);
            }
            GUI.color = Color.white;
        }

        void DrawWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 25));
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(_currentTab == 0, "Main", GUI.skin.button)) _currentTab = 0;
            if (GUILayout.Toggle(_currentTab == 1, "Cheats", GUI.skin.button)) _currentTab = 1;
            if (GUILayout.Toggle(_currentTab == 2, "Lab", GUI.skin.button)) _currentTab = 2;
            if (GUILayout.Toggle(_currentTab == 3, "Feedback", GUI.skin.button)) _currentTab = 3;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            bool changed = false;
            if (_currentTab == 0) changed = DrawGeneral();
            else if (_currentTab == 1) changed = DrawCheats();
            else if (_currentTab == 2) changed = DrawUpcoming();
            else if (_currentTab == 3) DrawFeedback();
            GUILayout.EndScrollView();
            
            GUILayout.Label("Created by Zaykus | Thanks to Abevol", _creditStyle);
            if (changed || GUI.changed) SaveToSettings();
        }

        bool DrawGeneral()
        {
            Title("Movement Controls");
            GUILayout.Label($"Travel Speed: {SpeedMultiplier:F1}x");
            SpeedMultiplier = GUILayout.HorizontalSlider(SpeedMultiplier, 1f, 10f);
            if (GUILayout.Button("Reset Speed to 1.5x")) SpeedMultiplier = 1.5f;

            Title("Stamina Bar Settings");
            ShowStaminaBar = GUILayout.Toggle(ShowStaminaBar, " Enable Stamina Bar");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Cycle Style")) { if (StaminaBarHolder.Instance) StaminaBarHolder.Instance.visualStyle = (StaminaBarHolder.Instance.visualStyle + 1) % 4; }
            if (GUILayout.Button("Cycle Position")) { if (StaminaBarHolder.Instance) StaminaBarHolder.Instance.positionMode = (StaminaBarHolder.Instance.positionMode + 1) % 6; }
            GUILayout.EndHorizontal();

            Title("Interface");
            DisplayTimes = GUILayout.Toggle(DisplayTimes, " Enable Royal Scroll HUD");
            EnableAccessibility = GUILayout.Toggle(EnableAccessibility, " Vocal Feedback (F5-F10)");
            return true;
        }

        bool DrawCheats()
        {
            if (!CheatsUnlocked) { if (GUILayout.Button("UNLOCK MOD MENU")) CheatsUnlocked = true; return true; }
            Title("Invincibility");
            InfiniteStamina = GUILayout.Toggle(InfiniteStamina, " Infinite Mount Stamina");
            Title("Economy Actions");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add 10 Coins")) GiveCurrency(10, false);
            if (GUILayout.Button("Add 5 Gems")) GiveCurrency(5, true);
            GUILayout.EndHorizontal();
            Title("Military Actions");
            if (GUILayout.Button("Recruit All Beggars")) ArmyManager.RecruitBeggars();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Drop Archer Bow")) ArmyManager.DropTools("Archer");
            if (GUILayout.Button("Drop Builder Hammer")) ArmyManager.DropTools("Builder");
            GUILayout.EndHorizontal();
            Title("Passive Buffs");
            HyperBuilders = GUILayout.Toggle(HyperBuilders, " Instant Construction");
            LargerCamps = GUILayout.Toggle(LargerCamps, " Expand Vagrant Camps");
            Title("Player Scaling");
            EnableSizeHack = GUILayout.Toggle(EnableSizeHack, " Enable Scale Hack");
            if (EnableSizeHack) {
                GUILayout.Label($"Scale: {TargetSize:F2}x");
                TargetSize = GUILayout.HorizontalSlider(TargetSize, 0.5f, 5.0f);
            }
            return true;
        }

        bool DrawUpcoming()
        {
            Title("Development Lab");
            GUI.color = Color.gray;
            GUILayout.Label("Testing for next update:");
            GUILayout.Toggle(false, " Self-Repairing Walls");
            GUILayout.Toggle(false, " Elite Knights - Thor/Hel");
            GUILayout.Toggle(false, " Lock Summer Season");
            GUILayout.Toggle(false, " Clear Weather - No Fog/Rain");
            GUILayout.Toggle(false, " Disable Blood Moons");
            GUILayout.Toggle(false, " Buoyant Currency - Water Fix");
            GUILayout.Toggle(false, " Rapid Citizen Housing");
            GUI.color = Color.white;
            return false;
        }

        void DrawFeedback()
        {
            Title("Feature Feedback");
            GUILayout.Label("<b>Legend:</b>\n[ ? ] Untested   [ OK ] Working   [ X ] Broken", new GUIStyle(GUI.skin.label){richText=true});
            GUILayout.Space(10);
            List<string> keys = new List<string>(_featureStatus.Keys);
            foreach (string key in keys) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(key, GUILayout.Width(220));
                int status = _featureStatus[key];
                string btnText = status == 0 ? "[ ? ]" : status == 1 ? "<color=lime>[ OK ]</color>" : "<color=red>[ X ]</color>";
                if (GUILayout.Button(btnText, _statusBtnStyle, GUILayout.Width(80))) { _featureStatus[key] = (status + 1) % 3; }
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(20);
            if (GUILayout.Button("COPY REPORT TO CLIPBOARD", GUILayout.Height(40))) {
                GenerateAndCopyReport();
            }
        }

        void GenerateAndCopyReport() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"--- Kingdom Enhanced Feedback Report ---");
            sb.AppendLine($"Game Version: {Application.version} | Mod Version: 1.5.0");
            foreach(var kvp in _featureStatus) { if(kvp.Value > 0) sb.AppendLine($"- {kvp.Key}: {(kvp.Value==1?"WORKING":"BROKEN")}"); }
            GUIUtility.systemCopyBuffer = sb.ToString();
            Speak("Report copied!");
        }

        void Title(string t) { GUILayout.Space(12); GUILayout.Label(t.ToUpper(), _headerStyle); GUILayout.Space(2); }

        void DrawHandCursor()
        {
            GUI.depth = -10000;
            float mx = Input.mousePosition.x; float my = Screen.height - Input.mousePosition.y;
            GUI.color = Color.black; GUI.Label(new Rect(mx + 1, my + 1, 40, 40), "☝", _cursorStyle);
            GUI.color = new Color(1f, 0.8f, 0.4f); GUI.Label(new Rect(mx, my, 40, 40), "☝", _cursorStyle);
            GUI.color = Color.white;
        }

        void LoadFromSettings() {
            ShowStaminaBar = Settings.ShowStaminaBar.Value; EnableAccessibility = Settings.EnableAccessibility.Value;
            CheatsUnlocked = Settings.CheatsUnlocked.Value; SpeedMultiplier = Settings.SpeedMultiplier.Value;
            InfiniteStamina = Settings.InfiniteStamina.Value; InvincibleWalls = Settings.InvincibleWalls.Value;
            HyperBuilders = Settings.HyperBuilders.Value; BetterKnight = Settings.BetterKnight.Value;
            LargerCamps = Settings.LargerCamps.Value; BetterCitizenHouses = Settings.BetterCitizenHouses.Value;
            EnableSizeHack = Settings.EnableSizeHack.Value; TargetSize = Settings.TargetSize.Value;
            DisplayTimes = Settings.DisplayTimes.Value;
        }

        public void SaveToSettings() {
            Settings.ShowStaminaBar.Value = ShowStaminaBar; Settings.EnableAccessibility.Value = EnableAccessibility;
            Settings.CheatsUnlocked.Value = CheatsUnlocked; Settings.SpeedMultiplier.Value = SpeedMultiplier;
            Settings.InfiniteStamina.Value = InfiniteStamina; Settings.InvincibleWalls.Value = InvincibleWalls;
            Settings.HyperBuilders.Value = HyperBuilders; Settings.BetterKnight.Value = BetterKnight;
            Settings.LargerCamps.Value = LargerCamps; Settings.BetterCitizenHouses.Value = BetterCitizenHouses;
            Settings.EnableSizeHack.Value = EnableSizeHack; Settings.TargetSize.Value = TargetSize;
            Settings.DisplayTimes.Value = DisplayTimes;
            Settings.Config.Save();
        }

        void GiveCurrency(int a, bool g) { 
            var p = Managers.Inst?.kingdom?.GetPlayer(0); 
            if (p?.wallet != null) {
                p.wallet.AddCurrency(g ? CurrencyType.Gems : CurrencyType.Coins, a);
                Speak($"<color={(g?"cyan":"yellow")}>+{(g?5:10)} {(g?"Gems":"Coins")}</color>");
            }
        }
    }
}