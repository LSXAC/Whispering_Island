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

    private Building_Menu_List_Object scene_info;

    public override void _Ready()
    {
        select_button.Pressed += () => OnSelectButton();
    }

    public void InitBuildingTypeUI(Building_Menu_List_Object scene_info)
    {
        Building_Node placeable = scene_info.scene.Instantiate() as Building_Node;
        title_label.Text = ((placeable_building)placeable).GetTitle();
        texture_rect.Texture = ((placeable_building)placeable).GetSprite().Texture;
        this.scene_info = scene_info;
    }

    private void OnSelectButton()
    {
        BuildMenu.instance.Visible = false;
        BuildMenu.instance.building_placer.InitBuildingFromBuildingMenu(scene_info);
    }
}
