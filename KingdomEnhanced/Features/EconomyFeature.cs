using UnityEngine;
using KingdomEnhanced.Core;
using Coatsink.Common;

namespace KingdomEnhanced.Features
{
    public class EconomyFeature : MonoBehaviour
    {
        private Player _player;

        void Start()
        {
            _player = GetComponent<Player>();
        }

        void Update()
        {
            if (_player == null) return;

            // PRESS F2
            if (Input.GetKeyDown(KeyCode.F2))
            {
                if (_player.wallet != null)
                {
                    // FIX: Don't fill to capacity (too messy). 
                    // Just give 40 coins. That's a full bag visually.
                    int safeAmount = 40;

                    _player.wallet.SetCurrency(CurrencyType.Coins, safeAmount);

                    Plugin.Instance.Log.LogInfo("Cheat: Wallet set to Safe Full (40 coins).");
                }
            }
        }
    }
}