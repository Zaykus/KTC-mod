using UnityEngine;
using System.Collections.Generic;
using System.Text;
using KingdomEnhanced.Features;
using KingdomEnhanced.Systems;
using KingdomEnhanced.Core;

namespace KingdomEnhanced.UI
{
    /// <summary>
    /// Main mod menu interface with settings and features
    /// </summary>
    public class ModMenu : MonoBehaviour
    {
        #region Settings
        // Core Features
        public static bool ShowStaminaBar;
        public static bool EnableAccessibility;
        public static bool DisplayTimes;
        
        // Cheats
        public static bool CheatsUnlocked;
        public static bool InfiniteStamina;
        public static bool InvincibleWalls;
        
        // Building Enhancements
        public static bool HyperBuilders;
        public static bool LargerCamps;
        public static bool BetterCitizenHouses;
        
        // Combat Enhancements
        public static bool BetterKnight;
        
        // Environmental
        public static bool LockSummer;
        public static bool ClearWeather;
        public static bool NoBloodMoons;
        public static bool CoinsStayDry;
        
        // Player Modifications
        public static bool EnableSizeHack;
        public static float TargetSize = 1.0f;
        public static float SpeedMultiplier = 1.0f;
        #endregion

        #region Notification System
        public class Notification
        {
            public string Message { get; set; }
            public float ExpiryTime { get; set; }
            public float Alpha { get; set; } = 1.0f;

            public Notification(string msg, float duration)
            {
                Message = msg;
                ExpiryTime = Time.time + duration;
            }

            public bool IsExpired()
            {
                return Time.time > ExpiryTime + 1.0f;
            }

            public void UpdateAlpha()
            {
                float timeLeft = ExpiryTime - Time.time;
                Alpha = timeLeft < 1.0f ? Mathf.Max(0, timeLeft) : 1.0f;
            }
        }

        private static readonly List<Notification> _notifications = new List<Notification>();
        private const float NotificationDuration = 5.0f;
        private const int MaxNotifications = 8;
        #endregion

        #region Feedback System
        private readonly Dictionary<string, int> _featureStatus = new Dictionary<string, int>
        {
            {"Movement Controls", 0},
            {"Stamina Bar", 0},
            {"HUD Display", 0},
            {"Coin Counter", 0},
            {"Infinite Stamina", 0},
            {"Economy Actions", 0},
            {"Military Actions", 0},
            {"Hyper Builders", 0},
            {"Larger Camps", 0},
            {"Player Scaling", 0},
            {"Greed Radar", 0}
        };

        public static string LastAccessMessage = "";
        public static float MessageTimer = 0f;
        #endregion

        #region UI State
        private bool _isVisible = false;
        private int _currentTab = 0;
        private Rect _windowRect = new Rect(30, 30, 440, 700);
        private Vector2 _scrollPosition;
        #endregion

        #region UI Resources
        private Texture2D _whiteTex;
        private GUIStyle _headerStyle;
        private GUIStyle _subHeaderStyle;
        private GUIStyle _cursorStyle;
        private GUIStyle _creditStyle;
        private GUIStyle _statusBtnStyle;
        private GUIStyle _notifStyle;
        private GUIStyle _tabStyle;
        private GUIStyle _buttonStyle;
        #endregion

        #region Constants
        private const string MOD_VERSION = "v2.0.0";
        private const string CREATOR_CREDIT = "Created by Zaykus | Thanks to Abevol";
        private const KeyCode MENU_TOGGLE_KEY = KeyCode.F1;
        #endregion

        #region Unity Lifecycle
        void Start()
        {
            InitializeResources();
            LoadFromSettings();
            Speak("Kingdom Enhanced initialized");
        }

        void Update()
        {
            HandleInput();
            UpdateNotifications();
        }

        void OnGUI()
        {
            InitializeStyles();
            DrawNotificationLog();

            if (!_isVisible) return;

            GUI.backgroundColor = new Color(0.08f, 0.08f, 0.08f, 0.96f);
            _windowRect = GUI.Window(0, _windowRect, (GUI.WindowFunction)DrawWindow, $"<b>KINGDOM ENHANCED {MOD_VERSION}</b>");
            DrawCustomCursor();
        }
        #endregion

