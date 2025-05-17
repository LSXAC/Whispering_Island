using Godot;

public partial class TransportBaseSave : Resource
{
    [Export]
    public Vector2 position = Vector2.Zero;

    [Export]
    public TransportBase.Direction from_direction = TransportBase.Direction.Right;

    [Export]
    public TransportBase.Direction to_direction = TransportBase.Direction.Left;

    [Export]
    public int current_rotation = 0;

    public TransportBaseSave() { }

    public TransportBaseSave(
        Vector2 pos,
        TransportBase.Direction f_direction,
        TransportBase.Direction t_direction,
        int curr_rot
    )
    {
        this.position = pos;
        this.from_direction = f_direction;
        this.to_direction = t_direction;
        this.current_rotation = curr_rot;
    }
}
