using DMT;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RH_CoreHarmony
{
    private const string RH_GamePrefSelectorId = "RH_GamePrefSelectorId";
    private static int LastSoundCheckMinute = 0;

    public class RH_CoreHarmony_Init : IHarmony
    {
        public void Start()
        {
            Debug.Log(" Loading Patch : " + GetType().ToString());
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(XUiC_NewContinueGame))]
    [HarmonyPatch("SaveGameOptions")]
    [HarmonyPatch(new Type[] { })]
    public class PatchXUiC_NewContinueGameSaveGameOptions
    {
        static void Postfix(XUiC_NewContinueGame __instance)
        {
            Log.Out("SaveGameOptions");
            RH_Options.SaveGameOptions();
        }
    }

    [HarmonyPatch(typeof(XUiC_NewContinueGame))]
    [HarmonyPatch("updateGameOptionValues")]
    [HarmonyPatch(new Type[] { })]
    public class PatchXUiC_NewContinueGameupdateGameOptionValues
    {
        static void Postfix(XUiC_NewContinueGame __instance)
        {
            foreach (XUiC_RH_GamePrefSelector xuiC_RH_GamePrefSelector in __instance.GetChildrenByType<XUiC_RH_GamePrefSelector>(null))
            {
                xuiC_RH_GamePrefSelector.SetCurrentValue();
            }
        }
    }

    [HarmonyPatch(typeof(XUiC_NewContinueGame))]
    [HarmonyPatch("startGame")]
    [HarmonyPatch(new Type[] { })]
    public class PatchXUiC_NewContinueGamestartGame
    {
        static void Postfix(XUiC_NewContinueGame __instance)
        {
            Log.Out("Reset T5 Radiation Data");
            RH_Core.T5Prefabs = null;
        }
    }

    #region Night Whisper Sounds
    [HarmonyPatch(typeof(GameManager))]
    [HarmonyPatch("Update")]
    [HarmonyPatch(new Type[] { })]
    public class PatchGameManagerUpdate
    {
        static void Postfix(GameManager __instance)
        {
            if (__instance.World != null)
            {
                int hour = GameUtils.WorldTimeToHours(__instance.World.worldTime);
                int minute = GameUtils.WorldTimeToMinutes(__instance.World.worldTime);
                if ((hour >= 22 
                    || hour < 4)
                    && minute % 10 == 0 
                    && minute != LastSoundCheckMinute)
                {
                    LastSoundCheckMinute = minute;
                    var rnd = new System.Random();
                    if(rnd.Next(0,15) % 15 == 0)
                    {
                        var next = rnd.Next(1, 4);
                        //Log.Out("Playing nightsounds" + next);
                        Audio.Manager.BroadcastPlay("nightsounds" + rnd.Next(1, 4));
                    } 
                }
            }
        }
    }
    #endregion
}

