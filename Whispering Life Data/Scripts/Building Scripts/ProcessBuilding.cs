using System;
using Godot;

public partial class ProcessBuilding : MachineBase
{
    public bool is_crafting = false;

    [Export]
    public int fuel_count = 0;

    [Export]
    public ItemInfo fuel_item_info;

    [Export]
    public Timer crafting_timer;

    public void OnCraftingTimerTimeout()
    {
        export_count++;
        is_crafting = false;
    }
}
