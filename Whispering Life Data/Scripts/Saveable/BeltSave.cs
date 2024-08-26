using Godot;

public partial class BeltSave : Resource
{
    public BeltSave() { }

    public BeltSave(
        Vector2 pos,
        Belt.BeltDirection f_d,
        Belt.BeltDirection t_d,
        ItemInfo item,
        int curr_rot
    )
    {
        this.position = pos;
        this.from_direction = f_d;
        this.to_direction = t_d;
        this.holded_item = item;
        this.current_rotation = curr_rot;
    }

    [Export]
    public Vector2 position = Vector2.Zero;

    [Export]
    public Belt.BeltDirection from_direction = Belt.BeltDirection.Right;

    [Export]
    public Belt.BeltDirection to_direction = Belt.BeltDirection.Left;

    [Export]
    public int current_rotation = 0;

    [Export]
    public ItemInfo holded_item = null;

    [Export]
    public Vector2 beltItem_position = Vector2.Zero;

    [Export]
    public bool beltItem_moving = false;
}
