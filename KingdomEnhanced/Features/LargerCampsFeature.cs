using UnityEngine;
using KingdomEnhanced.UI;
using KingdomEnhanced.Core;

namespace KingdomEnhanced.Features
{
    public class LargerCampsFeature : MonoBehaviour
    {
        private float _timer = 0f;

        void Update()
        {
            // Only run if the toggle in ModMenu is ON
            if (!ModMenu.LargerCamps) return;

            _timer += Time.deltaTime;

            // Apply settings every 5 seconds to catch new or reset camps
            if (_timer >= 5f)
            {
                var camps = Object.FindObjectsOfType<BeggarCamp>();
                if (camps != null)
                {
                    foreach (var camp in camps)
                    {
                        if (camp == null) continue;

                        try
                        {
                            // Direct field modification (Values matched to your DLLs)
                            camp.maxBeggars = 10;
                            camp.spawnInterval = 60f;

                            // Optional: Log for verification
                            // Plugin.Instance?.Log.LogDebug($"Updated camp: {camp.name}");
                        }
                        catch (System.Exception ex)
                        {
                            Plugin.Instance?.Log.LogWarning($"Error updating camp: {ex.Message}");
                        }
                    }
                }
                _timer = 0f;
            }
        }
    }
}