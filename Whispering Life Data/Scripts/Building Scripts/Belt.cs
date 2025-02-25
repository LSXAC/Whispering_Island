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
    public BeltDirection to_direction = BeltDirection.Left;

    [Export]
    public BeltDirection from_direction = BeltDirection.Right;

    [Export]
    public AnimatedSprite2D anim_sprite;

    [Export]
    public ConnectedBeltsManager cbm;

    public enum BeltDirection
    {
        Top,
        Right,
        Down,
        Left,
        NONE
    };

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

        if (area is BeltArea)
        {
            if (area.GetParent() is Belt && area.GetParent() is not BeltTunnel)
                if (area.GetParent<Belt>().can_receive_item())
                {
                    var item = item_holder.offload_item();
                    area.GetParent<Belt>().receive_item(item);
                }

            if (area.GetParent() is BeltSplitter && area.GetParent() is not BeltTunnel)
                area.GetParent<BeltSplitter>().GetNode<Timer>("CheckAreaTimer").Start();

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
        { // ^
            //if(cbm.HasConnectionTo(ConnectedBeltsManager.DIR.RIGHT))
            to_direction = BeltDirection.Top;
            from_direction = BeltDirection.Down;
            current_rotation = id;
        } // >
        if (id == 1)
        {
            to_direction = BeltDirection.Right;
            from_direction = BeltDirection.Left;
            current_rotation = id;
        } // v
        if (id == 2)
        {
            to_direction = BeltDirection.Down;
            from_direction = BeltDirection.Top;
            current_rotation = id;
        } // <
        if (id == 3)
        {
            to_direction = BeltDirection.Left;
            from_direction = BeltDirection.Right;
            current_rotation = id;
        }
        set_direction();
    }

    string dir = "";

    public void set_direction()
    {
        switch (to_direction)
        {
            case BeltDirection.Left:
                detector.Position = new Vector2(-24f, -8);
                from_direction = BeltDirection.Right;
                if (cbm != null && !ignore_self_detector)
                {
                    if (cbm.HasImportantDirection(ConnectedBeltsManager.DIR.UP, BeltDirection.Down))
                        from_direction = BeltDirection.Top;

                    if (
                        cbm.HasImportantDirection(ConnectedBeltsManager.DIR.DOWN, BeltDirection.Top)
                    )
                        from_direction = BeltDirection.Down;

                    if (
                        cbm.HasImportantDirection(ConnectedBeltsManager.DIR.UP, BeltDirection.Down)
                        && cbm.HasImportantDirection(
                            ConnectedBeltsManager.DIR.DOWN,
                            BeltDirection.Top
                        )
                    )
                        from_direction = BeltDirection.Right;
                }
                Debug.Print(from_direction.ToString());
                switch (from_direction)
                {
                    case BeltDirection.Right:
                        dir = "BELT_LEFT";
                        break;
                    case BeltDirection.Top:
                        dir = "BELT_CORNER_DOWN_LEFT";
                        break;
                    case BeltDirection.Down:
                        dir = "BELT_CORNER_UP_LEFT";
                        break;
                }
                break;
            case BeltDirection.Right:
                detector.Position = new Vector2(8f, -8);

                from_direction = BeltDirection.Left;

                if (cbm != null && !ignore_self_detector)
                {
                    if (cbm.HasImportantDirection(ConnectedBeltsManager.DIR.UP, BeltDirection.Down))
                        from_direction = BeltDirection.Top;

                    if (
                        cbm.HasImportantDirection(ConnectedBeltsManager.DIR.DOWN, BeltDirection.Top)
                    )
                        from_direction = BeltDirection.Down;

                    if (
                        cbm.HasImportantDirection(ConnectedBeltsManager.DIR.UP, BeltDirection.Down)
                        && cbm.HasImportantDirection(
                            ConnectedBeltsManager.DIR.DOWN,
                            BeltDirection.Top
                        )
                    )
                        from_direction = BeltDirection.Right;
                }
                switch (from_direction)
                {
                    case BeltDirection.Left:
                        dir = "BELT_RIGHT";
                        break;
                    case BeltDirection.Top:
                        dir = "BELT_CORNER_DOWN_RIGHT";
                        break;
                    case BeltDirection.Down:
                        dir = "BELT_CORNER_UP_RIGHT";
                        break;
                }
                break;
            case BeltDirection.Top:
                detector.Position = new Vector2(-8, -24);

                from_direction = BeltDirection.Down;
                if (cbm != null && !ignore_self_detector)
                {
                    if (
                        cbm.HasImportantDirection(
                            ConnectedBeltsManager.DIR.LEFT,
                            BeltDirection.Right
                        )
                    )
                        from_direction = BeltDirection.Left;

                    if (
                        cbm.HasImportantDirection(
                            ConnectedBeltsManager.DIR.RIGHT,
                            BeltDirection.Left
                        )
                    )
                        from_direction = BeltDirection.Right;

                    if (
                        cbm.HasImportantDirection(
                            ConnectedBeltsManager.DIR.LEFT,
                            BeltDirection.Right
                        )
                        && cbm.HasImportantDirection(
                            ConnectedBeltsManager.DIR.RIGHT,
                            BeltDirection.Left
                        )
                    )
                        from_direction = BeltDirection.Down;
                }
                switch (from_direction)
                {
                    case BeltDirection.Left:
                        dir = "BELT_CORNER_RIGHT_UP";
                        break;
                    case BeltDirection.Down:
                        dir = "BELT_UP";
                        break;
                    case BeltDirection.Right:
                        dir = "BELT_CORNER_LEFT_UP";
                        break;
                }
                break;
            case BeltDirection.Down:
                detector.Position = new Vector2(-8, 8);
                from_direction = BeltDirection.Top;
                if (cbm != null && !ignore_self_detector)
                {
                    if (
                        cbm.HasImportantDirection(
                            ConnectedBeltsManager.DIR.LEFT,
                            BeltDirection.Right
                        )
                    )
                        from_direction = BeltDirection.Left;

                    if (
                        cbm.HasImportantDirection(
                            ConnectedBeltsManager.DIR.RIGHT,
                            BeltDirection.Left
                        )
                    )
                        from_direction = BeltDirection.Right;

                    if (
                        cbm.HasImportantDirection(
                            ConnectedBeltsManager.DIR.LEFT,
                            BeltDirection.Right
                        )
                        && cbm.HasImportantDirection(
                            ConnectedBeltsManager.DIR.RIGHT,
                            BeltDirection.Left
                        )
                    )
                        from_direction = BeltDirection.Top;
                }
                switch (from_direction)
                {
                    case BeltDirection.Left:
                        dir = "BELT_CORNER_RIGHT_DOWN";
                        break;
                    case BeltDirection.Top:
                        dir = "BELT_DOWN";
                        break;
                    case BeltDirection.Right:
                        dir = "BELT_CORNER_LEFT_DOWN";
                        break;
                }
                break;
        }
        RefTimer ref_timer = GlobalAnimationTimer.INSTANCE.GetCurrentFrame();
        setFrame(ref_timer.frame, GlobalAnimationTimer.INSTANCE.TimeLeft);
    }

    public void setFrame(int frame, double diff)
    {
        GetTree().CreateTimer(diff).Timeout += () => SetAnim(frame);
    }

    private void SetAnim(int frame)
    {
        anim_sprite.Play(dir);
        anim_sprite.Frame = frame;
    }
}
