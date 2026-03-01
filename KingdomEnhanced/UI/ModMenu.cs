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

        // BINDINGS
        public Func<bool> GetValue;
        public Action<bool> SetValue;

        // SLIDER BINDINGS
        public Func<float> GetFloatValue;
        public Action<float> SetFloatValue;
        public float MinVal;
        public float MaxVal;

        // ACTIONS
        public Action OnAction;

        // LOCKS
        public Func<bool> IsLocked;
        public Func<string> GetLockReason;

        // CONFLICTS
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
            BuildFeatureMetadata();
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

        // Helper for Toggle features
        private static FeatureMeta F(string id, string label, TabCategory cat, string section, string desc, Func<bool> get, Action<bool> set, Func<bool> isLocked = null, Func<string> lockReason = null, Func<bool> hasConflict = null)
        {
            return new FeatureMeta {
                Id = id, Label = label, Section = section, Category = cat, Description = desc,
                GetValue = get, SetValue = set, OnAction = null,
                IsLocked = isLocked, GetLockReason = lockReason, HasConflict = hasConflict
            };
        }

        // Helper for Action buttons
        private static FeatureMeta Action(string id, string label, TabCategory cat, string section, string desc, Action act, Func<bool> isLocked = null, Func<string> lockReason = null)
        {
            return new FeatureMeta {
                Id = id, Label = label, Section = section, Category = cat, Description = desc,
                GetValue = null, SetValue = null, OnAction = act,
                IsLocked = isLocked, GetLockReason = lockReason, HasConflict = null
            };
        }

        // Helper for Sliders
        private static FeatureMeta Slider(string id, string label, TabCategory cat, string section, string desc, Func<float> get, Action<float> set, float min, float max)
        {
            return new FeatureMeta {
                Id = id, Label = label, Section = section, Category = cat, Description = desc,
                GetFloatValue = get, SetFloatValue = set, MinVal = min, MaxVal = max,
                IsLocked = null, GetLockReason = null, HasConflict = null
            };
        }

        private void BuildFeatureMetadata()
        {
            var list = new List<FeatureMeta>();

            // MAIN
            list.Add(F("show_stamina", "Energy Bar", TabCategory.Main, "HUD", "Shows or hides the stamina bar on the HUD.", () => ShowStaminaBar, v => ShowStaminaBar = v));
            list.Add(Action("stamina_style", "Cycle Energy Bar Style", TabCategory.Main, "HUD", "Changes the visual style of the Energy Bar.", () => CycleStaminaBarStyle(), () => !ShowStaminaBar, () => "Requires Energy Bar"));
            list.Add(Action("stamina_pos", "Cycle Energy Bar Position", TabCategory.Main, "HUD", "Changes the position of the Energy Bar on screen.", () => CycleStaminaBarPosition(), () => !ShowStaminaBar, () => "Requires Energy Bar"));
            list.Add(F("display_times", "HUD Display", TabCategory.Main, "HUD", "Toggles the entire in-game HUD overlay.", () => DisplayTimes, v => DisplayTimes = v));
            list.Add(Action("monitor_style", "Cycle Monitor Style", TabCategory.Main, "HUD", "Changes the visual style of the Kingdom Monitor panel.", () => KingdomMonitor.Instance?.NextStyle(), () => KingdomMonitor.Instance == null || !KingdomMonitor.Instance.IsVisible, () => "Requires Monitor"));

            list.Add(F("enable_tts", "Narrator (TTS)", TabCategory.Main, "Accessibility", "Reads menu interactions aloud using the system TTS engine.", () => EnableTTS, v => EnableTTS = v));
            list.Add(F("simplify_names", "Simplify Names", TabCategory.Main, "Accessibility", "Replaces payable object names with shorter labels.", () => SimplifyNames, v => SimplifyNames = v));
            list.Add(F("castle_announcer", "Castle Announcer", TabCategory.Main, "Accessibility", "Announces castle events via TTS.", () => EnableCastleAnnouncer, v => EnableCastleAnnouncer = v));

            list.Add(Slider("speed_mult", "Travel Speed", TabCategory.Main, "Movement", "Multiplies the monarch's base movement speed.", () => SpeedMultiplier, v => SpeedMultiplier = v, 0.5f, 10.0f));

            // CHEATS
            list.Add(F("infinite_stamina", "Infinite Mount Stamina", TabCategory.Cheats, "Invincibility", "Prevents mount stamina from depleting.", () => InfiniteStamina, v => InfiniteStamina = v, () => !CheatsUnlocked));

            list.Add(F("no_tool_cooldowns", "No Tool Cooldowns", TabCategory.Cheats, "Infinite Stone", "Removes the cooldown/timeout of all Hermit tools (Horn of Healing, Athena's Shield, Hermes' Staff, etc).", () => NoToolCooldowns, v => NoToolCooldowns = v, () => !CheatsUnlocked));
            list.Add(Slider("artemis_arrows", "Artemis Arrow Count", TabCategory.Cheats, "Infinite Stone", "How many arrows fall per cast of the Artemis Bow.", () => ArtemisArrowCount, v => ArtemisArrowCount = v, 1f, 50f));
            list.Add(Slider("artemis_range", "Artemis Range", TabCategory.Cheats, "Infinite Stone", "Multiplies the range across which arrows are spread.", () => ArtemisRangeMult, v => ArtemisRangeMult = v, 0.5f, 5.0f));
            list.Add(Slider("artemis_damage", "Artemis Arrow Damage", TabCategory.Cheats, "Infinite Stone", "Multiplies damage dealt per arrow.", () => ArtemisArrowDamageMult, v => ArtemisArrowDamageMult = v, 0.5f, 5.0f));

            list.Add(Action("add_10_coins", "+10 Coins", TabCategory.Cheats, "Economy", "Adds 10 coins instantly.", () => GiveCurrency(10, false), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("add_50_coins", "+50 Coins", TabCategory.Cheats, "Economy", "Adds 50 coins instantly.", () => GiveCurrency(50, false), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("add_5_gems", "+5 Gems", TabCategory.Cheats, "Economy", "Instant gem grant.", () => GiveCurrency(5, true), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("fill_wallet", "Fill Wallet to Max", TabCategory.Cheats, "Economy", "Fills coins and gems to capacity.", () => FillWallet(), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Slider("coin_income", "Coin Income", TabCategory.Cheats, "Economy", "Multiplies passive coin income rate.", () => CoinIncomeMult, v => CoinIncomeMult = v, 0.5f, 4.0f));
            list.Add(Slider("bag_drop", "Bag Drop Rate", TabCategory.Cheats, "Economy", "Multiplies bag drop rate.", () => BagDropMult, v => BagDropMult = v, 0.5f, 4.0f));

            list.Add(Action("recruit_beggars", "Recruit All Beggars", TabCategory.Cheats, "Military", "Forces all vagrants to immediately pick up tools and join.", () => ArmyManager.RecruitBeggars(), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("drop_archer", "Drop Archer Bow", TabCategory.Cheats, "Military", "Spawns an archer bow pickup at the monarch's position.", () => ArmyManager.DropTools("Archer"), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("drop_builder", "Drop Builder Hammer", TabCategory.Cheats, "Military", "Spawns a builder hammer pickup at the monarch's position.", () => ArmyManager.DropTools("Builder"), () => !CheatsUnlocked, () => "Locked"));
            
            list.Add(Action("kill_enemies", "Kill All Enemies", TabCategory.Cheats, "Military", "Instantly destroys all active Greed units.", () => ArmyManager.KillAllEnemies(), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("destroy_portals", "Destroy All Portals", TabCategory.Cheats, "Military", "Closes all active portals.", () => ArmyManager.DestroyAllPortals(), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("max_army", "Spawn Max Army", TabCategory.Cheats, "Military", "Fills all available archer/knight slots instantly.", () => ArmyManager.SpawnMaxArmy(), () => !CheatsUnlocked, () => "Locked"));

            list.Add(Slider("spawn_unit_count", "Unit Spawn Amount", TabCategory.Cheats, "Unit Spawner", "Select amount of units to spawn (1-50).", () => SpawnUnitCount, v => SpawnUnitCount = (int)v, 1f, 50f));
            list.Add(Action("spawn_u_vagrant", "Spawn Vagrants", TabCategory.Cheats, "Unit Spawner", "Spawns Vagrants (Baggers).", () => ArmyManager.SpawnUnit("Beggar", (int)SpawnUnitCount), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_u_villager", "Spawn Villagers", TabCategory.Cheats, "Unit Spawner", "Spawns Villagers.", () => ArmyManager.SpawnUnit("Peasant", (int)SpawnUnitCount), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_u_archer", "Spawn Archers", TabCategory.Cheats, "Unit Spawner", "Spawns Archers.", () => ArmyManager.SpawnUnit("Archer", (int)SpawnUnitCount), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_u_builder", "Spawn Builders", TabCategory.Cheats, "Unit Spawner", "Spawns Builders.", () => ArmyManager.SpawnUnit("Worker", (int)SpawnUnitCount), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_u_farmer", "Spawn Farmers", TabCategory.Cheats, "Unit Spawner", "Spawns Farmers.", () => ArmyManager.SpawnUnit("Farmer", (int)SpawnUnitCount), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_u_pikeman", "Spawn Pikemen", TabCategory.Cheats, "Unit Spawner", "Spawns Pikemen.", () => ArmyManager.SpawnUnit("Pikeman", (int)SpawnUnitCount), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_u_ninja", "Spawn Ninjas", TabCategory.Cheats, "Unit Spawner", "Spawns Ninjas (Shogun only).", () => ArmyManager.SpawnUnit("Ninja", (int)SpawnUnitCount), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_u_berserker", "Spawn Berserkers", TabCategory.Cheats, "Unit Spawner", "Spawns Berserkers (Norse only).", () => ArmyManager.SpawnUnit("Berserker", (int)SpawnUnitCount), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_u_knight", "Spawn Knights", TabCategory.Cheats, "Unit Spawner", "Spawns Knights.", () => ArmyManager.SpawnUnit("Knight", (int)SpawnUnitCount), () => !CheatsUnlocked, () => "Locked"));

            list.Add(Action("spawn_h_bakery", "Spawn Bakery Hermit", TabCategory.Cheats, "Hermit Spawner", "Spawns the Bakery Hermit.", () => ArmyManager.SpawnHermit("Bakery"), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_h_ballista", "Spawn Ballista Hermit", TabCategory.Cheats, "Hermit Spawner", "Spawns the Ballista Hermit.", () => ArmyManager.SpawnHermit("Ballista"), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_h_berserker", "Spawn Berserker Hermit", TabCategory.Cheats, "Hermit Spawner", "Spawns the Berserker Hermit (Norse only).", () => ArmyManager.SpawnHermit("Berserker"), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_h_fire", "Spawn Fire Hermit", TabCategory.Cheats, "Hermit Spawner", "Spawns the Fire Hermit.", () => ArmyManager.SpawnHermit("Fire"), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_h_horn", "Spawn Horn Hermit", TabCategory.Cheats, "Hermit Spawner", "Spawns the Horn Hermit.", () => ArmyManager.SpawnHermit("Horn"), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_h_stable", "Spawn Stable Hermit", TabCategory.Cheats, "Hermit Spawner", "Spawns the Stable Hermit.", () => ArmyManager.SpawnHermit("Stable"), () => !CheatsUnlocked, () => "Locked"));
            list.Add(Action("spawn_h_warrior", "Spawn Warrior Hermit", TabCategory.Cheats, "Hermit Spawner", "Spawns the Warrior Hermit.", () => ArmyManager.SpawnHermit("Warrior"), () => !CheatsUnlocked, () => "Locked"));

            list.Add(F("hyper_builders", "Instant Construction", TabCategory.Cheats, "Builders", "Buildings complete in one frame.", () => HyperBuilders, v => HyperBuilders = v, () => !CheatsUnlocked, null, () => HyperBuilders && LargerCamps));
            list.Add(F("larger_camps", "Expand Vagrant Camps", TabCategory.Cheats, "Builders", "Increases the maximum vagrant camp population.", () => LargerCamps, v => LargerCamps = v, () => !CheatsUnlocked, null, () => HyperBuilders && LargerCamps));
            
            // LAB
            list.Add(F("lock_summer", "Lock Summer Season", TabCategory.Lab, "World", "Prevents the season from advancing past summer.", () => LockSummer, v => LockSummer = v));
            list.Add(F("clear_weather", "Clear Weather", TabCategory.Lab, "World", "Disables rain and snow effects.", () => ClearWeather, v => ClearWeather = v, null, null, () => LockSummer && ClearWeather));
            list.Add(F("coins_stay_dry", "Buoyant Currency", TabCategory.Lab, "World", "Coins dropped in water float instead of sinking.", () => CoinsStayDry, v => CoinsStayDry = v));
            list.Add(F("no_blood_moons", "Disable Blood Moons", TabCategory.Lab, "World", "Prevents blood moon wave events.", () => NoBloodMoons, v => NoBloodMoons = v));
            
            list.Add(F("invincible_walls", "Self-Repairing Walls", TabCategory.Lab, "Structures", "Damaged walls automatically restore over time.", () => InvincibleWalls, v => InvincibleWalls = v));
            list.Add(F("better_citizen_houses", "Rapid Citizen Housing", TabCategory.Lab, "Structures", "Citizens move into houses faster.", () => BetterCitizenHouses, v => BetterCitizenHouses = v));
            list.Add(F("better_knight", "Elite Knights", TabCategory.Lab, "Combat", "Increases knight combat effectiveness.", () => BetterKnight, v => BetterKnight = v));
            
            list.Add(F("archer_fire_boost", "Archer Fire Rate Boost", TabCategory.Lab, "Units", "Archers shoot significantly faster.", () => ArcherFireBoost, v => ArcherFireBoost = v));
            list.Add(F("berserker_rage", "Berserker Rage Mode", TabCategory.Lab, "Units", "Berserkers enter rage state permanently.", () => BerserkerRage, v => BerserkerRage = v));
            list.Add(F("ninja_speed_boost", "Ninja Speed Boost", TabCategory.Lab, "Units", "Ninjas move faster.", () => NinjaSpeedBoost, v => NinjaSpeedBoost = v));

            // Cast RecruitCap (int) to float for slider
            list.Add(Slider("recruit_cap", "Recruit Cap", TabCategory.Lab, "Lab Rules", "Overrides max recruitable units. 0 for default.", () => RecruitCap, v => RecruitCap = (int)v, 0, 50));
            list.Add(Slider("tree_regrow", "Tree Regrowth", TabCategory.Lab, "World", "Multiplies tree regrowth speed.", () => TreeRegrowthMult, v => TreeRegrowthMult = v, 0.1f, 5.0f));
            
            list.Add(F("farm_output", "Farm Output Boost", TabCategory.Lab, "World", "Increases farm production yield.", () => FarmOutputBoost, v => FarmOutputBoost = v));
            list.Add(F("tower_fire", "Tower Fire Boost", TabCategory.Lab, "World", "Increases tower fire rate.", () => TowerFireBoost, v => TowerFireBoost = v));
            list.Add(F("ballista_boost", "Ballista Boost", TabCategory.Lab, "World", "Increases ballista damage/speed.", () => BallistaBoost, v => BallistaBoost = v));
            list.Add(F("instant_castle", "Instant Castle Upgrade", TabCategory.Lab, "World", "Castle upgrades finish immediately.", () => InstantCastle, v => InstantCastle = v));
            list.Add(F("instant_day_skip", "Instant Day Skip", TabCategory.Lab, "World", "Skips the day/night transition delays.", () => InstantDaySkip, v => InstantDaySkip = v));
            list.Add(F("animal_spawn", "Animal Spawn Boost", TabCategory.Lab, "World", "Increases animal spawn rates.", () => AnimalSpawnBoost, v => AnimalSpawnBoost = v));

            list.Add(Slider("steed_speed", "Steed Speed", TabCategory.Lab, "Steed", "Multiplies steed movement speed.", () => SteedSpeedMult, v => SteedSpeedMult = v, 0.5f, 3.0f));
            list.Add(F("charge_dmg", "Charge Damage Boost", TabCategory.Lab, "Steed", "Increases steed charge damage.", () => ChargeDmgBoost, v => ChargeDmgBoost = v));
            list.Add(Slider("buff_aura", "Buff Aura Duration", TabCategory.Lab, "Steed", "Extends duration of buff auras.", () => BuffAuraDuration, v => BuffAuraDuration = v, 1.0f, 10.0f));

            // HARD
            list.Add(F("no_crown_stealing", "No Crown Stealing", TabCategory.Hard, "Wave Control", "Greed units cannot steal the crown.", () => NoCrownStealing, v => NoCrownStealing = v, () => DifficultyRules.IsHardModeActive(), () => "Hard Mode"));

            list.Add(Slider("wave_size", "Wave Size", TabCategory.Hard, "Wave Sliders", "Multiplies the number of enemies per wave.", () => WaveSizeMult, v => WaveSizeMult = v, 0.1f, 5.0f));
            list.Add(Slider("enemy_speed", "Enemy Speed", TabCategory.Hard, "Wave Sliders", "Multiplies Greed unit movement speed.", () => EnemySpeedMult, v => EnemySpeedMult = v, 0.5f, 3.0f));
            list.Add(Slider("portal_rate", "Portal Spawn Rate", TabCategory.Hard, "Wave Sliders", "Multiplies portal spawn rate.", () => PortalSpawnRate, v => PortalSpawnRate = v, 0.1f, 5.0f));
            list.Add(Slider("queen_hp", "Greed Queen HP", TabCategory.Hard, "Wave Sliders", "Multiplies the Greed Queen's max health.", () => GreedQueenHPScale, v => GreedQueenHPScale = v, 0.5f, 5.0f));
            list.Add(Slider("threat", "Director Threat", TabCategory.Hard, "Wave Sliders", "Multiplies the Director's threat scaling.", () => DirectorThreatMult, v => DirectorThreatMult = v, 0.1f, 5.0f));

            _features = list.ToArray();
        }
        
        #endregion

        #region UI RENDERING
        private void DrawWindow(int id)
        {
            GUILayout.BeginVertical();

            // HEADER
            GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Height(HEADER_H));
            GUILayout.BeginVertical();
            GUILayout.Space(8);
            GUILayout.Label("Kingdom Enhanced", _styleTitle);
            GUILayout.Label(MOD_VERSION, _styleSubtitle);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            // BODY ROOT
            GUILayout.BeginHorizontal();

            // SIDEBAR
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

            // MAIN CONTENT TABS
            GUILayout.BeginVertical();
            _scrollPos[(int)_activeTab] = GUILayout.BeginScrollView(_scrollPos[(int)_activeTab]);
            DrawCurrentTab();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            // BODY ROOT END
            GUILayout.EndHorizontal();

            // DRAG / FOOTER
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

            // Draw dynamic features
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
            
            // Draw Static/Custom Tabs
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
            
            // Label
            GUILayout.Label(f.Label, _styleBodyText, GUILayout.Width(180));
            
            // Lock check
            bool isLocked = f.IsLocked != null && f.IsLocked();
            
            if (isLocked)
            {
                string reason = f.GetLockReason != null ? f.GetLockReason() : "Locked";
                GUILayout.Label($"<i>[{reason}]</i>", _styleLocked, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
                return;
            }

            // Conflict Warning
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

            // Draw Controls
            if (f.GetFloatValue != null)
            {
                // Slider
                float val = f.GetFloatValue();
                GUILayout.Label($"{val:F1}", _styleBodyText, GUILayout.Width(40));
                float newVal = GUILayout.HorizontalSlider(val, f.MinVal, f.MaxVal, GUILayout.ExpandWidth(true));
                if (Math.Abs(newVal - val) > 0.01f) f.SetFloatValue(newVal);
            }
            else if (f.OnAction != null)
            {
                // Action Button
                if (GUILayout.Button("Apply", _styleBtn, GUILayout.Width(100))) f.OnAction();
            }
            else if (f.GetValue != null)
            {
                // Toggle
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

            // Dynamic Guide Generation
            foreach (var f in _features)
            {
                GUILayout.Space(6);
                GUILayout.BeginHorizontal();
                GUILayout.Label($"<b>[{f.Category.ToString().ToUpper()}]</b> — {f.Label}", _styleTitle, GUILayout.Height(20));
                GUILayout.FlexibleSpace();
                
                // State/Value Display
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

            // Dynamic Report Generation
            foreach (var f in _features)
            {
                if (!_featureStatus.ContainsKey(f.Id)) _featureStatus[f.Id] = 0;
                int status = _featureStatus[f.Id];

                string statusLabel = status == 1 ? "<color=#44cc66>✓ Works</color>" : status == 2 ? "<color=#cc4444>✗ Broken</color>" : "<color=#888888>? Not Tested</color>";

                GUILayout.BeginHorizontal();
                
                // FIX: Safe substring to prevent crash on "Lab" or "Hard"
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
                    // Reset logic here
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
            // FIX: Draw from bottom left corner
            float y = Screen.height - 10; 
            
            // Iterate in reverse so newer messages appear lower (or higher depending on preference)
            // Standard: Iterate normally, but position mathematically, or iterate reverse.
            // Let's iterate reverse so index N (newest) is at the bottom.
            
            for (int i = _notifications.Count - 1; i >= 0; i--)
            {
                var n = _notifications[i];
                n.UpdateAlpha();
                if (n.Alpha <= 0) continue;

                GUI.color = new Color(n.TextColor.r, n.TextColor.g, n.TextColor.b, n.Alpha);
                
                y -= 26; // Move up for the next item
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

        // Overload 1: Used by internal ModMenu calls (text + color)
        public static void Speak(string text, Color color)
        {
            if (EnableTTS) TTSManager.Speak(text);
            _notifications.Add(new Notification(text, NotificationDuration, color));
        }

        // Overload 2: Used by external files (text only)
        public static void Speak(string text)
        {
            Speak(text, Color.white);
        }

        // Overload 3: Used by external files with interrupt flag
        public static void Speak(string text, bool interrupt)
        {
            // We call the base Speak logic. 
            // Note: If TTSManager requires the interrupt boolean, this might need adjustment, 
            // but TTSManager.Speak(string) is the safest known signature.
            if (EnableTTS) TTSManager.Speak(text);
            Speak(text, Color.white);
        }
        
        private void GiveCurrency(int amount, bool isGem)
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

        private void FillWallet()
        {
            var player = Managers.Inst?.kingdom?.GetPlayer(0);
            if (player == null || player.wallet == null) return;

            player.wallet.Coins = 100;
            Speak("Wallet Filled to Max!", C_GOLD);
        }

        private void CycleStaminaBarStyle()
        {
            if (StaminaBarHolder.Instance == null) return;
            StaminaBarHolder.Instance.visualStyle =
                (StaminaBarHolder.Instance.visualStyle + 1) % 4;
            Speak($"Style: {StaminaBarHolder.Instance.GetStyleName()}");
        }

        private void CycleStaminaBarPosition()
        {
            if (StaminaBarHolder.Instance == null) return;
            StaminaBarHolder.Instance.positionMode =
                (StaminaBarHolder.Instance.positionMode + 1) % 6;
            Speak($"Position: {StaminaBarHolder.Instance.GetPositionName()}");
        }

        #endregion
    }
}