using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CombinerRecipe : ProcessingRecipe
{
    [Export]
    public Item primary_input_item;

    [Export]
    public Array<Item> compatible_items;

    [Export]
    public Item output_item;

    [Export]
    public int processing_time_ms = 2000;

    [Export]
    public Array<UnlockRequirement> unlock_requirements;

    public override ItemInfo GetInputRequirement()
    {
        return primary_input_item?.info ?? null;
    }

    public override int GetAmountToProcess()
    {
        return primary_input_item != null && primary_input_item.amount > 0
            ? primary_input_item.amount
            : 1;
    }

    public override int GetAmountToProduce()
    {
        return output_item != null && output_item.amount > 0 ? output_item.amount : 1;
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
        return (int)output_item?.state;
    }

    public bool IsItemCompatible(ItemInfo item_info)
    {
        if (compatible_items == null || compatible_items.Count == 0)
            return false;

        foreach (Item compatible_item in compatible_items)
        {
            if (compatible_item != null && compatible_item.info.id == item_info.id)
                return true;
        }

        return false;
    }

    public bool CanCombine(ItemInfo first_item, ItemInfo second_item)
    {
        if (first_item == null || second_item == null)
            return false;

        if (primary_input_item == null)
            return false;

        if (primary_input_item.info.id == first_item.id)
            if (IsItemCompatible(second_item))
                return true;

        if (primary_input_item.info.id == second_item.id)
            if (IsItemCompatible(first_item))
                return true;

        return false;
    }
}
