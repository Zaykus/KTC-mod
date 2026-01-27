using UnityEngine;
using KingdomEnhanced.Features;
using KingdomEnhanced.Systems;
using KingdomEnhanced.Core;

namespace KingdomEnhanced.UI
{
    public class ModMenu : MonoBehaviour
    {
        // --- GLOBAL VARIABLES ---
        public static bool InvincibleWalls = false;
        public static bool ShowStaminaBar = true;
        public static bool EnableAccessibility = false;
        public static bool LockSummer = false;
        public static bool ClearWeather = false;
        public static bool NoBloodMoons = false;
        public static bool CheatsUnlocked = false;
        // ADDED THIS BACK so the button works
        public static bool HyperBuilders;

        // New Features from DLLs
        // Add this with the other booleans
        public static bool BetterCitizenHouses = false;
        public static bool DisplayTimes = false;
        public static bool CoinsStayDry = false;
        public static bool LargerCamps = false;
        public static bool BetterKnight = false;

        public static string LastAccessMessage = "";
        public static float MessageTimer = 0f;

        private bool _isVisible = false;
        private int _currentTab = 0;
        private Rect _windowRect = new Rect(50, 50, 320, 520);

        private Texture2D _whiteTex;
        private GUIStyle _solidStyle;

        void Start()
        {
            _whiteTex = new Texture2D(1, 1); _whiteTex.SetPixel(0, 0, Color.white); _whiteTex.Apply();
            _solidStyle = new GUIStyle() { normal = { background = _whiteTex } };

            ShowStaminaBar = Settings.ShowStaminaBar.Value;
            EnableAccessibility = Settings.EnableAccessibility.Value;
            CheatsUnlocked = Settings.CheatsUnlocked.Value;

            var speed = FindObjectOfType<SpeedFeature>();
            if (speed != null) speed.SpeedMultiplier = Settings.SpeedMultiplier.Value;

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
            var speed = FindObjectOfType<SpeedFeature>();
            if (speed != null) Settings.SpeedMultiplier.Value = speed.SpeedMultiplier;
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

        public static void Speak(string msg) { LastAccessMessage = msg; MessageTimer = 8.0f; KingdomEnhanced.Core.Plugin.Instance.Log.LogMessage(msg); }
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
            GUI.color = new Color(1f, 0.8f, 0.4f, 1f); GUI.Box(new Rect(_windowRect.x - 2, _windowRect.y - 2, _windowRect.width + 4, _windowRect.height + 4), "", _solidStyle);
            GUI.color = new Color(0.08f, 0.08f, 0.08f, 0.96f); GUI.Box(_windowRect, "", _solidStyle);
            GUI.color = Color.white; GUI.contentColor = new Color(1f, 0.85f, 0.45f);
            GUILayout.BeginArea(new Rect(_windowRect.x + 10, _windowRect.y + 10, _windowRect.width - 20, _windowRect.height - 20));
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(_currentTab == 0, "General", GUI.skin.button)) _currentTab = 0;
            if (GUILayout.Toggle(_currentTab == 1, "Cheats", GUI.skin.button)) _currentTab = 1;
            if (GUILayout.Toggle(_currentTab == 2, "World", GUI.skin.button)) _currentTab = 2;
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            if (_currentTab == 0) DrawGeneralTab(); else if (_currentTab == 1) DrawCheatsTab(); else if (_currentTab == 2) DrawWorldTab();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Press F1 to Close", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 10 });
            GUILayout.EndArea();
            DrawSoftwareCursor();
        }

