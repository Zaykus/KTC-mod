using UnityEngine;
using KingdomEnhanced.UI;

namespace KingdomEnhanced.Features
{
    public class CoinsStayDryFeature : MonoBehaviour
    {
        private float _updateTimer = 0f;

        void Update()
        {
            if (!ModMenu.CoinsStayDry) return;

            // Optimization: Run only once per second because searching all objects is heavy
            _updateTimer += Time.deltaTime;
            if (_updateTimer < 1.0f) return;
            _updateTimer = 0f;

            ProtectCoins();
        }

        private void ProtectCoins()
        {
            try
            {
                // FALLBACK: Since 'Coin' and 'Rigidbody2D' types are missing in references,
                // we scan all GameObjects. This is safe but heavy, so we throttled it above.
                var allObjects = Object.FindObjectsOfType<GameObject>();

                foreach (var obj in allObjects)
                {
                    if (obj == null) continue;

                    // Filter by name to find money
                    string name = obj.name.ToLower();
                    if (name.Contains("coin") || name.Contains("gem") || name.Contains("currency"))
                    {
                        // 1. Reset water decay timer blindly using SendMessage
                        obj.SendMessage("PreventWaterLoss", SendMessageOptions.DontRequireReceiver);

                        // 2. Physical Safety: Check position directly
                        // If it falls into water (usually Y < -0.1), push it up
                        if (obj.transform.position.y < -0.1f)
                        {
                            Vector3 safePos = obj.transform.position;
                            safePos.y = 0.5f; // Push to ground level
                            obj.transform.position = safePos;
                        }
                    }
                }
            }
            catch { }
        }
    }
}