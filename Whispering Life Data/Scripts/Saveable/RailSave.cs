using System;
using Godot;

public partial class RailSave : TransportBaseSave
{
    public RailSave() { }

    public RailSave(
        Vector2 pos,
        TransportBase.Direction f_d,
        TransportBase.Direction t_d,
        bool has_minecart,
        int curr_rot
    )
    {
        this.position = pos;
        this.from_direction = f_d;
        this.to_direction = t_d;
        this.has_minecart = has_minecart;
        this.current_rotation = curr_rot;
    }

    [Export]
    public bool has_minecart = false;
}
