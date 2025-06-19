using System;
using System.Diagnostics;
using Godot;

public partial class BuildingMenuChild : Control
{
    [Export]
    public TextureRect textureRect;

    [Export]
    public Label title_label,
        description_label;

    [Export]
    public Button build_button;

    [Export]
    public Item_Row_Manager item_row_manager;

    private Building_Menu_List_Object_Info building_type;

    public override void _Ready()
    {
        build_button.Pressed += () => OnSelectButton();
    }

    public void InitBuildingMenuChild(Building_Menu_List_Object_Info building_type)
    {
        this.building_type = building_type;
        Building_Node placeable = building_type.scene.Instantiate() as Building_Node;
        try
        {
            title_label.Text = ((placeable_building)placeable).GetTitle();
            textureRect.Texture = ((placeable_building)placeable).GetSprite().Texture;
            description_label.Text = ((placeable_building)placeable).GetDescription();

            Recipe recipe = new Recipe();
            recipe = building_type.recipe;

            if (item_row_manager.CanCreate(recipe.requiered_items))
                build_button.Disabled = false;
            else
                build_button.Disabled = true;
        }
        catch (Exception e)
        {
            Debug.Print("Objekt: " + placeable.Name + " has problems");
            Debug.Print(e.StackTrace);
        }
    }

    public void OnSelectButton()
    {
        Building_Menu.instance.Visible = false;
        Building_Menu.instance.building_placer.InitBuildingFromBuildingMenu(building_type);
    }
}
