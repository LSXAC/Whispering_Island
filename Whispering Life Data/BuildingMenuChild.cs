using System;
using Godot;

public partial class BuildingMenuChild : Control
{
    [Export]
    public TextureRect textureRect;

    [Export]
    public Label title_label,
        description_label;

    //ref to Resource_child

    [Export]
    public Button build_button;

    [Export]
    public Item_Row_Manager item_row_manager;

    private BuildingType building_type;
    private PackedScene scene_ref;

    public override void _Ready()
    {
        build_button.Pressed += () => OnSelectButton();
    }

    public void InitBuildingMenuChild(BuildingType building_type)
    {
        this.scene_ref = building_type.building_scene;

        Building_Node placeable = scene_ref.Instantiate() as Building_Node;
        title_label.Text = ((placeable_building)placeable).GetTitle();
        textureRect.Texture = ((placeable_building)placeable).GetSprite().Texture;
        description_label.Text = ((placeable_building)placeable).GetDescription();
        //Recipes

        Recipe recipe = new Recipe();
        recipe = building_type.building_recipe as Recipe;

        if (item_row_manager.CanCreate(recipe.requiered_items))
            build_button.Disabled = false;
        else
            build_button.Disabled = true;
    }

    public void OnSelectButton()
    {
        Building_Menu.instance.Visible = false;
        Building_Menu.instance.building_placer.InitBuilding(scene_ref);
    }
}
