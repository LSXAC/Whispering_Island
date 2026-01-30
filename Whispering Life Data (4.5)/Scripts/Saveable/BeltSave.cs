using Godot;

public partial class BeltSave : TransportBaseSave
{
    public BeltSave() { }

    public BeltSave(TransportBaseSave tbs, ItemInfo item_resource, int curr_rot)
    {
        this.position = tbs.position;
        this.from_direction = tbs.from_direction;
        this.to_direction = tbs.to_direction;
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
