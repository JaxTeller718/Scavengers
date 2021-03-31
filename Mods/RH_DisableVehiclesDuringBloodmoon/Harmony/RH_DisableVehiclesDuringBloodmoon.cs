using System;
using HarmonyLib;
using System.Reflection;
using DMT;
using UnityEngine;

public class RH_DisableVehiclesDuringBloodmoon
{
    public class RH_DisableVehiclesDuringBloodmoon_Init : IHarmony
    {
        public void Start()
        {
            Debug.Log(" Loading Patch : " + GetType().ToString());
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(VPEngine))]
    [HarmonyPatch("Update")]
    [HarmonyPatch(new Type[] { typeof(float) })]
    public class PatchVPEngineUpdate
    {
        static bool Prefix(VPEngine __instance, float _dt)
        {
            var num = (int)SkyManager.dayCount;
            var bloodMoonDay = GameStats.GetInt(EnumGameStats.BloodMoonDay);
            var isBloodMoon = (num == bloodMoonDay && SkyManager.TimeOfDay() >= 22f) || (num > 1 && num == bloodMoonDay + 1 && SkyManager.TimeOfDay() <= 6f);

            if (isBloodMoon)
            {
                MethodInfo method = AccessTools.Method(typeof(VPEngine), "stopEngine");
                method.Invoke(__instance, new object[] { false });

                return false;
            }

            return true;
        }
    }
}