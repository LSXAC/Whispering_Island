using System;
using Godot;

public partial class MouseArea : Area2D
{
    private ShaderMaterial outline_shader = ResourceLoader.Load<ShaderMaterial>(
        "res://Shader Objects/Outline_Shader.tres"
    );
    private ShaderMaterial remove_outline_shader = ResourceLoader.Load<ShaderMaterial>(
        "res://Shader Objects/Remove_Outline_Color.tres"
    );

    [Export]
    private Building_Node building_node;

    [Export]
    public Sprite2D building_sprite;

    public void OnMouseEntered()
    {
        if (Game_Manager.building_mode == Game_Manager.BuildingMode.None)
        {
            building_sprite.Material = outline_shader;
            building_node.mouse_inside = true;
            if (!building_node.title.ToString().ToUpper().Contains("BELT"))
                hover_menu.InitHoverMenu(building_node);
        }
        else if (Game_Manager.building_mode == Game_Manager.BuildingMode.Removing)
        {
            building_sprite.Material = remove_outline_shader;
            building_node.mouse_inside = true;
        }
    }

    public void OnMouseLeaved()
    {
        building_sprite.Material = null;
        building_node.mouse_inside = false;
        hover_menu.DisableHoverMenu();
    }
}
