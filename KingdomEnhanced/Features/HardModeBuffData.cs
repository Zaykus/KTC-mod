using UnityEngine;
using System;
#if IL2CPP
using KingdomEnhanced.Shared.Attributes;
#endif

namespace KingdomEnhanced.Features
{
#if IL2CPP
    [RegisterTypeInIl2Cpp]
#endif
    public class HardModeBuffData : MonoBehaviour
    {
        public int OriginalMaxHp;
        public bool IsBuffed;

#if IL2CPP
        public HardModeBuffData(IntPtr ptr) : base(ptr) { }
#endif
    }
}
