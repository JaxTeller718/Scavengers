using System;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine;
using DMT;

public class RH_LootContainersTakeOnly
{
    [HarmonyPatch(typeof(XUiC_ItemStack))]
    [HarmonyPatch("HandleMoveToPreferredLocation")]
    [HarmonyPatch(new Type[] {  })]
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
