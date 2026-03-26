using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public abstract partial class ProcessBuilding : MachineBase
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
    protected abstract IProcessingRecipe GetRecipeFromInputSlot();

    protected abstract bool SelectAndCheckCanCraft();

    protected virtual void OnProcessingComplete(IProcessingRecipe recipe)
    {
        // Zu überschreiben durch Subklassen
    }

    protected virtual void OnProcessingTick(IProcessingRecipe recipe)
    {
        // Zu überschreiben durch Subklassen
    }

    protected abstract ProcessingTab GetUIUpdater();
    protected abstract void OpenGameMenuTab();

    protected abstract int GetSlotIndexByPurpose(SlotPurpose purpose);

    public ItemInfo GetItemResource(int slotIndex)
    {
        if (item_array[slotIndex] == null)
            return null;

        return Inventory.ITEM_TYPES[(Inventory.ITEM_ID)item_array[slotIndex].item_id];
    }

    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        GetUIUpdater().SetReferenceBuilding(this);
        OpenGameMenuTab();
    }

    public void OnCraftingTimerTimeout()
    {
        var ui_updater = GetUIUpdater();
        if (ui_updater == null || ui_updater.process_building != this)
            return;

        ui_updater.SetMachineProgressbar(progress);
        ui_updater.UpdateFuelProgressbar((int)((double)fuel_left / max_fuel_count * 100));

        // Check if input slot is empty
        int input_idx = GetSlotIndexByPurpose(SlotPurpose.INPUT);
        if (item_array[input_idx] == null)
        {
            StopCrafting();
            return;
        }

        IProcessingRecipe recipe = GetRecipeFromInputSlot();
        if (recipe == null)
        {
            StopCrafting();
            return;
        }

        if (item_array[input_idx].amount < recipe.GetAmountToProcess())
        {
            StopCrafting();
            return;
        }

        if (progress >= 100)
        {
            ExecuteRecipe(recipe);
            return;
        }

        progress += 5;
        fuel_left -= 1;

        OnProcessingTick(recipe);

        if (hover_menu.instance.current_object == this)
            hover_menu.InitHoverMenu(this);
    }

    protected virtual void ExecuteRecipe(IProcessingRecipe recipe)
    {
        int output_idx = GetSlotIndexByPurpose(SlotPurpose.OUTPUT);
        int input_idx = GetSlotIndexByPurpose(SlotPurpose.INPUT);

        // Add output oder create new
        ItemInfo output_info = recipe.GetOutputItem();
        if (output_info != null)
        {
            if (item_array[output_idx] != null)
                item_array[output_idx].amount += recipe.GetAmountToProcess();
            else
                item_array[output_idx] = new ItemSave(
                    (int)output_info.id,
                    recipe.GetAmountToProcess(),
                    -1,
                    recipe.GetItemState()
                );
        }

        // Consume input
        item_array[input_idx].amount -= recipe.GetAmountToProcess();

        is_crafting = false;
        process_timer.Stop();
        progress = 0;

        var ui_updater = GetUIUpdater();
        if (ui_updater != null && ui_updater.process_building == this)
        {
            ui_updater.SetMachineProgressbar(progress);
            ui_updater.UpdateUI();
        }

        OnProcessingComplete(recipe);
    }

    public void StopCrafting()
    {
        is_crafting = false;
        process_timer.Stop();
        progress = 0;

        var ui_updater = GetUIUpdater();
        if (ui_updater != null && ui_updater.process_building == this)
            ui_updater.SetMachineProgressbar(progress);
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

        var ui_updater = GetUIUpdater();
        if (ui_updater == null)
            return;

        Label description = ui_updater.description_Label;
        if (description == null)
            return;

        int input_idx = GetSlotIndexByPurpose(SlotPurpose.INPUT);

        if (item_array[input_idx] == null)
        {
            description.Text = TranslationServer.Translate("FURNACE_MENU_DESC_NO_RESOURCE");
            return;
        }

        if (!SelectAndCheckCanCraft())
        {
            description.Text = TranslationServer.Translate("FURNACE_MENU_DESC_NO_RECIPE");
            return;
        }

        if (!RefuelIfNeeded())
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
        if (ui_updater.process_building == this)
            ui_updater.UpdateUI();
        process_timer.Start();
    }

    protected virtual bool RefuelIfNeeded()
    {
        int fuel_idx = GetSlotIndexByPurpose(SlotPurpose.FUEL);

        if (fuel_left >= 20)
            return true;

        if (item_array[fuel_idx] == null)
            return false;

        if (item_array[fuel_idx].amount <= 0)
            return false;

        ItemInfo fuel_info = GetItemResource(fuel_idx);
        BurnableAttribute burnable = fuel_info?.GetAttributeOrNull<BurnableAttribute>();

        if (burnable == null)
            return false;

        fuel_left += burnable.burntime;
        item_array[fuel_idx].amount--;
        return true;
    }

    public void ResetExportSlot()
    {
        int output_idx = GetSlotIndexByPurpose(SlotPurpose.OUTPUT);
        item_array[output_idx] = null;
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
        for (int i = 0; i < 4; i++)
            machine_save.furnace_slots[i] = item_array[i];
        machine_save.fuel_left = fuel_left;
        return machine_save;
    }

    public void NotifyItemsChanged()
    {
        try
        {
            var ui_updater = GetUIUpdater();
            if (ui_updater != null && ui_updater.process_building == this)
            {
                ui_updater.UpdateUI();
            }
        }
        catch
        {
            // Ignoriere Fehler falls UI noch nicht initialisiert ist
        }
    }
}
