using Godot.Collections;

public interface IProcessingRecipe
{
    ItemInfo GetInputRequirement();

    int GetAmountToProcess();

    ItemInfo GetOutputItem();

    int GetProcessingTime();

    Array<UnlockRequirement> GetUnlockRequirements();

    bool IsUnlocked();
}