        void DrawGeneralTab()
        {
            GUILayout.Label(":: QUALITY OF LIFE ::");
            var speed = FindObjectOfType<SpeedFeature>();
            if (speed)
            {
                GUILayout.Label($"Travel Speed: <color=cyan>{speed.SpeedMultiplier:F1}x</color>");
                float oldS = speed.SpeedMultiplier;
                speed.SpeedMultiplier = GUILayout.HorizontalSlider(speed.SpeedMultiplier, 1f, 2.2f);
                if (GUILayout.Button("Reset to Default (1.5x)")) speed.SpeedMultiplier = 1.5f;
                if (oldS != speed.SpeedMultiplier) AutoSave();
            }
            GUILayout.Space(10);
            GUILayout.Label(":: VISUALS ::");
            if (GUILayout.Button($"Stamina Bar: {(ShowStaminaBar ? "ON" : "OFF")}")) ToggleStaminaBar();
            if (GUILayout.Button("Cycle Bar Style")) CycleStaminaStyle();
            if (GUILayout.Button("Cycle Position")) CycleStaminaPos();
            if (StaminaBarHolder.Instance != null && StaminaBarHolder.Instance.positionMode == 5)
            {
                if (Input.GetMouseButtonUp(0)) AutoSave();
            }
            GUILayout.Space(10);
            GUILayout.Label(":: ACCESSIBILITY ::");
            if (GUILayout.Button($"Screen Reader Info: {(EnableAccessibility ? "ON" : "OFF")}")) ToggleAccessibility();
        }

