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
    public Timer machineTimer;

    [Export]
    public Button switch_button;
    public ProcessBuilding process_building;
    public static FurnaceTab INSTANCE = null;

    public override void _Ready()
    {
        INSTANCE = this;
        SetMachineProgressbar(100);
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
        if (process_building.machine_enabled)
        {
            SetMachineProgressbar(100);
            safty_panel.Visible = true;
        }
        else
        {
            SetMachineProgressbar(0);
            safty_panel.Visible = false;
        }
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

    public void OnMachineTimeOut()
    {
        if (process_building.machine_enabled)
        {
            if (machineProgressbar.Value < 100)
                machineProgressbar.Value += 2;
            if (machineProgressbar.Value >= 100)
            {
                machineTimer.Stop();
                switch_button.Disabled = false;
                process_building.machine_enabled = true;
            }
        }
        else if (!process_building.machine_enabled)
        {
            if (machineProgressbar.Value >= 2)
                machineProgressbar.Value -= 2;
            if (machineProgressbar.Value <= 0)
            {
                machineTimer.Stop();
                safty_panel.Visible = false;
                switch_button.Disabled = false;
            }
        }
    }

    public void OnMachineStateButton()
    {
        machineTimer.Start();
        switch_button.Disabled = true;
        if (process_building.machine_enabled)
        {
            process_building.machine_enabled = false;
            switch_button.Text = "Enable Machine";
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
            switch_button.Text = "Disable Machine";
        }
    }
}
