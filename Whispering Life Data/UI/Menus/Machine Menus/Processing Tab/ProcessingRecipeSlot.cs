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
            select_button.Pressed += OnSelectRecipe;
    }

    public void InitRecipe(
        ProcessingRecipe recipe,
        ProcessBuilding building,
        RecipeOverviewPanel panel
    )
    {
        this.recipe = recipe;
        this.building = building;
        this.overview_panel = panel;

        if (recipe == null)
            return;

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
                }

                ItemInfo secondary_input = combiner_recipe.GetSecondaryInputRequirement();
                if (secondary_input != null)
                {
                    required_items.Add(
                        new Item(secondary_input, combiner_recipe.GetSecondaryAmountToProcess())
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
                }
            }
            else
            {
                ItemInfo input_req = recipe.GetInputRequirement();

                if (input_req != null)
                {
                    required_items.Add(new Item(input_req, recipe.GetAmountToProcess()));
                }
            }

            if (required_items.Count > 0)
                input_row_manager.SetResourcesOnUI(required_items, no_dev_list: true);
        }

        if (output_item_display != null)
        {
            ItemInfo output_item = recipe.GetOutputItem();

            if (output_item != null)
            {
                output_item_display.InitItemUI(
                    new Item(output_item, recipe.GetAmountToProduce()),
                    with_name: true
                );
            }
        }

        UpdateButtonState();
    }

    private void OnSelectRecipe()
    {
        if (building == null || recipe == null)
            return;

        building.SelectRecipe(recipe);
        overview_panel?.UpdateRecipeButtonStates();
    }

    public void UpdateButtonState()
    {
        if (select_button == null)
            return;

        bool is_selected = building != null && building.selected_recipe == recipe;

        if (is_selected)
        {
            select_button.AddThemeColorOverride("font_color", Colors.Gold);
            select_button.Text = "Selected";
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
            select_button.Pressed -= OnSelectRecipe;
    }
}
