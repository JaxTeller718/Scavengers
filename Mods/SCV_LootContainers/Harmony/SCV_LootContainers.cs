using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

public class SCV_LootContainers : IModApi
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
    [HarmonyPatch(typeof(XUiC_ItemStack))]
    [HarmonyPatch("HandleMoveToPreferredLocation")]
    [HarmonyPatch(new Type[] { })]
    class PatchXUiC_ItemStackHandleMoveToPreferredLocation
    {
        static bool Prefix(XUiC_ItemStack __instance)
        {
            if (__instance.StackLocation == XUiC_ItemStack.StackLocationTypes.Backpack
                || __instance.StackLocation == XUiC_ItemStack.StackLocationTypes.ToolBelt)
            {
                var childByType2 = __instance.xui.FindWindowGroupByName(XUiC_LootWindowGroup.ID).GetChildByType<XUiC_LootContainer>();
                var localTileEntity = AccessTools.Field(typeof(XUiC_LootContainer), "localTileEntity").GetValue(childByType2) as TileEntityLootContainer;

                if (childByType2 != null && localTileEntity != null && !localTileEntity.bPlayerStorage)
                    return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(XUiC_ItemStack))]
    [HarmonyPatch("HandleStackSwap")]
    [HarmonyPatch(new Type[] { })]
    class PatchXUiC_ItemStackHandleStackSwap
    {
        static bool Prefix(XUiC_ItemStack __instance)
        {
            // Left click
            if (__instance.StackLocation == XUiC_ItemStack.StackLocationTypes.Workstation
                && __instance.ViewComponent.Controller.GetParentWindow().ID == "windowOutput")
                return false;

            if (__instance.StackLocation == XUiC_ItemStack.StackLocationTypes.LootContainer)
            {
                var childByType2 = __instance.xui.FindWindowGroupByName(XUiC_LootWindowGroup.ID).GetChildByType<XUiC_LootContainer>();
                var localTileEntity = AccessTools.Field(typeof(XUiC_LootContainer), "localTileEntity").GetValue(childByType2) as TileEntityLootContainer;

                if (childByType2 != null && localTileEntity != null && !localTileEntity.bPlayerStorage)
                    return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(XUiC_ItemStack))]
    [HarmonyPatch("HandleDropOne")]
    [HarmonyPatch(new Type[] { })]
    class PatchXUiC_ItemStackHandleDropOne
    {
        static bool Prefix(XUiC_ItemStack __instance)
        {
            // Right Click
            if (__instance.StackLocation == XUiC_ItemStack.StackLocationTypes.Workstation
                && __instance.ViewComponent.Controller.GetParentWindow().ID == "windowOutput")
                return false;

            if (__instance.StackLocation == XUiC_ItemStack.StackLocationTypes.LootContainer)
            {
                var childByType2 = __instance.xui.FindWindowGroupByName(XUiC_LootWindowGroup.ID).GetChildByType<XUiC_LootContainer>();
                var localTileEntity = AccessTools.Field(typeof(XUiC_LootContainer), "localTileEntity").GetValue(childByType2) as TileEntityLootContainer;

                if (childByType2 != null && localTileEntity != null && !localTileEntity.bPlayerStorage)
                    return false;
            }

            return true;
        }
    }
}

