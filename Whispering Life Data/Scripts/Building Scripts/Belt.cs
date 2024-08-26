using Godot;

public partial class Belt : placeable_building
{
    [Export]
    public ItemHolder item_holder;

    [Export]
    public Detector detector;

    public enum BeltDirection
    {
        Top,
        Right,
        Down,
        Left
    };

    [Export]
    public BeltDirection to_direction = BeltDirection.Left;

    [Export]
    public BeltDirection from_direction = BeltDirection.Right;

    public int current_rotation = 0;

    public override void _Ready()
    {
        set_direction();
        building_collider_manager = GetNode<Node2D>("BuildingAreas") as Building_Collider_Manager;
    }

    public bool can_receive_item()
    {
        return item_holder.GetChildCount() == 0;
    }

    public void receive_item(Node2D item)
    {
        item_holder.receive_item(item);
    }

    public void OnDetectorBeltDetected(Area2D area)
    {
        if (area is BeltArea)
        {
            if (area.GetParent() is Belt)
                if (area.GetParent<Belt>().can_receive_item())
                {
                    var item = item_holder.offload_item();
                    area.GetParent<Belt>().receive_item(item);
                }
            if (area.GetParent() is Trashcan)
                if (area.GetParent<Trashcan>().can_receive_item())
                {
                    var item = item_holder.offload_item();
                    area.GetParent<Trashcan>().receive_item(item);
                }
            if (area.GetParent() is Taker)
                if (area.GetParent<Taker>().can_receive_item())
                {
                    if (item_holder.hasBeltItem())
                    {
                        ItemInfo ii = item_holder.GetBeltItem().GetItemInfo();
                        if (ii != area.GetParent<Taker>().building.import_item_info)
                            return;
                    }
                    var item = item_holder.offload_item();
                    area.GetParent<Taker>().receive_item(item);
                }
        }
    }

    public void OnItemHolderItemHeld()
    {
        detector.Detect();
    }

    public void Set_Rotation(int id)
    {
        if (id == 0)
        {
            to_direction = BeltDirection.Top;
            from_direction = BeltDirection.Down;
            current_rotation = id;
        }
        if (id == 1)
        {
            to_direction = BeltDirection.Right;
            from_direction = BeltDirection.Left;
            current_rotation = id;
        }
        if (id == 2)
        {
            to_direction = BeltDirection.Down;
            from_direction = BeltDirection.Top;
            current_rotation = id;
        }
        if (id == 3)
        {
            to_direction = BeltDirection.Left;
            from_direction = BeltDirection.Right;
            current_rotation = id;
        }
        set_direction();
    }

    public void set_direction()
    {
        switch (to_direction)
        {
            case BeltDirection.Left:
                detector.Position = new Vector2(-24f, -8);
                switch (from_direction)
                {
                    case BeltDirection.Right:
                        sprite.Frame = 1;
                        break;
                    case BeltDirection.Top:
                        sprite.Frame = 9;
                        break;
                    case BeltDirection.Down:
                        sprite.Frame = 2;
                        break;
                }
                break;
            case BeltDirection.Right:
                detector.Position = new Vector2(8f, -8);
                switch (from_direction)
                {
                    case BeltDirection.Left:
                        sprite.Frame = 11;
                        break;
                    case BeltDirection.Top:
                        sprite.Frame = 10;
                        break;
                    case BeltDirection.Down:
                        sprite.Frame = 3;
                        break;
                }
                break;
            case BeltDirection.Top:
                detector.Position = new Vector2(-8, -24);
                switch (from_direction)
                {
                    case BeltDirection.Left:
                        sprite.Frame = 12;
                        break;
                    case BeltDirection.Down:
                        sprite.Frame = 7;
                        break;
                    case BeltDirection.Right:
                        sprite.Frame = 8;
                        break;
                }
                break;
            case BeltDirection.Down:
                detector.Position = new Vector2(-8, 8);
                switch (from_direction)
                {
                    case BeltDirection.Left:
                        sprite.Frame = 4;
                        break;
                    case BeltDirection.Top:
                        sprite.Frame = 5;
                        break;
                    case BeltDirection.Right:
                        sprite.Frame = 0;
                        break;
                }
                break;
        }
    }
}
