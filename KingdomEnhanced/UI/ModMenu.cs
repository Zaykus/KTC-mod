using UnityEngine;
using KingdomEnhanced.Features;
using KingdomEnhanced.Systems;
using KingdomEnhanced.Core;

namespace KingdomEnhanced.UI
{
    public class ModMenu : MonoBehaviour
    {
        // --- FEATURE TOGGLES (Read by Managers) ---
        public static bool InvincibleWalls = false;
        public static bool ShowStaminaBar = true;
        public static bool EnableAccessibility = false;
        public static bool LockSummer = false;
        public static bool ClearWeather = false;
        public static bool NoBloodMoons = false;
        public static bool CheatsUnlocked = false;
        
        // Manager Toggles
        public static bool HyperBuilders = false;
        public static bool BetterCitizenHouses = false;
        public static bool DisplayTimes = false;
        public static bool CoinsStayDry = false;
        public static bool LargerCamps = false;
        public static bool BetterKnight = false;
        
        // Player Toggles
        public static bool InfiniteStamina = true; // Moved from StaminaFeature
        public static bool EnableSizeHack = false; // Moved from PlayerSizeFeature
        public static float TargetSize = 1.0f;
        public static float SpeedMultiplier = 1.5f;

        // --- UI VARIABLES ---
        public static string LastAccessMessage = "";
        public static float MessageTimer = 0f;

        private bool _isVisible = false;
        private int _currentTab = 0;
        private Rect _windowRect = new Rect(50, 50, 340, 550); // Slightly wider
        private Vector2 _scrollPosition; // For scrolling

        private Texture2D _whiteTex;
        private GUIStyle _solidStyle;

        void Start()
        {
            _whiteTex = new Texture2D(1, 1); _whiteTex.SetPixel(0, 0, Color.white); _whiteTex.Apply();
            _solidStyle = new GUIStyle() { normal = { background = _whiteTex } };

            // Load saved settings
            ShowStaminaBar = Settings.ShowStaminaBar.Value;
            EnableAccessibility = Settings.EnableAccessibility.Value;
            CheatsUnlocked = Settings.CheatsUnlocked.Value;
            SpeedMultiplier = Settings.SpeedMultiplier.Value;

            if (StaminaBarHolder.Instance != null)
            {
                StaminaBarHolder.Instance.enableStaminaBar = ShowStaminaBar;
                StaminaBarHolder.Instance.visualStyle = Settings.BarStyle.Value;
                StaminaBarHolder.Instance.positionMode = Settings.BarPosition.Value;
                StaminaBarHolder.Instance.manualX = Settings.ManualX.Value;
                StaminaBarHolder.Instance.manualY = Settings.ManualY.Value;
            }
        }

        void AutoSave()
        {
            Settings.ShowStaminaBar.Value = ShowStaminaBar;
            Settings.EnableAccessibility.Value = EnableAccessibility;
            Settings.CheatsUnlocked.Value = CheatsUnlocked;
            Settings.SpeedMultiplier.Value = SpeedMultiplier;
            
            if (StaminaBarHolder.Instance != null)
            {
                Settings.BarStyle.Value = StaminaBarHolder.Instance.visualStyle;
                Settings.BarPosition.Value = StaminaBarHolder.Instance.positionMode;
                Settings.ManualX.Value = StaminaBarHolder.Instance.manualX;
                Settings.ManualY.Value = StaminaBarHolder.Instance.manualY;
            }
            Settings.Config.Save();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) { _isVisible = !_isVisible; Speak(_isVisible ? "Menu Open" : "Menu Closed"); }
            if (MessageTimer > 0) { MessageTimer -= Time.deltaTime; if (MessageTimer <= 0) LastAccessMessage = ""; }
        }

        public static void Speak(string msg) { LastAccessMessage = msg; MessageTimer = 8.0f; Plugin.Instance.Log.LogMessage(msg); }
        void LateUpdate() { if (_isVisible) { Cursor.visible = true; Cursor.lockState = CursorLockMode.None; } }

