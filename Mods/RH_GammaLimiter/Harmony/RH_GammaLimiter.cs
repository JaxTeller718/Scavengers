using HarmonyLib;
using UnityEngine;
using System.Reflection;
using DMT;

public class RH_GammaLimiter
{
    public class RH_GammaLimiter_Init : IHarmony
    {
        public void Start()
        {
            Debug.Log(" Loading Patch : " + GetType().ToString());
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(XUiC_OptionsVideo))]
    [HarmonyPatch("applyChanges")]
    class PatchXUiC_OptionsVideoapplyChanges
    {
        static void Postfix(XUiC_OptionsVideo __instance, ref XUiC_ComboBoxFloat ___comboBrightness)
        {
            float gamma = Mathf.Min((float)___comboBrightness.Value, 0.60f);
            GamePrefs.Set(EnumGamePrefs.OptionsGfxBrightness, gamma);
        }
    }
}
