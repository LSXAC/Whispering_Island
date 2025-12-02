using System;
using Godot;

[GlobalClass]
public partial class PlayerStat : Resource
{
    [Export]
    public PlayerStats.TOOLTYPE tool_type;

    [Export]
    public float value = 0.0f;
}
