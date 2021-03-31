using System;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine;
using DMT;

public class RH_IncreaseBagSize
{
    public class RH_IncreaseBagSizeInit : IHarmony
    {
        public void Start()
        {
            Debug.Log(" Loading Patch : " + GetType().ToString());
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(Bag))]
    [HarmonyPatch("GetSlots")]
    static class PatchBagGetSlots
    {
        static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_S)
                {
                    codes[i].operand = (int)60;
                    break;
                }
            }
            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(Bag))]
    [HarmonyPatch("SetSlots")]
    [HarmonyPatch(new Type[] { typeof(ItemStack[]) })]
    static class PatchBagSetSlots
    {
        static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_S)
                {
                    codes[i].operand = (int)60;
                    break;
                }
            }
            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(Bag))]
    [HarmonyPatch("AddItem")]
    [HarmonyPatch(new Type[] { typeof(ItemStack) })]
    static class PatchBagAddItem
    {
        static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4)
                {
                    codes[i].operand = (float)60;
                    break;
                }
            }
            return codes.AsEnumerable();
        }
    }
}
