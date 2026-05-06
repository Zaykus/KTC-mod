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
    public class ModData : MonoBehaviour
    {
#if IL2CPP
        public ModData(IntPtr ptr) : base(ptr) { }
#endif

        // Archer
        public float baseFireRate;
        
        // Berserker, Ninja
        public float baseSpeed;
        
        // Farm
        public float baseCoinYield;
        
        // Bolt
        public int baseDamage;
        public float baseForce;
        
        // Portal
        public float baseSpawnInterval;
        
        // Mover
        public float moverBaseSpeed;
        
        // ArtemisBow
        public int artemisBaseArrows;
        public float artemisBaseRange;
        public int artemisBaseDamage;
        
        // Knight
        public int knightBaseHp;
        
        // Worker
        public float workerBaseSpeed;
        public float workerBaseWorkTime;

        // Catapult
        public float baseCrankRate;
        public float baseCrankRateFormation;

        // Ballista
        public float ballistaFractionalWork;

        public bool isInitialized = false;

        public static ModData GetOrAdd(GameObject obj)
        {
            var data = obj.GetComponent<ModData>();
            if (data == null)
            {
                data = obj.AddComponent<ModData>();
            }
            return data;
        }
    }
}
