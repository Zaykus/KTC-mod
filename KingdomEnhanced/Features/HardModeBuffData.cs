using BepInEx.Unity.IL2CPP;
using UnityEngine;
using System;

namespace KingdomEnhanced.Features
{
    public class HardModeBuffData : MonoBehaviour
    {
        public int OriginalMaxHp;
        public bool IsBuffed;

        public HardModeBuffData(IntPtr ptr) : base(ptr) { }
    }
}