        #region Initialization
        private void InitializeResources()
        {
            _whiteTex = new Texture2D(1, 1);
            _whiteTex.SetPixel(0, 0, Color.white);
            _whiteTex.Apply();
        }

        private void InitializeStyles()
        {
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 15,
                    normal = { textColor = new Color(1f, 0.75f, 0f) }
                };
            }

            if (_subHeaderStyle == null)
            {
                _subHeaderStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 12,
                    normal = { textColor = new Color(0.9f, 0.9f, 0.9f) }
                };
            }

            if (_cursorStyle == null)
            {
                _cursorStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 30,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.white }
                };
            }

            if (_creditStyle == null)
            {
                _creditStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 11,
                    alignment = TextAnchor.LowerRight,
                    normal = { textColor = Color.gray }
                };
            }

            if (_statusBtnStyle == null)
            {
                _statusBtnStyle = new GUIStyle(GUI.skin.button)
                {
                    fontStyle = FontStyle.Bold,
                    richText = true
                };
            }

            if (_notifStyle == null)
            {
                _notifStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    richText = true,
                    alignment = TextAnchor.MiddleLeft
                };
            }

            if (_tabStyle == null)
            {
                _tabStyle = new GUIStyle(GUI.skin.button)
                {
                    fontStyle = FontStyle.Bold
                };
            }

            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontStyle = FontStyle.Normal
                };
            }
        }
        #endregion

        #region Input Handling
        private void HandleInput()
        {
            if (Input.GetKeyDown(MENU_TOGGLE_KEY))
            {
                _isVisible = !_isVisible;
                Speak(_isVisible ? "Menu Active" : "Menu Hidden");
            }

            if (MessageTimer > 0)
            {
                MessageTimer -= Time.deltaTime;
            }
        }
        #endregion

        #region Notification Management
        public static void Speak(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            _notifications.Insert(0, new Notification(message, NotificationDuration));

            if (_notifications.Count > MaxNotifications)
            {
                _notifications.RemoveAt(_notifications.Count - 1);
            }

            LastAccessMessage = message;
            MessageTimer = 5.0f;
            Debug.Log($"[ModMenu] {message}");
        }

        private void UpdateNotifications()
        {
            _notifications.RemoveAll(n => n.IsExpired());
        }

        private void DrawNotificationLog()
        {
            const float startY = 100f;
            const float startX = 20f;
            const float spacing = 25f;
            float yPosition = Screen.height - startY;

            for (int i = 0; i < _notifications.Count; i++)
            {
                var notification = _notifications[i];
                notification.UpdateAlpha();

                if (notification.Alpha <= 0) continue;

                float currentY = yPosition - (i * spacing);

                // Background
                GUI.color = new Color(0, 0, 0, notification.Alpha * 0.6f);
                GUI.Box(new Rect(startX - 5, currentY, 320, 22), "");

                // Text
                GUI.color = new Color(1, 1, 1, notification.Alpha);
                GUI.Label(new Rect(startX, currentY, 400, 25), notification.Message, _notifStyle);
            }

            GUI.color = Color.white;
        }
        #endregion

        #region Window Drawing
        private void DrawWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 25));

            DrawTabBar();
            
            GUILayout.Space(10);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            
            bool settingsChanged = DrawCurrentTab();
            
            GUILayout.EndScrollView();
            
            DrawCredits();

            if (settingsChanged || GUI.changed)
            {
                SaveToSettings();
            }
        }

        private void DrawTabBar()
        {
            GUILayout.BeginHorizontal();
            
            DrawTab(0, "Main");
            DrawTab(1, "Cheats");
            DrawTab(2, "Lab");
            DrawTab(3, "Feedback");
            
            GUILayout.EndHorizontal();
        }

        private void DrawTab(int tabIndex, string tabName)
        {
            if (GUILayout.Toggle(_currentTab == tabIndex, tabName, _tabStyle))
            {
                _currentTab = tabIndex;
            }
        }

        private bool DrawCurrentTab()
        {
            switch (_currentTab)
            {
                case 0: return DrawMainTab();
                case 1: return DrawCheatsTab();
                case 2: return DrawLabTab();
                case 3: DrawFeedbackTab(); return false;
                default: return false;
            }
        }

        private void DrawCredits()
        {
            GUILayout.Label(CREATOR_CREDIT, _creditStyle);
        }
        #endregion

        #region Tab: Main
        private bool DrawMainTab()
        {
            // Movement Section
            DrawSectionHeader("Movement");
            GUILayout.Label($"Travel Speed: {SpeedMultiplier:F1}x");
            SpeedMultiplier = GUILayout.HorizontalSlider(SpeedMultiplier, 1f, 10f);
            GUILayout.Space(5);

            // Stamina Bar Section
            DrawSectionHeader("Stamina Bar");
            ShowStaminaBar = GUILayout.Toggle(ShowStaminaBar, " Enable Energy Bar");
            
            if (ShowStaminaBar)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Cycle Style", _buttonStyle))
                {
                    CycleStaminaBarStyle();
                }
                if (GUILayout.Button("Cycle Position", _buttonStyle))
                {
                    CycleStaminaBarPosition();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(5);

            // Interface Section
            DrawSectionHeader("Interface");
            DisplayTimes = GUILayout.Toggle(DisplayTimes, " Enable HUD Display");
            
            if (DisplayTimes)
            {
                GUILayout.Label("  • Shows: Day/Night, Time, Coin Count", new GUIStyle(GUI.skin.label) 
                { 
                    fontSize = 10, 
                    normal = { textColor = new Color(0.7f, 0.7f, 0.7f) } 
                });
            }
            
            EnableAccessibility = GUILayout.Toggle(EnableAccessibility, " Vocal Feedback (F5-F10)");
            GUILayout.Space(5);

            return true;
        }

        private void CycleStaminaBarStyle()
        {
            if (StaminaBarHolder.Instance != null)
            {
                StaminaBarHolder.Instance.visualStyle = (StaminaBarHolder.Instance.visualStyle + 1) % 4;
                Speak($"Stamina bar style changed");
            }
        }

        private void CycleStaminaBarPosition()
        {
            if (StaminaBarHolder.Instance != null)
            {
                StaminaBarHolder.Instance.positionMode = (StaminaBarHolder.Instance.positionMode + 1) % 6;
                Speak($"Stamina bar position changed");
            }
        }
        #endregion

        #region Tab: Cheats
        private bool DrawCheatsTab()
        {
            if (!CheatsUnlocked)
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("🔓 UNLOCK CHEAT MENU", GUILayout.Height(50)))
                {
                    CheatsUnlocked = true;
                    Speak("Cheat menu unlocked!");
                }
                GUILayout.FlexibleSpace();
                return true;
            }

            // Invincibility
            DrawSectionHeader("Invincibility");
            InfiniteStamina = GUILayout.Toggle(InfiniteStamina, " Infinite Mount Stamina");
            GUILayout.Space(5);

            // Economy
            DrawSectionHeader("Economy");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("💰 Add 10 Coins", _buttonStyle))
            {
                GiveCurrency(10, false);
            }
            if (GUILayout.Button("💎 Add 5 Gems", _buttonStyle))
            {
                GiveCurrency(5, true);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            // Military
            DrawSectionHeader("Military");
            if (GUILayout.Button("Recruit All Beggars", _buttonStyle))
            {
                ArmyManager.RecruitBeggars();
            }
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Drop Archer Bow", _buttonStyle))
            {
                ArmyManager.DropTools("Archer");
            }
            if (GUILayout.Button("Drop Builder Hammer", _buttonStyle))
            {
                ArmyManager.DropTools("Builder");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            // Passive Buffs
            DrawSectionHeader("Passive Buffs");
            HyperBuilders = GUILayout.Toggle(HyperBuilders, " Instant Construction");
            LargerCamps = GUILayout.Toggle(LargerCamps, " Expand Vagrant Camps");
            GUILayout.Space(5);

            // Player Scaling
            DrawSectionHeader("Player Scaling");
            EnableSizeHack = GUILayout.Toggle(EnableSizeHack, " Enable Scale Hack");
            
            if (EnableSizeHack)
            {
                GUILayout.Label($"Scale: {TargetSize:F2}x");
                TargetSize = GUILayout.HorizontalSlider(TargetSize, 0.5f, 5.0f);
            }

            return true;
        }
        #endregion

        #region Tab: Lab
        private bool DrawLabTab()
        {
            DrawSectionHeader("Development Lab");
            
            GUI.color = new Color(0.7f, 0.7f, 0.7f);
            GUILayout.Label("Testing features for future updates:", new GUIStyle(GUI.skin.label) 
            { 
                fontStyle = FontStyle.Italic,
                fontSize = 11
            });
            GUILayout.Space(8);
            
            GUILayout.Toggle(false, " Self-Repairing Walls");
            GUILayout.Toggle(false, " Elite Knights - Thor/Hel");
            GUILayout.Toggle(false, " Lock Summer Season");
            GUILayout.Toggle(false, " Clear Weather - No Fog/Rain");
            GUILayout.Toggle(false, " Disable Blood Moons");
            GUILayout.Toggle(false, " Buoyant Currency - Water Fix");
            GUILayout.Toggle(false, " Rapid Citizen Housing");
            GUILayout.Toggle(false, " Advanced Structure Tracking");
            
            GUI.color = Color.white;
            
            GUILayout.Space(15);
            GUILayout.Label("These features are in development and not yet functional.", 
                new GUIStyle(GUI.skin.label) 
                { 
                    fontSize = 10,
                    fontStyle = FontStyle.Italic,
                    normal = { textColor = new Color(0.6f, 0.6f, 0.6f) }
                });
            
            return false;
        }
        #endregion

        #region Tab: Feedback
        private void DrawFeedbackTab()
        {
            DrawSectionHeader("Feature Feedback");
            
            GUILayout.Label(
                "<b>Help improve the mod by reporting feature status!</b>\n\n" +
                "<b>Legend:</b>\n" +
                "[ ? ] Untested   <color=lime>[ OK ]</color> Working   <color=red>[ X ]</color> Broken",
                new GUIStyle(GUI.skin.label) { richText = true, fontSize = 11 }
            );
            
            GUILayout.Space(15);

            var keys = new List<string>(_featureStatus.Keys);
            foreach (string key in keys)
            {
                DrawFeatureStatusButton(key);
            }

            GUILayout.Space(20);
            
            if (GUILayout.Button("📋 COPY REPORT TO CLIPBOARD", GUILayout.Height(45)))
            {
                GenerateAndCopyReport();
            }
            
            GUILayout.Space(10);
            GUILayout.Label("Report will be copied to your clipboard for easy sharing.", 
                new GUIStyle(GUI.skin.label) 
                { 
                    fontSize = 10,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
                });
        }

        private void DrawFeatureStatusButton(string featureName)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(featureName, GUILayout.Width(220));
            
            int status = _featureStatus[featureName];
            string buttonText = GetStatusButtonText(status);
            
            if (GUILayout.Button(buttonText, _statusBtnStyle, GUILayout.Width(100)))
            {
                _featureStatus[featureName] = (status + 1) % 3;
            }
            
            GUILayout.EndHorizontal();
        }

        private string GetStatusButtonText(int status)
        {
            switch (status)
            {
                case 0: return "[ ? ]";
                case 1: return "<color=lime>[ OK ]</color>";
                case 2: return "<color=red>[ X ]</color>";
                default: return "[ ? ]";
            }
        }

        private void GenerateAndCopyReport()
        {
            var report = new StringBuilder();
            report.AppendLine("╔══════════════════════════════════════════╗");
            report.AppendLine("║   Kingdom Enhanced Feedback Report      ║");
            report.AppendLine("╚══════════════════════════════════════════╝");
            report.AppendLine();
            report.AppendLine($"Game Version: {Application.version}");
            report.AppendLine($"Mod Version: {MOD_VERSION}");
            report.AppendLine($"Report Date: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();
            report.AppendLine("Feature Status:");
            report.AppendLine("─────────────────────────────────────────");
            
            foreach (var feature in _featureStatus)
            {
                if (feature.Value > 0)
                {
                    string status = feature.Value == 1 ? "✓ WORKING" : "✗ BROKEN";
                    report.AppendLine($"  {status,-12} {feature.Key}");
                }
            }
            
            report.AppendLine();
            report.AppendLine("Thank you for your feedback!");
            
            GUIUtility.systemCopyBuffer = report.ToString();
            Speak("Feedback report copied to clipboard!");
        }
        #endregion

        #region UI Helpers
        private void DrawSectionHeader(string title)
        {
            GUILayout.Space(12);
            GUILayout.Label(title.ToUpper(), _headerStyle);
            GUILayout.Space(4);
        }

        private void DrawCustomCursor()
        {
            GUI.depth = -10000;
            
            float mouseX = Input.mousePosition.x;
            float mouseY = Screen.height - Input.mousePosition.y;

            // Shadow
            GUI.color = Color.black;
            GUI.Label(new Rect(mouseX + 1, mouseY + 1, 40, 40), "☝", _cursorStyle);
            
            // Main cursor
            GUI.color = new Color(1f, 0.8f, 0.4f);
            GUI.Label(new Rect(mouseX, mouseY, 40, 40), "☝", _cursorStyle);
            
            GUI.color = Color.white;
        }
        #endregion

        #region Settings Management
        private void LoadFromSettings()
        {
            try
            {
                ShowStaminaBar = Settings.ShowStaminaBar.Value;
                EnableAccessibility = Settings.EnableAccessibility.Value;
                CheatsUnlocked = Settings.CheatsUnlocked.Value;
                SpeedMultiplier = Settings.SpeedMultiplier.Value;
                InfiniteStamina = Settings.InfiniteStamina.Value;
                InvincibleWalls = Settings.InvincibleWalls.Value;
                HyperBuilders = Settings.HyperBuilders.Value;
                BetterKnight = Settings.BetterKnight.Value;
                LargerCamps = Settings.LargerCamps.Value;
                BetterCitizenHouses = Settings.BetterCitizenHouses.Value;
                EnableSizeHack = Settings.EnableSizeHack.Value;
                TargetSize = Settings.TargetSize.Value;
                DisplayTimes = Settings.DisplayTimes.Value;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ModMenu] Error loading settings: {ex.Message}");
            }
        }

        public void SaveToSettings()
        {
            try
            {
                Settings.ShowStaminaBar.Value = ShowStaminaBar;
                Settings.EnableAccessibility.Value = EnableAccessibility;
                Settings.CheatsUnlocked.Value = CheatsUnlocked;
                Settings.SpeedMultiplier.Value = SpeedMultiplier;
                Settings.InfiniteStamina.Value = InfiniteStamina;
                Settings.InvincibleWalls.Value = InvincibleWalls;
                Settings.HyperBuilders.Value = HyperBuilders;
                Settings.BetterKnight.Value = BetterKnight;
                Settings.LargerCamps.Value = LargerCamps;
                Settings.BetterCitizenHouses.Value = BetterCitizenHouses;
                Settings.EnableSizeHack.Value = EnableSizeHack;
                Settings.TargetSize.Value = TargetSize;
                Settings.DisplayTimes.Value = DisplayTimes;
                
                Settings.Config.Save();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ModMenu] Error saving settings: {ex.Message}");
            }
        }
        #endregion

        #region Game Actions
        private void GiveCurrency(int amount, bool isGems)
        {
            try
            {
                var player = Managers.Inst?.kingdom?.GetPlayer(0);
                if (player?.wallet == null)
                {
                    Speak("<color=red>Error: Player wallet not found</color>");
                    return;
                }

                var currencyType = isGems ? CurrencyType.Gems : CurrencyType.Coins;
                player.wallet.AddCurrency(currencyType, amount);
                
                string color = isGems ? "cyan" : "yellow";
                string currencyName = isGems ? "Gems" : "Coins";
                string icon = isGems ? "💎" : "💰";
                Speak($"<color={color}>{icon} +{amount} {currencyName}</color>");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ModMenu] Error giving currency: {ex.Message}");
                Speak("<color=red>Error adding currency</color>");
            }
        }
        #endregion
    }
}