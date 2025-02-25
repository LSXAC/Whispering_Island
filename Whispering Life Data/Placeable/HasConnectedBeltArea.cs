using System;
using System.Diagnostics;
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
        if (area is BeltArea ba)
        {
            Debug.Print("Connected: " + Name + " | Dir: " + ba.GetParent<Belt>().to_direction);
            cbm.connected_belts[(int)dir].connected = true;
            cbm.connected_belts[(int)dir].to_direction = ba.GetParent<Belt>().to_direction;
            cbm.GetParent<Belt>().set_direction();
        }
    }

    public void OnAreaExited(Area2D area)
    {
        if (area is BeltArea ba)
        {
            Debug.Print("Disconnected: " + Name);
            cbm.connected_belts[(int)dir].connected = false;
            cbm.connected_belts[(int)dir].to_direction = Belt.BeltDirection.NONE;
            cbm.GetParent<Belt>().set_direction();
        }
    }
}
