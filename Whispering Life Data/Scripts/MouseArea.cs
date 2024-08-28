using System;
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

    public void OnMouseEntered()
    {
        if (building_node == null || Game_Manager.In_Cutscene)
        {
            return;
        }
        OutlineBuilding();
    }

    public void OnMouseClick()
    {
        if (Global.GetDistanceToPlayer(this.GlobalPosition) >= 20f)
            return;
        if (GetParent().HasNode("Actionable"))
            GetParent().GetNode<Actionable>("Actionable").Action();
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

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton buttonevent)
            if (buttonevent.Pressed)
            {
                OnMouseClick();
            }
    }
}
