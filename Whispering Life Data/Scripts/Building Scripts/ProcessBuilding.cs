using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class ProcessBuilding : MachineBase
{
    public bool is_crafting = false;

    [Export]
    public int fuel_count = 0;

    [Export]
    public ItemInfo fuel_item_info;

    [Export]
    public Timer crafting_timer;

    [Export]
    public Timer state_timer;

    [Export]
    public Array<Machine_Recipe> recipes = new Array<Machine_Recipe>();
    public int ui_progress = 0;
    public int current_recipe = 0;
    int progress = 0;

    public void OnCraftingTimerTimeout()
    {
        FurnaceTab.INSTANCE.UpdateProgressbar(progress);
        if (progress >= 100)
        {
            export_item_info = recipes[current_recipe].export_item_info;
            export_count += recipes[current_recipe].export_amount;
            is_crafting = false;
            crafting_timer.Stop();
            progress = 0;
            FurnaceTab.INSTANCE.UpdateProgressbar(progress);
            FurnaceTab.INSTANCE.UpdateFurnaceUI();
            return;
        }
        progress += 5;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (is_crafting)
            return;
        if (!machine_enabled)
            return;
        if (import_item_info == null)
            return;
        if (!CanCraft())
            return;
        if (import_count <= recipes[current_recipe].import_amount - 1)
            return;

        is_crafting = true;
        import_count -= recipes[current_recipe].import_amount;
        FurnaceTab.INSTANCE.UpdateFurnaceUI();
        crafting_timer.Start();
    }

    private bool CanCraft()
    {
        if (import_item_info == null)
            return false;
        for (int i = 0; i < recipes.Count; i++)
            if (recipes[i].import_item_info == import_item_info)
            {
                current_recipe = i;
                if (export_item_info == null)
                    return true;
                else if (export_item_info == recipes[i].export_item_info)
                    return true;
            }
        return false;
    }

    public void OnMachineTimeOut()
    {
        if (machine_enabled)
        {
            if (ui_progress < 100)
                ui_progress += 2;

            if (ui_progress >= 100)
            {
                state_timer.Stop();
                machine_enabled = true;
                if (FurnaceTab.INSTANCE.process_building == this)
                    FurnaceTab.INSTANCE.switch_button.Disabled = false;
            }
            if (FurnaceTab.INSTANCE.process_building == this)
                FurnaceTab.INSTANCE.SetMachineProgressbar(ui_progress);
        }
        else if (!machine_enabled)
        {
            if (ui_progress >= 2)
                ui_progress -= 2;

            if (ui_progress <= 0)
            {
                state_timer.Stop();
                if (FurnaceTab.INSTANCE.process_building == this)
                {
                    FurnaceTab.INSTANCE.safty_panel.Visible = false;
                    FurnaceTab.INSTANCE.switch_button.Disabled = false;
                }
            }
            if (FurnaceTab.INSTANCE.process_building == this)
                FurnaceTab.INSTANCE.SetMachineProgressbar(ui_progress);
        }
    }
}
