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
        if (area is PathConnectArea ba)
        {
            if (ba.GetParent() is TransportBase)
            {
                Debug.Print(
                    "Connected: " + Name + " | Dir: " + ba.GetParent<TransportBase>().to_direction
                );
                cbm.connected_belts[(int)dir].connected = true;
                cbm.connected_belts[(int)dir].to_direction =
                    ba.GetParent<TransportBase>().to_direction;
                cbm.GetParent<TransportBase>().set_direction();
            }
            if (ba.GetParent() is Giver)
            {
                Debug.Print("Connected: " + Name + " | Dir: " + ba.GetParent<Giver>().to_direction);
                cbm.connected_belts[(int)dir].connected = true;
                cbm.connected_belts[(int)dir].to_direction = ba.GetParent<Giver>().to_direction;
                cbm.GetParent<TransportBase>().set_direction();
            }
        }
    }

    public void OnAreaExited(Area2D area)
    {
        if (area is PathConnectArea ba)
        {
            if (ba.GetParent() is TransportBase)
            {
                Debug.Print("Disconnected: " + Name);
                cbm.connected_belts[(int)dir].connected = false;
                cbm.connected_belts[(int)dir].to_direction = TransportBase.Direction.NONE;
                cbm.GetParent<TransportBase>().set_direction();
            }
        }
    }
}
