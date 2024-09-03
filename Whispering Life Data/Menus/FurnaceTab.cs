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

    [Export]
    public ProgressBar machineProgressbar;

    [Export]
    public ProgressBar workingProgressbar;

    [Export]
    public Button switch_button;

    [Export]
    public Label working_label;
    public ProcessBuilding process_building;
    public static FurnaceTab INSTANCE = null;

    public override void _Ready()
    {
        INSTANCE = this;
    }

    public void SetMachineProgressbar(int amount)
    {
        machineProgressbar.Value = amount;
    }

    public void UpdateProgressbar(int amount)
    {
        workingProgressbar.Value = amount;
    }

    public void SetProcessBuilding(ProcessBuilding process_building)
    {
        this.process_building = process_building;
        SetMachineProgressbar(process_building.ui_progress);
        if (process_building.ui_progress == 0 || process_building.ui_progress == 100)
            switch_button.Disabled = false;

        if (!process_building.machine_enabled)
        {
            ChangeStateLabel(true);
            safty_panel.Visible = false;
        }
        else
        {
            ChangeStateLabel(false);
            safty_panel.Visible = true;
        }
    }

    public void ClearProcessBuilding()
    {
        process_building = null;
        GameMenu.INSTANCE.OnCloseFurnaceTab();
    }

    public void OnMachineStateButton()
    {
        process_building.state_timer.Start();
        switch_button.Disabled = true;
        if (process_building.machine_enabled)
        {
            process_building.machine_enabled = false;
            ChangeStateLabel(true);
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
            ChangeStateLabel(false);
        }
    }

    private void ChangeStateLabel(bool state)
    {
        if (state)
        {
            switch_button.Text = "Enable Machine";
            working_label.Text = "Machine Offline - No I/O";
        }
        else
        {
            switch_button.Text = "Disable Machine";
            working_label.Text = "Machine Online - Working";
        }
    }
}
