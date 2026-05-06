using System.Collections.Generic;
using UnityEngine;

namespace KingdomEnhanced.Features
{
    public static class UnitCacheManager
    {
        public static HashSet<Archer> Archers = new HashSet<Archer>();
        public static HashSet<Worker> Workers = new HashSet<Worker>();
        public static HashSet<Knight> Knights = new HashSet<Knight>();
        public static HashSet<Ninja> Ninjas = new HashSet<Ninja>();
        public static HashSet<Berserker> Berserkers = new HashSet<Berserker>();
        public static HashSet<Castle> Castles = new HashSet<Castle>();
        public static HashSet<BeggarCamp> BeggarCamps = new HashSet<BeggarCamp>();
        public static HashSet<Enemy> Enemies = new HashSet<Enemy>();
        public static HashSet<Peasant> Peasants = new HashSet<Peasant>();
        public static HashSet<Farmer> Farmers = new HashSet<Farmer>();
        public static HashSet<Pikeman> Pikemen = new HashSet<Pikeman>();
        public static HashSet<Beggar> Beggars = new HashSet<Beggar>();
        public static HashSet<Ballista> Ballistas = new HashSet<Ballista>();
        public static HashSet<Catapult> Catapults = new HashSet<Catapult>();
        public static HashSet<Wall> Walls = new HashSet<Wall>();
        public static HashSet<Portal> Portals = new HashSet<Portal>();

        private static bool _coldBootDone = false;

        public static void CheckColdBoot()
        {
            if (_coldBootDone) return;
            _coldBootDone = true;

            Archers = new HashSet<Archer>(Object.FindObjectsByType<Archer>(FindObjectsSortMode.None));
            Workers = new HashSet<Worker>(Object.FindObjectsByType<Worker>(FindObjectsSortMode.None));
            Knights = new HashSet<Knight>(Object.FindObjectsByType<Knight>(FindObjectsSortMode.None));
            Ninjas = new HashSet<Ninja>(Object.FindObjectsByType<Ninja>(FindObjectsSortMode.None));
            Berserkers = new HashSet<Berserker>(Object.FindObjectsByType<Berserker>(FindObjectsSortMode.None));
            Castles = new HashSet<Castle>(Object.FindObjectsByType<Castle>(FindObjectsSortMode.None));
            BeggarCamps = new HashSet<BeggarCamp>(Object.FindObjectsByType<BeggarCamp>(FindObjectsSortMode.None));
            Enemies = new HashSet<Enemy>(Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None));
            Peasants = new HashSet<Peasant>(Object.FindObjectsByType<Peasant>(FindObjectsSortMode.None));
            Farmers = new HashSet<Farmer>(Object.FindObjectsByType<Farmer>(FindObjectsSortMode.None));
            Pikemen = new HashSet<Pikeman>(Object.FindObjectsByType<Pikeman>(FindObjectsSortMode.None));
            Beggars = new HashSet<Beggar>(Object.FindObjectsByType<Beggar>(FindObjectsSortMode.None));
            Ballistas = new HashSet<Ballista>(Object.FindObjectsByType<Ballista>(FindObjectsSortMode.None));
            Catapults = new HashSet<Catapult>(Object.FindObjectsByType<Catapult>(FindObjectsSortMode.None));
            Walls = new HashSet<Wall>(Object.FindObjectsByType<Wall>(FindObjectsSortMode.None));
            Portals = new HashSet<Portal>(Object.FindObjectsByType<Portal>(FindObjectsSortMode.None));
            
            KingdomEnhanced.Core.Plugin.Instance.LogSource.LogInfo("[UnitCacheManager] Cold boot complete. Cached active entities.");
        }
    }
}
