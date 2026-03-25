using System;
using System.Collections.Generic;
using Godot;

public partial class ProcessingTab : SlotUpdater
{
    [Export]
    public Slot export_slot;

    [Export]
    public Slot import_slot;

    [Export]
    public Slot fuel_slot;

    [Export]
    public Label fuel_label;

    [Export]
    public ProgressBar machine_progress_bar;

    [Export]
    public ProgressBar fuel_progress_bar;

    [Export]
    public Button switch_button;

    [Export]
    public Label working_label;

    [Export]
    public Label description_label;

    public static ProcessingTab instance;

    public enum SlotType
    {
        IMPORT,
        EXPORT,
        FUEL
    }

    public ProcessBuilding process_building;

    private readonly Dictionary<SlotType, Slot> slot_map = new();

    public override void _Ready()
    {
        instance = this;
        slot_map[SlotType.IMPORT] = import_slot;
        slot_map[SlotType.EXPORT] = export_slot;
        slot_map[SlotType.FUEL] = fuel_slot;
    }

    public override void UpdateSlot(int index, SlotItemUI slot_item_ui)
    {
        ClearSlot(index);
        UpdateUI();
    }

    public override void ClearSlot(int index)
    {
        SlotType slot_type = (SlotType)index;
        if (slot_map.TryGetValue(slot_type, out var slot))
            slot.ClearSlotItem();
    }

    public override ItemInfo GetItemInfo(int index)
    {
        if (process_building == null)
            throw new Exception("No Process Building set");

        return Inventory.ITEM_TYPES[(Inventory.ITEM_ID)process_building.item_array[index].item_id];
    }

    public void ClearInfo(SlotType type)
    {
        process_building.item_array[(int)type] = null;
    }

    public void UpdateInfoHalf(SlotType type)
    {
        if (process_building?.item_array[(int)type] == null)
            return;

        process_building.item_array[(int)type].amount = (int)(
            process_building.item_array[(int)type].amount / 2.0
        );
        UpdateUI();
    }

    public void SetMachineProgressbar(int amount) => machine_progress_bar.Value = amount;

    public void UpdateFuelProgressbar(int amount)
    {
        fuel_progress_bar.Value = amount;
        fuel_label.Text = $"{process_building.fuel_left}/{process_building.max_fuel_count}";
    }

    public void SetProcessBuilding(ProcessBuilding process_building)
    {
        this.process_building = process_building;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (process_building == null)
            return;

        ClearAllSlots();
        UpdateMachineProgressBars();
        UpdateMachineState();
        UpdateSlotItems();
    }

    public void ClearProcessBuilding()
    {
        OvertakeItems();
        process_building = null;
        GameMenu.instance.OnCloseFurnaceTab();
    }

    public void OnMachineStateButton()
    {
        if (process_building.machine_enabled)
            DisableMachine();
        else
            EnableMachine();

        process_building.state_timer.Start();
        switch_button.Disabled = true;
    }

    private void EnableMachine()
    {
        OvertakeItems();
        process_building.EnableMachine();
        process_building.inEndTransition = true;
        ChangeEndStateLabel(false);
        ChangeTransitionStateLabel(false);
    }

    private void DisableMachine()
    {
        ChangeEndStateLabel(true);
        ChangeTransitionStateLabel(true);
        process_building.DisableMachine();
        process_building.inStartTransition = true;
    }

    private void ClearAllSlots()
    {
        foreach (var slot in slot_map.Values)
            slot.ClearSlotItem();
    }

    private void UpdateMachineProgressBars()
    {
        if (process_building.ui_progress == 0 || process_building.ui_progress == 100)
            switch_button.Disabled = false;

        SetMachineProgressbar(process_building.ui_progress);
        UpdateFuelProgressbar(
            (int)((double)process_building.fuel_left / process_building.max_fuel_count * 100)
        );
    }

    private void UpdateMachineState()
    {
        bool is_machine_idle =
            !process_building.machine_enabled
            && !process_building.inStartTransition
            && !process_building.inEndTransition;

        if (is_machine_idle)
            ChangeEndStateLabel(true);
        else
        {
            if (process_building.inStartTransition)
                ChangeTransitionStateLabel(true);
            else if (process_building.inEndTransition)
                ChangeTransitionStateLabel(false);
            else
                ChangeEndStateLabel(false);
        }
    }

    private void UpdateSlotItems()
    {
        foreach (SlotType slot_type in Enum.GetValues(typeof(SlotType)))
        {
            var item_save = process_building.item_array[(int)slot_type];
            if (item_save != null && item_save.amount > 0)
            {
                var item = new Item(
                    GetItemInfo((int)slot_type),
                    item_save.amount,
                    state: (Item.STATE)item_save.state
                );
                slot_map[slot_type].SetItem(item);
            }
        }
    }

    public void ChangeEndStateLabel(bool state)
    {
        if (state)
        {
            switch_button.Text = TranslationServer.Translate("FURNACE_MENU_ENABLE_MACHINE");
            working_label.Text = TranslationServer.Translate("FURNACE_MENU_NOT_WORKING");
        }
        else
        {
            switch_button.Text = TranslationServer.Translate("FURNACE_MENU_DISABLE_MACHINE");
            working_label.Text = TranslationServer.Translate("FURNACE_MENU_WORKING");
        }
    }

    public void ChangeTransitionStateLabel(bool state)
    {
        working_label.Text = state
            ? TranslationServer.Translate("FURNACE_MENU_COOLING_DOWN")
            : TranslationServer.Translate("FURNACE_MENU_HEATING_UP");
    }

    private void OvertakeItems()
    {
        OvertakeSlotItem(SlotType.EXPORT, export_slot);
        OvertakeSlotItem(SlotType.IMPORT, import_slot);
        OvertakeSlotItem(SlotType.FUEL, fuel_slot);
    }

    private void OvertakeSlotItem(SlotType slot_type, Slot slot)
    {
        var slot_ui = slot.GetSlotItemUI();

        if (slot_ui == null)
            ClearInfo(slot_type);
        else
        {
            process_building.item_array[(int)slot_type] = new ItemSave(
                (int)slot_ui.item.info.id,
                slot_ui.item.amount,
                -1,
                (int)slot_ui.item.state
            );
        }
    }
}
