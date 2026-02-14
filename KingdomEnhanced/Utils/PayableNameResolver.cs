using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using KingdomEnhanced.UI; // For ModMenu reference if needed

namespace KingdomEnhanced.Utils
{
    public static class PayableNameResolver
    {
        // v1.2.1: Comprehensive Object Mapping
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
            { "Hermes Shade", "Hermes Statue" },
            { "Statue Pike", "Pikeman Statue" },
            { "Tree Cypress", "Tree" },
            { "Tree Pine", "Tree" },
            { "Tree Wild Pear", "Tree" },
            { "Tree Olive", "Tree" },
            { "BBB", "Banner" },
            { "Shop Scythe", "Farmer Shop" },
            // Units
            { "Beggar", "Vagrant" },
            { "Villager", "Citizen" },
            { "Ronin", "Ronin" },
            { "Hoplite", "Hoplite" },
            { "Slinger", "Slinger" },
            // Mounts
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
            // Buildings
            { "Quarry", "Stone Quarry" },
            { "Mine", "Iron Mine" },
            { "Dojo", "Dojo" },
            { "Ballista", "Ballista Tower" },
            { "Bakery", "Bakery" },
            { "Stable", "Stable" },
            { "Horn", "Horn Wall" },
            { "Lighthouse", "Lighthouse" },
            { "Citizen House", "Citizen House" },
            { "Forge", "Forge" }
        };

        // Cached Regex patterns
        private static readonly Regex _digitDashRegex = new Regex(@"[\d-]", RegexOptions.Compiled);
        private static readonly Regex _trailingUpperRegex = new Regex(@"\s[A-Z]$", RegexOptions.Compiled);
        private static readonly Regex _pNumberRegex = new Regex(@"\sP\d+", RegexOptions.Compiled);
        private static readonly Regex _camelCaseRegex = new Regex("([a-z])([A-Z])", RegexOptions.Compiled);
        private static readonly Regex _multiSpaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex _parenRegex = new Regex(@"\s*\(.*?\)", RegexOptions.Compiled);
        private static readonly Regex _biomeRegex = new Regex(
            @"(?i)\b(bamboo|iron|stone|dead|lands|scaffold|wreck|grove|grace|pin|sale|jade|norse|shogun|dire|plague|europe|greece|cypress|pine|olive|wild|pear|p2|olympus|dynasty|viking|challenge)\b", RegexOptions.Compiled);

        public static string CleanName(string original)
        {
            if (string.IsNullOrEmpty(original)) return "";

            // 0. Pre-Trim
            string s = original.Trim();

            // 1. Basic Unity Cleanup
            s = s.Replace("(Clone)", "").Replace("_", " ");

            // 1.5: Remove parenthesized content
            s = _parenRegex.Replace(s, "");
            
            // 1.6: Remove P-Numbers
            s = _pNumberRegex.Replace(s, "");

            // 2. Remove Internal IDs/Numbers
            s = _digitDashRegex.Replace(s, "");

            // 3. Strict Mode: Remove single trailing Uppercase letters
            s = _trailingUpperRegex.Replace(s, "");

            // 4. Split CamelCase
            s = _camelCaseRegex.Replace(s, "$1 $2");

            // v1.2.1: Check explicit mapping first
            s = _multiSpaceRegex.Replace(s, " ").Trim();
            
            if (_nameMapping.ContainsKey(s)) return _nameMapping[s];

            // v1.5: Simplify Names Toggle
            if (ModMenu.SimplifyNames)
            {
                s = _biomeRegex.Replace(s, "");
                s = _multiSpaceRegex.Replace(s, " ");
            }

            return s.Trim();
        }
        
        public static string GetShopTypeName(RaycastHit hit) /* Placeholder signature - adapting from original logic relying on PayableShop.ShopType enum which isn't available here without reference. 
                                                              * Actually, let's keep it simple and handle enum mapping if possible or just string based.
                                                              * The original used PayableShop.ShopType enum. We need to respect that dependency or replicate it.
                                                              * Since PayableShop is game code, we might not have it in Utils namespace easily if not imported? 
                                                              * Wait, PayloadShop is likely in Assembly-CSharp.
                                                              */
        {
             return "";
        }
        
        // We'll use a dynamic or object approach if we want to decouple, but for now let's assume valid access or move the method that needs ShopType to here but it needs the enum.
        // Let's just create the method that takes the enum if we can resolve it, otherwise we'll keep it simple.
        
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
