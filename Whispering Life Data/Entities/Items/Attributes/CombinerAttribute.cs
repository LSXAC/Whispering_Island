using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CombinerAttribute : ItemAttributeBase, IProcessingRecipe
{
    [Export]
    public Item output_item;

    [Export]
    public int amount_to_produce = 1;

    [Export]
    public int processing_time_ms = 2000; // Standard: 2 Sekunden

    [Export]
    public Array<Item> compatible_items; // Items, mit denen dieses Item kombiniert werden kann

    [Export]
    public Array<UnlockRequirement> unlock_requirements;

    public override string GetNameOfAttribute()
    {
        return TranslationServer.Translate("COMBINABLE") + "\n";
    }

    // IProcessingRecipe Implementation
    public ItemInfo GetInputRequirement()
    {
        return null; // Input wird aus dem Slot gelesen, nicht vom Attribute
    }

    public int GetAmountToProcess()
    {
        return amount_to_produce;
    }

    public ItemInfo GetOutputItem()
    {
        return output_item?.info ?? null;
    }

    public int GetProcessingTime()
    {
        return processing_time_ms;
    }

    public Array<UnlockRequirement> GetUnlockRequirements()
    {
        return unlock_requirements;
    }

    public bool IsUnlocked()
    {
        if (unlock_requirements == null || unlock_requirements.Count == 0)
            return true;

        return GlobalFunctions.CheckResearchRequirements(unlock_requirements);
    }

    public int GetItemState()
    {
        return (int)output_item.state;
    }
}