        void OnGUI()
        {
            if (EnableAccessibility && !string.IsNullOrEmpty(LastAccessMessage))
            {
                var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 14, wordWrap = true };
                float w = Screen.width * 0.6f; float h = style.CalcHeight(new GUIContent(LastAccessMessage), w) + 15f;
                GUI.color = new Color(0, 0, 0, 0.85f); GUI.Box(new Rect((Screen.width / 2) - (w / 2), Screen.height - h - 20, w, h), "", _solidStyle);
                GUI.color = Color.white; GUI.Label(new Rect((Screen.width / 2) - (w / 2), Screen.height - h - 20, w, h), LastAccessMessage, style);
            }

            if (!_isVisible) return;

            // Draw Window Background
            GUI.color = new Color(1f, 0.8f, 0.4f, 1f); GUI.Box(new Rect(_windowRect.x - 2, _windowRect.y - 2, _windowRect.width + 4, _windowRect.height + 4), "", _solidStyle);
            GUI.color = new Color(0.08f, 0.08f, 0.08f, 0.96f); GUI.Box(_windowRect, "", _solidStyle);
            GUI.color = Color.white; GUI.contentColor = new Color(1f, 0.85f, 0.45f);

            GUILayout.BeginArea(new Rect(_windowRect.x + 10, _windowRect.y + 10, _windowRect.width - 20, _windowRect.height - 20));
            
