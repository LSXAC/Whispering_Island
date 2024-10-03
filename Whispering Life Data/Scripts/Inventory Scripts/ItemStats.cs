using System;
using System.ComponentModel;
using Godot;

[GlobalClass]
public partial class ItemStats : Resource
{
    [Export]
    public StatsPanel.stat_types type;

    [Export(PropertyHint.Range, "-50,50,0")]
    public float bonus = 0;
}
