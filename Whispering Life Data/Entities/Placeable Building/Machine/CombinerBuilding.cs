using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class CombinerBuilding : ProcessBuilding
{
    public enum SlotType
    {
        INPUT_1 = 0,
        OUTPUT = 2,
        FUEL = 3,
        INPUT_2 = 1,
    };

    [Export]
    public Array<CombinerRecipe> combiner_recipes = new Array<CombinerRecipe>();

    public override void _Ready()
    {
        base._Ready();

        if (item_array.Length < 4)
            System.Array.Resize(ref item_array, 4);

        // Kopiere combiner_recipes in die generische recipes Array
        CollectAvailableRecipes();
    }

    protected override void CollectAvailableRecipes()
    {
        recipes.Clear();
        foreach (CombinerRecipe recipe in combiner_recipes)
        {
            recipes.Add(recipe);
        }
    }

    protected override ProcessingRecipe GetRecipeFromInputSlot()
    {
        if (selected_recipe != null)
        {
            if (
                item_array[(int)SlotType.INPUT_1] != null
                && item_array[(int)SlotType.INPUT_2] != null
            )
            {
                ItemInfo info1 = Inventory.ITEM_TYPES[
                    (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_1].item_id
                ];
                ItemInfo info2 = Inventory.ITEM_TYPES[
                    (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_2].item_id
                ];

                CombinerRecipe combiner_recipe = selected_recipe as CombinerRecipe;
                if (
                    combiner_recipe != null
                    && combiner_recipe.CanCombine(info1, info2)
                    && item_array[(int)SlotType.INPUT_1].amount
                        >= combiner_recipe.GetAmountToProcess()
                )
                    return combiner_recipe;
            }
            return null;
        }

        if (item_array[(int)SlotType.INPUT_1] == null || item_array[(int)SlotType.INPUT_2] == null)
            return null;

        ItemInfo info1_default = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_1].item_id
        ];
        ItemInfo info2_default = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_2].item_id
        ];

        foreach (CombinerRecipe combiner_recipe in combiner_recipes)
        {
            if (combiner_recipe != null && combiner_recipe.IsUnlocked())
            {
                if (
                    combiner_recipe.CanCombine(info1_default, info2_default)
                    && item_array[(int)SlotType.INPUT_1].amount
                        >= combiner_recipe.GetAmountToProcess()
                )
                    return combiner_recipe;
            }
        }

        return null;
    }

    protected override bool SelectAndCheckCanCraft()
    {
        if (item_array[(int)SlotType.INPUT_1] == null || item_array[(int)SlotType.INPUT_2] == null)
            return false;

        ItemInfo input1_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_1].item_id
        ];
        ItemInfo input2_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_2].item_id
        ];

        ProcessingRecipe recipe = null;

        if (selected_recipe != null)
        {
            CombinerRecipe combiner_recipe = selected_recipe as CombinerRecipe;
            if (combiner_recipe == null)
                return false;

            if (
                !combiner_recipe.IsUnlocked()
                || !combiner_recipe.CanCombine(input1_info, input2_info)
                || item_array[(int)SlotType.INPUT_1].amount < combiner_recipe.GetAmountToProcess()
            )
                return false;

            recipe = combiner_recipe;
        }
        else
        {
            foreach (CombinerRecipe combiner_recipe in combiner_recipes)
            {
                if (
                    combiner_recipe != null
                    && combiner_recipe.IsUnlocked()
                    && combiner_recipe.CanCombine(input1_info, input2_info)
                    && item_array[(int)SlotType.INPUT_1].amount
                        >= combiner_recipe.GetAmountToProcess()
                )
                {
                    recipe = combiner_recipe;
                    break;
                }
            }
        }

        if (recipe == null)
            return false;

        if (item_array[(int)SlotType.FUEL] == null && fuel_left <= 0)
            return false;

        if (item_array[(int)SlotType.INPUT_1].amount < recipe.GetAmountToProcess())
            return false;

        ItemInfo expected_output = recipe.GetOutputItem();

        if (expected_output != null)
        {
            if (item_array[(int)SlotType.OUTPUT] == null)
                return true;
            else if (item_array[(int)SlotType.OUTPUT].item_id != (int)expected_output.id)
                return false;
            else if (item_array[(int)SlotType.OUTPUT].state != recipe.GetItemState())
                return false;
        }

        return true;
    }

    protected override CombinerTab GetUIUpdater()
    {
        return CombinerTab.instance;
    }

    public override int GetSlotIndexByPurpose(SlotPurpose purpose)
    {
        return purpose switch
        {
            SlotPurpose.INPUT => (int)SlotType.INPUT_1, // Primärer Input
            SlotPurpose.OUTPUT => (int)SlotType.OUTPUT,
            SlotPurpose.FUEL => (int)SlotType.FUEL,
            SlotPurpose.AUXILIARY => (int)SlotType.INPUT_2, // Sekundärer Input
            _ => -1
        };
    }

    protected override bool CanItemFitInInputSlot(Inventory.ITEM_ID item_id)
    {
        ItemInfo item_info = Inventory.ITEM_TYPES[item_id];
        if (item_info == null)
            return false;

        if (selected_recipe == null)
            return true;

        CombinerRecipe combiner_recipe = selected_recipe as CombinerRecipe;
        if (combiner_recipe != null)
        {
            if (item_array[(int)SlotType.INPUT_2] == null)
            {
                if (
                    combiner_recipe.GetInputRequirement() != null
                    && combiner_recipe.GetInputRequirement().id == item_info.id
                )
                    return true;

                if (combiner_recipe.IsItemCompatible(item_info))
                    return true;

                return false;
            }

            ItemInfo input2_info = Inventory.ITEM_TYPES[
                (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_2].item_id
            ];

            return combiner_recipe.CanCombine(item_info, input2_info);
        }

        return false;
    }

    protected override bool CanItemFitInAuxiliarySlot(Inventory.ITEM_ID item_id)
    {
        ItemInfo item_info = Inventory.ITEM_TYPES[item_id];
        if (item_info == null)
            return false;

        if (item_array[(int)SlotType.INPUT_1] == null)
            return true;

        ItemInfo input1_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_1].item_id
        ];

        if (selected_recipe == null)
            return true;

        CombinerRecipe combiner_recipe = selected_recipe as CombinerRecipe;
        if (combiner_recipe != null)
            return combiner_recipe.CanCombine(input1_info, item_info);

        return false;
    }

    protected override void OpenGameMenuTab()
    {
        GameMenu.instance.OnOpenPoisonCombinerTab();
    }
}
