using System;
using Godot;

public partial class MouseArea : Area2D
{
    private ShaderMaterial outline_shader = ResourceLoader.Load<ShaderMaterial>(
        "res://Outline_Shader.tres"
    );

    [Export]
    private Building_Node building_node;

    [Export]
    public Sprite2D building_sprite;

    public void OnMouseEntered()
    {
        building_sprite.Material = outline_shader;
        building_node.mouse_inside = true;
        hover_menu.InitHoverMenu(building_node);
    }

    public void OnMouseLeaved()
    {
        building_sprite.Material = null;
        building_node.mouse_inside = false;
        hover_menu.DisableHoverMenu();
    }
}
