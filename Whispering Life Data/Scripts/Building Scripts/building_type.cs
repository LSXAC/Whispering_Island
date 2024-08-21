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

    private PackedScene scene_ref;

    public override void _Ready()
    {
        select_button.Pressed += () => OnSelectButton();
    }

    public void InitBuildingTypeUI(PackedScene scene_ref)
    {
        Building_Node placeable = scene_ref.Instantiate() as Building_Node;
        if (placeable is placeable_building)
        {
            title_label.Text = ((placeable_building)placeable).building_name;
            texture_rect.Texture = ((placeable_building)placeable).building_texture;
        }
        this.scene_ref = scene_ref;
    }

    private void OnSelectButton()
    {
        Game_Manager.in_building_mode = true;
        Building_Menu.instance.Visible = false;
        Building_Menu.instance.building_placer.InitBuilding(scene_ref);
    }
}
