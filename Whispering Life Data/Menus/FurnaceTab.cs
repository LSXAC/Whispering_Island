using System;
using Godot;

public partial class FurnaceTab : ColorRect
{
    [Export]
    public Slot export_slot;

    [Export]
    public Slot import_slot;

    [Export]
    public Slot fuel_slot;

    [Export]
    public Panel safty_panel;
    public ProcessBuilding process_building;
    public static FurnaceTab INSTANCE = null;

    public override void _Ready()
    {
        INSTANCE = this;
    }

    public void SetProcessBuilding(ProcessBuilding process_building)
    {
        this.process_building = process_building;
    }

    public void ClearProcessBuilding()
    {
        process_building = null;
        GameMenu.INSTANCE.OnCloseFurnaceTab();
    }

    public void OnVisiblityChange()
    {
        //ClearProcessBuilding();
    }

    public void OnMachineStateButton()
    {
        if (process_building.machine_enabled)
        {
            safty_panel.Visible = false;
            process_building.machine_enabled = false;
        }
        else
        {
            if (export_slot.GetItem() == null)
                process_building.export_count = 0;
            else
                process_building.export_count = export_slot.GetItem().amount;

            if (import_slot.GetItem() == null)
                process_building.import_count = 0;
            else
                process_building.import_count = import_slot.GetItem().amount;

            if (fuel_slot.GetItem() == null)
                process_building.fuel_count = 0;
            else
                process_building.fuel_count = fuel_slot.GetItem().amount;

            safty_panel.Visible = true;
            process_building.machine_enabled = true;
        }
    }
}
