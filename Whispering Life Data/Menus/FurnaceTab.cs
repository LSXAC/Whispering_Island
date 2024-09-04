using System;
using System.Diagnostics;
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
    public static FurnaceTab INSTANCE = null;
    public ProcessBuilding process_building;

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
        UpdateFurnaceUI();
    }

    public void UpdateFurnaceUI()
    {
        if (process_building == null)
            return;
        Debug.Print("Clear");
        export_slot.ClearItem();
        import_slot.ClearItem();
        fuel_slot.ClearItem();

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
        SetMachineProgressbar(process_building.ui_progress);
        UpdateProgressbar(process_building.progress);

        if (process_building.export_item_info != null && process_building.export_count != 0)
            export_slot.SetItem(process_building.export_item_info, process_building.export_count);

        if (process_building.import_item_info != null && process_building.import_count != 0)
            import_slot.SetItem(process_building.import_item_info, process_building.import_count);

        if (process_building.fuel_item_info != null && process_building.fuel_count != 0)
            fuel_slot.SetItem(process_building.fuel_item_info, process_building.fuel_count);
    }

    public void ClearProcessBuilding()
    {
        OvertakeItems();
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
            OvertakeItems();
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

    private void OvertakeItems()
    {
        if (export_slot.GetItem() == null)
        {
            process_building.export_item_info = null;
            process_building.export_count = 0;
        }
        else
        {
            process_building.export_item_info = export_slot.GetItem().item_info;
            process_building.export_count = export_slot.GetItem().amount;
        }

        if (import_slot.GetItem() == null)
        {
            process_building.import_item_info = null;
            process_building.import_count = 0;
        }
        else
        {
            process_building.import_item_info = import_slot.GetItem().item_info;
            process_building.import_count = import_slot.GetItem().amount;
        }

        if (fuel_slot.GetItem() == null)
        {
            process_building.fuel_item_info = null;
            process_building.fuel_count = 0;
        }
        else
        {
            process_building.fuel_item_info = fuel_slot.GetItem().item_info;
            process_building.fuel_count = fuel_slot.GetItem().amount;
        }
    }
}
