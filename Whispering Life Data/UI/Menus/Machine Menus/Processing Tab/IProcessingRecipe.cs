using Godot;
using Godot.Collections;

public abstract partial class ProcessingRecipe : Resource
{
    public abstract ItemInfo GetInputRequirement();

    public abstract int GetAmountToProcess(); // Benötigte INPUT-Menge

    public abstract int GetAmountToProduce(); // Produzierte OUTPUT-Menge

    public abstract ItemInfo GetOutputItem();

    public abstract int GetProcessingTime();

    public abstract int GetItemState();

    public abstract Array<UnlockRequirement> GetUnlockRequirements();

    public abstract bool IsUnlocked();
}
