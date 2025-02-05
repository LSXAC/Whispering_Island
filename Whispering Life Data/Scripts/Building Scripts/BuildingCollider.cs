using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class BuildingCollider : Area2D
{
    [Export]
    public bool on_building_layer = false;

    [Export]
    private Texture2D grid_outline_green;

    [Export]
    private Texture2D grid_outline_red;
    private TextureRect rect;
    public Array<placeable_building.TILETYPE> types;
    private bool activated = true;

    // Called when the node enters the scene tree for the first time.

    public override void _Ready()
    {
        rect = GetNode<TextureRect>("TextureRect");
        MakeInvisibleRect(false);
    }

    public override void _Process(double delta)
    {
        if (Building_Placer.current_building == GetParent().GetParent())
            MakeInvisibleRect(true);
        else if (rect.Texture == grid_outline_green)
            MakeInvisibleRect(false);
    }

    public void OnBodyEntered(Node2D node)
    {
        Calc();
    }

    public void Calc()
    {
        bool on_building = false;
        foreach (Node2D node in GetOverlappingBodies())
        {
            if (node is Building_Node n)
            {
                if (!n.disable_collision)
                {
                    on_building = false;
                    break;
                }
            }

            if (types == null)
                continue;

            foreach (placeable_building.TILETYPE type in types)
                if (node.IsInGroup(type.ToString()))
                    on_building = true;
        }
        if (on_building)
        {
            on_building_layer = true;
            rect.Texture = grid_outline_green;
        }
        else
        {
            on_building_layer = false;
            rect.Texture = grid_outline_red;
        }
    }

    private void MakeInvisibleRect(bool state)
    {
        activated = state;
        rect.Visible = state;
    }

    public void OnBodyLeaved(Node2D node)
    {
        Calc();
    }
}
