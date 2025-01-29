using System;
using System.Diagnostics;
using Godot;

public partial class MouseArea : Area2D
{
    [Export]
    private Building_Node building_node;

    [Export]
    public Sprite2D building_sprite;

    private ShaderMaterial outline_shader = ResourceLoader.Load<ShaderMaterial>(
        "res://Shader Objects/Outline_Shader.tres"
    );
    private ShaderMaterial remove_outline_shader = ResourceLoader.Load<ShaderMaterial>(
        "res://Shader Objects/Remove_Outline_Color.tres"
    );

    private void OutlineBuilding()
    {
        if (Game_Manager.building_mode == Game_Manager.BuildingMode.None)
        {
            if (GetParent() is ResourceObject)
                if (((ResourceObject)GetParent()).in_cooldown)
                    return;

            building_sprite.Material = outline_shader;
            building_node.mouse_inside = true;
            hover_menu.InitHoverMenu(building_node);
            Debug.Print("Hover Menu");
        }
        else if (Game_Manager.building_mode == Game_Manager.BuildingMode.Removing)
        {
            building_sprite.Material = remove_outline_shader;
            building_node.mouse_inside = true;
        }
    }

    public void OnMouseEntered()
    {
        if (building_node == null || Game_Manager.In_Cutscene)
        {
            return;
        }
        OutlineBuilding();
    }

    public void OnMouseLeaved()
    {
        building_sprite.Material = null;
        if (building_node != null)
        {
            building_node.mouse_inside = false;
            hover_menu.DisableHoverMenu();
        }
    }
}
