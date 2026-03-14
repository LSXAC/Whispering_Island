using System;
using Godot;
using Godot.Collections;

public partial class MagicPowerManager : Node2D
{
    public static MagicPowerManager instance;

    public override void _Ready()
    {
        instance = this;
    }
}
