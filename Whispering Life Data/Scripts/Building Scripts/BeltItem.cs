using System;
using System.Diagnostics;
using Godot;

public partial class BeltItem : CharacterBody2D
{
    [Export]
    public Item item = null;
    public Sprite2D sprite;
    public bool moving = false;

    public override void _Process(double delta)
    {
        if (!moving)
            if (Position != Vector2.Zero)
            {
                Velocity = -Position.Normalized();
                moving = true;
            }
        if (moving && Math.Abs(Position.DistanceTo(Vector2.Zero)) < 0.2f)
        {
            moving = false;
            Velocity = Vector2.Zero;
            Position = Vector2.Zero;
        }
        MoveAndSlide();
    }

    public ItemInfo GetItemInfo()
    {
        if (item != null)
            return item.info;
        return null;
    }

    public void Init(Item item)
    {
        sprite = GetNode<Sprite2D>("BellItemSprite");
        this.item = item;
        this.sprite.Texture = item.info.texture;
    }

    public void InitAtPosition(Vector2 position)
    {
        Position = position;
    }
}
