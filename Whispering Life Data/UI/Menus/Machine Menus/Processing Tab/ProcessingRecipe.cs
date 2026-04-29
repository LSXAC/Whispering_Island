using Godot;
using Godot.Collections;

public abstract partial class ProcessingRecipe : Resource
{
    public abstract ItemInfo GetInputRequirement();

    public abstract int GetAmountToProcess();

    public abstract int GetAmountToProduce();

    public abstract ItemInfo GetOutputItem();

    public abstract int GetProcessingTime();

    public abstract int GetItemState();

    public abstract Array<UnlockRequirement> GetUnlockRequirements();

    public abstract bool IsUnlocked();
}
