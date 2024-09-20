using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class CraftingMenu : PanelContainer
{
    //TODO After Demo
    //Automate UI Update, Externate Recipies,

    [Export]
    public Control parent;

    public PackedScene recipe_slot = ResourceLoader.Load<PackedScene>("res://item_recipe.tscn");

    [Export]
    public Array<Recipe> crafting_recipies = new Array<Recipe>();

    public static CraftingMenu INSTANCE = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        INSTANCE = this;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public void ReloadUIRecipes()
    {
        foreach (Control c in parent.GetChildren())
            c.QueueFree();

        for (int i = 0; i < crafting_recipies.Count; i++)
        {
            itemRecipeUI irUI = (itemRecipeUI)recipe_slot.Instantiate();
            irUI.outputUI.InitItemUI(
                crafting_recipies[i].output_item.item_info.item_name,
                crafting_recipies[i].output_item.amount,
                crafting_recipies[i].output_item.item_info.texture
            );
            parent.AddChild(irUI);

            //Get whole Inventory onces, to save performance, instead of checken for every item the
            Array<Item> itemsInInventory = Inventory.INSTANCE.GetListOfItemsInInventory();
            irUI.InitResourceItems(
                crafting_recipies[i].requiered_items,
                itemsInInventory,
                crafting_recipies[i].output_item
            );
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
