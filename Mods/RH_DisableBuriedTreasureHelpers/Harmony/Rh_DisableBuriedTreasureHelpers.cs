using DMT;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Rh_DisableBuriedTreasureHelpers
{
    public class Rh_DisableBuriedTreasureHelpers_Init : IHarmony
    {
        public void Start()
        {
            Debug.Log(" Loading Patch : " + GetType().ToString());
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(BoundaryProjector))]
    [HarmonyPatch("Update")]
    [HarmonyPatch(new Type[] { })]
    public class PatchBoundaryProjectorUpdate
    {
        static bool Prefix(BoundaryProjector __instance, ref List<BoundaryProjector.ProjectorEntry> ___ProjectorList)
        {
            if (RH_Options.VideoOptions.ShowBuriedTreasureHelper)
                return true;

            for (int i = 0; i < ___ProjectorList.Count; i++)
            {
                if (___ProjectorList[i] != null && ___ProjectorList[i].Projector.gameObject.activeSelf)
                {
                    BoundaryProjector.ProjectorEntry projectorEntry = ___ProjectorList[i];

                    Color color = projectorEntry.Projector.material.color;
                    // Set opacity of circle to zero (0f) and hence not visible.
                    projectorEntry.Projector.material.color = new Color(color.r, color.g, color.b, 0f);
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(ObjectiveTreasureChest))]
    [HarmonyPatch("Current_BlockDestroy")]
    [HarmonyPatch(new Type[] { typeof(Block), typeof(Vector3i) })]
    public class PatchObjectiveTreasureChestCurrent_BlockDestroy
    {
        static bool Prefix(ObjectiveTreasureChest __instance)
        {
            return RH_Options.VideoOptions.ShowBuriedTreasureHelper;
        }
    }
}