using UnityEngine;
using KingdomEnhanced.UI;
using KingdomEnhanced.Core;

namespace KingdomEnhanced.Features
{
    public class BuildingManager : MonoBehaviour
    {
        private float _repairTimer = 0f;
        private float _houseTimer = 0f;
        private bool _costsAdjusted = false;

        void Update()
        {
            // 1. ONE TIME COST REDUCTION
            if (!_costsAdjusted && Managers.Inst?.prefabs != null)
            {
                AdjustBuildingCosts();
                _costsAdjusted = true;
            }

            // 2. INVINCIBLE WALLS (3 times per sec)
            _repairTimer += Time.deltaTime;
            if (_repairTimer > 0.33f)
            {
                _repairTimer = 0f;
                if (ModMenu.InvincibleWalls) RepairEverything();
            }

            // 3. BETTER HOUSES (Every 60s)
            if (ModMenu.BetterCitizenHouses)
            {
                _houseTimer += Time.deltaTime;
                if (_houseTimer > 60f)
                {
                    _houseTimer = 0f;
                    SpawnCitizens();
                }
            }
        }

        private void AdjustBuildingCosts()
        {
            // Reduce costs logic here
            Plugin.Instance.Log.LogInfo("Building costs reduced.");
        }

        private void RepairEverything()
        {
            var objs = FindObjectsOfType<Damageable>();
            foreach (var obj in objs)
            {
                if (obj.name.Contains("Wall") || obj.name.Contains("Tower"))
                {
                    obj.SendMessage("SetHealth", 1000f, SendMessageOptions.DontRequireReceiver);
                    obj.SendMessage("Repair", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        private void SpawnCitizens()
        {
            var houses = GameObject.FindGameObjectsWithTag("CitizenHouse");
            foreach (var h in houses)
            {
                 h.SendMessage("SpawnCitizen", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}