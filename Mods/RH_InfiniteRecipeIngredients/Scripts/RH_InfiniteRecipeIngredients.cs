using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

public class XUiC_RH_IngredientList : XUiC_IngredientList
{
    public XUiV_Button btnPageDown;
    public XUiV_Button btnPageUp;
    public XUiV_Label lblPageNumber;
    public int page;
    public int indicatorCount;
    public string currentRecipeName;

    public override void Init()
    {
        base.Init();

        //Parent.Parent.GetChildById("pageUp").OnPress += new XUiEvent_OnPressEventHandler(HandlePageUpPress);
        //Parent.Parent.GetChildById("pageDown").OnPress += new XUiEvent_OnPressEventHandler(HandlePageDownPress);
        btnPageDown = (Parent.Parent.GetChildById("pageDown").ViewComponent as XUiV_Button);
        btnPageUp = (Parent.Parent.GetChildById("pageUp").ViewComponent as XUiV_Button);
        lblPageNumber = (Parent.Parent.GetChildById("pagenumber").ViewComponent as XUiV_Label);
    }

    private void HandlePageUpPress(XUiController _sender, EventArgs _e)
    {
        if ((page + 1) * 5 < Recipe.ingredients.Count)
        {
            page = page + 1;
            IsDirty = true;
        }
    }

    private void HandlePageDownPress(XUiController _sender, EventArgs _e)
    {
        if (page > 0)
        {
            page = page - 1;
            IsDirty = true;
        }
    }

    public override void OnOpen()
    {
        page = 0;
        indicatorCount = 0;
        base.OnOpen();
    }

    public override void Update(float _dt)
    {
        var ingredientEntries = AccessTools.Field(typeof(XUiC_IngredientList), "ingredientEntries").GetValue(this) as List<XUiController>;
        var btnPageUpIsOver = AccessTools.Field(typeof(XUiV_Button), "isOver").GetValue(btnPageUp) as bool?;

        // Check for new or changed recipe.
        if (Recipe != null && (string.IsNullOrEmpty(currentRecipeName) || (Recipe.GetName() != currentRecipeName)))
        {
            page = 0;
            btnPageDown.IsVisible = false;
            indicatorCount = 0;
            if (Recipe.ingredients.Count <= ingredientEntries.Count)
                btnPageUp.IsVisible = false;
            else
                btnPageUp.IsVisible = true;
            currentRecipeName = Recipe.GetName();
            IsDirty = true;
        }

        if (indicatorCount < 1 && btnPageUpIsOver.HasValue &&  !btnPageUpIsOver.Value)
        {
            var indicator = GameTimer.Instance.ticks / 20;
            if (indicator % 2 == 0)
            {
                btnPageUp.CurrentColor = btnPageUp.DefaultSpriteColor;
            }
            else
            {
                btnPageUp.CurrentColor = new Color(0.1921f, 0.3568f, 0.7960f, 0.75f);
            }
        }

        if (btnPageUpIsOver.HasValue && btnPageUpIsOver.Value && indicatorCount < 1)
            indicatorCount = 1;

        if (IsDirty)
        {
            if (Recipe != null)
            {
                int count = ingredientEntries.Count;
                int count2 = Recipe.ingredients.Count;
                int craftingTier = (int)EffectManager.GetValue(PassiveEffects.CraftingTier, null, 1f, xui.playerUI.entityPlayer, Recipe, Recipe.tags, true, true, true, true, 1, true);
                for (int i = 0; i < count; i++)
                {
                    if (ingredientEntries[i] is XUiC_IngredientEntry)
                    {
                        int num = page * count + i;
                        ItemStack itemStack = (num < count2) ? Recipe.ingredients[num].Clone() : null;
                        if (itemStack != null && Recipe.UseIngredientModifier)
                        {
                            itemStack.count = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)itemStack.count, xui.playerUI.entityPlayer, Recipe, FastTags.Parse(itemStack.itemValue.ItemClass.GetItemName()), true, true, true, true, craftingTier, true);
                        }
                        ((XUiC_IngredientEntry)ingredientEntries[i]).Ingredient = itemStack;
                    }
                }
                lblPageNumber.Text = (page + 1).ToString();

                // Hide/show PageDown button 
                if (page == 0)
                   btnPageDown.IsVisible = false;
                else
                    btnPageDown.IsVisible = true;

                // Hide/show PageUp button 
                if ((page + 1) * 5 >= Recipe.ingredients.Count)
                    btnPageUp.IsVisible = false;
                else
                    btnPageUp.IsVisible = true;
            }
            else
            {
                int count3 = ingredientEntries.Count;
                for (int j = 0; j < count3; j++)
                {
                    if (ingredientEntries[j] is XUiC_IngredientEntry)
                    {
                        ((XUiC_IngredientEntry)ingredientEntries[j]).Ingredient = null;
                    }
                }
            }
            IsDirty = false;
        }

        base.Update(_dt);
    }
}