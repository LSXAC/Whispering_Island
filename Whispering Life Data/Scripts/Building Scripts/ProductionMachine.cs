using System;
using Godot;

public partial class ProductionMachine : MachineBase
{
    public void OnSpawnTimeout()
    {
        import_count++;
    }
}
