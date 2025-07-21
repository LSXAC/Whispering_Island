using System;
using Godot;

public partial class LightBehaviour : PointLight2D
{
    private float energy = 0;

    public override void _Ready()
    {
        energy = Energy;
        Energy = energy * TimeManager.light_factor;
    }

    public override void _Process(double delta)
    {
        if (Energy != energy * TimeManager.light_factor)
            Energy = energy * TimeManager.light_factor;
    }
}
