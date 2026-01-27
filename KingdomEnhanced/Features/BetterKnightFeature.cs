using UnityEngine;
using KingdomEnhanced.UI;

namespace KingdomEnhanced.Features
{
    public class BetterKnightFeature : MonoBehaviour
    {
        private float _updateTimer = 0f;

        void Update()
        {
            if (!ModMenu.BetterKnight) return;

            _updateTimer += Time.deltaTime;
            if (_updateTimer < 5f) return;
            _updateTimer = 0f;

            var knights = Object.FindObjectsOfType<Knight>();
            foreach (var knight in knights)
            {
                if (knight == null) continue;
                knight.SendMessage("SetHealth", 30f, SendMessageOptions.DontRequireReceiver);
                knight.SendMessage("SetMoveSpeed", 5f, SendMessageOptions.DontRequireReceiver);
                knight.SendMessage("BoostDamage", 1.5f, SendMessageOptions.DontRequireReceiver);
                knight.SendMessage("BoostDefense", 1.5f, SendMessageOptions.DontRequireReceiver);
                knight.SendMessage("IncreaseAttackSpeed", 1.25f, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}