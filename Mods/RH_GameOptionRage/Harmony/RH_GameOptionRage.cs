using System;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using DMT;
using System.Collections.Generic;

public class RH_GameOptionRage
{
    public class RH_GameOptionRage_Init : IHarmony
    {
        public void Start()
        {
            Debug.Log(" Loading Patch : " + GetType().ToString());
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(EntityZombie))]
    [HarmonyPatch("StartRage")]
    [HarmonyPatch(new Type[] { typeof(float), typeof(float) })]
    public class PatchEntityZombieStartRage
    {
        static bool Prefix(EntityZombie __instance)
        {
            if (!RH_Options.GameOptions.ZombieRage)
                return false;

            return true;
        }
    }
}
