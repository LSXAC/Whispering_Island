using System;
using System.Diagnostics;
using Godot;

public abstract partial class Building_Node : Node2D
{
    [Export]
    public string title = "";

    [Export]
    public string description = "";

    [Export]
    public Sprite2D building_sprite;

    public bool mouse_inside = false;

    public abstract void OnMouseClick();

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton buttonevent)
            if (buttonevent.Pressed && mouse_inside)
            {
                OnMouseClick();
            }
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
        if (building_sprite == null)
        {
            Debug.Print("Kein Sprite referenziert!", this);
            return new Vector2(0, 0);
        }
        return new Vector2(Position.X, Position.Y - building_sprite.Texture.GetSize().Y - 16f);
    }
}
