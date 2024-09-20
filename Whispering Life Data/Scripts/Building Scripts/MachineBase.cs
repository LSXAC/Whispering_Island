using System;
using Godot;

public partial class MachineBase : placeable_building
{
    public enum MachineType
    {
        WOODFARM,
        FURNACE,
        QUARRY,
        CHEST
    }

    [Export]
    public bool machine_enabled = false;

    [Export]
    public MachineType type;
}
