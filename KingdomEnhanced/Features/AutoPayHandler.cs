using System;
using System.Collections;
using UnityEngine;
#if IL2CPP
using Il2CppInterop.Runtime.Attributes;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using KingdomEnhanced.Shared.Attributes;
#endif
using KingdomEnhanced.Core;

namespace KingdomEnhanced.Features
{
#if IL2CPP
    [RegisterTypeInIl2Cpp]
#endif
    public class AutoPayHandler : MonoBehaviour
    {
#if IL2CPP
        public AutoPayHandler(IntPtr ptr) : base(ptr) { }
#endif
        public void StartPayment(Payable payable, int amount, Player player)
        {
#if IL2CPP
            StartCoroutine(PayRoutine(payable, amount, player).WrapToIl2Cpp());
#else
            StartCoroutine(PayRoutine(payable, amount, player));
#endif
        }

#if IL2CPP
        [HideFromIl2Cpp]
#endif
        private IEnumerator PayRoutine(Payable payable, int amount, Player player)
        {
            yield return null;

            for (int i = 0; i < amount; i++)
            {
                if (payable == null) yield break;

                if (payable.CanPay(player))
                {
                    try
                    {
                        payable.Pay();
                    }
                    catch (System.Exception ex)
                    {
                        Plugin.Instance?.LogSource.LogWarning($"AutoPay Error: {ex.Message}");
                    }
                }
                else
                {
                    yield break;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
