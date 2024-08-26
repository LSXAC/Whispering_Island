using Godot;

public partial class ItemHolder : Node2D
{
    [Export]
    public bool moving_item = false;

    [Signal]
    public delegate void item_heldEventHandler();

    [Export]
    public float speed = 10;

    public override void _PhysicsProcess(double delta)
    {
        if (GetChildCount() == 0)
            return;

        if (!moving_item)
            EmitSignal("item_held");

        var item = GetChild<Node2D>(0);
        if (item is Node2D)
        {
            item.Position = item.Position.MoveToward(Vector2.Zero, speed * (float)delta);
            if (item.Position == Vector2.Zero)
            {
                item.Position = Vector2.Zero;
                hold_item();
            }
        }
    }

    public void receive_item(Node2D item)
    {
        item.Reparent(this, true);
        moving_item = true;
    }

    public Node2D offload_item()
    {
        var item = GetChild<Node2D>(0);
        return item;
    }

    public Vector2 GetBeltItemPosition()
    {
        if (hasBeltItem())
            return GetBeltItem().Position;
        return Vector2.Zero;
    }

    public BeltItem GetBeltItem()
    {
        return GetChild<BeltItem>(0);
    }

    public bool hasBeltItem()
    {
        if (GetChildCount() != 0)
        {
            if (GetBeltItem() is BeltItem)
                return true;
        }
        return false;
    }

    public void hold_item()
    {
        moving_item = false;
        EmitSignal("item_held");
    }
}
