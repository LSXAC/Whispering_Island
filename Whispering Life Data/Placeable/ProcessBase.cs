using System;
using Godot;
using Godot.Collections;

public partial class ProcessBase : placeable_building
{
    [Export]
    public float input_count = 0;

    [Export]
    public float export_count = 0;
}
