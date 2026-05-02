using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KingdomEnhanced.Features;
using KingdomEnhanced.Systems;
using KingdomEnhanced.Core;
using KingdomEnhanced.Shared;

namespace KingdomEnhanced.UI
{
    public enum TabCategory { Main, Cheats, Lab, Hard, Info, Guide, Settings, Report }

    public struct FeatureMeta
    {
        public string Id;
        public string Label;
        public string Section;
        public TabCategory Category;
        public string Description;

        
        public Func<bool> GetValue;
        public Action<bool> SetValue;

        
        public Func<float> GetFloatValue;
        public Action<float> SetFloatValue;
        public float MinVal;
        public float MaxVal;

        
        public Action OnAction;

        
        public Func<bool> IsLocked;
        public Func<string> GetLockReason;

        
        public Func<bool> HasConflict;
    }

    public class ModMenu : MonoBehaviour
    {
        #region PUBLIC SETTINGS
        public static bool ShowStaminaBar;
        public static bool EnableAccessibility;
        public static bool EnableTTS           = true;
        public static bool NarratorQueueMode   = false;
        public static bool SimplifyNames       = true;
        public static bool EnableCastleAnnouncer = false;
        public static bool DebugZones = false;
        public static bool DisplayTimes;
        public static bool CheatsUnlocked;
        public static bool InfiniteStamina;
        public static bool InvincibleWalls;
        public static bool NoToolCooldowns;
        public static float ArtemisArrowCount    = 6f;
        public static float ArtemisRangeMult     = 1.0f;
        public static float ArtemisArrowDamageMult = 1.0f;
        public static bool HyperBuilders;
        public static bool LargerCamps;
        public static bool BetterCitizenHouses;
        public static bool BetterKnight;
        public static bool LockSummer;
        public static bool ClearWeather;
        public static bool NoBloodMoons;
        public static bool CoinsStayDry;
        public static bool EnableSizeHack = false;
        public static float  TargetSize      = 1.0f;
        public static float  SpeedMultiplier = 1.0f;
        public static bool   ShowGreedCounter = false;

        public static float CoinIncomeMult  = 1.0f;
        public static float BagDropMult     = 1.0f;

        public static bool  ArcherFireBoost  = false;
        public static bool  BerserkerRage    = false;
        public static bool  NinjaSpeedBoost  = false;
        public static int   RecruitCap       = 0;
        public static float TreeRegrowthMult = 1.0f;
        public static bool  AnimalSpawnBoost = false;
        public static bool  InstantDaySkip   = false;
        public static bool  FarmOutputBoost  = false;
        public static bool  TowerFireBoost   = false;
        public static bool  BallistaBoost    = false;
        public static bool  InstantCastle    = false;

        public static float SteedSpeedMult   = 1.0f;
        public static bool  ChargeDmgBoost   = false;
        public static float BuffAuraDuration = 1.0f;

        public static float WaveSizeMult       = 1.0f;
        public static float EnemySpeedMult     = 1.0f;
        public static float PortalSpawnRate    = 1.0f;
        public static bool  NoCrownStealing    = false;
        public static float GreedQueenHPScale  = 1.0f;
        public static float DirectorThreatMult = 1.0f;

        public static float WindowScale   = 1.0f;
        public static float MenuOpacity   = 0.98f;
        public static float MonitorOpacity = 0.95f;

        public static string LastAccessMessage = "";
        public static float  MessageTimer      = 0f;
        public static float  SpawnUnitCount    = 1f;

        #endregion

        #region COLORS
        private static readonly Color C_BG              = new Color(0.06f, 0.05f, 0.03f);
        public static readonly Color C_PANEL            = new Color(0.10f, 0.08f, 0.05f);
        public static readonly Color C_CARD             = new Color(0.13f, 0.10f, 0.06f);
        public static readonly Color C_BORDER           = new Color(0.55f, 0.42f, 0.18f);
        public static readonly Color C_GOLD             = new Color(0.90f, 0.72f, 0.30f);
        private static readonly Color C_GOLD_DIM        = new Color(0.70f, 0.55f, 0.20f);
        public static readonly Color C_TEXT             = new Color(0.95f, 0.90f, 0.75f);
        public static readonly Color C_TEXT_DIM         = new Color(0.60f, 0.55f, 0.45f);
        public static readonly Color C_ACCENT_ACTIVE    = new Color(1.00f, 0.80f, 0.35f);
        private static readonly Color C_DANGER          = new Color(0.75f, 0.25f, 0.20f);

