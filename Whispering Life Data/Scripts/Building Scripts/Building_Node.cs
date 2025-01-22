using System;
using System.Diagnostics;
using Godot;

public abstract partial class Building_Node : Node2D
{
    [Export]
    private string title = "";

    [Export]
    private string description = "";

    [Export]
    public Sprite2D sprite;

    [Export]
    public CollisionShape2D collision_shape;

    public bool mouse_inside = false;

    public abstract void OnMouseClick();

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton buttonevent)
            if (buttonevent.ButtonIndex == MouseButton.Left && buttonevent.Pressed && mouse_inside)
            {
                OnMouseClick();
            }
    }

    public Sprite2D GetSprite()
    {
        if (sprite != null)
            return sprite;
        return null;
    }

    public string GetTitle()
    {
        if (title.Length == 0)
            Debug.Print("Title is Empty in: ", this);
        return title;
    }

    public string GetDescription()
    {
        if (description.Length == 0)
            Debug.Print("Description is Empty in: ", this);
        return description;
    }

    public Vector2 GetBuildingPosition()
    {
        if (GetSprite() == null)
        {
            Debug.Print("Kein Sprite referenziert! Position für Hovermenu nicht erkannt", this);
            return new Vector2(0, 0);
        }
        return new Vector2(sprite.GlobalPosition.X, sprite.GlobalPosition.Y - 40f);
    }
}
