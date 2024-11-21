using Godot;
using Godot.Collections;

public partial class CraftingMenu : PanelContainer
{
    [Export]
    public Control parent;

    public PackedScene recipe_slot = ResourceLoader.Load<PackedScene>("res://item_recipe.tscn");

    [Export]
    public Array<Recipe> crafting_recipies = new Array<Recipe>();

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public void ReloadUIRecipes()
    {
        foreach (Control c in parent.GetChildren())
            c.QueueFree();

        for (int i = 0; i < crafting_recipies.Count; i++)
        {
            if (crafting_recipies[i].unlockRequirements != null)
                if (crafting_recipies[i].unlockRequirements.Count > 0)
                    if (
                        !GlobalFunctions.CheckAllRequirements(
                            crafting_recipies[i].unlockRequirements
                        )
                    )
                        continue;

            Recipe recipe = crafting_recipies[i];

            itemRecipeUI irUI = (itemRecipeUI)recipe_slot.Instantiate();
            irUI.craftingMenu = this;
            irUI.output_item_hbox.InitItemUI(
                recipe.output_item.item_info.item_name,
                recipe.output_item.amount,
                recipe.output_item.item_info.texture
            );
            parent.AddChild(irUI);

            //Get whole Inventory onces, to save performance, instead of checken for every item the
            irUI.InitResourceItems(recipe.requiered_items, recipe.output_item);
            irUI.button_id = i;
            irUI.craft_button.Pressed += () => irUI.CraftItem();

            //Check if all h_box_items return true!
            if (irUI.can_craft)
                irUI.craft_button.Disabled = false;
            else
                irUI.craft_button.Disabled = true;
        }
    }

    public void OnVisiblityChange()
    {
        ReloadUIRecipes();
    }
}
