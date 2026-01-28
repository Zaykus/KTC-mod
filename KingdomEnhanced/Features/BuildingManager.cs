using UnityEngine;
using KingdomEnhanced.UI;

namespace KingdomEnhanced.Features
{
    public class BuildingManager : MonoBehaviour
    {
        private float _repairTimer = 0f;

        void Update()
        {
            if (Managers.Inst == null || Managers.Inst.game == null) return;
            if (!Managers.Inst.game.state.ToString().Contains("Playing")) return;

            // WALL REPAIR ONLY (Stable)
            if (ModMenu.InvincibleWalls)
            {
                _repairTimer += Time.deltaTime;
                if (_repairTimer > 1.0f)
                {
                    _repairTimer = 0f;
                    RepairStructures();
                }
            }
        }

        private void RepairStructures()
        {
            var objs = FindObjectsOfType<Damageable>();
            foreach (var obj in objs)
            {
                if (obj == null) continue;
                if (obj.name.Contains("Wall") || obj.name.Contains("Tower"))
                {
                    obj.SendMessage("SetHealth", 1000f, SendMessageOptions.DontRequireReceiver);
                    obj.SendMessage("Repair", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}