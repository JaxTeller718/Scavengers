using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform;
using Audio;
using UnityEngine;
using System.Reflection;

namespace SCV_QualityDegradation
{
    public class SCV_QualityDegradation : IModApi
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
    }
    
        [HarmonyPatch] //this is how to access a private method with harmony2
        public class patch
        {
            [HarmonyReversePatch]
            [HarmonyPatch(typeof(XUiC_RecipeStack), "giveExp")]
            public static void PatchGiveExp(XUiC_RecipeStack original, ItemValue _iv, ItemClass _ic)
            {
                //Original method
                throw new NotImplementedException("Dummytext");
            }

            [HarmonyReversePatch]
            [HarmonyPatch(typeof(XUiC_RecipeStack), "AddItemToInventory")]

            public static bool PatchAddItemToInventory(XUiC_RecipeStack original)
            {
                //Original method
                throw new NotImplementedException("Dummytext");
            }
        }

        [HarmonyPatch(typeof(XUiC_RecipeStack))]
        [HarmonyPatch("outputStack")]
        class PatchXUiC_RecipeStackoutputStack
        {
            static bool Prefix(XUiC_RecipeStack __instance,
                ref bool __result,
                ref Recipe ___recipe,
                ref ItemValue ___originalItem,
                ref ItemValue ___outputItemValue,
                ref int ___outputQuality,
                ref int ___startingEntityId,
                ref int ___amountToRepair,
                ref bool ___playSound,
                ref bool ___isInventoryFull)
            {
                if (___recipe == null)
                {
                    __result = false;
                    return false;
                }
                EntityPlayerLocal entityPlayer = __instance.xui.playerUI.entityPlayer;
                if (entityPlayer == null)
                {
                    __result = false;
                    return false;
                }
                ItemValue returnedMod = null;
                ItemValue[] returnedCosmeticMod = null;
                if (___originalItem == null || ___originalItem.Equals(ItemValue.None))
                {
                    ___outputItemValue = new ItemValue(___recipe.itemValueType, ___outputQuality, ___outputQuality, false, null, 1f);
                    ItemClass itemClass = ___outputItemValue.ItemClass;
                    if (___outputItemValue == null)
                    {
                        __result = false;
                        return false;
                    }
                    if (itemClass == null)
                    {
                        __result = false;
                        return false;
                    }
                    if (entityPlayer.entityId == ___startingEntityId)
                    {
                        var giveExpMethod = AccessTools.Method(typeof(XUiC_RecipeStack), "giveExp");
                        giveExpMethod.Invoke(__instance, new object[] { ___outputItemValue, itemClass });
                    }
                    if (___recipe.GetName().Equals("meleeToolRepairT0StoneAxe"))
                    {
                        PlatformManager.NativePlatform.AchievementManager.SetAchievementStat(EnumAchievementDataStat.StoneAxeCrafted, 1);
                    }
                    else if (___recipe.GetName().Equals("woodFrameBlockVariantHelper"))
                    {
                        PlatformManager.NativePlatform.AchievementManager.SetAchievementStat(EnumAchievementDataStat.WoodFrameCrafted, 1);
                    }
                }
                else if (___amountToRepair > 0)
                {
                    ItemValue itemValue2 = ___originalItem.Clone();
                    itemValue2.UseTimes -= ___amountToRepair;
                    ItemClass itemClass2 = itemValue2.ItemClass;
                    if (itemValue2.UseTimes < 0)
                    {
                        itemValue2.UseTimes = 0;
                    }
                    ___outputItemValue = itemValue2.Clone();
                    if (___originalItem.Quality <= 1)
                    {
                        if (___outputItemValue.CosmeticMods.Length > 0)
                        {
                            returnedCosmeticMod = ___outputItemValue.CosmeticMods;
                        }
                        ___outputItemValue = null;
                        Manager.BroadcastPlay(itemClass2.Properties.Values[ItemClass.PropSoundDestroy]);
                    }
                    else
                    {
                        float num = 1f;
                        var newQuality = ___outputItemValue.Quality - (int)num;
                        ___outputItemValue.Quality = newQuality;

                        var tempItem = new ItemValue(___outputItemValue.type, ___outputItemValue.Quality, ___outputItemValue.Quality, false, null, 1f);
                        if (tempItem.Modifications.Length < ___outputItemValue.Modifications.Length)
                        {
                            if (___outputItemValue.Modifications[___outputItemValue.Modifications.Length - 1] != null)
                            {
                                returnedMod = ___outputItemValue.Modifications[___outputItemValue.Modifications.Length - 1].Clone();
                            }
                            ItemValue[] modifications = ___outputItemValue.Modifications;
                            Array.Resize(ref modifications, ___outputItemValue.Modifications.Length - 1);
                            ___outputItemValue.Modifications = modifications;
                        }
                    }
                    QuestEventManager.Current.RepairedItem(___outputItemValue);
                    ___amountToRepair = 0;
                }

                var windowGroup = AccessTools.Field(typeof(XUiController), "windowGroup").GetValue(__instance) as XUiWindowGroup;
                XUiC_WorkstationOutputGrid childByType = windowGroup.Controller.GetChildByType<XUiC_WorkstationOutputGrid>();
                if (childByType != null && (___originalItem == null || ___originalItem.Equals(ItemValue.None)))
                {
                    ItemStack itemStack = new ItemStack(___outputItemValue, ___recipe.count);
                    ItemStack[] slots = childByType.GetSlots();
                    bool flag = false;
                    for (int i = 0; i < slots.Length; i++)
                    {
                        if (slots[i].CanStackWith(itemStack))
                        {
                            slots[i].count += ___recipe.count;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        for (int j = 0; j < slots.Length; j++)
                        {
                            if (slots[j].IsEmpty())
                            {
                                slots[j] = itemStack;
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        childByType.SetSlots(slots);
                        childByType.UpdateData(slots);
                        childByType.IsDirty = true;
                        QuestEventManager.Current.CraftedItem(itemStack);
                        if (___playSound)
                        {
                            if (___recipe.craftingArea != null)
                            {
                                WorkstationData workstationData = CraftingManager.GetWorkstationData(___recipe.craftingArea);
                                if (workstationData != null)
                                {
                                    Manager.PlayInsidePlayerHead(workstationData.CraftCompleteSound);
                                }
                            }
                            else
                            {
                                Manager.PlayInsidePlayerHead("craft_complete_item");
                            }
                        }
                    }
                    else
                    {
                        var AddItemToInventoryMethod = AccessTools.Method(typeof(XUiC_RecipeStack), "AddItemToInventory");
                        var addItemToInventory = AddItemToInventoryMethod.Invoke(__instance, new object[] { }) as bool?;

                        if (addItemToInventory.HasValue && !addItemToInventory.Value)
                        {
                            ___isInventoryFull = true;
                            string text = "No room in workstation output, crafting has been halted until space is cleared.";
                            if (Localization.Exists("wrnWorkstationOutputFull"))
                            {
                                text = Localization.Get("wrnWorkstationOutputFull");
                            }
                            GameManager.ShowTooltip(entityPlayer, text);
                            Manager.PlayInsidePlayerHead("ui_denied");
                            __result = false;
                            return false;
                        }
                    }
                }
                else
                {
                    if (!__instance.xui.dragAndDrop.CurrentStack.IsEmpty() && __instance.xui.dragAndDrop.CurrentStack.itemValue.ItemClass is ItemClassQuest)
                    {
                        __result = false;
                        return false;
                    }
                    ItemStack itemStack2 = new ItemStack(___outputItemValue, ___recipe.count);
                    if (!__instance.xui.PlayerInventory.AddItemNoPartial(itemStack2, false))
                    {
                        if (itemStack2.count != ___recipe.count)
                        {
                            __instance.xui.PlayerInventory.DropItem(itemStack2);
                            QuestEventManager.Current.CraftedItem(itemStack2);
                            __result = true;
                            return false;
                        }
                        ___isInventoryFull = true;
                        string text2 = "No room in inventory, crafting has been halted until space is cleared.";
                        if (Localization.Exists("wrnInventoryFull"))
                        {
                            text2 = Localization.Get("wrnInventoryFull");
                        }
                        GameManager.ShowTooltip(entityPlayer, text2);
                        Manager.PlayInsidePlayerHead("ui_denied");
                        __result = false;
                        return false;
                    }
                    else
                    {
                        if (___originalItem != null && !___originalItem.IsEmpty())
                        {
                            if (___recipe.ingredients.Count > 0)
                            {
                                QuestEventManager.Current.ScrappedItem(___recipe.ingredients[0]);
                            }
                        }
                        else
                        {
                            itemStack2.count = ___recipe.count - itemStack2.count;
                            QuestEventManager.Current.CraftedItem(itemStack2);
                        }
                        if (___playSound)
                        {
                            Manager.PlayInsidePlayerHead("craft_complete_item");
                        }
                    }
                }
                if (!___isInventoryFull)
                {
                    ___originalItem = ItemValue.None.Clone();
                }
                if (returnedMod != null)
                {
                    ItemStack itemStack3 = new ItemStack(returnedMod, 1);
                    if (!__instance.xui.PlayerInventory.AddItemNoPartial(itemStack3, false))
                    {
                        __instance.xui.PlayerInventory.DropItem(itemStack3);
                    }
                }

                if (returnedCosmeticMod != null && returnedCosmeticMod.Length > 0)
                {
                    for (int i = 0; i < returnedCosmeticMod.Length; i++)
                    {
                        ItemStack itemStack3 = new ItemStack(returnedCosmeticMod[i], 1);
                        if (!__instance.xui.PlayerInventory.AddItemNoPartial(itemStack3, false))
                        {
                            __instance.xui.PlayerInventory.DropItem(itemStack3);
                        }
                    }
                }

                __result = true;
                return false;
            }
        }
    }

