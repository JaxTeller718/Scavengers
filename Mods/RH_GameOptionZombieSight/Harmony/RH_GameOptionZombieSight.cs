using System;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using DMT;
using System.Collections.Generic;

public class RH_GameOptionZombieSight
{
    public class RH_GameOptionZombieSight_Init : IHarmony
    {
        public void Start()
        {
            Debug.Log(" Loading Patch : " + GetType().ToString());
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(EntityAlive))]
    [HarmonyPatch("CopyPropertiesFromEntityClass")]
    public class PatchEntityAliveCopyPropertiesFromEntityClass
    {
        static void Postfix(EntityAlive __instance, ref float ___sightRange)
        {
            ___sightRange += RH_Options.GameOptions.SightRange;
        }
    }
}

