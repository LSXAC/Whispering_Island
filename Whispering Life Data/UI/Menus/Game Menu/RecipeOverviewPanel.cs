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
        GD.PrintErr("[RecipeOverviewPanel] === ReloadRecipes START ===");

        if (current_building == null)
        {
            GD.PrintErr("[RecipeOverviewPanel] ❌ current_building is NULL!");
            return;
        }

        GD.PrintErr(
            $"[RecipeOverviewPanel] ✓ current_building set: {current_building.GetType().Name}"
        );

        if (recipe_container != null)
        {
            GD.PrintErr("[RecipeOverviewPanel] Clearing old recipe slots...");
            foreach (Control child in recipe_container.GetChildren())
                child.QueueFree();
        }
        else
        {
            GD.PrintErr("[RecipeOverviewPanel] ❌ recipe_container is NULL!");
            return;
        }

        GD.PrintErr($"[RecipeOverviewPanel] requires_recipe: {current_building.requires_recipe}");

        if (!current_building.requires_recipe)
        {
            GD.PrintErr(
                "[RecipeOverviewPanel] ℹ requires_recipe is FALSE - skipping recipe display"
            );
            return;
        }

        Godot.Collections.Array available_recipes = current_building.GetAvailableRecipes();
        GD.PrintErr(
            $"[RecipeOverviewPanel] available_recipes count: {(available_recipes != null ? available_recipes.Count : "NULL")}"
        );

        if (available_recipes == null || available_recipes.Count == 0)
        {
            GD.PrintErr("[RecipeOverviewPanel] ❌ No recipes available!");
            return;
        }

        int loaded_count = 0;
        int filtered_count = 0;

        foreach (ProcessingRecipe recipe in available_recipes)
        {
            if (recipe == null)
            {
                GD.PrintErr("[RecipeOverviewPanel] ⚠ Skipping NULL recipe");
                continue;
            }

            GD.PrintErr($"[RecipeOverviewPanel] Processing recipe: {recipe.GetType().Name}");

            Array<UnlockRequirement> requirements = recipe.GetUnlockRequirements();
            GD.PrintErr(
                $"[RecipeOverviewPanel]   - UnlockRequirements count: {(requirements != null ? requirements.Count : "NULL")}"
            );

            if (requirements != null && requirements.Count > 0)
            {
                bool is_unlocked = GlobalFunctions.CheckResearchRequirements(requirements);
                GD.PrintErr($"[RecipeOverviewPanel]   - IsUnlocked: {is_unlocked}");

                if (!is_unlocked)
                {
                    GD.PrintErr($"[RecipeOverviewPanel]   ↷ Filtered out (not unlocked)");
                    filtered_count++;
                    continue;
                }
            }

            if (recipe_container != null)
            {
                ProcessingRecipeSlot recipe_slot = null;

                GD.PrintErr(
                    $"[RecipeOverviewPanel]   - recipe_slot_scene: {(recipe_slot_scene != null ? "LOADED" : "NULL")}"
                );

                if (recipe_slot_scene != null)
                {
                    try
                    {
                        recipe_slot = recipe_slot_scene.Instantiate<ProcessingRecipeSlot>();
                        GD.PrintErr("[RecipeOverviewPanel]   ✓ Instantiated from scene");
                    }
                    catch (Exception e)
                    {
                        GD.PrintErr(
                            $"[RecipeOverviewPanel]   ❌ Failed to instantiate from scene: {e.Message}"
                        );
                        recipe_slot = null;
                    }
                }
                else
                {
                    GD.PrintErr(
                        "[RecipeOverviewPanel]   ! Creating slot programmatically (no scene)"
                    );
                    recipe_slot = new ProcessingRecipeSlot();
                }

                if (recipe_slot != null)
                {
                    recipe_slot.InitRecipe(recipe, current_building, this);
                    recipe_container.AddChild(recipe_slot);
                    loaded_count++;
                    GD.PrintErr(
                        $"[RecipeOverviewPanel]   ✓ Added to container (total: {loaded_count})"
                    );
                }
                else
                {
                    GD.PrintErr("[RecipeOverviewPanel]   ❌ recipe_slot is still NULL!");
                }
            }
        }

        GD.PrintErr(
            $"[RecipeOverviewPanel] === ReloadRecipes END === Loaded: {loaded_count}, Filtered: {filtered_count} ==="
        );
    }
}
