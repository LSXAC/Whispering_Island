using System;
using System.Diagnostics;
using Godot;

public partial class MouseArea : Area2D
{
    private ShaderMaterial outline_shader = ResourceLoader.Load<ShaderMaterial>(
        "res://Shader/Outline_Shader.tres"
    );
    private ShaderMaterial remove_outline_shader = ResourceLoader.Load<ShaderMaterial>(
        "res://Shader/Remove_Outline_Color.tres"
    );

    private Building_Node building_node;

    public override void _Ready()
    {
        building_node = GetParent<Building_Node>();
        if (building_node == null)
        {
            Debug.Print("MouseArea is not a child of Building_Node");
            return;
        }

        if (building_node.disable_collision)
        {
            CollisionShape2D collision_shape = GetNode<CollisionShape2D>("CollisionShape2D");
            collision_shape.Disabled = true;
        }
    }

    private void OutlineBuilding()
    {
        if (building_node.GetSprite() == null)
        {
            Debug.Print("No sprite referenced! Position for hover menu not recognized", this);
            return;
        }

        if (GameManager.building_mode == GameManager.BuildingMode.None)
        {
            if (GetParent() is MineableObject)
                if (((MineableObject)GetParent()).in_cooldown)
                    return;

            building_node.GetSprite().Material = outline_shader;
            building_node.mouse_inside = true;
            hover_menu.InitHoverMenu(building_node);
            Debug.Print("Hover Menu");
        }
        else if (GameManager.building_mode == GameManager.BuildingMode.Removing)
        {
            building_node.GetSprite().Material = remove_outline_shader;
            building_node.mouse_inside = true;
        }
    }

    public void OnMouseEntered()
    {
        if (building_node == null || GameManager.In_Cutscene)
        {
            return;
        }
        OutlineBuilding();
    }

    public void OnMouseLeaved()
    {
        if (building_node == null)
        {
            return;
        }
        if (building_node.GetSprite() == null)
        {
            Debug.Print("No sprite referenced! Position for hover menu not recognized", this);
            return;
        }

        building_node.GetSprite().Material = null;
        if (building_node != null)
        {
            building_node.mouse_inside = false;
            hover_menu.DisableHoverMenu();
        }
    }
}
