using Godot;
using Godot.Collections;

public partial class CraftingMenu : PanelContainer
{
    [Export]
    public Control parent;

    public PackedScene recipe_slot = ResourceLoader.Load<PackedScene>("res://item_recipe.tscn");

    [Export]
    public Array<Resource> crafting_recipies = new Array<Resource>();

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public void ReloadUIRecipes()
    {
        foreach (Control c in parent.GetChildren())
            c.QueueFree();

        for (int i = 0; i < crafting_recipies.Count; i++)
        {
            Recipe recipe = new Recipe();
            recipe = crafting_recipies[i] as Recipe;
            itemRecipeUI irUI = (itemRecipeUI)recipe_slot.Instantiate();
            irUI.craftingMenu = this;
            irUI.outputUI.InitItemUI(
                recipe.output_item.item_info.item_name,
                recipe.output_item.amount,
                recipe.output_item.item_info.texture
            );
            parent.AddChild(irUI);

            //Get whole Inventory onces, to save performance, instead of checken for every item the
            irUI.InitResourceItems(recipe.requiered_items, recipe.output_item);
            irUI.button_id = i;
            irUI.craft_button.Pressed += () => irUI.CraftItem();

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
