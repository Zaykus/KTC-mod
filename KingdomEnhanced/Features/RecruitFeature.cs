using UnityEngine;
using KingdomEnhanced.UI;

namespace KingdomEnhanced.Features
{
    public class RecruitFeature : MonoBehaviour
    {
        public static void RecruitAllBeggars()
        {
            var beggars = Object.FindObjectsOfType<Beggar>();
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Characters/Peasant");
            Transform gameLayer = GameObject.FindGameObjectWithTag("GameLayer")?.transform;

            if (prefab == null)
            {
                ModMenu.Speak("Error: Peasant prefab missing.");
                return;
            }

            int count = 0;
            foreach (var b in beggars)
            {
                if (b == null || !b.gameObject.activeInHierarchy) continue;

                // FIX: Force Z to 0 (The gameplay plane)
                // Beggars in camps are sometimes at Z=5 or Z=10, which makes them invisible if we spawn a Peasant there.
                Vector3 pos = b.transform.position;
                pos.z = 0f;

                b.gameObject.SetActive(false);
                Object.Destroy(b.gameObject);

                GameObject newCitizen = Object.Instantiate(prefab, pos, Quaternion.identity);
                if (gameLayer != null) newCitizen.transform.SetParent(gameLayer);

                count++;
            }
            ModMenu.Speak($"Recruited {count} beggars to Z=0.");
        }
    }
}