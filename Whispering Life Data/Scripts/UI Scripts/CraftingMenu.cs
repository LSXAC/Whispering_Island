using Godot;
using Godot.Collections;

public partial class CraftingMenu : PanelContainer
{
    [Export]
    public Control parent;

    public PackedScene recipe_slot = ResourceLoader.Load<PackedScene>("res://item_recipe.tscn");
    public PackedScene no_recipies = ResourceLoader.Load<PackedScene>("res://No_Recipies.tscn");

    [Export]
    public Array<Recipe> crafting_recipies = new Array<Recipe>();

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public void ReloadUIRecipes()
    {
        foreach (Control c in parent.GetChildren())
            c.QueueFree();

        int times = 0;
        for (int i = 0; i < crafting_recipies.Count; i++)
        {
            if (crafting_recipies[i].unlock_requirements != null)
                if (crafting_recipies[i].unlock_requirements.Count > 0)
                    if (
                        !GlobalFunctions.CheckResearchRequirements(
                            crafting_recipies[i].unlock_requirements
                        )
                    )
                        continue;
            times++;
            Recipe recipe = crafting_recipies[i];

            itemRecipeUI irUI = (itemRecipeUI)recipe_slot.Instantiate();
            irUI.craftingMenu = this;
            irUI.output_item_hbox.InitItemUI(
                recipe.output_item.info.name,
                recipe.output_item.amount,
                recipe.output_item.info.texture
            );
            parent.AddChild(irUI);

            //Get whole Inventory onces, to save performance, instead of checken for every item the
            irUI.InitResourceItems(recipe.required_items, recipe.output_item);
            irUI.button_id = i;
            irUI.craft_button.Pressed += () => irUI.CraftItem();

            //Check if all h_box_items return true!
            if (irUI.can_craft)
                irUI.craft_button.Disabled = false;
            else
                irUI.craft_button.Disabled = true;
        }

        if (times == 0)
        {
            Panel p = no_recipies.Instantiate() as Panel;
            parent.AddChild(p);
        }
    }

    public void OnVisiblityChange()
    {
        ReloadUIRecipes();
    }
}
