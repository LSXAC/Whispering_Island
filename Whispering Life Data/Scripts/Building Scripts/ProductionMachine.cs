using System;
using Godot;

public partial class ProductionMachine : MachineBase
{
    [Export]
    public ItemInfo production_item_info;

    [Export]
    public int production_count = 0;

    public void OnSpawnTimeout()
    {
        production_count++;
    }
}
