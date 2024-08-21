using System;
using Godot;

public partial class BuildingCollider : Area2D
{
    [Export]
    public bool on_building_layer = false;

    [Export]
    private Texture2D grid_outline_green;

    [Export]
    private Texture2D grid_outline_red;
    private TextureRect rect;
    private bool activated = true;

    // Called when the node enters the scene tree for the first time.

    public override void _Ready()
    {
        rect = GetNode<TextureRect>("TextureRect");
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
        if (node.IsInGroup("BuildingCollision"))
        {
            on_building_layer = true;
            rect.Texture = grid_outline_green;
        }
    }

    private void MakeInvisibleRect(bool state)
    {
        activated = state;
        rect.Visible = state;
    }

    public void OnBodyLeaved(Node2D node)
    {
        on_building_layer = false;
        rect.Texture = grid_outline_red;
    }
}
