using UnityEngine;
using KingdomEnhanced.UI;
using System;

namespace KingdomEnhanced.Features
{
    public class BetterCitizenHouseFeature : MonoBehaviour
    {
        private float _spawnTimer = 0f;
        private float _spawnTime = 60f;
        private int _populationLimit = 10;

        void Start() { }

        void Update()
        {
            if (!ModMenu.BetterCitizenHouses) return;

            _spawnTimer += Time.deltaTime;
            if (_spawnTimer < _spawnTime) return;
            _spawnTimer = 0f;

            SpawnCitizensAtHouses();
        }

        private void SpawnCitizensAtHouses()
        {
            try
            {
                var houses = GameObject.FindGameObjectsWithTag("CitizenHouse");
                if (houses == null || houses.Length == 0) return;

                int currentPeasants = GetUnemployedPeasantCount();
                if (_populationLimit != -1 && currentPeasants >= _populationLimit)
                {
                    return;
                }

                foreach (var house in houses)
                {
                    if (house != null)
                    {
                        house.SendMessage("SpawnCitizen", SendMessageOptions.DontRequireReceiver);
                        house.SendMessage("SpawnPeasant", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            catch (Exception ex)
            {
                KingdomEnhanced.Core.Plugin.Instance?.Log.LogWarning($"BetterCitizenHouseFeature error: {ex.Message}");
            }
        }

        private int GetUnemployedPeasantCount()
        {
            try
            {
                var peasants = UnityEngine.Object.FindObjectsOfType<Peasant>();
                return peasants != null ? peasants.Length : 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}