using System;
using System.Threading;
using Godot;

public partial class RailSave : TransportBaseSave
{
    [Export]
    public Vector2 minecart_position = Vector2.Zero;

    [Export]
    public bool has_minecart = false;

    [Export]
    public ItemSave[] chest_items = new ItemSave[20];

    public RailSave() { }

    public RailSave(
        TransportBaseSave tbs,
        bool has_minecart,
        Vector2 minecart_position,
        ItemSave[] chest_array,
        int curr_rot
    )
    {
        this.position = tbs.position;
        this.from_direction = tbs.from_direction;
        this.to_direction = tbs.to_direction;
        this.has_minecart = has_minecart;
        this.minecart_position = minecart_position;
        this.chest_items = chest_array;
        this.current_rotation = curr_rot;
    }
}
