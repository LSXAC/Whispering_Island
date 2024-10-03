using System;
using Godot;

public partial class StatsPanel : Panel
{
    [Export]
    public VBoxContainer stats_container;

    public enum stat_types
    {
        Attack,
        Defense,
        Foresty,
        Mining,
        Farming
    };

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    public void UpdateStats() { }
}
