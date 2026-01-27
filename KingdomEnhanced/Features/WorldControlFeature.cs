using UnityEngine;
using KingdomEnhanced.UI;

namespace KingdomEnhanced.Features
{
    public class WorldControlFeature : MonoBehaviour
    {
        private float _timer = 0f;

        void Update()
        {
            // Optimization: Run once per second
            _timer += Time.deltaTime;
            if (_timer < 1.0f) return;
            _timer = 0f;

            if (Managers.Inst == null || Managers.Inst.director == null) return;
            var d = Managers.Inst.director;

            // 1. LOCK SUMMER
            if (ModMenu.LockSummer)
            {
                d.SendMessage("SetSeason", 1, SendMessageOptions.DontRequireReceiver);
            }

            // 2. CLEAR WEATHER
            if (ModMenu.ClearWeather)
            {
                d.SendMessage("SetWeather", 0, SendMessageOptions.DontRequireReceiver);
            }

            // 3. NO BLOOD MOON
            if (ModMenu.NoBloodMoons)
            {
                // Removed the property check that caused the build error
                // Blindly force the "No Blood Moon" state
                d.SendMessage("SetIsBloodMoonToday", false, SendMessageOptions.DontRequireReceiver);
                d.SendMessage("StopRedMoon", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}