            // Tabs
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(_currentTab == 0, "General", GUI.skin.button)) _currentTab = 0;
            if (GUILayout.Toggle(_currentTab == 1, "Cheats", GUI.skin.button)) _currentTab = 1;
            if (GUILayout.Toggle(_currentTab == 2, "World", GUI.skin.button)) _currentTab = 2;
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Scroll View Start
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            if (_currentTab == 0) DrawGeneralTab();
            else if (_currentTab == 1) DrawCheatsTab();
            else if (_currentTab == 2) DrawWorldTab();

            GUILayout.EndScrollView();
            // Scroll View End

            GUILayout.FlexibleSpace();
            GUILayout.Label("Press F1 to Close", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 10 });
            GUILayout.EndArea();

            DrawSoftwareCursor();
        }

         void DrawGeneralTab()
        {
            GUILayout.Label(":: PLAYER STATS ::");
            
            // INCREASED MAX SPEED TO 10.0
            GUILayout.Label($"Travel Speed: <color=cyan>{SpeedMultiplier:F1}x</color>");
            if (SpeedMultiplier > 3.0f) 
            {
                GUI.color = Color.red;
                GUILayout.Label("WARNING: High speed may glitch physics!", new GUIStyle(GUI.skin.label){ fontSize = 10, fontStyle = FontStyle.Bold });
                GUI.color = Color.white;
            }

            float oldS = SpeedMultiplier;
            // Changed 2.2f to 10.0f
            SpeedMultiplier = GUILayout.HorizontalSlider(SpeedMultiplier, 1f, 10.0f);
            
            if (GUILayout.Button("Reset to Default (1.5x)")) SpeedMultiplier = 1.5f;
            if (oldS != SpeedMultiplier) AutoSave();

            GUILayout.Space(10);
            GUILayout.Label(":: STAMINA VISUALS ::");
            if (GUILayout.Button($"Stamina Bar: {(ShowStaminaBar ? "ON" : "OFF")}")) { ShowStaminaBar = !ShowStaminaBar; if (StaminaBarHolder.Instance != null) StaminaBarHolder.Instance.enableStaminaBar = ShowStaminaBar; AutoSave(); }
            if (GUILayout.Button("Cycle Bar Style")) { if (StaminaBarHolder.Instance != null) { StaminaBarHolder.Instance.visualStyle = (StaminaBarHolder.Instance.visualStyle + 1) % 4; AutoSave(); } }
            
            GUILayout.Space(10);
            GUILayout.Label(":: ACCESSIBILITY ::");
            if (GUILayout.Button($"Screen Reader Info: {(EnableAccessibility ? "ON" : "OFF")}")) { EnableAccessibility = !EnableAccessibility; Speak("Accessibility " + (EnableAccessibility ? "On" : "Off")); AutoSave(); }
        }

        void DrawCheatsTab()
        {
            if (!CheatsUnlocked)
            {
                GUILayout.Space(20);
                GUI.color = Color.red; 
                GUILayout.Label("WARNING:\nCheats break game balance.\nUse for fun only!", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold }); 
                GUI.color = Color.white; 
                GUILayout.Space(10); 
                if (GUILayout.Button("I Understand, Unlock Cheats")) { CheatsUnlocked = true; AutoSave(); }
                return;
            }

            GUILayout.Label(":: GOD MODES ::");
            if (GUILayout.Button($"Infinite Stamina: {(InfiniteStamina ? "ON" : "OFF")}")) InfiniteStamina = !InfiniteStamina;
            if (GUILayout.Button($"Invincible Walls: {(InvincibleWalls ? "ON" : "OFF")}")) InvincibleWalls = !InvincibleWalls;
            
            GUILayout.Space(10);
            GUILayout.Label(":: WEALTH ::");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Coins +40")) GiveCurrency(40, false);
            if (GUILayout.Button("Gems +10")) GiveCurrency(10, true);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Force Banker Payout")) PlayerManager.ForceBankerPayout();

            GUILayout.Space(10);
            GUILayout.Label(":: ARMY & WORKERS ::");
            if (GUILayout.Button("Recruit All Beggars")) ArmyManager.RecruitBeggars();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Drop Bows")) ArmyManager.DropTools("Archer");
            if (GUILayout.Button("Drop Hammers")) ArmyManager.DropTools("Builder");
            GUILayout.EndHorizontal();

            // Toggles
            GUILayout.Space(5);
            HyperBuilders = GUILayout.Toggle(HyperBuilders, "Hyper Builders (Fast Work)");
            BetterKnight = GUILayout.Toggle(BetterKnight, "Super Knights");
            LargerCamps = GUILayout.Toggle(LargerCamps, "Larger Beggar Camps");
            BetterCitizenHouses = GUILayout.Toggle(BetterCitizenHouses, "Better Houses (Auto Spawn)");

            GUILayout.Space(10);
            GUILayout.Label(":: FUN ::");
            EnableSizeHack = GUILayout.Toggle(EnableSizeHack, "Enable Size Hack");
            if (EnableSizeHack)
            {
                GUILayout.Label($"Size: {TargetSize:F2}");
                TargetSize = GUILayout.HorizontalSlider(TargetSize, 0.5f, 3.0f);
            }
        }

        void DrawWorldTab()
        {
            if (!CheatsUnlocked) { GUILayout.Label("Unlock Cheats first."); return; }

            GUILayout.Label(":: WORLD CONTROL ::");
            LockSummer = GUILayout.Toggle(LockSummer, "Lock Summer Season");
            ClearWeather = GUILayout.Toggle(ClearWeather, "Force Clear Weather");
            NoBloodMoons = GUILayout.Toggle(NoBloodMoons, "Prevent Blood Moons");
            
            GUILayout.Space(10);
            GUILayout.Label(":: UTILITY ::");
            DisplayTimes = GUILayout.Toggle(DisplayTimes, "Display Game Time");
            CoinsStayDry = GUILayout.Toggle(CoinsStayDry, "Prevent Coins Drowning");
        }

        // Helpers
        void DrawSoftwareCursor() { float mx = Input.mousePosition.x; float my = Screen.height - Input.mousePosition.y; GUI.depth = -9999; GUI.color = Color.yellow; GUI.Box(new Rect(mx, my, 12, 12), "", _solidStyle); GUI.color = Color.red; GUI.Box(new Rect(mx + 4, my + 4, 4, 4), "", _solidStyle); GUI.color = Color.white; }
        void GiveCurrency(int amount, bool isGem) { var p = Managers.Inst?.kingdom?.GetPlayer(0); if (p?.wallet != null) p.wallet.SetCurrency(isGem ? CurrencyType.Gems : CurrencyType.Coins, amount); }
    }
}