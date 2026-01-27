using UnityEngine;

namespace KingdomEnhanced.Features
{
    public class StaminaFeature : MonoBehaviour
    {
        public static bool InfiniteStamina = true;
        private Player _player;

        void Start() => _player = GetComponent<Player>();

        // We change 'Update' to 'LateUpdate'
        // This runs AFTER the game tries to drain stamina, overriding it perfectly.
        void LateUpdate()
        {
            if (!InfiniteStamina || _player == null || _player.steed == null) return;

            // Force stamina to be exactly 100 (Full)
            // This stops the bar from flickering down and up
            _player.steed.Stamina = 100f;

            // Also reset the "Tired" timer so the horse never breathes heavily
            _player.steed._tiredTimer = 0f;
        }
    }
}