using System.Collections;
using UnityEngine;
using Il2CppInterop.Runtime.Attributes;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using KingdomEnhanced.Core;

namespace KingdomEnhanced.Features
{
    public class AutoPayHandler : MonoBehaviour
    {
        public void StartPayment(Payable payable, int amount, Player player)
        {
            StartCoroutine(PayRoutine(payable, amount, player).WrapToIl2Cpp());
        }

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
                        Plugin.Instance?.Log.LogWarning($"AutoPay Error: {ex.Message}");
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
