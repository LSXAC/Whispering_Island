using System;
using Godot;

public partial class MagicGenerator : MachineBase
{
    [Export]
    public float generation_rate = 1f;

    [Export]
    public float fuel_left = 1f;

    [Export]
    public int time_left = 0;
}
