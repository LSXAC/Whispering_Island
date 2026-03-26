using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SmeltableAttribute : ItemAttributeBase, IProcessingRecipe
{
    [Export]
    public Item smelted_to_item;

    [Export]
    public int amount_to_smelt = 1;

    [Export]
    public int export_amount = 1;

    [Export]
    public int processing_time_ms = 2000; // Standard: 2 Sekunden

    [Export]
    public Array<UnlockRequirement> unlock_requirements;

    public override string GetNameOfAttribute()
    {
        return TranslationServer.Translate("SMELTABLE") + "\n";
    }

    // IProcessingRecipe Implementation
    public ItemInfo GetInputRequirement()
    {
        return null; // Input wird aus dem Slot gelesen, nicht vom Attribute
    }

    public int GetAmountToProcess()
    {
        return export_amount;
    }

    public ItemInfo GetOutputItem()
    {
        return smelted_to_item?.info ?? null;
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
}
