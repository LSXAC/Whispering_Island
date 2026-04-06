using System;
using Godot;
using Godot.Collections;

public partial class ProcessingRecipeSlot : Control
{
    [Export]
    public ItemRowManager input_row_manager;

    [Export]
    public h_box_item output_item_display;

    [Export]
    public Button select_button;

    public ProcessingRecipe recipe;
    public ProcessBuilding building;
    public RecipeOverviewPanel overview_panel;

    public override void _Ready()
    {
        if (select_button != null)
        {
            select_button.Pressed += OnSelectRecipe;
        }
    }

    public void InitRecipe(
        ProcessingRecipe recipe,
        ProcessBuilding building,
        RecipeOverviewPanel panel
    )
    {
        GD.PrintErr("[ProcessingRecipeSlot.InitRecipe] START");
        GD.PrintErr(
            $"[ProcessingRecipeSlot.InitRecipe]   recipe: {(recipe != null ? recipe.GetType().Name : "NULL")}"
        );
        GD.PrintErr(
            $"[ProcessingRecipeSlot.InitRecipe]   building: {(building != null ? building.GetType().Name : "NULL")}"
        );
        GD.PrintErr(
            $"[ProcessingRecipeSlot.InitRecipe]   panel: {(panel != null ? panel.GetType().Name : "NULL")}"
        );

        this.recipe = recipe;
        this.building = building;
        this.overview_panel = panel;

        if (recipe == null)
        {
            GD.PrintErr("[ProcessingRecipeSlot.InitRecipe] ❌ recipe is NULL, returning");
            return;
        }

        GD.PrintErr(
            $"[ProcessingRecipeSlot.InitRecipe]   input_row_manager: {(input_row_manager != null ? "✓ FOUND" : "❌ NULL")}"
        );
        GD.PrintErr(
            $"[ProcessingRecipeSlot.InitRecipe]   output_item_display: {(output_item_display != null ? "✓ FOUND" : "❌ NULL")}"
        );
        GD.PrintErr(
            $"[ProcessingRecipeSlot.InitRecipe]   select_button: {(select_button != null ? "✓ FOUND" : "❌ NULL")}"
        );

        if (input_row_manager != null)
        {
            Array<Item> required_items = new Array<Item>();

            if (recipe is CombinerRecipe combiner_recipe)
            {
                ItemInfo primary_input = combiner_recipe.GetInputRequirement();
                if (primary_input != null)
                {
                    required_items.Add(
                        new Item(primary_input, combiner_recipe.GetAmountToProcess())
                    );
                    GD.PrintErr(
                        $"[ProcessingRecipeSlot.InitRecipe]   ✓ Added primary input: {primary_input.name}"
                    );
                }

                ItemInfo secondary_input = combiner_recipe.GetSecondaryInputRequirement();
                if (secondary_input != null)
                {
                    required_items.Add(
                        new Item(secondary_input, combiner_recipe.GetSecondaryAmountToProcess())
                    );
                    GD.PrintErr(
                        $"[ProcessingRecipeSlot.InitRecipe]   ✓ Added secondary input: {secondary_input.name}"
                    );
                }
            }
            else if (recipe is StateChangingRecipe state_changing_recipe)
            {
                ItemInfo primary_input = state_changing_recipe.GetInputRequirement();
                if (primary_input != null)
                {
                    required_items.Add(
                        new Item(primary_input, state_changing_recipe.GetAmountToProcess())
                    );
                    GD.PrintErr(
                        $"[ProcessingRecipeSlot.InitRecipe]   ✓ Added primary input: {primary_input.name}"
                    );
                }

                ItemInfo secondary_input = state_changing_recipe.GetSecondaryInputRequirement();
                if (secondary_input != null)
                {
                    required_items.Add(
                        new Item(
                            secondary_input,
                            state_changing_recipe.GetSecondaryAmountToProcess()
                        )
                    );
                    GD.PrintErr(
                        $"[ProcessingRecipeSlot.InitRecipe]   ✓ Added secondary input: {secondary_input.name}"
                    );
                }
            }
            else
            {
                ItemInfo input_req = recipe.GetInputRequirement();
                GD.PrintErr(
                    $"[ProcessingRecipeSlot.InitRecipe]   input_req: {(input_req != null ? input_req.name : "NULL")}"
                );

                if (input_req != null)
                {
                    required_items.Add(new Item(input_req, recipe.GetAmountToProcess()));
                    GD.PrintErr($"[ProcessingRecipeSlot.InitRecipe]   ✓ Set input requirements");
                }
            }

            if (required_items.Count > 0)
            {
                input_row_manager.SetResourcesOnUI(required_items, no_dev_list: true);
            }
        }
        else
        {
            GD.PrintErr(
                "[ProcessingRecipeSlot.InitRecipe]   ⚠ input_row_manager null, skipping input"
            );
        }

        if (output_item_display != null)
        {
            ItemInfo output_item = recipe.GetOutputItem();
            GD.PrintErr(
                $"[ProcessingRecipeSlot.InitRecipe]   output_item: {(output_item != null ? output_item.name : "NULL")}"
            );

            if (output_item != null)
            {
                output_item_display.InitItemUI(
                    new Item(output_item, recipe.GetAmountToProduce()),
                    with_name: true
                );
                GD.PrintErr($"[ProcessingRecipeSlot.InitRecipe]   ✓ Set output item");
            }
        }
        else
        {
            GD.PrintErr(
                "[ProcessingRecipeSlot.InitRecipe]   ⚠ output_item_display null, skipping output"
            );
        }

        UpdateButtonState();
        GD.PrintErr("[ProcessingRecipeSlot.InitRecipe] END");
    }

    private void OnSelectRecipe()
    {
        if (building == null || recipe == null)
            return;

        building.SelectRecipe(recipe);
        overview_panel?.ReloadRecipes();
    }

    public void UpdateButtonState()
    {
        if (select_button == null)
            return;

        bool is_selected = building != null && building.selected_recipe == recipe;

        if (is_selected)
        {
            select_button.AddThemeColorOverride("font_color", Colors.Gold);
            select_button.Text = "✓ Selected";
        }
        else
        {
            select_button.RemoveThemeColorOverride("font_color");
            select_button.Text = "Select";
        }
    }

    public override void _ExitTree()
    {
        if (select_button != null)
        {
            select_button.Pressed -= OnSelectRecipe;
        }
    }
}