        public static readonly Color C_ON               = new Color(0.22f, 0.72f, 0.32f, 1f);
        public static readonly Color C_OFF              = new Color(0.45f, 0.18f, 0.18f, 1f);
        public static readonly Color C_BTN              = C_CARD;
        public static readonly Color C_BTN_HOT          = new Color(0.18f, 0.14f, 0.08f, 1f);
        public static readonly Color C_DANGER_BG        = C_DANGER;
        public static readonly Color C_LOCK             = new Color(0.96f, 0.35f, 0.35f, 1f);
        public static readonly Color C_LOCK_BG          = new Color(0.35f, 0.10f, 0.10f, 1f);
        public static readonly Color C_SOON_BG          = new Color(0.22f, 0.22f, 0.22f, 1f);

        #endregion

        #region CONSTANTS
        private static readonly string[] TAB_LABELS = { "MAIN", "CHEATS", "LAB", "HARD", "INFO", "GUIDE", "SETTINGS", "REPORT" };
        private const float SIDEBAR_W = 140f;
        private const float HEADER_H = 60f;
        private static readonly string MOD_VERSION = ModVersion.DISPLAY;
        private const string CREATOR_CREDIT = "by Zaykus | Thanks to Abevol";
        private const KeyCode MENU_TOGGLE_KEY = KeyCode.F1;

        #endregion

        #region STATE
        private bool         _isVisible = false;
        private TabCategory  _activeTab = TabCategory.Main;
        private Rect         _windowRect = new Rect(30, 110, 600, 500);
        private bool         _isResizing = false;
        private Vector2[]    _scrollPos = new Vector2[8];

        private string _feedbackMsg = "";
        private float  _feedbackTimer = 0f;

        private bool  _resetConfirmPending = false;
        private float _resetConfirmTimer   = 0f;

        private FeatureMeta[] _features;
        private Dictionary<string, int> _featureStatus = new Dictionary<string, int>();

        #endregion

        #region NOTIFICATIONS
        public class Notification
        {
            public string Message;
            public float ExpiryTimestamp;
            public float Alpha = 1f;
            public Color TextColor;

            public Notification(string msg, float dur, Color color)
            {
                Message    = msg;
                ExpiryTimestamp = Time.unscaledTime + dur;
                TextColor  = color;
            }

            public bool IsExpired() => Time.unscaledTime > ExpiryTimestamp + 1f;

            public void UpdateAlpha()
            {
                float left = ExpiryTimestamp - Time.unscaledTime;
                Alpha = left < 1f ? Mathf.Max(0f, left) : 1f;
            }
        }

        private static readonly List<Notification> _notifications = new List<Notification>();
        private const float NotificationDuration = 5f;
        private const int MaxNotifications = 8;

        #endregion

        #region STYLES
        private GUIStyle _styleWindow;
        private GUIStyle _styleTitle;
        private GUIStyle _styleSubtitle;
        private GUIStyle _styleSectionLabel;
        private GUIStyle _styleBodyText;
        private GUIStyle _styleDimText;
        private GUIStyle _styleBtn;
        private GUIStyle _styleBtnDim;
        private GUIStyle _styleTabBtn;
        private GUIStyle _stylePill;
        private GUIStyle _styleNotif;
        private GUIStyle _styleCredit;
        private GUIStyle _styleLocked;
        private GUIStyle _styleCard;
        private bool _stylesBuilt = false;

        #endregion

        #region LIFECYCLE
        void Start()
        {
            _features = ModMenuFeatures.Build();
            LoadFromSettings();
            TTSManager.Initialize();
            Speak("Kingdom Enhanced initialized", C_ON);
        }

        void Update()
        {
            if (Input.GetKeyDown(MENU_TOGGLE_KEY))
            {
                _isVisible = !_isVisible;
                Cursor.visible = _isVisible;
                Cursor.lockState = CursorLockMode.None;
                if (!_isVisible) SaveToSettings();
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                DisplayTimes = !DisplayTimes;
                Settings.DisplayTimes.Value = DisplayTimes;
            }

            if (Input.GetKeyDown(KeyCode.F3))
                KingdomEnhanced.Features.KingdomMonitor.Instance?.Toggle();

            if (MessageTimer > 0f) MessageTimer -= Time.deltaTime;
            if (_feedbackTimer > 0f) _feedbackTimer -= Time.deltaTime;
            if (_resetConfirmTimer > 0f) { _resetConfirmTimer -= Time.deltaTime; if (_resetConfirmTimer <= 0f) _resetConfirmPending = false; }

            _notifications.RemoveAll(n => n.IsExpired());
        }

