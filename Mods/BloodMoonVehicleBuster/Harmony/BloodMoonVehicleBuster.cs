//Harmony Patch: Disables vehicles with an engine during blood moon.
//Author: Mythix(dino)
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using DMT;

public class Mythix_NovehicleBM
{
    public class Mythix_NovehicleBM_Init : IHarmony
    {
        public void Start()
        {
            Debug.Log(" Loading Patch: " + this.GetType().ToString());
            Harmony harmony = new Harmony("Mythix.NovehicleBM.Patch");
            harmony.PatchAll();
        }
    }
    [HarmonyPatch(typeof(VehiclePart))]
    [HarmonyPatch("IsBroken")]
    public class VehicleDisabler
    {
        static bool Postfix(bool __result, VehiclePart __instance)
        {
            bool isBloodMoon = SkyManager.BloodMoon();
            if (__instance is VPEngine && isBloodMoon)
            {
                try
                {
                    foreach (EntityPlayerLocal entityplayer in GameManager.Instance.World.GetLocalPlayers())
                    {
                        if (entityplayer.AttachedToEntity && ((entityplayer.AttachedToEntity.GetType()) != typeof(EntityBicycle)))
                        {
                            GameManager.ShowTooltip(entityplayer, Localization.Get("BMEngineWarning"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Caught exception: " + ex);
                }
                return true;
            }
            return __result;
        }
    }
    //[HarmonyPatch(typeof(VehiclePart))]
    //[HarmonyPatch("IsBroken")]
    //public class novehicleBM
    //{
    //    static bool Prefix(VehiclePart __instance)
    //    {
    //        bool isBloodMoon = SkyManager.BloodMoon();
    //        Log.Warning("Bloodmoon: " + isBloodMoon);
    //        return true;
    //    }

    //}
}