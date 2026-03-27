using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class StateChangingRecipe : ProcessingRecipe
{
    [Export]
    public Array<Item> variable_input_items;

    [Export]
    public Item fixed_input_item;

    [Export]
    public Item output_item;

    [Export]
    public int output_state = (int)Item.STATE.NORMAL;

    [Export]
    public int processing_time_ms = 2000;

    [Export]
    public Array<UnlockRequirement> unlock_requirements;

    public override ItemInfo GetInputRequirement()
    {
        return fixed_input_item?.info ?? null;
    }

    public override int GetAmountToProcess()
    {
        return fixed_input_item?.amount ?? 1;
    }

    public override int GetAmountToProduce()
    {
        return output_item?.amount ?? 1;
    }

    public override ItemInfo GetOutputItem()
    {
        return output_item?.info ?? null;
    }

    public override int GetProcessingTime()
    {
        return processing_time_ms;
    }

    public override Array<UnlockRequirement> GetUnlockRequirements()
    {
        return unlock_requirements;
    }

    public override bool IsUnlocked()
    {
        if (unlock_requirements == null || unlock_requirements.Count == 0)
            return true;

        return GlobalFunctions.CheckResearchRequirements(unlock_requirements);
    }

    public override int GetItemState()
    {
        return output_state;
    }

    public bool IsVariableItemCompatible(ItemInfo item_info)
    {
        if (variable_input_items == null || variable_input_items.Count == 0)
            return false;

        foreach (Item variable_item in variable_input_items)
        {
            if (variable_item != null && variable_item.info.id == item_info.id)
                return true;
        }

        return false;
    }

    public bool CanCombine(ItemInfo first_item, ItemInfo second_item)
    {
        if (first_item == null || second_item == null)
            return false;

        if (fixed_input_item == null)
            return false;

        if (IsVariableItemCompatible(first_item) && fixed_input_item.info.id == second_item.id)
            return true;

        if (IsVariableItemCompatible(second_item) && fixed_input_item.info.id == first_item.id)
            return true;

        return false;
    }
}
