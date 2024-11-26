using System;
using Godot;
using Godot.Collections;

public partial class MachineBase : placeable_building
{
    [Export]
    public bool machine_enabled = false;

    [Export]
    public Array<Giver> givers;
}
