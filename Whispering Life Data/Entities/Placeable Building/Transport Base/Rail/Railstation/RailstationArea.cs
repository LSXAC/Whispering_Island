using System;
using System.Diagnostics;
using Godot;

public partial class RailstationArea : Area2D
{
    [Export]
    public STATION rail_station_point;

    RailStation rail_station;

    public override void _Ready()
    {
        base._Ready();
        rail_station = GetParent<RailStation>();
    }

    public enum STATION
    {
        IMPORT,
        EXPORT
    }

    public void OnAreaEntered(Area2D area)
    {
        Debug.Print("Object Entered Railstation: " + area.GetParent().Name);
        if (area.GetParent() is Minecart cart)
            rail_station.ConnectMinecart(cart, rail_station_point);
    }
}
