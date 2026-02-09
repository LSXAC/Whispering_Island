using System;
using System.Diagnostics;
using Godot;

public partial class MouseArea : Area2D
{
    private ShaderMaterial outline_shader = ResourceLoader.Load<ShaderMaterial>(
        ResourceUid.UidToPath("uid://cxcu1ol3mxbdf")
    );
    private ShaderMaterial remove_outline_shader = ResourceLoader.Load<ShaderMaterial>(
        ResourceUid.UidToPath("uid://gk1o8yjs6f6y")
    );
    private ShaderMaterial wind_shader = ResourceLoader.Load<ShaderMaterial>(
        ResourceUid.UidToPath("uid://8ujwgfeo1fil")
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
        if (Logger.NodeIsNull(building_node?.sprite_anim_manager?.GetCanvasItem()))
        {
            Debug.Print("No sprite referenced! Position for hover menu not recognized", this);
            return;
        }

        if (GameManager.building_mode == GameManager.BuildingMode.None)
        {
            building_node.sprite_anim_manager.GetCanvasItem().Material = outline_shader;
            building_node.mouse_inside = true;
            hover_menu.InitHoverMenu(building_node);
        }
        else if (GameManager.building_mode == GameManager.BuildingMode.Removing)
        {
            building_node.sprite_anim_manager.GetCanvasItem().Material = remove_outline_shader;
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
        if (Logger.NodeIsNull(building_node?.sprite_anim_manager?.GetCanvasItem()))
        {
            Debug.Print("No sprite referenced! Position for hover menu not recognized", this);
            return;
        }

        if (building_node.use_wind)
            building_node.sprite_anim_manager.GetCanvasItem().Material = wind_shader;
        else
            building_node.sprite_anim_manager.GetCanvasItem().Material = null;

        if (building_node != null)
        {
            building_node.mouse_inside = false;
            hover_menu.DisableHoverMenu();
        }
    }
}