        void OnGUI()
        {
            BuildStyles();
            DrawFeedbackOverlay();
            DrawNotificationLog();

            if (!_isVisible) return;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Color originalBg = GUI.backgroundColor;
            GUI.backgroundColor = new Color(C_BG.r, C_BG.g, C_BG.b, MenuOpacity);

            _windowRect = GUI.Window(9900, _windowRect, (GUI.WindowFunction)DrawWindow, GUIContent.none, _styleWindow);

            GUI.backgroundColor = originalBg;
        }

        private void BuildStyles()
        {
            if (_stylesBuilt) return;
            _stylesBuilt = true;

            _styleWindow = new GUIStyle(GUI.skin.window) {
                padding = new RectOffset(0, 0, 0, 0),
                normal = { background = GUI.skin.box.normal.background },
                onNormal = { background = GUI.skin.box.normal.background }
            };

            _styleTitle = new GUIStyle(GUI.skin.label) {
                fontSize = 16, fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter, richText = true,
                normal = { textColor = C_GOLD }
            };

            _styleSubtitle = new GUIStyle(GUI.skin.label) {
                fontSize = 11, alignment = TextAnchor.MiddleCenter,
                normal = { textColor = C_TEXT_DIM }
            };

            _styleSectionLabel = new GUIStyle(GUI.skin.label) {
                fontSize = 12, fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(4, 4, 6, 4),
                margin = new RectOffset(0, 0, 0, 0),
                normal = { textColor = C_GOLD }
            };

            _styleBodyText = new GUIStyle(GUI.skin.label) {
                fontSize = 12, richText = true, wordWrap = true,
                normal = { textColor = C_TEXT }
            };

            _styleDimText = new GUIStyle(GUI.skin.label) {
                fontSize = 11, fontStyle = FontStyle.Italic,
                richText = true,
                normal = { textColor = C_TEXT_DIM }
            };

            _styleBtn = new GUIStyle(GUI.skin.button) {
                fontSize = 12, richText = true,
                padding  = new RectOffset(8, 8, 5, 5),
                normal = { textColor = C_TEXT },
                hover = { textColor = C_GOLD },
                active = { textColor = C_ACCENT_ACTIVE },
            };

            _styleBtnDim = new GUIStyle(GUI.skin.button) {
                fontSize = 12, richText = true,
                padding  = new RectOffset(8, 8, 5, 5),
                normal = { textColor = C_TEXT_DIM },
                hover = { textColor = C_GOLD },
                active = { textColor = C_ACCENT_ACTIVE },
            };

            _styleTabBtn = new GUIStyle(GUI.skin.button) {
                fontSize = 13, fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(16, 8, 8, 8),
                margin = new RectOffset(0, 0, 2, 2),
                normal = { textColor = C_TEXT_DIM, background = null },
                hover = { textColor = C_GOLD, background = null },
                active = { textColor = C_ACCENT_ACTIVE, background = null }
            };

            _stylePill = new GUIStyle(GUI.skin.button);
            _stylePill.alignment = TextAnchor.MiddleCenter;
            _stylePill.fontStyle = FontStyle.Bold;
            _stylePill.fixedHeight = 24;
            _stylePill.fontSize = 10;
            _stylePill.normal.textColor = Color.white;
            _stylePill.padding = new RectOffset(4, 4, 2, 2);
            _stylePill.margin = new RectOffset(0, 0, 2, 2);
            _stylePill.border = new RectOffset(2, 2, 2, 2);
            _styleNotif = new GUIStyle(GUI.skin.label) {
                fontSize = 13, fontStyle = FontStyle.Bold,
                richText = true, alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white }
            };

            _styleCredit = new GUIStyle(GUI.skin.label) {
                fontSize = 9, alignment = TextAnchor.LowerRight,
                normal = { textColor = C_TEXT_DIM }
            };

            _styleLocked = new GUIStyle(GUI.skin.label) {
                fontSize = 11, fontStyle = FontStyle.Italic,
                richText = true,
                padding = new RectOffset(0, 0, 4, 4),
                normal = { textColor = C_LOCK }
            };

            _styleCard = new GUIStyle(GUI.skin.box) {
                padding = new RectOffset(8, 8, 8, 8),
                margin = new RectOffset(4, 4, 4, 4)
            };
        }

        #endregion

