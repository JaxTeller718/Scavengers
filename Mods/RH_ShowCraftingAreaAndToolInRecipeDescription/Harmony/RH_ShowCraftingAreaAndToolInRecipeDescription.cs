using System;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using DMT;

public class RH_ShowCraftingAreaAndToolInRecipeDescription
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

    [HarmonyPatch(typeof(XUiC_CraftingInfoWindow))]
    [HarmonyPatch("GetBindingValue")]
    [HarmonyPatch(new Type[] { typeof(string), typeof(BindingItem) }, new ArgumentType[] { ArgumentType.Ref, ArgumentType.Normal })]
    class PatchXUiC_CraftingInfoWindowGetBindingValue
    {
        static void Postfix(XUiC_CraftingInfoWindow __instance, ref bool __result, ref string value, ref BindingItem binding, ref Recipe ___recipe)
        {
            string fieldName = binding.FieldName;
            if (fieldName != null && fieldName == "itemdescription")
            {
                string text = string.Empty;
                if (___recipe != null)
                {
                    ItemClass forId = ItemClass.GetForId(___recipe.itemValueType);
                    if (forId != null)
                    {
                        if (forId.IsBlock())
                        {
                            string descriptionKey = Block.list[___recipe.itemValueType].DescriptionKey;
                            if (Localization.Exists(descriptionKey))
                            {
                                text = Localization.Get(descriptionKey);
                            }
                        }
                        else
                        {
                            string itemDescriptionKey = forId.GetItemDescriptionKey();
                            if (Localization.Exists(itemDescriptionKey))
                            {
                                text = Localization.Get(itemDescriptionKey);
                            }
                        }
                        if (!string.IsNullOrEmpty(___recipe.craftingArea))
                        {
                            text = string.Format("{0} ({1} : {2})", text, Localization.Get("CraftingArea"), Localization.Get(___recipe.craftingArea));
                        }
                        if (___recipe.craftingToolType > 0)
                        {
                            ItemClass itemClass = ItemClass.list[___recipe.craftingToolType];
                            if (itemClass != null)
                            {
                                text = string.Format("{0} ({1} : {2})", text, Localization.Get("Tool"), itemClass.GetLocalizedItemName());
                            }
                        }
                        if (forId.IsBlock())
                        {
                            var block = Block.list[___recipe.itemValueType];

                            if(block != null)
                            {
                                var lootListId = 0;
                                if(block is BlockLoot)
                                {
                                    var blockLoot = block as BlockLoot;
                                    var lootList = AccessTools.Field(typeof(BlockLoot), "lootList").GetValue(blockLoot) as int?;
                                    if(lootList.HasValue)
                                        lootListId = lootList.Value;
                                }
                                else if(block is BlockSecureLoot)
                                {
                                    var blockSecureLoot = block as BlockSecureLoot;
                                    var lootList = AccessTools.Field(typeof(BlockSecureLoot), "lootList").GetValue(blockSecureLoot) as int?;
                                    if (lootList.HasValue)
                                        lootListId = lootList.Value;
                                }

                                if (lootListId > 0 )
                                {
                                    if(LootContainer.lootList[lootListId].size != null)
                                        text = string.Format("{0} ({1} : {2})", text, Localization.Get("Slots"), LootContainer.lootList[lootListId].size.x * LootContainer.lootList[lootListId].size.y);
                                }
                            }
                        }
                    }
                }
                value = text;
                __result = true;
            }
        }
    }
}
