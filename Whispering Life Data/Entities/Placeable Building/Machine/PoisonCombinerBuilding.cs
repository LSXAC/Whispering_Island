using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class PoisonCombinerBuilding : ProcessBuilding
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

    // State Changing Mode
    [Export]
    public bool enable_state_changing_mode = false;

    [Export]
    public Array<Item> state_changing_variable_items = new Array<Item>();

    [Export]
    public Item state_changing_fixed_item;

    [Export]
    public int state_changing_output_state = (int)Item.STATE.NORMAL;

    public override void _Ready()
    {
        base._Ready();

        if (item_array.Length < 4)
            System.Array.Resize(ref item_array, 4);
    }

    protected override ProcessingRecipe GetRecipeFromInputSlot()
    {
        // Wenn StateChanging Mode aktiviert ist
        if (enable_state_changing_mode)
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

                if (IsStateChangingCompatible(info1, info2))
                {
                    var recipe = CreateStateChangingRecipe();
                    if (
                        recipe != null
                        && item_array[(int)SlotType.INPUT_1].amount >= recipe.GetAmountToProcess()
                    )
                        return recipe;
                }
            }
            return null;
        }

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

    private bool IsStateChangingCompatible(ItemInfo first_item, ItemInfo second_item)
    {
        if (state_changing_fixed_item == null)
            return false;

        if (state_changing_fixed_item.info.id == first_item.id)
            return IsStateChangingVariableItem(second_item);

        if (state_changing_fixed_item.info.id == second_item.id)
            return IsStateChangingVariableItem(first_item);

        return false;
    }

    private bool IsStateChangingVariableItem(ItemInfo item_info)
    {
        if (state_changing_variable_items == null || state_changing_variable_items.Count == 0)
            return false;

        foreach (Item variable_item in state_changing_variable_items)
            if (variable_item != null && variable_item.info.id == item_info.id)
                return true;

        return false;
    }

    private StateChangingRecipe CreateStateChangingRecipe()
    {
        var recipe = new StateChangingRecipe();
        recipe.variable_input_items = state_changing_variable_items;
        recipe.fixed_input_item = state_changing_fixed_item;
        recipe.output_state = state_changing_output_state;
        recipe.processing_time_ms = 2000;
        return recipe;
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

        if (enable_state_changing_mode)
        {
            if (!IsStateChangingCompatible(input1_info, input2_info))
                return false;

            recipe = CreateStateChangingRecipe();
        }
        else if (selected_recipe != null)
        {
            recipe = selected_recipe;
            if (recipe == null)
                return false;

            CombinerRecipe combiner_recipe = recipe as CombinerRecipe;
            if (combiner_recipe == null || !combiner_recipe.CanCombine(input1_info, input2_info))
                return false;
        }
        else
        {
            foreach (CombinerRecipe combiner_recipe in combiner_recipes)
            {
                if (combiner_recipe != null && combiner_recipe.IsUnlocked())
                {
                    if (combiner_recipe.CanCombine(input1_info, input2_info))
                    {
                        recipe = combiner_recipe;
                        break;
                    }
                }
            }
        }

        if (recipe == null)
            return false;

        if (!recipe.IsUnlocked())
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
        }

        return true;
    }

    protected override CombinerTab GetUIUpdater()
    {
        return CombinerTab.instance;
    }

    protected override int GetSlotIndexByPurpose(SlotPurpose purpose)
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

        if (enable_state_changing_mode)
        {
            if (item_array[(int)SlotType.INPUT_2] == null)
            {
                return IsStateChangingVariableItem(item_info)
                    || (
                        state_changing_fixed_item != null
                        && state_changing_fixed_item.info.id == item_info.id
                    );
            }

            ItemInfo input2_info = Inventory.ITEM_TYPES[
                (Inventory.ITEM_ID)item_array[(int)SlotType.INPUT_2].item_id
            ];
            return IsStateChangingCompatible(item_info, input2_info);
        }

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

        if (enable_state_changing_mode)
            return IsStateChangingCompatible(input1_info, item_info);

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
