using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SmeltableRecipe : ProcessingRecipe
{
    [Export]
    public Item input_item;

    [Export]
    public Item output_item;

    [Export]
    public int processing_time_ms = 2000;

    [Export]
    public Array<UnlockRequirement> unlock_requirements;

    public override ItemInfo GetInputRequirement()
    {
        return input_item?.info ?? null;
    }

    public override int GetAmountToProcess()
    {
        return input_item != null && input_item.amount > 0 ? input_item.amount : 1;
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
}
