using System;
using System.Linq;
using HarmonyLib;
using KingdomEnhanced.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if IL2CPP
using Il2CppSystem.Collections.Generic;
#else
using System.Collections.Generic;
#endif

namespace KingdomEnhanced.Features
{
    public class DifficultyUIPatch
    {
        [HarmonyPatch(typeof(BiomeSelect), nameof(BiomeSelect.ShowBiomeSelect))]
        public static class BiomeSelect_ShowBiomeSelect_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(BiomeSelect __instance)
            {
                try
                {
                    List<DifficultySelector> selectorList = __instance._difficultySelectorsList;

                    if (selectorList == null)
                    {
                        Plugin.Instance.LogSource.LogError("DifficultyUIPatch: selector list is null.");
                        return;
                    }

                    if (selectorList.Count > 5)
                        return;

                    var template = selectorList[selectorList.Count - 1];
                    var parent = __instance._difficultySelectorParent;

                    if (template == null || parent == null)
                    {
                        Plugin.Instance.LogSource.LogError("DifficultyUIPatch: template or parent is null.");
                        return;
                    }

                    foreach (HardModePreset preset in Enum.GetValues(typeof(HardModePreset)))
                    {
                        if (preset == HardModePreset.None) continue;

                        var templateGO = template.RectXForm.gameObject;
                        var newGO = UnityEngine.Object.Instantiate(templateGO, parent);
                        newGO.name = $"DifficultySelector_{preset}";

                        var newSelector = new DifficultySelector();
                        newSelector._rectXForm = newGO.GetComponent<RectTransform>();
                        newSelector._difficultyImages = new List<Image>();
                        newSelector._difficultyTexts = new List<Text>();

                        newSelector._difficultyMaterial = template._difficultyMaterial;
                        newSelector._materialCopy = template._materialCopy;

                        foreach (var img in newGO.GetComponentsInChildren<Image>(true))
                            newSelector._difficultyImages.Add(img);

                        foreach (var txt in newGO.GetComponentsInChildren<Text>(true))
                        {
                            var loc = txt.GetComponent<LocalizedText>();
                            if (loc != null)
                                UnityEngine.Object.Destroy(loc);

                            newSelector._difficultyTexts.Add(txt);
                            txt.text = HardModePresets.PresetNames[preset];
                        }

                        foreach (var tmp in newGO.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
                        {
                            var locTMP = tmp.GetComponent<LocalizedText>();
                            if (locTMP != null)
                                UnityEngine.Object.Destroy(locTMP);
                            
                            tmp.text = HardModePresets.PresetNames[preset];
                        }

                        newSelector._difficultyIndex = (DifficultyData.DifficultyLevel)(int)preset;
                        selectorList.Add(newSelector);
                    }
                }
                catch (Exception e)
                {
                    Plugin.Instance.LogSource.LogError($"DifficultyUIPatch: failed to inject UI: {e}");
                }
            }
        }

        [HarmonyPatch(typeof(Menu), "UpdateMenuStatus")]
        public static class Menu_UpdateMenuStatus_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(Menu __instance)
            {
                try
                {
                    var preset = HardModeFeature.GetActivePreset();
                    if (preset != HardModePreset.None)
                    {
                        if (__instance.difficultyLocText != null)
                            UnityEngine.Object.Destroy(__instance.difficultyLocText);
                        
                        
                        var textMesh = __instance.difficultyText.GetComponent<TMPro.TextMeshProUGUI>();
                        if (textMesh != null)
                        {
                            textMesh.text = $"DIFFICULTY: {HardModePresets.PresetNames[preset].ToUpper()}";
                        }
                    }
                }
                catch (Exception e)
                {
                    Plugin.Instance.LogSource.LogError($"Menu Custom Difficulty Text Patch failed: {e}");
                }
            }
        }
    }
}
