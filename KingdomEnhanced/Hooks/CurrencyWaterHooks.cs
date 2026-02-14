using HarmonyLib;
using KingdomEnhanced.UI;
using UnityEngine;

namespace KingdomEnhanced.Hooks
{
    /// <summary>
    /// Prevents coins from sinking when CoinsStayDry is enabled.
    /// Uses Update loop approach instead of Collider2D trigger patching
    /// to avoid IL2CPP interop issues with physics callbacks.
    /// </summary>
    public class CoinBuoyancy : MonoBehaviour
    {
        private static float _checkTimer;
        
        void Update()
        {
            if (!ModMenu.CoinsStayDry) return;
            
            _checkTimer += Time.deltaTime;
            if (_checkTimer < 0.5f) return;
            _checkTimer = 0f;
            
            // Find coins that have fallen below water level
            var coins = FindObjectsOfType<DroppableCurrency>();
            foreach (var coin in coins)
            {
                if (coin == null || !coin.gameObject.activeInHierarchy) continue;
                // If coin Y position is below water line (typically -0.5f in KTC)
                if (coin.transform.position.y < -0.5f)
                {
                    var pos = coin.transform.position;
                    pos.y = 0.2f; // Place above water
                    coin.transform.position = pos;
                }
            }
        }
    }
}
