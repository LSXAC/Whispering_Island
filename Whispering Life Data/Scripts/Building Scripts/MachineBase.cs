using System;
using Godot;
using Godot.Collections;

public partial class MachineBase : placeable_building
{
    [Export]
    public bool machine_enabled = false;

    [Export]
    public Array<Giver> givers;

    [Export]
    public Array<Taker> takers;

    public void DisableTakers()
    {
        if (takers == null)
            return;

        foreach (Taker taker in takers)
        {
            taker.DisableMonitorable();
        }
    }
}
