using UnityEngine;
using KingdomEnhanced.UI;
using KingdomEnhanced.Core;
using System.Reflection;

namespace KingdomEnhanced.Features
{
    public class PlayerManager : MonoBehaviour
    {
        private Player _player;
        private float _defaultSpeed = -1f;

        void Start() => _player = GetComponent<Player>();

        void Update()
        {
            if (_player == null) return;

            // 1. SPEED CHEAT
            if (_player.steed != null)
            {
                if (_defaultSpeed == -1f) _defaultSpeed = _player.steed.runSpeed;
                
                float mult = ModMenu.SpeedMultiplier;
                bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                
                if (mult > 0.1f)
                    _player.steed.runSpeed = sprint ? _defaultSpeed * mult : _defaultSpeed;
            }

            // 2. SIZE CHEAT
            if (ModMenu.EnableSizeHack)
            {
                float dir = 1f;
                if (_player.mover != null && _player.mover.GetDirection() == Side.Left) dir = -1f;
                _player.transform.localScale = new Vector3(ModMenu.TargetSize * dir, ModMenu.TargetSize, 1f);
            }

            // 3. ECONOMY HOTKEYS
            if (Input.GetKeyDown(KeyCode.F2))
            {
                if (_player.wallet != null)
                {
                    _player.wallet.SetCurrency(CurrencyType.Coins, 50);
                    ModMenu.Speak("Cheat: Wallet Refilled.");
                }
            }
        }

        void LateUpdate()
        {
            if (ModMenu.InfiniteStamina && _player != null && _player.steed != null)
            {
                _player.steed.Stamina = 100f;
                _player.steed._tiredTimer = 0f;
            }
        }

        public static void ForceBankerPayout()
        {
            var banker = FindObjectOfType<Banker>();
            if (banker != null)
            {
                banker.SendMessage("AddCoins", 100, SendMessageOptions.DontRequireReceiver);
                banker.SendMessage("DropCoins", SendMessageOptions.DontRequireReceiver);
                ModMenu.Speak("Banker Payout Triggered!");
            }
            else ModMenu.Speak("Banker not found.");
        }
    }
}