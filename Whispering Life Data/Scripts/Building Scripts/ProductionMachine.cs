using System;
using Godot;

public partial class ProductionMachine : MachineBase
{
    [Export]
    public ItemInfo production_item_info;

    [Export]
    public int production_count = 0;

    public int progress = 0;

    public void OnSpawnTimeout()
    {
        if (progress >= 100)
        {
            production_count++;
            progress = 0;
        }
        progress += 5;
        if (hover_menu.instance.current_object == this)
            hover_menu.InitHoverMenu(this);
    }
}
