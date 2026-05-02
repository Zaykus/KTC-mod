using System;
using System.Reflection;
using UnityEngine;
using KingdomEnhanced.Systems;
using KingdomEnhanced.UI;
using KingdomEnhanced.Utils;

namespace KingdomEnhanced.Features
{
    /// <summary>
    /// Accessibility reporting features. Handles F5-F10 hotkey status reports.
    /// </summary>
    public static class AccessibilityReportHandler
    {
        private static readonly System.Collections.Generic.Dictionary<Type, PropertyInfo> _priceCache = new();

        public static void CheckCompassAndSafety(Player player)
        {
            string dir = (player.mover.GetDirection() == Side.Left) ? "Left" : "Right";

            string threat = "Safe";
            string timeOfDay = "Day";
            string borderInfo = "";
            try
            {
                var enemyMgr = UnityEngine.Object.FindObjectOfType<EnemyManager>();
                if (enemyMgr != null && enemyMgr.IsDangerous)
                    threat = "DANGER";

                var kingdom = UnityEngine.Object.FindObjectOfType<Kingdom>();
                if (kingdom != null && !kingdom.isDaytime)
                    timeOfDay = "Night";

                var walls = UnityEngine.Object.FindObjectsOfType<Wall>();
                if (walls != null && walls.Length > 0)
                {
                    float min = float.MaxValue;
                    foreach (var w in walls)
                    {
                        float d = Mathf.Abs(w.transform.position.x - player.transform.position.x);
                        if (d < min) min = d;
                    }
                    if (min < float.MaxValue) borderInfo = $", {Mathf.RoundToInt(min)}m to Wall";
                }
            }
            catch { }

            ModMenu.Speak($"Facing {dir}, {timeOfDay}, {threat}{borderInfo}");
        }

        public static void ReportWallet(Player player)
        {
            int coins = player.wallet.GetCurrency(CurrencyType.Coins);
            int gems = player.wallet.GetCurrency(CurrencyType.Gems);
            ModMenu.Speak($"{coins} Gold, {gems} Gems");
        }

        public static void ReportWorld()
        {
            var d = Managers.Inst.director;
            string time = d.IsDaytime ? "Day" : "Night";
            ModMenu.Speak($"Day {d.CurrentIslandDays}, {time}");
        }

        public static void ReportMount(Player player)
        {
            if (player.steed == null) return;
            string n = PayableNameResolver.CleanName(player.steed.name);
            string status = player.steed.IsTired ? "Tired" : "Ready";
            ModMenu.Speak($"{n}, {status}");
        }

        public static void ReportCompanions()
        {
            int archers = UnityEngine.Object.FindObjectsOfType<Archer>().Length;
            int workers = UnityEngine.Object.FindObjectsOfType<Worker>().Length;
            int peasants = UnityEngine.Object.FindObjectsOfType<Peasant>().Length;
            int knights = UnityEngine.Object.FindObjectsOfType<Knight>().Length;

            ModMenu.Speak($"{archers} Archers, {workers} Workers, {peasants} Peasants, {knights} Knights");
        }

        public static void ReportDetailedInfo(Player player)
        {
            var current = player.selectedPayable as MonoBehaviour ?? GetClosestPayable(player);
            if (current == null)
            {
                ModMenu.Speak("No object selected.");
                return;
            }

            string name = PayableNameResolver.CleanName(current.name);
            string currency = GetCurrencyName(current);
            int price = 0;

            var currentType = current.GetType();
            if (!_priceCache.TryGetValue(currentType, out var priceProp))
            {
                priceProp = currentType.GetProperty("Price");
                _priceCache[currentType] = priceProp;
            }
            if (priceProp != null) price = (int)priceProp.GetValue(current, null);

            string levelInfo = "";
            var wall = current.GetComponent<Wall>();
            if (wall != null) levelInfo = $", Level {wall.level}";

            var castle = current.GetComponent<Castle>();
            if (castle != null) levelInfo = $", Level {(int)castle.level}";

            var tower = current.GetComponent<Tower>();
            if (tower != null)
            {
                name = "Watchtower";
                levelInfo = $", Level {tower.level}";
            }

            ModMenu.Speak($"{name}, {price} {currency}{levelInfo}");
        }

        public static void DumpPayableInfo(Player player)
        {
            var current = player.selectedPayable as MonoBehaviour ?? GetClosestPayable(player);

            if (current == null)
            {
                ModMenu.Speak("No object found to inspect.");
                return;
            }

            ModMenu.Speak($"Inspecting: {current.name}");
            Debug.Log($"[DEBUG] Inspecting {current.name} ({current.GetType().Name})");

            var fields = current.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var f in fields)
            {
                Debug.Log($"   Field: {f.Name} = {f.GetValue(current)}");
            }

            var props = current.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var p in props)
            {
                try
                {
                    Debug.Log($"   Property: {p.Name} = {p.GetValue(current, null)}");
                }
                catch { }
            }

            ModMenu.Speak("Dumped fields to log file.");
        }

        private static MonoBehaviour GetClosestPayable(Player player)
        {
            if (Managers.Inst == null || Managers.Inst.payables == null) return null;

            float searchRange = 18.0f;
            float playerX = player.transform.position.x;

            MonoBehaviour closest = null;
            float closestDist = float.MaxValue;

            foreach (var p in Managers.Inst.payables.AllPayables)
            {
                if (p == null) continue;
                var mb = p as MonoBehaviour;
                if (mb == null || !mb.gameObject.activeInHierarchy) continue;

                if (player.steed != null && mb.gameObject == player.steed.gameObject) continue;

                float dist = Mathf.Abs(mb.transform.position.x - playerX);
                if (dist < searchRange && dist < closestDist)
                {
                    closest = mb;
                    closestDist = dist;
                }
            }
            return closest;
        }

        private static string GetCurrencyName(MonoBehaviour target)
        {
            if (target.name.Contains("Gem Guard") || target.name.Contains("GemKeeper")) return "gems";

            try
            {
                var type = target.GetType();
                var fieldNames = new[] { "currency", "priceType", "coinType", "paymentType" };

                foreach (var fieldName in fieldNames)
                {
                    FieldInfo field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        object value = field.GetValue(target);
                        if (value != null)
                        {
                            string sVal = value.ToString().ToLower();
                            if (sVal.Contains("gem")) return "gems";
                            if (sVal.Contains("coin") || sVal.Contains("gold")) return "coins";
                        }
                    }

                    PropertyInfo prop = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (prop != null)
                    {
                        object value = prop.GetValue(target, null);
                        if (value != null)
                        {
                            string sVal = value.ToString().ToLower();
                            if (sVal.Contains("gem")) return "gems";
                            if (sVal.Contains("coin") || sVal.Contains("gold")) return "coins";
                        }
                    }
                }
            }
            catch { }

            return "coins";
        }
    }
}
