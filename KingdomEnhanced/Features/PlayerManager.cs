using System;
using UnityEngine;
using KingdomEnhanced.UI;
#if IL2CPP
using KingdomEnhanced.Shared.Attributes;
#endif

namespace KingdomEnhanced.Features
{
#if IL2CPP
    [RegisterTypeInIl2Cpp]
#endif
    public class PlayerManager : MonoBehaviour
    {
#if IL2CPP
        public PlayerManager(IntPtr ptr) : base(ptr) { }
#endif
        private Player _player;
        private float _defaultSpeed = -1f;

        void Start() => _player = GetComponent<Player>();

        void Update()
        {
            if (_player == null) return;

            if (_player.steed != null && _defaultSpeed == -1f)
                _defaultSpeed = _player.steed.runSpeed;

            if (ModMenu.EnableSizeHack)
            {
                float dir = 1f;
                if (_player.mover != null && _player.mover.GetDirection() == Side.Left) dir = -1f;
                _player.transform.localScale = new Vector3(ModMenu.TargetSize * dir, ModMenu.TargetSize, 1f);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                if (!DifficultyRules.CanAddCoins())
                {
                    ModMenu.Speak($"<color=red>🔒 Wallet Refill locked in {HardModeFeature.GetActivePreset()}</color>");
                }
                else if (_player.wallet != null)
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
                var s = _player.steed;
                s.Stamina = 100f;
                if (s.IsTired) s._tiredTimer = -1f;
            }
        }

        public static void ForceBankerPayout()
        {
            var banker = FindFirstObjectByType<Banker>();
            if (banker != null)
            {
                banker.SendMessage("AddCoins", 100, SendMessageOptions.DontRequireReceiver);
                banker.SendMessage("DropCoins", SendMessageOptions.DontRequireReceiver);
                ModMenu.Speak("Banker Payout Triggered!");
            }
            else ModMenu.Speak("Banker not found.");
        }

        void OnDestroy()
        {
            _defaultSpeed = -1f;
        }
    }
}