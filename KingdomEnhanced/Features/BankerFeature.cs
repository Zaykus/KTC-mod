using UnityEngine;

namespace KingdomEnhanced.Features
{
    public class BankerFeature : MonoBehaviour
    {
        public static void InstantBankInterest()
        {
            var banker = Object.FindObjectOfType<Banker>();
            if (banker != null)
            {
                // 1. Force the banker to have a lot of coins internally
                // In many versions, he has a private field or uses SendMessage
                banker.SendMessage("AddCoins", 100, SendMessageOptions.DontRequireReceiver);

                // 2. Trigger the "Pay Player" state
                // This forces him to walk out and throw coins at you
                banker.SendMessage("DropCoins", SendMessageOptions.DontRequireReceiver);
                banker.SendMessage("GiveCoinsToPlayer", SendMessageOptions.DontRequireReceiver);

                KingdomEnhanced.UI.ModMenu.Speak("The Banker has been paid. Look for coins!");
            }
            else
            {
                KingdomEnhanced.UI.ModMenu.Speak("Banker not found! Is it stone age yet?");
            }
        }
    }
}