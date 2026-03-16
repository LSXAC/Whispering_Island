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

    public override void _Ready()
    {
        base._Ready();
    }

    public void GeneratePower(MagicPowerListener listener)
    {
        listener.AddPowerGeneration(generation_rate);
    }

    public override void Load(Resource save)
    {
        base.Load(save);
    }

    public override Resource Save()
    {
        PlaceableSave save = (PlaceableSave)base.Save();
        return save;
    }
}
