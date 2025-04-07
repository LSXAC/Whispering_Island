using System;
using System.Diagnostics;
using Godot;

public partial class TransportBase : placeable_building
{
    [Export]
    public ItemHolder item_holder;

    [Export]
    public Detector detector;

    [Export]
    public bool ignore_self_detector = false;

    [Export]
    public Direction to_direction = Direction.Left;

    [Export]
    public Direction from_direction = Direction.Right;

    [Export]
    public ConnectedBeltsManager cbm;

    [Export]
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

    public bool can_receive_item()
    {
        return item_holder.GetChildCount() == 0;
    }

    public void receive_item(Node2D item)
    {
        item_holder.receive_item(item);
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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
