using System;
using System.Linq; // Added for Count()
using UnityEngine;
using KingdomEnhanced.Core;
using KingdomEnhanced.UI;

namespace KingdomEnhanced.Features
{
    public class KingdomMonitor : MonoBehaviour
    {
        private Kingdom _kingdom;
        private EnemyManager _enemyManager;
        
        private bool _isVisible = true;
        private Rect _windowRect = new Rect(10, 10, 250, 350);

        private void Start()
        {
            _kingdom = FindObjectOfType<Kingdom>();
            _enemyManager = FindObjectOfType<EnemyManager>();
            
            if (_kingdom == null) Plugin.Instance.Log.LogWarning("KingdomMonitor: Kingdom not found.");
            if (_enemyManager == null) Plugin.Instance.Log.LogWarning("KingdomMonitor: EnemyManager not found.");
        }



        private void OnGUI()
        {
            if (!_isVisible) return;

            // Simple styling
            GUI.skin.box.fontSize = 14;
            GUI.skin.label.fontSize = 14;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;

            _windowRect = GUI.Window(999, _windowRect, (GUI.WindowFunction)DrawWindow, "Kingdom Monitor (F4)");
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();

            // 1. Time & Season
            string cycle = "Unknown";
            int day = 0;
            if (Managers.Inst != null && Managers.Inst.director != null)
            {
               day = Managers.Inst.director.CurrentIslandDays;
            }
            
            if (_kingdom != null)
            {
                cycle = _kingdom.isDaytime ? "Day" : "Night";
            }
            // Bold Header
            GUILayout.Label($"<b>Day {day} ({cycle})</b>");

            // 2. Blood Moon / Threat
            if (_enemyManager != null)
            {
                bool danger = _enemyManager.IsDangerous;
                string status = danger ? "<color=red>DANGER</color>" : "<color=green>SAFE</color>";
                GUILayout.Label($"Threat: {status}");
            }
            
            // 3. Wallet (Player 1)
            if (_kingdom != null && _kingdom.playerOne != null && _kingdom.playerOne.wallet != null)
            {
                 int coins = _kingdom.playerOne.wallet.GetCurrency(CurrencyType.Coins);
                 int gems = _kingdom.playerOne.wallet.GetCurrency(CurrencyType.Gems);
                 GUILayout.Label($"Wallet: {coins} Coins, {gems} Gems");
            }

            GUILayout.Space(5);

            // 4. Citizens (Census)
            GUILayout.Label($"<b>Population</b>");
            GUILayout.Label($"Archers: {_archerCount}");
            GUILayout.Label($"Workers: {_workerCount}");
            GUILayout.Label($"Peasants: {_peasantCount}");

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        // Census Data
        private int _archerCount;
        private int _workerCount;
        private int _peasantCount;
        private float _nextCensusTime;

        private void Update()
        {
            // Toggle with F4 (F3 is used by AccessibilityFeature)
            if (Input.GetKeyDown(KeyCode.F4))
            {
                _isVisible = !_isVisible;
            }

            // Run Census every 1 second if visible
            if (_isVisible && Time.time > _nextCensusTime)
            {
                _nextCensusTime = Time.time + 1.0f;
                _archerCount = FindObjectsOfType<Archer>().Length;
                _workerCount = FindObjectsOfType<Worker>().Length;
                _peasantCount = FindObjectsOfType<Peasant>().Length;
            }
        }
    }
}
