using UnityEngine;
using System;
using System.Reflection;
using KingdomEnhanced.UI;

namespace KingdomEnhanced.Features
{
    public class WalletFeature : MonoBehaviour
    {
        private bool _hasHackedLimit = false;
        private float _timer = 0f;

        void Update()
        {
            // Run less frequently to save performance (every 0.5s)
            _timer += Time.deltaTime;
            if (_timer > 0.5f)
            {
                _timer = 0f;

                if (!_hasHackedLimit) ApplyInfiniteLimit();

                ShrinkBagAndCoins();
            }
        }

        void ShrinkBagAndCoins()
        {
            if (Managers.Inst == null || Managers.Inst.kingdom == null) return;
            var player = Managers.Inst.kingdom.GetPlayer(0);

            if (player != null && player.wallet != null)
            {
                // 1. Shrink the Bag Object (Visuals only)
                if (player.wallet.transform.localScale.x > 0.7f)
                {
                    player.wallet.transform.localScale = new Vector3(0.668f, 0.668f, 0.668f);
                }

                // 2. Shrink Coins (Visuals only)
                // We REMOVED the Physics/Rigidbody hacks here to stop the glitching!
                Transform bagTransform = player.wallet.transform;
                int count = bagTransform.childCount;

                for (int i = 0; i < count; i++)
                {
                    Transform coin = bagTransform.GetChild(i);
                    if (coin == null) continue;

                    // Only shrink if it's too big
                    if (coin.localScale.x > 0.4f)
                    {
                        string n = coin.name.ToLower();
                        if (n.Contains("coin") || n.Contains("gem") || n.Contains("diamond"))
                        {
                            coin.localScale = new Vector3(0.35f, 0.35f, 1f);
                        }
                    }
                }
            }
        }

        public void ApplyInfiniteLimit()
        {
            // Keep the limit hack, it works fine
            try
            {
                Type bagType = null;
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.FullName.StartsWith("UnityEngine") || asm.FullName.StartsWith("System")) continue;
                    try { foreach (var t in asm.GetTypes()) { if (t.Name == "CurrencyBag") { bagType = t; break; } } } catch { }
                    if (bagType != null) break;
                }
                if (bagType != null)
                {
                    var field = bagType.GetField("OVERFLOW_LIMIT", BindingFlags.NonPublic | BindingFlags.Static);
                    if (field != null) { field.SetValue(null, 9999); _hasHackedLimit = true; }
                }
            }
            catch { }
        }
    }
}