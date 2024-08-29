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
    private Sprite2D sprite;

    [Export]
    public CollisionShape2D collision_shape;

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

    public Sprite2D GetSprite()
    {
        if (sprite != null)
            return sprite;
        return new Sprite2D();
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
        if (sprite == null)
        {
            Debug.Print("Kein Sprite referenziert!", this);
            return new Vector2(0, 0);
        }
        return new Vector2(Position.X, Position.Y - sprite.Texture.GetSize().Y - 16f);
    }
}
