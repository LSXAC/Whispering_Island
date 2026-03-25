using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class ProcessBuilding : MachineBase
{
    [Export]
    public ItemSave[] item_array = [null, null, null];

    [Export]
    public int max_fuel_count = 250;

    [Export]
    public int fuel_left = 0;

    [Export]
    public Timer state_timer;

    public bool is_crafting = false;
    public int ui_progress = 0;
    public int progress = 0;

    public bool inStartTransition = false;
    public bool inEndTransition = false;

    public ItemInfo GetItemResource(ProcessingTab.SlotType slotType)
    {
        if (item_array[(int)slotType] == null)
            return null;

        return Inventory.ITEM_TYPES[(Inventory.ITEM_ID)item_array[(int)slotType].item_id];
    }

    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        ProcessingTab.instance.SetProcessBuilding(this);
        GameMenu.instance.OnOpenProcessingTab();
    }

    public void OnCraftingTimerTimeout()
    {
        if (ProcessingTab.instance.process_building == this)
            ProcessingTab.instance.SetMachineProgressbar(progress);

        if (ProcessingTab.instance.process_building == this)
            ProcessingTab.instance.UpdateFuelProgressbar(
                (int)((double)fuel_left / max_fuel_count * 100)
            );

        ItemInfo import_item_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)ProcessingTab.SlotType.IMPORT].item_id
        ];
        SmeltableAttribute smeltable = import_item_info.GetAttributeOrNull<SmeltableAttribute>();

        if (progress >= 100)
        {
            if (item_array[(int)ProcessingTab.SlotType.EXPORT] != null)
                item_array[(int)ProcessingTab.SlotType.EXPORT].amount += smeltable
                    .smelted_to_item
                    .amount;
            else
                item_array[(int)ProcessingTab.SlotType.EXPORT] = new ItemSave(
                    (int)smeltable.smelted_to_item.info.id,
                    smeltable.smelted_to_item.amount,
                    -1,
                    (int)smeltable.smelted_to_item.state
                );

            is_crafting = false;
            process_timer.Stop();
            progress = 0;
            if (ProcessingTab.instance.process_building == this)
                ProcessingTab.instance.SetMachineProgressbar(progress);
            if (ProcessingTab.instance.process_building == this)
                ProcessingTab.instance.UpdateUI();
            return;
        }

        progress += 5;
        fuel_left -= 1;

        if (hover_menu.instance.current_object == this)
            hover_menu.InitHoverMenu(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (machine_enabled && !has_enough_magic_power)
        {
            DisableMachine();
            state_timer.Paused = true;
        }
        else if (!machine_enabled && has_enough_magic_power)
        {
            EnableMachine();
            state_timer.Paused = false;
        }

        if (is_crafting)
            return;

        Label description = ProcessingTab.instance.description_label;

        if (item_array[(int)ProcessingTab.SlotType.IMPORT] == null)
        {
            description.Text = TranslationServer.Translate("FURNACE_MENU_DESC_NO_RESOURCE");
            return;
        }

        if (!SelectAndCheckCanCraft())
        {
            description.Text = TranslationServer.Translate("FURNACE_MENU_DESC_NO_RECIPE");
            return;
        }

        ItemInfo import_item_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)ProcessingTab.SlotType.IMPORT].item_id
        ];
        SmeltableAttribute smeltable = import_item_info.GetAttributeOrNull<SmeltableAttribute>();

        if (fuel_left < 20)
            if (item_array[(int)ProcessingTab.SlotType.FUEL] != null)
            {
                if (item_array[(int)ProcessingTab.SlotType.FUEL].amount > 0)
                {
                    ItemInfo info = Inventory.ITEM_TYPES[
                        (Inventory.ITEM_ID)item_array[(int)ProcessingTab.SlotType.FUEL].item_id
                    ];
                    BurnableAttribute attribute = info.GetAttributeOrNull<BurnableAttribute>();
                    if (attribute != null)
                    {
                        fuel_left += attribute.burntime;
                        item_array[(int)ProcessingTab.SlotType.FUEL].amount--;
                    }
                    else
                    {
                        description.Text = TranslationServer.Translate("FURNACE_MENU_DESC_NO_FUEL");
                        return;
                    }
                }
                else
                {
                    description.Text = TranslationServer.Translate("FURNACE_MENU_DESC_NO_FUEL");
                    return;
                }
            }
            else
            {
                description.Text = TranslationServer.Translate("FURNACE_MENU_DESC_NO_FUEL");
                return;
            }

        if (!machine_enabled)
        {
            description.Text = "";
            return;
        }
        else
            description.Text = TranslationServer.Translate("FURNACE_MENU_DESC");

        is_crafting = true;
        item_array[(int)ProcessingTab.SlotType.IMPORT].amount -= smeltable.amount_to_smelt;
        if (ProcessingTab.instance.process_building == this)
            ProcessingTab.instance.UpdateUI();
        process_timer.Start();
    }

    private bool SelectAndCheckCanCraft()
    {
        if (item_array[(int)ProcessingTab.SlotType.IMPORT] == null)
            return false;

        ItemInfo import_item_info = Inventory.ITEM_TYPES[
            (Inventory.ITEM_ID)item_array[(int)ProcessingTab.SlotType.IMPORT].item_id
        ];
        SmeltableAttribute smeltable = import_item_info.GetAttributeOrNull<SmeltableAttribute>();

        if (smeltable == null)
            return false;

        if (smeltable.unlock_requirements != null || smeltable.unlock_requirements.Count > 0)
            if (!GlobalFunctions.CheckResearchRequirements(smeltable.unlock_requirements))
                return false;

        if (item_array[(int)ProcessingTab.SlotType.IMPORT].amount < smeltable.amount_to_smelt)
            return false;

        if (item_array[(int)ProcessingTab.SlotType.EXPORT] == null)
            return true;
        else if (
            item_array[(int)ProcessingTab.SlotType.EXPORT].item_id
            != (int)smeltable.smelted_to_item.info.id
        )
            return false;

        return true;
    }

    public void ResetExportSlot()
    {
        item_array[(int)ProcessingTab.SlotType.EXPORT] = null;
    }

    public void OnMachineTimeOut()
    {
        Button switch_button = ProcessingTab.instance.switch_button;
        ProcessBuilding process_building = ProcessingTab.instance.process_building;

        if (!inEndTransition)
        {
            switch_button.Disabled = true;
            if (ui_progress > 0)
                ui_progress -= 2;

            if (ui_progress <= 0)
            {
                ui_progress = 0;
                if (process_building == this)
                    switch_button.Disabled = false;

                ProcessingTab.instance.ChangeEndStateLabel(true);
                state_timer.Stop();
                inStartTransition = false;
            }
            if (process_building == this)
                ProcessingTab.instance.SetMachineProgressbar(ui_progress);
            return;
        }

        if (!inStartTransition)
        {
            switch_button.Disabled = true;
            if (ui_progress < 100)
                ui_progress += 2;

            if (ui_progress >= 100)
            {
                ui_progress = 100;
                ProcessingTab.instance.ChangeEndStateLabel(false);
                if (process_building == this)
                    switch_button.Disabled = false;
                state_timer.Stop();
                inEndTransition = false;
            }
            if (process_building == this)
                ProcessingTab.instance.SetMachineProgressbar(ui_progress);
        }
    }

    public override void Load(Resource save)
    {
        if (save is MachineSave machine_save)
        {
            base.Load(save);
            for (int i = 0; i < machine_save.furnace_slots.Length; i++)
                item_array[i] = machine_save.furnace_slots[i];

            fuel_left = machine_save.fuel_left;
        }
        else
            Logger.PrintWrongSaveType();
    }

    public override Resource Save()
    {
        MachineSave machine_save = (MachineSave)base.Save();
        for (int i = 0; i < 3; i++)
            machine_save.furnace_slots[i] = item_array[i];
        machine_save.fuel_left = fuel_left;
        return machine_save;
    }
}
