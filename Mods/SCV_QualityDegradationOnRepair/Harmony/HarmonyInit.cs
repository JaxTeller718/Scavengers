﻿using DMT;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

public class SCV_QualityDegradationOnRepair_Init : IModApi
{
    public void InitMod(Mod _modInstance)
    {
        Debug.Log(" Loading Patch: " + this.GetType().ToString());

        // Reduce extra logging stuff
        Application.SetStackTraceLogType(UnityEngine.LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(UnityEngine.LogType.Warning, StackTraceLogType.None);

        var harmony = new HarmonyLib.Harmony(GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}