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
                Velocity = -Position.Normalized() * 5f;
                moving = true;
            }
        if (moving && Position.DistanceTo(Vector2.Zero) < 0.2f)
        {
            moving = false;
            Velocity = Vector2.Zero;
            Position = Vector2.Zero;
        }
        MoveAndSlide();
    }

    public void InitBeltItem(Item item)
    {
        sprite = GetNode<Sprite2D>("BellItemSprite");
        this.item = item;
        this.sprite.Texture = item.item_info.texture;
    }

    public void InitBeltItemAtPosition(Vector2 position)
    {
        Position = position;
    }
}
