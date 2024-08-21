using System;
using Godot;

public partial class ProcessBuilding : ProcessBase
{
    public bool is_crafting = false;

    [Export]
    public Timer crafting_timer;

    public void OnCraftingTimerTimeout()
    {
        export_count++;
        is_crafting = false;
    }
}
