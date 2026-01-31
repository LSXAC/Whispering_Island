using Godot;
using Godot.Collections;

public partial class CraftingMenu : PanelContainer
{
    [Export]
    public Control parent;

    [Export]
    public CATEGORY category;

    [Export]
    public PackedScene recipe_slot = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/crafting_menu_crafting_line.tscn"
    );
    public PackedScene no_recipies = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/crafting_menu_message_no_recipies.tscn"
    );

    public enum CATEGORY
    {
        BASIC,
        TOOLPARTS,
        TOOLS,
        ARMOR,
        AGRICULTURE,
        MACHINEPARTS
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public void ReloadUIRecipes()
    {
        foreach (Control c in parent.GetChildren())
            c.QueueFree();

        Array<CraftingRecipe> recipies = Database.instance.crafting_recipies_list;
        int times = 0;
        for (int i = 0; i < recipies.Count; i++)
        {
            if (recipies[i].category != category)
                continue;
            if (recipies[i].unlock_requirements != null)
                if (recipies[i].unlock_requirements.Count > 0)
                    if (!GlobalFunctions.CheckResearchRequirements(recipies[i].unlock_requirements))
                        continue;
            times++;
            CraftingRecipe recipe = recipies[i];

            itemRecipeUI irUI = (itemRecipeUI)recipe_slot.Instantiate();
            irUI.craftingMenu = this;
            irUI.output_item_hbox.InitItemUI(recipe.output_item, true);
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
