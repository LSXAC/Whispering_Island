using System;
using System.Diagnostics;
using Godot;

public partial class TransportBase : placeable_building
{
    [Export]
    public bool ignore_self_detector = false;

    [Export]
    public Direction to_direction = Direction.Left;

    [Export]
    public Direction from_direction = Direction.Right;
    public ItemHolder item_holder;
    public Area2D path_connect_area;
    public Detector detector;
    public ConnectedBeltsManager cbm;

    public AnimationManager12D anim_manager12D;
    public int current_rotation = 0;

    public enum Direction
    {
        Top,
        Right,
        Down,
        Left,
        NONE
    };

    public override void _Ready()
    {
        base._Ready();

        path_connect_area = GetNode<Area2D>("PathConnectArea");
        if (HasNode("ConnectedBeltsManager"))
            cbm = GetNode<ConnectedBeltsManager>("ConnectedBeltsManager");
        anim_manager12D = GetNode<AnimationManager12D>("12DAnimationManager");
        detector = GetNode<Detector>("Detector");
        item_holder = GetNode<ItemHolder>("ItemHolder");
        set_direction();
    }

    public bool can_receive_item()
    {
        if (Logger.NodeIsNull(item_holder))
            return false;
        return item_holder.GetChildCount() == 0;
    }

    public void receive_item(Node2D item)
    {
        if (Logger.NodeIsNull(item_holder))
            return;
        item_holder.receive_item(item);
    }

    public void RotateLeft()
    {
        current_rotation--;
        if (current_rotation == -1)
            current_rotation = 3;
        Set_Rotation(current_rotation);
    }

    public void RotateRight()
    {
        current_rotation++;
        if (current_rotation == 4)
            current_rotation = 0;
        Set_Rotation(current_rotation);
    }

    public void SetRotationAndDisableMonitoring(int rotation)
    {
        Set_Rotation(rotation);
        if (Logger.NodeIsNotNull(path_connect_area))
            path_connect_area.Monitorable = false;
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
        if (
            !Logger.NodeIsNotNull(cbm)
            || !Logger.NodeIsNotNull(detector)
            || !Logger.NodeIsNotNull(anim_manager12D)
        )
            return;

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
                Debug.Print("From Direction: " + from_direction.ToString());
                anim_manager12D.SetAnimation(from_direction, to_direction, can_be_build_on_air);
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
                anim_manager12D.SetAnimation(from_direction, to_direction, can_be_build_on_air);
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
                anim_manager12D.SetAnimation(from_direction, to_direction, can_be_build_on_air);
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
                anim_manager12D.SetAnimation(from_direction, to_direction, can_be_build_on_air);
                break;
        }
    }

    public override Resource Save()
    {
        TransportBaseSave tbs = new TransportBaseSave(
            Position,
            from_direction,
            to_direction,
            current_rotation
        );
        return tbs;
    }

    public override void Load(Resource save)
    {
        if (save is TransportBaseSave belt_save)
        {
            Position = belt_save.position;
            from_direction = belt_save.from_direction;
            to_direction = belt_save.to_direction;
            set_direction();
            Set_Rotation(belt_save.current_rotation);
        }
        else
            Logger.PrintWrongSaveType();
    }

    public void InitBeltItem(BeltSave belt_save)
    {
        PackedScene belt_item_scene = ResourceLoader.Load<PackedScene>(
            ResourceUid.UidToPath("uid://dkue7sa7xyeyr")
        );
        if (Logger.NodeIsNull(belt_item_scene))
            return;

        BeltItem belt_item = (BeltItem)belt_item_scene.Instantiate();
        Item item = new Item(belt_save.belt_holding_item_resource, 1);
        belt_item.Init(item);
        item_holder.moving_item = belt_save.belt_item_is_moving;
        belt_item.Position = belt_save.belt_item_position;
        item_holder.AddChild(belt_item);
    }
}
