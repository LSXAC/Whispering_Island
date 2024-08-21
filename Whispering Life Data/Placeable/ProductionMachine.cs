using System;
using Godot;

public partial class ProductionMachine : ProcessBase
{
    public void OnSpawnTimeout()
    {
        input_count++;
    }
}
