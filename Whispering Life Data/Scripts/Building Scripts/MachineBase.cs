using System;
using Godot;
using Godot.Collections;

public partial class MachineBase : placeable_building
{
    [Export]
    public bool machine_enabled = false;

    public Array<Giver> givers = new Array<Giver>();

    public Array<Taker> takers = new Array<Taker>();

    public override void _Ready()
    {
        base._Ready();

        takers.Clear();
        givers.Clear();

        foreach (Node node in GetChildren())
        {
            if (node is Taker taker)
                takers.Add(taker);
            if (node is Giver giver)
                givers.Add(giver);
        }

        if (givers.Count == 0 || takers.Count == 0)
            Logger.PrintEmptyList();
    }

    public void DisableTakers()
    {
        foreach (Taker taker in takers)
            taker.DisableMonitorable();
    }

    public void DisableGivers()
    {
        foreach (Giver giver in givers)
            giver.DisableMonitoring();
    }

    public void EnableTakers()
    {
        foreach (Taker taker in takers)
            taker.EnableMonitorable();
    }

    public void EnableGivers()
    {
        foreach (Giver giver in givers)
            giver.EnableMonitoring();
    }

    public override Resource Save()
    {
        return new MachineSave(building_id, Position, Scale, machine_enabled);
    }

    public override void Load(Resource save)
    {
        if (save is MachineSave machine_save)
        {
            Position = machine_save.pos;
            Scale = machine_save.scale;
            machine_enabled = machine_save.machine_enabled;
        }
        else
            Logger.PrintWrongSaveType();
    }
}
