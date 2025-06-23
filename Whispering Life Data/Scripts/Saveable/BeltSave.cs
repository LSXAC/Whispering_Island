using Godot;

public partial class BeltSave : TransportBaseSave
{
    public BeltSave() { }

    public BeltSave(
        Vector2 pos,
        TransportBase.Direction f_d,
        TransportBase.Direction t_d,
        ItemInfo item_resource,
        int curr_rot
    )
    {
        this.position = pos;
        this.from_direction = f_d;
        this.to_direction = t_d;
        this.belt_holding_item_resource = item_resource;
        this.current_rotation = curr_rot;
    }

    [Export]
    public ItemInfo belt_holding_item_resource = null;

    [Export]
    public Vector2 belt_item_position = Vector2.Zero;

    [Export]
    public bool belt_item_is_moving = false;
}
