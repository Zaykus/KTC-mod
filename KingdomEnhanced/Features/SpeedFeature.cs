using UnityEngine;

namespace KingdomEnhanced.Features
{
    public class SpeedFeature : MonoBehaviour
    {
        public float SpeedMultiplier = 2.0f;
        private Player _player;
        private float _defaultSpeed = -1f;

        void Start() => _player = GetComponent<Player>();

        void Update()
        {
            if (_player == null || _player.steed == null) return;
            if (_defaultSpeed == -1f) _defaultSpeed = _player.steed.runSpeed;

            // Apply Speed when Shift is held
            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            _player.steed.runSpeed = sprint ? _defaultSpeed * SpeedMultiplier : _defaultSpeed;
        }
    }
}