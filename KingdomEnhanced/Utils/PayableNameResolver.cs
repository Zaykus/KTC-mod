using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using KingdomEnhanced.UI; 

namespace KingdomEnhanced.Utils
{
    public static class PayableNameResolver
    {
        private static readonly Dictionary<string, string> _nameMapping = new Dictionary<string, string>
        {
            { "P1", "Peasant" },
            { "P2", "Worker" }, 
            { "Griffin", "Griffin Mount" },
            { "Stag", "Stag Mount" },
            { "Warhorse", "Warhorse Mount" },
            { "Unicorn", "Unicorn Mount" },
            { "Lizard", "Lizard Mount" },
            { "Bear", "Bear Mount" },
            { "Beetle", "Beetle Mount" },
            { "Scaffold", "Construction" },
            { "Boat Sail Position", "Boat" },
            { "Boat Sale Position", "Boat" },
            { "Border", "Portal" },
            { "Tower B", "Tower" },
            { "Tower A", "Tower" },
            { "Tower C", "Tower" },
            { "Tree Pin", "Tree" },
            { "Shop Hammer", "Builder Shop" },
            { "Shop Bow", "Archer Shop" },
            { "Shop Hammer Deadlands", "Builder Shop" },
            { "Shop Bow Deadlands", "Archer Shop" },
            { "Shop Hammer Dead Lands", "Builder Shop" },
            { "Shop Bow Dead Lands", "Archer Shop" },
            { "Tree Deadlands", "Dead Tree" },
            { "Tree Noleaves Deadlands", "Bare Dead Tree" },
            { "Tree Dead Lands", "Dead Tree" },
            { "Tree Noleaves Dead Lands", "Bare Dead Tree" },
            { "Castle Deadlands", "Castle" },
            { "Castle Dead Lands", "Castle" },
            { "Hermes Shade", "Hermes Statue" },
            { "Statue Pike", "Pikeman Statue" },
            { "Tree Cypress", "Tree" },
            { "Tree Pine", "Tree" },
            { "Tree Wild Pear", "Tree" },
            { "Tree Olive", "Tree" },
            { "Statue Archer", "Archer Statue" },
            { "Statue Builder", "Builder Statue" },
            { "Statue Farmer", "Farmer Statue" },
            { "Statue Knight", "Knight Statue" },
            { "BBB", "Banner" },
            { "Shop Scythe", "Farmer Shop" },
            { "Beggar", "Vagrant" },
            { "Villager", "Citizen" },
            { "Ronin", "Ronin" },
            { "Hoplite", "Hoplite" },
            { "Slinger", "Slinger" },
            { "Gamigin", "Gamigin Mount" },
            { "Gined", "Gined Mount" },
            { "Wolf", "Fenrir Mount" },
            { "Reindeer", "Reindeer Mount" },
            { "Sleipnir", "Sleipnir Mount" },
            { "Cat Chariot", "Cat Chariot" },
            { "Kelpie", "Kelpie Mount" },
            { "Hippocampus", "Hippocampus Mount" },
            { "Cerberus", "Cerberus Mount" },
            { "Pegasus", "Pegasus Mount" },
            { "Donkey", "Donkey Mount" },
            { "Quarry", "Stone Quarry" },
            { "Mine", "Iron Mine" },
            { "Dojo", "Dojo" },
            { "Ballista", "Ballista Tower" },
            { "Bakery", "Bakery" },
            { "Stable", "Stable" },
            { "Horn", "Horn Wall" },
            { "Lighthouse", "Lighthouse" },
            { "Lighthouse undeveloped", "Lighthouse" },
            { "Citizen House", "Citizen House" },
            { "Forge", "Forge" }
        };

        private static readonly Regex _digitDashRegex = new Regex(@"[\d-]", RegexOptions.Compiled);
        private static readonly Regex _trailingUpperRegex = new Regex(@"\s[A-Z]$", RegexOptions.Compiled);
        private static readonly Regex _pNumberRegex = new Regex(@"\sP\d+", RegexOptions.Compiled);
        private static readonly Regex _camelCaseRegex = new Regex("([a-z])([A-Z])", RegexOptions.Compiled);
        private static readonly Regex _multiSpaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex _parenRegex = new Regex(@"\s*\(.*?\)", RegexOptions.Compiled);
        private static readonly Regex _biomeRegex = new Regex(
            @"(?i)\b(bamboo|iron|stone|dead|lands|scaffold|wreck|grove|grace|pin|sale|jade|norse|norselands|shogun|dire|plague|europe|greece|cypress|pine|olive|wild|pear|p2|olympus|dynasty|viking|challenge|hickory|oak|birch|apple|cherry|palm|spruce|fir|willow|maple|walnut|chestnut)\b", RegexOptions.Compiled);

        public static string CleanName(string original)
        {
            if (string.IsNullOrEmpty(original)) return "";

            string s = original.Trim();

            s = s.Replace("(Clone)", "").Replace("_", " ");

            s = _parenRegex.Replace(s, "");
            s = _pNumberRegex.Replace(s, "");
            s = _digitDashRegex.Replace(s, "");
            s = _trailingUpperRegex.Replace(s, "");
            s = _camelCaseRegex.Replace(s, "$1 $2");
            s = _multiSpaceRegex.Replace(s, " ").Trim();

            if (_nameMapping.TryGetValue(s, out string mapped)) return mapped;

            if (ModMenu.SimplifyNames)
            {
                s = _biomeRegex.Replace(s, "");
                s = _multiSpaceRegex.Replace(s, " ").Trim();
            }

            if (_nameMapping.TryGetValue(s, out string mappedAfterStrip)) return mappedAfterStrip;

            return s;
        }
        
        public static string GetShopTypeName(RaycastHit hit) 
        {
             return "";
        }
        
        public static string GetShopTypeName(PayableShop.ShopType type)
        {
            switch(type)
            {
                case PayableShop.ShopType.Bow: return "Archer Shop";
                case PayableShop.ShopType.Hammer: return "Builder Shop";
                case PayableShop.ShopType.Scythe: return "Farmer Shop";
                case PayableShop.ShopType.PikeLeft: 
                case PayableShop.ShopType.PikeRight: return "Pikeman Shop";
                case PayableShop.ShopType.ShieldShopLeft:
                case PayableShop.ShopType.ShieldShopRight: return "Shield Shop";
                case PayableShop.ShopType.Forge: return "Forge";
                case PayableShop.ShopType.NinjaLeft:
                case PayableShop.ShopType.NinjaRight: return "Ninja House";
                case PayableShop.ShopType.WorkshopLeft:
                case PayableShop.ShopType.WorkshopRight: return "Catapult Workshop";
                default: return "";
            }
        }
    }
}
