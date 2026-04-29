using System;
using Godot;
using Godot.Collections;

public partial class RecipeOverviewPanel : PanelContainer
{
    [Export]
    public Control recipe_container;

    [Export]
    public PackedScene recipe_slot_scene = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://bcjpv15dugfyi")
    );

    private ProcessBuilding current_building = null;

    public override void _Ready() { }

    public void SetReferenceBuilding(ProcessBuilding building)
    {
        current_building = building;
        ReloadRecipes();
    }

    public void ReloadRecipes()
    {
        if (current_building == null)
        {
            return;
        }

        if (recipe_container != null)
        {
            foreach (Control child in recipe_container.GetChildren())
                child.QueueFree();
        }
        else
        {
            return;
        }

        if (!current_building.requires_recipe)
        {
            return;
        }

        Godot.Collections.Array available_recipes = current_building.GetAvailableRecipes();

        if (available_recipes == null || available_recipes.Count == 0)
        {
            return;
        }

        int loaded_count = 0;
        int filtered_count = 0;

        foreach (ProcessingRecipe recipe in available_recipes)
        {
            if (recipe == null)
                continue;

            Array<UnlockRequirement> requirements = recipe.GetUnlockRequirements();

            if (requirements != null && requirements.Count > 0)
            {
                bool is_unlocked = GlobalFunctions.CheckResearchRequirements(requirements);

                if (!is_unlocked)
                {
                    continue;
                }
            }

            if (recipe_container != null)
            {
                ProcessingRecipeSlot recipe_slot = null;

                if (recipe_slot_scene != null)
                {
                    try
                    {
                        recipe_slot = recipe_slot_scene.Instantiate<ProcessingRecipeSlot>();
                    }
                    catch (Exception e) { }
                }
                else
                {
                    recipe_slot = new ProcessingRecipeSlot();
                }

                if (recipe_slot != null)
                {
                    recipe_slot.InitRecipe(recipe, current_building, this);
                    recipe_container.AddChild(recipe_slot);
                    loaded_count++;
                }
                else { }
            }
        }
    }
}
