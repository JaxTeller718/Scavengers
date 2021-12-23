using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

public class SCV_DisableVehiclesOnBloodMoon : IModApi
{ 


        public void InitMod(Mod _modInstance)
    {
        Log.Out(" Loading Patch: " + GetType());

        // Reduce extra logging stuff
        Application.SetStackTraceLogType(UnityEngine.LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(UnityEngine.LogType.Warning, StackTraceLogType.None);

        var harmony = new HarmonyLib.Harmony(GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}
[HarmonyPatch(typeof(VehiclePart))]
[HarmonyPatch("IsBroken")]
public class VehicleDisabler
{
    static bool Postfix(bool __result, VehiclePart __instance)
    {
        bool isBloodMoon = SkyManager.IsBloodMoonVisible();
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
