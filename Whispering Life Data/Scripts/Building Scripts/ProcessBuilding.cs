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

    int progress = 0;

    public void OnCraftingTimerTimeout()
    {
        FurnaceTab.INSTANCE.UpdateProgressbar(progress);
        if (progress >= 100)
        {
            export_count++;
            InventoryItem ii = new InventoryItem();
            ii.init(export_item_info);
            FurnaceTab.INSTANCE.export_slot.UpdateFurnaceItem(ii, 1);
            is_crafting = false;
            crafting_timer.Stop();
            progress = 0;
            FurnaceTab.INSTANCE.UpdateProgressbar(progress);
            return;
        }
        progress += 5;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (is_crafting)
            return;
        if (import_count <= 1)
            return;

        is_crafting = true;
        import_count -= 2;
        InventoryItem ii = new InventoryItem();
        ii.init(import_item_info);
        FurnaceTab.INSTANCE.import_slot.UpdateFurnaceItem(ii, -2);
        crafting_timer.Start();
    }
}
