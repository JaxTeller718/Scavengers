using System;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine;
using DMT;

public class RH_IncreaseWanderingSpawns
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

    [HarmonyPatch(typeof(AIWanderingHordeSpawner), MethodType.Constructor)]
    [HarmonyPatch("AIWanderingHordeSpawner")]
    [HarmonyPatch(new Type[] { typeof(World), typeof(AIWanderingHordeSpawner.HordeArrivedDelegate), typeof(List<AIDirectorPlayerState>), typeof(int), typeof(ulong), typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(int), typeof(bool), })]
    static class PatchAIWanderingHordeSpawner
    {
        static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_S)
                {
                    codes[i].opcode = OpCodes.Ldc_I4;
                    codes[i].operand = (int)10000;
                    break;
                }
            }
            return codes.AsEnumerable(); 
        }
    }  
}
