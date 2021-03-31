using System;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using DMT;
using static RH_Options;

public class RH_HideXPPopup
{
    public class RH_HideXPPopup_Init : IHarmony
    {
        public void Start()
        {
            Debug.Log(" Loading Patch : " + GetType().ToString());
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(XUiC_CollectedItemList))]
    [HarmonyPatch("AddIconNotification")]
    [HarmonyPatch(new Type[] { typeof(string), typeof(int), typeof(bool) })]
    public class PatchXUiC_CollectedItemListAddIconNotification
    {
        //NOTE : Requires XP icon to be called ui_game_symbol_xp

        static bool Prefix(XUiC_CollectedItemList __instance, ref string iconNotifier, ref int count, ref bool _bAddOnlyIfNotExisting)
        {
            if(iconNotifier == "ui_game_symbol_xp" && !VideoOptions.ShowXPPopUp)
                return false;

            return true;
        }
    }
}
