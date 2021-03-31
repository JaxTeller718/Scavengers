using System;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine;
using DMT;

public class RH_ReduceSpawnDistanceOfWanderingSpawns
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

    [HarmonyPatch(typeof(AIDirectorHordeComponent))]
    [HarmonyPatch("FindWanderingHordeTargets")]
    [HarmonyPatch(new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(List<AIDirectorPlayerState>) }, new ArgumentType[] { ArgumentType.Out, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal })]
    static class PatchAIDirectorHordeComponentFindWanderingHordeTargets
    {
        static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 92f)
                {
                    codes[i].operand = 70f;
                }
            }
            return codes.AsEnumerable();
        }
    }
}
