using System;
using Godot;

public partial class HasConnectedBeltArea : Area2D
{
    [Export]
    public ConnectedBeltsManager.DIR dir;
    private ConnectedBeltsManager cbm;

    public override void _Ready()
    {
        cbm = GetParent<ConnectedBeltsManager>();
    }

    public void OnAreaEntered(Area2D area)
    {
        if (area is BeltArea)
            cbm.connected_belts[(int)dir] = true;
    }

    public void OnAreaExited(Area2D area)
    {
        if (area is BeltArea)
            cbm.connected_belts[(int)dir] = false;
    }
}
