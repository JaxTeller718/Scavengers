using HarmonyLib;
using System.Reflection;
using UnityEngine;
using DMT;

public class RH_StopPickupVehicles
{
    public class Init : IHarmony
    {
        public void Start()
        {
            Debug.Log(" Loading Patch : " + GetType().ToString());
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(EntityVehicle))]
    [HarmonyPatch("GetActivationCommands")]
    class PatchEntityVehicleGetActivationCommands
    {
        static void Postfix(EntityVehicle __instance , EntityActivationCommand[] __result)
        {
            if (__instance.EntityClass.entityClassName.ToLower().Contains("bicycle") 
                || __instance.EntityClass.entityClassName.ToLower().Contains("vehicleguppyboat2")
                || __instance.EntityClass.entityClassName.ToLower().Contains("vehicleguppyboat3")
                || __instance.EntityClass.entityClassName.ToLower().Contains("guppyvehiclehangglider")
                || GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled))
                return;

            for (var i = 0; i < __result.Length; i++)
            {
                if (__result[i].text == "take")
                {
                    __result[i].enabled = false;
                }
            }
        }
    }
}