        void DrawCheatsTab()
        {
            if (!CheatsUnlocked)
            {
                GUILayout.FlexibleSpace(); GUI.color = Color.red; GUILayout.Label("WARNING:\nBreaking game balance.\nUse for fun only!", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold }); GUI.color = Color.white; GUILayout.Space(10); if (GUILayout.Button("I Understand, Unlock Cheats")) { CheatsUnlocked = true; AutoSave(); }
                GUILayout.FlexibleSpace(); return;
            }
            GUILayout.Label(":: GOD MODES ::");
            if (GUILayout.Button($"Infinite Stamina: {(StaminaFeature.InfiniteStamina ? "ON" : "OFF")}")) ToggleInfStamina();
            if (GUILayout.Button($"Invincible Walls: {(InvincibleWalls ? "ON" : "OFF")}")) ToggleInvincibleWalls();
            GUILayout.Space(10);
            GUILayout.Label(":: UNLIMITED WEALTH ::");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Coins +40")) GiveCurrency(40, false);
            if (GUILayout.Button("Gems +10")) GiveCurrency(10, true);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(":: INSTANT ARMY ::");
            if (GUILayout.Button("Promote All to Archers")) CitizenFeature.PromoteAll("Archer");
            if (GUILayout.Button("Promote All to Builders")) CitizenFeature.PromoteAll("Builder");
            if (GUILayout.Button("Recruit All Beggars")) RecruitFeature.RecruitAllBeggars();
            // HYPER BUILDER BUTTON
            if (GUILayout.Button("Hyper Builders: " + (HyperBuilders ? "ON" : "OFF")))
            {
                HyperBuilders = !HyperBuilders;
                Speak("Hyper Builders " + (HyperBuilders ? "On" : "Off"));
                // Force update all existing workers immediately
                if (HyperBuilders)
                {
                    KingdomEnhanced.Features.BuilderFeature.ForceUpdateAllWorkers();
                }
            }
            GUILayout.Space(10);
            GUILayout.Label(":: QUALITY OF LIFE EXTRAS ::");
            // Add this button logic
            if (GUILayout.Button($"Better Citizen Houses: <color={(BetterCitizenHouses ? "lime" : "red")}>{(BetterCitizenHouses ? "ON" : "OFF")}</color>"))
            {
                BetterCitizenHouses = !BetterCitizenHouses;
                Speak("Better Citizen Houses " + (BetterCitizenHouses ? "Enabled" : "Disabled"));
            }
            if (GUILayout.Button("Display Timers: " + (DisplayTimes ? "ON" : "OFF")))
            {
                DisplayTimes = !DisplayTimes;
                Speak("Display Timers " + (DisplayTimes ? "On" : "Off"));
            }
            if (GUILayout.Button($"Coins Stay Dry: <color={(CoinsStayDry ? "lime" : "red")}>{(CoinsStayDry ? "ON" : "OFF")}</color>"))
            {
                CoinsStayDry = !CoinsStayDry; Speak("Coins Stay Dry " + (CoinsStayDry ? "On" : "Off"));
            }
            if (GUILayout.Button($"Larger Camps: <color={(LargerCamps ? "lime" : "red")}>{(LargerCamps ? "ON" : "OFF")}</color>"))
            {
                LargerCamps = !LargerCamps; Speak("Larger Camps " + (LargerCamps ? "On" : "Off"));
            }
            if (GUILayout.Button($"Better Knight: <color={(BetterKnight ? "lime" : "red")}>{(BetterKnight ? "ON" : "OFF")}</color>"))
            {
                BetterKnight = !BetterKnight; Speak("Better Knight " + (BetterKnight ? "On" : "Off"));
            }
            var speed = FindObjectOfType<SpeedFeature>();
            if (speed)
            {
                GUILayout.Label($"Super Speed: <color=cyan>{speed.SpeedMultiplier:F1}x</color>");
                float oldS = speed.SpeedMultiplier;
                speed.SpeedMultiplier = GUILayout.HorizontalSlider(speed.SpeedMultiplier, 1f, 10f);
                if (GUILayout.Button("Reset to Default (1.5x)")) speed.SpeedMultiplier = 1.5f;
                if (oldS != speed.SpeedMultiplier) AutoSave();
            }
        }

        void DrawWorldTab()
        {
            if (!CheatsUnlocked) { GUILayout.FlexibleSpace(); GUI.color = Color.red; GUILayout.Label("Unlock Cheats in [Cheats] tab first.", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold }); GUI.color = Color.white; GUILayout.FlexibleSpace(); return; }
            GUILayout.Label(":: WORLD CONTROL ::");
            if (GUILayout.Button($"Lock Summer: {(LockSummer ? "ON" : "OFF")}")) ToggleSummer();
            if (GUILayout.Button($"Clear Weather: {(ClearWeather ? "ON" : "OFF")}")) ToggleWeather();
            if (GUILayout.Button($"Prevent Red Moon: {(NoBloodMoons ? "ON" : "OFF")}")) ToggleBloodMoon();
        }
        // Helpers
        void ToggleStaminaBar() { ShowStaminaBar = !ShowStaminaBar; if (StaminaBarHolder.Instance != null) StaminaBarHolder.Instance.enableStaminaBar = ShowStaminaBar; AutoSave(); }
        void CycleStaminaStyle() { if (StaminaBarHolder.Instance != null) { StaminaBarHolder.Instance.visualStyle = (StaminaBarHolder.Instance.visualStyle + 1) % 4; AutoSave(); } }
        void CycleStaminaPos() { if (StaminaBarHolder.Instance != null) { StaminaBarHolder.Instance.positionMode = (StaminaBarHolder.Instance.positionMode + 1) % 6; AutoSave(); } }
        void ToggleAccessibility() { EnableAccessibility = !EnableAccessibility; Speak("Accessibility " + (EnableAccessibility ? "On" : "Off")); AutoSave(); }
        void ToggleInfStamina() { StaminaFeature.InfiniteStamina = !StaminaFeature.InfiniteStamina; Speak("Inf Stamina: " + StaminaFeature.InfiniteStamina); }
        void ToggleInvincibleWalls() { InvincibleWalls = !InvincibleWalls; Speak("Invincible Walls: " + InvincibleWalls); }
        void ToggleSummer() { LockSummer = !LockSummer; Speak("Lock Summer " + LockSummer); }
        void ToggleWeather() { ClearWeather = !ClearWeather; Speak("Weather Clear " + ClearWeather); }
        void ToggleBloodMoon() { NoBloodMoons = !NoBloodMoons; Speak("Red Moon Prevention " + NoBloodMoons); }
        void DrawSoftwareCursor() { float mx = Input.mousePosition.x; float my = Screen.height - Input.mousePosition.y; GUI.depth = -9999; GUI.color = Color.yellow; GUI.Box(new Rect(mx, my, 12, 12), "", _solidStyle); GUI.color = Color.red; GUI.Box(new Rect(mx + 4, my + 4, 4, 4), "", _solidStyle); GUI.color = Color.white; }
        void GiveCurrency(int amount, bool isGem) { var p = Managers.Inst?.kingdom?.GetPlayer(0); if (p?.wallet != null) p.wallet.SetCurrency(isGem ? CurrencyType.Gems : CurrencyType.Coins, amount); }
    }
}