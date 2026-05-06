using System;
using UnityEngine;
#if IL2CPP
using KingdomEnhanced.Shared.Attributes;
#endif

namespace KingdomEnhanced.Features
{
#if IL2CPP
    [RegisterTypeInIl2Cpp]
#endif
    public class UnitCacheRegistrar : MonoBehaviour
    {
#if IL2CPP
        public UnitCacheRegistrar(IntPtr ptr) : base(ptr) { }
#endif

        private Archer _archer;
        private Worker _worker;
        private Knight _knight;
        private Ninja _ninja;
        private Berserker _berserker;
        private Castle _castle;
        private BeggarCamp _beggarCamp;
        private Enemy _enemy;
        private Peasant _peasant;
        private Farmer _farmer;
        private Pikeman _pikeman;
        private Beggar _beggar;
        private Ballista _ballista;
        private Catapult _catapult;
        private Wall _wall;
        private Portal _portal;

        private void Awake()
        {
            _archer = GetComponent<Archer>();
            _worker = GetComponent<Worker>();
            _knight = GetComponent<Knight>();
            _ninja = GetComponent<Ninja>();
            _berserker = GetComponent<Berserker>();
            _castle = GetComponent<Castle>();
            _beggarCamp = GetComponent<BeggarCamp>();
            _enemy = GetComponent<Enemy>();
            _peasant = GetComponent<Peasant>();
            _farmer = GetComponent<Farmer>();
            _pikeman = GetComponent<Pikeman>();
            _beggar = GetComponent<Beggar>();
            _ballista = GetComponent<Ballista>();
            _catapult = GetComponent<Catapult>();
            _wall = GetComponent<Wall>();
            _portal = GetComponent<Portal>();
        }

        private void OnEnable()
        {
            if (_archer != null) UnitCacheManager.Archers.Add(_archer);
            if (_worker != null) UnitCacheManager.Workers.Add(_worker);
            if (_knight != null) UnitCacheManager.Knights.Add(_knight);
            if (_ninja != null) UnitCacheManager.Ninjas.Add(_ninja);
            if (_berserker != null) UnitCacheManager.Berserkers.Add(_berserker);
            if (_castle != null) UnitCacheManager.Castles.Add(_castle);
            if (_beggarCamp != null) UnitCacheManager.BeggarCamps.Add(_beggarCamp);
            if (_enemy != null) UnitCacheManager.Enemies.Add(_enemy);
            if (_peasant != null) UnitCacheManager.Peasants.Add(_peasant);
            if (_farmer != null) UnitCacheManager.Farmers.Add(_farmer);
            if (_pikeman != null) UnitCacheManager.Pikemen.Add(_pikeman);
            if (_beggar != null) UnitCacheManager.Beggars.Add(_beggar);
            if (_ballista != null) UnitCacheManager.Ballistas.Add(_ballista);
            if (_catapult != null) UnitCacheManager.Catapults.Add(_catapult);
            if (_wall != null) UnitCacheManager.Walls.Add(_wall);
            if (_portal != null) UnitCacheManager.Portals.Add(_portal);
        }

        private void OnDisable()
        {
            if (_archer != null) UnitCacheManager.Archers.Remove(_archer);
            if (_worker != null) UnitCacheManager.Workers.Remove(_worker);
            if (_knight != null) UnitCacheManager.Knights.Remove(_knight);
            if (_ninja != null) UnitCacheManager.Ninjas.Remove(_ninja);
            if (_berserker != null) UnitCacheManager.Berserkers.Remove(_berserker);
            if (_castle != null) UnitCacheManager.Castles.Remove(_castle);
            if (_beggarCamp != null) UnitCacheManager.BeggarCamps.Remove(_beggarCamp);
            if (_enemy != null) UnitCacheManager.Enemies.Remove(_enemy);
            if (_peasant != null) UnitCacheManager.Peasants.Remove(_peasant);
            if (_farmer != null) UnitCacheManager.Farmers.Remove(_farmer);
            if (_pikeman != null) UnitCacheManager.Pikemen.Remove(_pikeman);
            if (_beggar != null) UnitCacheManager.Beggars.Remove(_beggar);
            if (_ballista != null) UnitCacheManager.Ballistas.Remove(_ballista);
            if (_catapult != null) UnitCacheManager.Catapults.Remove(_catapult);
            if (_wall != null) UnitCacheManager.Walls.Remove(_wall);
            if (_portal != null) UnitCacheManager.Portals.Remove(_portal);
        }

        public static void EnsureAttached(GameObject obj)
        {
            if (obj != null && obj.GetComponent<UnitCacheRegistrar>() == null)
            {
                obj.AddComponent<UnitCacheRegistrar>();
            }
        }
    }
}
