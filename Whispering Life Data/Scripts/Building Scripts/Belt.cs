using System.Diagnostics;
using Godot;

public partial class Belt : placeable_building
{
    [Export]
    public ItemHolder item_holder;

    [Export]
    public Detector detector;

    [Export]
    public Area2D connected_belt_area;

    [Export]
    public bool ignore_self_detector = false;

    [Export]
    public Direction to_direction = Direction.Left;

    [Export]
    public Direction from_direction = Direction.Right;

    [Export]
    public ConnectedBeltsManager cbm;

    public enum Direction
    {
        Top,
        Right,
        Down,
        Left,
        NONE
    };

    [Export]
    public AnimationManager12D anim_manager12D;

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
        if (ignore_self_detector)
            return;

        if (area is PathConnectArea)
        {
            if (area.GetParent() is Belt && area.GetParent() is not BeltTunnel)
                if (area.GetParent<Belt>().can_receive_item())
                {
                    var item = item_holder.offload_item();
                    area.GetParent<Belt>().receive_item(item);
                }

            if (area.GetParent() is BeltTunnel)
                if (area.GetParent<BeltTunnel>().is_tunnel_connected)
                    if (area.GetParent<BeltTunnel>().connected_itemholder != null)
                        if (area.GetParent<BeltTunnel>().item_holder.GetChildCount() == 0)
                        {
                            Debug.Print("Deposit Item");
                            var item = item_holder.offload_item();
                            area.GetParent<BeltTunnel>().from_Belt = true;
                            area.GetParent<BeltTunnel>().GetNode<Timer>("CheckTimer").Start();
                            area.GetParent<BeltTunnel>().item_holder.receive_item(item);
                        }

            if (area.GetParent() is Taker)
            {
                if (area.GetParent().GetParent() is ProcessBuilding)
                {
                    if (area.GetParent<Taker>().can_receive_item())
                    {
                        if (item_holder.hasBeltItem())
                        {
                            ItemInfo ii = item_holder.GetBeltItem().GetItemInfo();
                            if (
                                ((ProcessBuilding)area.GetParent<Taker>().building).GetItemInfo(
                                    FurnaceTab.SlotType.IMPORT
                                ) != null
                            )
                                if (
                                    ii
                                    != (
                                        (ProcessBuilding)area.GetParent<Taker>().building
                                    ).GetItemInfo(FurnaceTab.SlotType.IMPORT)
                                )
                                    return;
                                else
                                {
                                    var item2 = item_holder.offload_item();
                                    ((ProcessBuilding)area.GetParent<Taker>().building)
                                        .item_array[(int)FurnaceTab.SlotType.IMPORT]
                                        .amount += 1;
                                    area.GetParent<Taker>().receive_item(item2);
                                    return;
                                }
                            var item = item_holder.offload_item();
                            ((ProcessBuilding)area.GetParent<Taker>().building).item_array[
                                (int)FurnaceTab.SlotType.IMPORT
                            ] = new ItemSave((int)ii.unique_id, 1);
                            area.GetParent<Taker>().receive_item(item);
                        }
                    }
                }
                if (area.GetParent().GetParent() is Chest)
                {
                    var item = item_holder.offload_item();
                    if (area.GetParent<Taker>().can_receive_item((BeltItem)item))
                        area.GetParent<Taker>().receive_item(item);
                }
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
            to_direction = Direction.Top;
            from_direction = Direction.Down;
            current_rotation = id;
        }
        if (id == 1)
        {
            to_direction = Direction.Right;
            from_direction = Direction.Left;
            current_rotation = id;
        }
        if (id == 2)
        {
            to_direction = Direction.Down;
            from_direction = Direction.Top;
            current_rotation = id;
        }
        if (id == 3)
        {
            to_direction = Direction.Left;
            from_direction = Direction.Right;
            current_rotation = id;
        }
        set_direction();
    }

    public void set_direction()
    {
        switch (to_direction)
        {
            case Direction.Left:
                detector.Position = new Vector2(-24f, -8);
                from_direction = Direction.Right;
                if (cbm != null && !ignore_self_detector)
                {
                    if (cbm.HasImportantDirection(ConnectedBeltsManager.DIR.UP, Direction.Down))
                        from_direction = Direction.Top;

                    if (cbm.HasImportantDirection(ConnectedBeltsManager.DIR.DOWN, Direction.Top))
                        from_direction = Direction.Down;

                    if (
                        cbm.HasImportantDirection(ConnectedBeltsManager.DIR.UP, Direction.Down)
                        && cbm.HasImportantDirection(ConnectedBeltsManager.DIR.DOWN, Direction.Top)
                    )
                        from_direction = Direction.Right;
                }
                Debug.Print(from_direction.ToString());
                anim_manager12D.SetAnimation(from_direction, to_direction);
                break;
            case Direction.Right:
                detector.Position = new Vector2(8f, -8);

                from_direction = Direction.Left;

                if (cbm != null && !ignore_self_detector)
                {
                    if (cbm.HasImportantDirection(ConnectedBeltsManager.DIR.UP, Direction.Down))
                        from_direction = Direction.Top;

                    if (cbm.HasImportantDirection(ConnectedBeltsManager.DIR.DOWN, Direction.Top))
                        from_direction = Direction.Down;

                    if (
                        cbm.HasImportantDirection(ConnectedBeltsManager.DIR.UP, Direction.Down)
                        && cbm.HasImportantDirection(ConnectedBeltsManager.DIR.DOWN, Direction.Top)
                    )
                        from_direction = Direction.Right;
                }
                anim_manager12D.SetAnimation(from_direction, to_direction);
                break;
            case Direction.Top:
                detector.Position = new Vector2(-8, -24);

                from_direction = Direction.Down;
                if (cbm != null && !ignore_self_detector)
                {
                    if (cbm.HasImportantDirection(ConnectedBeltsManager.DIR.LEFT, Direction.Right))
                        from_direction = Direction.Left;

                    if (cbm.HasImportantDirection(ConnectedBeltsManager.DIR.RIGHT, Direction.Left))
                        from_direction = Direction.Right;

                    if (
                        cbm.HasImportantDirection(ConnectedBeltsManager.DIR.LEFT, Direction.Right)
                        && cbm.HasImportantDirection(
                            ConnectedBeltsManager.DIR.RIGHT,
                            Direction.Left
                        )
                    )
                        from_direction = Direction.Down;
                }
                anim_manager12D.SetAnimation(from_direction, to_direction);
                break;
            case Direction.Down:
                detector.Position = new Vector2(-8, 8);
                from_direction = Direction.Top;
                if (cbm != null && !ignore_self_detector)
                {
                    if (cbm.HasImportantDirection(ConnectedBeltsManager.DIR.LEFT, Direction.Right))
                        from_direction = Direction.Left;

                    if (cbm.HasImportantDirection(ConnectedBeltsManager.DIR.RIGHT, Direction.Left))
                        from_direction = Direction.Right;

                    if (
                        cbm.HasImportantDirection(ConnectedBeltsManager.DIR.LEFT, Direction.Right)
                        && cbm.HasImportantDirection(
                            ConnectedBeltsManager.DIR.RIGHT,
                            Direction.Left
                        )
                    )
                        from_direction = Direction.Top;
                }
                anim_manager12D.SetAnimation(from_direction, to_direction);
                break;
        }
    }
}
