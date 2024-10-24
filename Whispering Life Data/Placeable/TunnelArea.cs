using System;
using System.Diagnostics;
using Godot;

public partial class TunnelArea : Area2D
{
    public void OnAreaEntered(Area2D area)
    {
        Debug.Print(area.Name);
        if (area.Name.Equals("BeltArea"))
        {
            //Dir = Dir  L --> R     L --> R -> true
            if (area.GetParent() == GetParent())
                return;

            Debug.Print(((BeltTunnel)GetParent()).to_direction.ToString());
            Debug.Print(((BeltTunnel)area.GetParent()).to_direction.ToString());
            if (
                ((BeltTunnel)GetParent()).to_direction
                    == ((BeltTunnel)area.GetParent()).to_direction
                && ((BeltTunnel)GetParent()).is_tunnel_connected == false
                && ((BeltTunnel)area.GetParent()).is_tunnel_connected == false
            )
            {
                ((BeltTunnel)GetParent()).is_tunnel_connected = true;
                ((BeltTunnel)GetParent()).connected_itemholder = (
                    (BeltTunnel)area.GetParent()
                ).item_holder;
                ((BeltTunnel)area.GetParent()).connected_itemholder = (
                    (BeltTunnel)GetParent()
                ).item_holder;
                ((BeltTunnel)area.GetParent()).is_tunnel_connected = true;
            }

            ((BeltTunnel)GetParent()).ResetCheckArea();
            ((BeltTunnel)area.GetParent()).ResetCheckArea();
        }
    }
}
