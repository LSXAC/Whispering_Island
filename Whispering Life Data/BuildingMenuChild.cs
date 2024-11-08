using System;
using Godot;

public partial class BuildingMenuChild : Control
{
    [Export]
    public TextureRect textureRect;

    [Export]
    public Label title_label,
        description_label;

    [Export]
    public HBoxContainer resource_parent_container;

    //ref to Resource_child

    [Export]
    public Button build_button;

    public PackedScene recipe_slot = ResourceLoader.Load<PackedScene>("res://item_recipe.tscn");

    private BuildingType building_type;
    private PackedScene scene_ref;

    public override void _Ready()
    {
        build_button.Pressed += () => OnSelectButton();
    }

    public void InitBuildingMenuChild(BuildingType building_type, int id)
    {
        Building_Node placeable = scene_ref.Instantiate() as Building_Node;
        title_label.Text = ((placeable_building)placeable).GetTitle();
        textureRect.Texture = ((placeable_building)placeable).GetSprite().Texture;
        description_label.Text = ((placeable_building)placeable).GetDescription();
        this.scene_ref = building_type.building_scene;
        //Recipes

        Recipe recipe = new Recipe();
        recipe = building_type.building_recipe as Recipe;
        itemRecipeUI irUI = (itemRecipeUI)recipe_slot.Instantiate();
        irUI.outputUI.InitItemUI(
            recipe.output_item.item_info.item_name,
            recipe.output_item.amount,
            recipe.output_item.item_info.texture
        );
        resource_parent_container.AddChild(irUI);

        //Get whole Inventory onces, to save performance, instead of checken for every item the
        irUI.InitResourceItems(recipe.requiered_items, recipe.output_item);
        irUI.button_id = id;
        irUI.craft_button.Pressed += () => irUI.CraftItem();

        if (irUI.can_craft)
            irUI.craft_button.Disabled = false;
        else
            irUI.craft_button.Disabled = true;
    }

    public void OnSelectButton()
    {
        Building_Menu.instance.Visible = false;
        Building_Menu.instance.building_placer.InitBuilding(scene_ref);
    }
}
