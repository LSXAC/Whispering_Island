using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class AlchemyLabBuilding : ProcessBuilding
{
    public enum SlotType
    {
        PRIMARY_INPUT = 0,
        SECONDARY_INPUT = 1,
        OUTPUT = 2,
        FUEL = 3,
    };

    [Export]
    public Array<StateChangingRecipe> state_changing_recipes = new Array<StateChangingRecipe>();

    public override void _Ready()
    {
        base._Ready();

        if (item_array.Length < 4)
            System.Array.Resize(ref item_array, 4);
    }

    protected override ProcessingRecipe GetRecipeFromInputSlot()
    {
        if (
            item_array[(int)SlotType.PRIMARY_INPUT] == null
            || item_array[(int)SlotType.SECONDARY_INPUT] == null
        )
            return null;

        ItemInfo info1 = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.PRIMARY_INPUT].item_id
        ];
        ItemInfo info2 = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.SECONDARY_INPUT].item_id
        ];

        // Suche nach passendem StateChangingRecipe
        foreach (StateChangingRecipe recipe in state_changing_recipes)
        {
            if (recipe != null && recipe.IsUnlocked())
            {
                if (
                    recipe.CanCombine(info1, info2)
                    && item_array[(int)SlotType.PRIMARY_INPUT].amount >= recipe.GetAmountToProcess()
                )
                    return recipe;
            }
        }

        return null;
    }

    protected override bool SelectAndCheckCanCraft()
    {
        if (
            item_array[(int)SlotType.PRIMARY_INPUT] == null
            || item_array[(int)SlotType.SECONDARY_INPUT] == null
        )
            return false;

        ItemInfo input1_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.PRIMARY_INPUT].item_id
        ];
        ItemInfo input2_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.SECONDARY_INPUT].item_id
        ];

        ProcessingRecipe recipe = null;

        foreach (StateChangingRecipe state_recipe in state_changing_recipes)
        {
            if (state_recipe != null && state_recipe.IsUnlocked())
            {
                if (state_recipe.CanCombine(input1_info, input2_info))
                {
                    recipe = state_recipe;
                    break;
                }
            }
        }

        if (recipe == null)
            return false;

        if (!recipe.IsUnlocked())
            return false;

        if (item_array[(int)SlotType.PRIMARY_INPUT].amount < recipe.GetAmountToProcess())
            return false;

        if (item_array[(int)SlotType.FUEL] == null || fuel_left <= 0)
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

    protected override ProcessingTab GetUIUpdater()
    {
        // TODO: Erstelle eine AlchemyLabTab UI-Klasse oder verwende eine bestehende
        return null;
    }

    protected override int GetSlotIndexByPurpose(SlotPurpose purpose)
    {
        return purpose switch
        {
            SlotPurpose.INPUT => (int)SlotType.PRIMARY_INPUT,
            SlotPurpose.OUTPUT => (int)SlotType.OUTPUT,
            SlotPurpose.FUEL => (int)SlotType.FUEL,
            SlotPurpose.AUXILIARY => (int)SlotType.SECONDARY_INPUT,
            _ => -1
        };
    }

    protected override bool CanItemFitInInputSlot(Inventory.ITEM_ID item_id)
    {
        ItemInfo item_info = Inventory.ITEM_TYPES[item_id];
        if (item_info == null)
            return false;

        if (item_array[(int)SlotType.SECONDARY_INPUT] == null)
            return true;

        ItemInfo secondary_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.SECONDARY_INPUT].item_id
        ];

        // Prüfe ob Item mit sekundärem Input kompatibel ist
        foreach (StateChangingRecipe recipe in state_changing_recipes)
        {
            if (recipe != null && recipe.CanCombine(item_info, secondary_info))
                return true;
        }

        return false;
    }

    protected override bool CanItemFitInAuxiliarySlot(Inventory.ITEM_ID item_id)
    {
        ItemInfo item_info = Inventory.ITEM_TYPES[item_id];
        if (item_info == null)
            return false;

        if (item_array[(int)SlotType.PRIMARY_INPUT] == null)
            return true;

        ItemInfo primary_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)SlotType.PRIMARY_INPUT].item_id
        ];

        // Prüfe ob Item mit primärem Input kompatibel ist
        foreach (StateChangingRecipe recipe in state_changing_recipes)
        {
            if (recipe != null && recipe.CanCombine(primary_info, item_info))
                return true;
        }

        return false;
    }

    protected override void OpenGameMenuTab()
    {
        // TODO: Implementiere GameMenu.instance.OnOpenAlchemyLabTab();
    }
}