        #region UI RENDERING
        private void DrawWindow(int id)
        {
            GUILayout.BeginVertical();

            
            GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Height(HEADER_H));
            GUILayout.BeginVertical();
            GUILayout.Space(8);
            GUILayout.Label("Kingdom Enhanced", _styleTitle);
            GUILayout.Label(MOD_VERSION, _styleSubtitle);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            
            GUILayout.BeginHorizontal();

            
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(SIDEBAR_W));
            GUILayout.Space(10);
            for (int i = 0; i < TAB_LABELS.Length; i++)
            {
                bool active = (int)_activeTab == i;
                Color originalColor = GUI.color;
                GUI.color = active ? C_GOLD : Color.white;
                
                if (GUILayout.Button(TAB_LABELS[i], _styleTabBtn, GUILayout.Height(32)))
                {
                    if ((int)_activeTab != i) _scrollPos[i] = Vector2.zero;
                    _activeTab = (TabCategory)i;
                }
                    
                GUI.color = originalColor;
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label(CREATOR_CREDIT, _styleCredit);
            GUILayout.Space(10);
            GUILayout.EndVertical();

            
            GUILayout.BeginVertical();
            _scrollPos[(int)_activeTab] = GUILayout.BeginScrollView(_scrollPos[(int)_activeTab]);
            DrawCurrentTab();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            
            GUILayout.EndHorizontal();

            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Rect resizeRect = GUILayoutUtility.GetRect(20, 20);
            GUI.Label(resizeRect, "➘", new GUIStyle(_styleDimText) { alignment = TextAnchor.LowerRight });
            HandleResize(resizeRect);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, _windowRect.width, HEADER_H));
            
            if (GUI.changed) SaveToSettings();
        }

        private void HandleResize(Rect grip)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && grip.Contains(e.mousePosition))
            { _isResizing = true; e.Use(); }
            else if (e.type == EventType.MouseUp) _isResizing = false;

            if (_isResizing && e.type == EventType.MouseDrag)
            {
                _windowRect.width  = Mathf.Max(500, _windowRect.width  + e.delta.x);
                _windowRect.height = Mathf.Max(400, _windowRect.height + e.delta.y);
            }
        }

        private void LoadFromSettings()
        {
            ShowStaminaBar        = Settings.ShowStaminaBar.Value;
            DisplayTimes          = Settings.DisplayTimes.Value;
            ShowGreedCounter      = Settings.ShowGreedCounter.Value;
            EnableAccessibility   = Settings.EnableAccessibility.Value;
            EnableTTS             = Settings.EnableTTS.Value;
            NarratorQueueMode     = Settings.NarratorQueueMode.Value;
            SimplifyNames         = Settings.SimplifyNames.Value;
            EnableCastleAnnouncer = Settings.EnableCastleAnnouncer.Value;
            DebugZones            = Settings.DebugZones.Value;
            CheatsUnlocked        = Settings.CheatsUnlocked.Value;
            SpeedMultiplier       = Settings.SpeedMultiplier.Value;
            InfiniteStamina       = Settings.InfiniteStamina.Value;
            InvincibleWalls       = Settings.InvincibleWalls.Value;
            NoToolCooldowns       = Settings.NoToolCooldowns.Value;
            ArtemisArrowCount     = Settings.ArtemisArrowCount.Value;
            ArtemisRangeMult      = Settings.ArtemisRangeMult.Value;
            ArtemisArrowDamageMult = Settings.ArtemisArrowDamageMult.Value;
            CoinIncomeMult        = Settings.CoinIncomeMult.Value;
            BagDropMult           = Settings.BagDropMult.Value;
            HyperBuilders         = Settings.HyperBuilders.Value;
            LargerCamps           = Settings.LargerCamps.Value;
            BetterKnight          = Settings.BetterKnight.Value;
            BetterCitizenHouses   = Settings.BetterCitizenHouses.Value;
            LockSummer            = Settings.LockSummer.Value;
            ClearWeather          = Settings.ClearWeather.Value;
            NoBloodMoons          = Settings.NoBloodMoons.Value;
            CoinsStayDry          = Settings.CoinsStayDry.Value;
            ArcherFireBoost       = Settings.ArcherFireBoost.Value;
            BerserkerRage         = Settings.BerserkerRage.Value;
            NinjaSpeedBoost       = Settings.NinjaSpeedBoost.Value;
            RecruitCap            = Settings.RecruitCapOverride.Value;
            TreeRegrowthMult      = Settings.TreeRegrowthMult.Value;
            AnimalSpawnBoost      = Settings.AnimalSpawnBoost.Value;
            InstantDaySkip        = Settings.InstantDaySkip.Value;
            FarmOutputBoost       = Settings.FarmOutputBoost.Value;
            TowerFireBoost        = Settings.TowerFireBoost.Value;
            BallistaBoost         = Settings.BallistaBoost.Value;
            InstantCastle         = Settings.InstantCastle.Value;
            EnableSizeHack        = Settings.EnableSizeHack.Value;
            TargetSize            = Settings.TargetSize.Value;
            SteedSpeedMult        = Settings.SteedSpeedMult.Value;
            ChargeDmgBoost        = Settings.ChargeDmgBoost.Value;
            BuffAuraDuration      = Settings.BuffAuraDuration.Value;
            WaveSizeMult          = Settings.WaveSizeMult.Value;
            EnemySpeedMult        = Settings.EnemySpeedMult.Value;
            PortalSpawnRate       = Settings.PortalSpawnRate.Value;
            NoCrownStealing       = Settings.NoCrownStealing.Value;
            GreedQueenHPScale     = Settings.GreedQueenHPScale.Value;
            DirectorThreatMult    = Settings.DirectorThreatMult.Value;
        }

        private void SaveToSettings()
        {
            Settings.ShowStaminaBar.Value        = ShowStaminaBar;
            Settings.DisplayTimes.Value          = DisplayTimes;
            Settings.ShowGreedCounter.Value      = ShowGreedCounter;
            Settings.EnableAccessibility.Value   = EnableAccessibility;
            Settings.EnableTTS.Value             = EnableTTS;
            Settings.NarratorQueueMode.Value     = NarratorQueueMode;
            Settings.SimplifyNames.Value         = SimplifyNames;
            Settings.EnableCastleAnnouncer.Value = EnableCastleAnnouncer;
            Settings.DebugZones.Value            = DebugZones;
            Settings.CheatsUnlocked.Value        = CheatsUnlocked;
            Settings.SpeedMultiplier.Value       = SpeedMultiplier;
            Settings.InfiniteStamina.Value       = InfiniteStamina;
            Settings.InvincibleWalls.Value       = InvincibleWalls;
            Settings.NoToolCooldowns.Value       = NoToolCooldowns;
            Settings.ArtemisArrowCount.Value      = ArtemisArrowCount;
            Settings.ArtemisRangeMult.Value       = ArtemisRangeMult;
            Settings.ArtemisArrowDamageMult.Value = ArtemisArrowDamageMult;
            Settings.CoinIncomeMult.Value        = CoinIncomeMult;
            Settings.BagDropMult.Value           = BagDropMult;
            Settings.HyperBuilders.Value         = HyperBuilders;
            Settings.LargerCamps.Value           = LargerCamps;
            Settings.BetterKnight.Value          = BetterKnight;
            Settings.BetterCitizenHouses.Value   = BetterCitizenHouses;
            Settings.LockSummer.Value            = LockSummer;
            Settings.ClearWeather.Value          = ClearWeather;
            Settings.NoBloodMoons.Value          = NoBloodMoons;
            Settings.CoinsStayDry.Value          = CoinsStayDry;
            Settings.ArcherFireBoost.Value       = ArcherFireBoost;
            Settings.BerserkerRage.Value         = BerserkerRage;
            Settings.NinjaSpeedBoost.Value       = NinjaSpeedBoost;
            Settings.RecruitCapOverride.Value    = RecruitCap;
            Settings.TreeRegrowthMult.Value      = TreeRegrowthMult;
            Settings.AnimalSpawnBoost.Value      = AnimalSpawnBoost;
            Settings.InstantDaySkip.Value        = InstantDaySkip;
            Settings.FarmOutputBoost.Value       = FarmOutputBoost;
            Settings.TowerFireBoost.Value        = TowerFireBoost;
            Settings.BallistaBoost.Value         = BallistaBoost;
            Settings.InstantCastle.Value         = InstantCastle;
            Settings.EnableSizeHack.Value        = EnableSizeHack;
            Settings.TargetSize.Value            = TargetSize;
            Settings.SteedSpeedMult.Value        = SteedSpeedMult;
            Settings.ChargeDmgBoost.Value        = ChargeDmgBoost;
            Settings.BuffAuraDuration.Value      = BuffAuraDuration;
            Settings.WaveSizeMult.Value          = WaveSizeMult;
            Settings.EnemySpeedMult.Value        = EnemySpeedMult;
            Settings.PortalSpawnRate.Value       = PortalSpawnRate;
            Settings.NoCrownStealing.Value       = NoCrownStealing;
            Settings.GreedQueenHPScale.Value     = GreedQueenHPScale;
            Settings.DirectorThreatMult.Value    = DirectorThreatMult;
        }

        private void DrawCurrentTab()
        {
            if (_activeTab == TabCategory.Cheats && !CheatsUnlocked)
            {
                DrawCheatsGate();
                return;
            }

            
            string lastSection = null;
            bool inCard = false;

            foreach (var f in _features.Where(f => f.Category == _activeTab))
            {
                if (f.Section != lastSection)
                {
                    if (inCard) GUILayout.EndVertical(); 
                    GUILayout.BeginVertical(_styleCard);
                    inCard = true;
                    GuiHelper.DrawSection(f.Section, _styleSectionLabel);
                    lastSection = f.Section;
                }

                DrawFeatureRow(f);
            }
            
            if (inCard) GUILayout.EndVertical();
            
            
            switch (_activeTab)
            {
                case TabCategory.Info: DrawInfoTab(); break;
                case TabCategory.Settings: DrawSettingsTab(); break;
                case TabCategory.Guide: DrawGuideTab(); break;
                case TabCategory.Report: DrawReportTab(); break;
                case TabCategory.Lab: DrawLabExtras(); break;
            }
        }

        private void DrawLabExtras()
        {
            GUILayout.BeginVertical(_styleCard);
            GuiHelper.DrawSection("TIME CONTROLS", _styleSectionLabel);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Instantly Time Jump", _styleBodyText, GUILayout.Width(180));
            
            if (GUILayout.Button("Skip Daytime", _styleBtn, GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
                Features.WorldManager.SkipDaytime();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Skip Nighttime", _styleBtn, GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
                Features.WorldManager.SkipNighttime();
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawFeatureRow(FeatureMeta f)
        {
            GUILayout.BeginHorizontal();
            
            
            GUILayout.Label(f.Label, _styleBodyText, GUILayout.Width(180));
            
            
            bool isLocked = f.IsLocked != null && f.IsLocked();
            
            if (isLocked)
            {
                string reason = f.GetLockReason != null ? f.GetLockReason() : "Locked";
                GUILayout.Label($"<i>[{reason}]</i>", _styleLocked, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
                return;
            }

            
            if (f.HasConflict != null && f.HasConflict())
            {
                GUI.color = Color.yellow;
                GUILayout.Label("!", _styleBodyText, GUILayout.Width(20));
                GUI.color = Color.white;
            }
            else
            {
                GUILayout.Space(24);
            }

            
            if (f.GetFloatValue != null)
            {
                
                float val = f.GetFloatValue();
                GUILayout.Label($"{val:F1}", _styleBodyText, GUILayout.Width(40));
                float newVal = GUILayout.HorizontalSlider(val, f.MinVal, f.MaxVal, GUILayout.ExpandWidth(true));
                if (Math.Abs(newVal - val) > 0.01f) f.SetFloatValue(newVal);
            }
            else if (f.OnAction != null)
            {
                
                if (GUILayout.Button("Apply", _styleBtn, GUILayout.Width(100))) f.OnAction();
            }
            else if (f.GetValue != null)
            {
                
                bool val = f.GetValue();

                GUI.backgroundColor = val ? C_ON : C_OFF;
                if (GUILayout.Button(val ? "ON" : "OFF", _stylePill, GUILayout.Width(60)))
                {
                    f.SetValue(!val);
                }
                GUI.backgroundColor = Color.white;
            }

            GUILayout.EndHorizontal();
        }

        private void DrawCheatsGate()
        {
            GUILayout.BeginVertical(_styleCard);
            GuiHelper.DrawSection("ACCESS REQUIRED", _styleSectionLabel);
            GUILayout.Label("Cheats are disabled by default to preserve gameplay progression.", _styleBodyText);
            GUILayout.Space(10);
            
            if (GUILayout.Button("Unlock Cheats", _styleBtn, GUILayout.Height(32)))
            {
                CheatsUnlocked = true;
                Settings.CheatsUnlocked.Value = true;
                Speak("Cheats unlocked", C_ON);
            }
            GUILayout.EndVertical();
        }

        private void DrawGuideTab()
        {
            GUILayout.BeginVertical(_styleCard);
            GuiHelper.DrawSection("FEATURE REFERENCE", _styleSectionLabel);
            GUILayout.Label("All features and settings explained dynamically.", _styleSubtitle);
            GUILayout.Space(10);

            
            foreach (var f in _features)
            {
                GUILayout.Space(6);
                GUILayout.BeginHorizontal();
                GUILayout.Label($"<b>[{f.Category.ToString().ToUpper()}]</b> — {f.Label}", _styleTitle, GUILayout.Height(20));
                GUILayout.FlexibleSpace();
                
                
                string stateStr = "";
                Color stateColor = Color.white;

                if (f.IsLocked != null && f.IsLocked())
                {
                    stateStr = f.GetLockReason != null ? f.GetLockReason() : "Locked";
                    stateColor = C_LOCK;
                }
                else if (f.GetFloatValue != null)
                {
                    stateStr = $"{f.GetFloatValue():F1}x";
                }
                else if (f.OnAction != null)
                {
                    stateStr = "Action";
                }
                else if (f.GetValue != null)
                {
                    stateStr = f.GetValue() ? "ON" : "OFF";
                    stateColor = f.GetValue() ? C_ON : Color.white;
                }

                GUILayout.Label(stateStr, new GUIStyle(_styleTitle) { normal = { textColor = stateColor } }, GUILayout.Height(20));
                GUILayout.EndHorizontal();
                
                GUILayout.Label(f.Description, _styleDimText);
                GUILayout.Space(4);
            }

            GUILayout.EndVertical();
        }

        private void DrawReportTab()
        {
            GUILayout.BeginVertical(_styleCard);
            GuiHelper.DrawSection("TESTING REPORT", _styleSectionLabel);
            GUILayout.Label("Mark each feature after testing. Copy to Clipboard to share your report.", _styleDimText);
            GUILayout.Space(8);

            
            foreach (var f in _features)
            {
                if (!_featureStatus.ContainsKey(f.Id)) _featureStatus[f.Id] = 0;
                int status = _featureStatus[f.Id];

                string statusLabel = status == 1 ? "<color=#44cc66>✓ Works</color>" : status == 2 ? "<color=#cc4444>✗ Broken</color>" : "<color=#888888>? Not Tested</color>";

                GUILayout.BeginHorizontal();
                
                
                string catName = f.Category.ToString().ToUpper();
                string catDisplay = catName.Length >= 4 ? catName.Substring(0, 4) : catName;
                GUILayout.Label($"<b>[{catDisplay}]</b>", _styleBodyText, GUILayout.Width(60));
                
                GUILayout.Label(f.Label, _styleBodyText, GUILayout.Width(180));
                GUILayout.Label(statusLabel, _styleBodyText, GUILayout.ExpandWidth(true));

                Color prev = GUI.color;
                
                GUI.color = status == 1 ? C_ON : Color.white;
                if (GUILayout.Button("Works", _styleBtn, GUILayout.Width(60))) _featureStatus[f.Id] = status == 1 ? 0 : 1;
                
                GUI.color = status == 2 ? C_LOCK : Color.white;
                if (GUILayout.Button("Broken", _styleBtn, GUILayout.Width(60))) _featureStatus[f.Id] = status == 2 ? 0 : 2;
                
                GUI.color = prev;
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(14);
            if (GUILayout.Button("Copy Report to Clipboard", _styleBtn, GUILayout.Height(36)))
            {
                var sb = new StringBuilder();
                sb.AppendLine($"=== Kingdom Enhanced — Testing Report ({System.DateTime.Now:yyyy-MM-dd HH:mm}) ===");
                sb.AppendLine($"Version  : {MOD_VERSION}");
                
                foreach(var cat in (TabCategory[])Enum.GetValues(typeof(TabCategory)))
                {
                    var items = _features.Where(x => x.Category == cat).ToList();
                    if (items.Count == 0) continue;

                    sb.AppendLine();
                    sb.AppendLine($"--- {cat.ToString().ToUpper()} ---");
                    foreach (var f in items)
                    {
                        int s = _featureStatus.ContainsKey(f.Id) ? _featureStatus[f.Id] : 0;
                        string statusStr = s == 1 ? "[Works]     " : s == 2 ? "[BROKEN]    " : "[Not Tested]";
                        sb.AppendLine($"  {statusStr}  {f.Label}");
                    }
                }

                GUIUtility.systemCopyBuffer = sb.ToString();
                ShowFeedback("Report copied to clipboard!");
            }

            GUILayout.EndVertical();
        }

        private void DrawInfoTab()
        {
            GUILayout.BeginVertical(_styleCard);
            GuiHelper.DrawSection("MOD INFORMATION", _styleSectionLabel);
            GUILayout.Label($"Kingdom Enhanced v{MOD_VERSION}", _styleTitle);
            GUILayout.Space(10);
            GUILayout.Label("Developer: Zaykus", _styleBodyText);
            GUILayout.Label("Special Thanks: Abevol", _styleBodyText);
            GUILayout.Space(20);
            GUILayout.Label("Press F1 to toggle this menu.", _styleDimText);
            GUILayout.Label("Press F3 to toggle the Kingdom Monitor.", _styleDimText);
            GUILayout.Label("Press F4 to toggle HUD Display.", _styleDimText);
            GUILayout.EndVertical();
        }

        private void DrawSettingsTab()
        {
            GUILayout.BeginVertical(_styleCard);
            GuiHelper.DrawSection("GLOBAL SETTINGS", _styleSectionLabel);

            GUILayout.Label("Window Scale", _styleBodyText);
            WindowScale = GUILayout.HorizontalSlider(WindowScale, 0.5f, 2.0f);
            
            GUILayout.Label("Menu Opacity", _styleBodyText);
            MenuOpacity = GUILayout.HorizontalSlider(MenuOpacity, 0.5f, 1.0f);

            GUILayout.Space(10);

            if (GUILayout.Button("Reset All Settings", _styleBtn))
            {
                if (!_resetConfirmPending)
                {
                    _resetConfirmPending = true;
                    _resetConfirmTimer = 3f;
                }
                else
                {
                    
                    ShowFeedback("Settings Reset!");
                    _resetConfirmPending = false;
                }
            }
            
            if (_resetConfirmPending)
            {
                GUILayout.Label("Press again within 3 seconds to confirm.", _styleLocked);
            }

            GUILayout.EndVertical();
        }
        
        private void DrawFeedbackOverlay()
        {
            if (_feedbackTimer <= 0) return;
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
            GUI.color = new Color(1, 1, 1, Mathf.Clamp01(_feedbackTimer));
            GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 50, 400, 100), _feedbackMsg, style);
            GUI.color = Color.white;
        }

        private void DrawNotificationLog()
        {
            
            float y = Screen.height - 10; 
            
            
            
            
            
            for (int i = _notifications.Count - 1; i >= 0; i--)
            {
                var n = _notifications[i];
                n.UpdateAlpha();
                if (n.Alpha <= 0) continue;

                GUI.color = new Color(n.TextColor.r, n.TextColor.g, n.TextColor.b, n.Alpha);
                
                y -= 26; 
                var rect = new Rect(10, y, 300, 24);
                GUI.Label(rect, n.Message, _styleNotif);
            }
            GUI.color = Color.white;
        }

        private void ShowFeedback(string msg)
        {
            _feedbackMsg = msg;
            _feedbackTimer = 2f;
        }

        
        public static void Speak(string text, Color color)
        {
            if (EnableTTS) TTSManager.Speak(text);
            _notifications.Add(new Notification(text, NotificationDuration, color));
        }

        
        public static void Speak(string text)
        {
            Speak(text, Color.white);
        }

        
        public static void Speak(string text, bool interrupt)
        {
            
            
            
            if (EnableTTS) TTSManager.Speak(text);
            Speak(text, Color.white);
        }
        
        public static void GiveCurrency(int amount, bool isGem)
        {
            var player = Managers.Inst?.kingdom?.GetPlayer(0);
            if (player == null || player.wallet == null) return;

            if (isGem)
            {
                player.wallet.Gems = Mathf.Min(100, player.wallet.Gems + amount);
                Speak($"+{amount} Gems", C_GOLD);
            }
            else
            {
                player.wallet.Coins = Mathf.Min(100, player.wallet.Coins + amount);
                Speak($"+{amount} Coins", C_GOLD);
            }
        }

        public static void FillWallet()
        {
            var player = Managers.Inst?.kingdom?.GetPlayer(0);
            if (player == null || player.wallet == null) return;

            player.wallet.Coins = 100;
            Speak("Wallet Filled to Max!", C_GOLD);
        }

        public static void CycleStaminaBarStyle()
        {
            if (StaminaBarHolder.Instance == null) return;
            StaminaBarHolder.Instance.visualStyle =
                (StaminaBarHolder.Instance.visualStyle + 1) % 4;
            Speak($"Style: {StaminaBarHolder.Instance.GetStyleName()}");
        }

        public static void CycleStaminaBarPosition()
        {
            if (StaminaBarHolder.Instance == null) return;
            StaminaBarHolder.Instance.positionMode =
                (StaminaBarHolder.Instance.positionMode + 1) % 6;
            Speak($"Position: {StaminaBarHolder.Instance.GetPositionName()}");
        }

        #endregion
    }
}