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
