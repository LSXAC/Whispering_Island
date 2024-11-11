using System;
using System.Runtime.CompilerServices;
using Godot;

public partial class building_type : Panel
{
    [Export]
    private Label title_label;

    [Export]
    private TextureRect texture_rect;

    [Export]
    private Button select_button;

    private BuildingType building__type;

    public override void _Ready()
    {
        select_button.Pressed += () => OnSelectButton();
    }

    public void InitBuildingTypeUI(BuildingType building_type)
    {
        Building_Node placeable = building_type.building_scene.Instantiate() as Building_Node;
        title_label.Text = ((placeable_building)placeable).GetTitle();
        texture_rect.Texture = ((placeable_building)placeable).GetSprite().Texture;
        this.building__type = building_type;
    }

    private void OnSelectButton()
    {
        Building_Menu.instance.Visible = false;
        Building_Menu.instance.building_placer.InitBuilding(building__type);
    }
}
