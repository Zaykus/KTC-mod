using UnityEngine;
using KingdomEnhanced.UI;

namespace KingdomEnhanced.Features
{
    public class BuildingFeature : MonoBehaviour
    {
        private float _timer = 0f;

        void Update()
        {
            if (!ModMenu.InvincibleWalls) return;

            // Optimization: Run only 3 times per second
            _timer += Time.deltaTime;
            if (_timer < 0.33f) return;
            _timer = 0f;

            // Find all Damageable objects (Walls, Towers, etc.)
            // The compiler knows 'Damageable' exists, just not its properties
            var damageables = Object.FindObjectsOfType<Damageable>();

            foreach (var d in damageables)
            {
                if (d == null || d.gameObject == null) continue;

                string name = d.gameObject.name.ToLower();
                string tag = d.gameObject.tag;

                // Heuristic: Only heal Walls, Towers, or generic Player Structures
                if (name.Contains("wall") || name.Contains("tower") || tag.Contains("PlayerStructure"))
                {
                    // Use SendMessage to avoid "Definition not found" errors
                    // 1000f is typically enough to look full
                    d.SendMessage("SetHealth", 1000f, SendMessageOptions.DontRequireReceiver);
                    d.SendMessage("Repair", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}