using UnityEngine;

namespace KingdomEnhanced.Features
{
    public class PlayerSizeFeature : MonoBehaviour
    {
        public static bool EnableSizeHack = false;
        public static float TargetSize = 1.0f;

        private Player _player;
        private float _lastSize = 1.0f;

        void Start() => _player = GetComponent<Player>();

        void Update()
        {
            if (_player == null) return;

            // If the feature is ON, force the size
            if (EnableSizeHack)
            {
                // Smoothly scale to target size
                if (Mathf.Abs(_lastSize - TargetSize) > 0.01f)
                {
                    _player.transform.localScale = new Vector3(TargetSize, TargetSize, 1f);
                    _lastSize = TargetSize;
                }
            }
            else if (_lastSize != 1.0f)
            {
                // Reset to normal if turned off
                _player.transform.localScale = Vector3.one;
                _lastSize = 1.0f;
            }
        }
    }
}