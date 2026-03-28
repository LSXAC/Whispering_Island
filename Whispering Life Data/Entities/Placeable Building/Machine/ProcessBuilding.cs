using System;
using System.Collections.Generic;
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
    public Godot.Collections.Array recipes = new Godot.Collections.Array();

    public bool is_crafting = false;
    public int ui_progress = 0;
    public int progress = 0;

    // Recipe Selection
    public ProcessingRecipe selected_recipe = null;
    protected abstract ProcessingRecipe GetRecipeFromInputSlot();

    protected abstract bool SelectAndCheckCanCraft();

    protected virtual void OnProcessingComplete(ProcessingRecipe recipe) { }

    protected virtual void OnProcessingTick(ProcessingRecipe recipe) { }

    protected abstract ProcessingTab GetUIUpdater();
    protected abstract void OpenGameMenuTab();

    protected abstract int GetSlotIndexByPurpose(SlotPurpose purpose);

    public ItemInfo GetItemResource(int slotIndex)
    {
        if (item_array[slotIndex] == null)
            return null;

        return Inventory.ITEM_TYPES[(Inventory.ITEM_ID)item_array[slotIndex].item_id];
    }

    public override void _Ready()
    {
        base._Ready();
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
        // Check if input slot is empty
        int input_idx = GetSlotIndexByPurpose(SlotPurpose.INPUT);
        if (item_array[input_idx] == null)
        {
            GD.PrintErr($"[CRAFTING] Input slot is empty!");
            StopCrafting();
            return;
        }

        ProcessingRecipe recipe = GetRecipeFromInputSlot();
        if (recipe == null)
        {
            GD.PrintErr($"[CRAFTING] Recipe not found for {item_array[input_idx].item_id}");
            StopCrafting();
            return;
        }

        if (item_array[input_idx].amount < recipe.GetAmountToProcess())
        {
            GD.PrintErr(
                $"[CRAFTING] Not enough items! Have: {item_array[input_idx].amount}, Need: {recipe.GetAmountToProcess()}"
            );
            StopCrafting();
            return;
        }

        if (progress >= 100)
        {
            GD.PrintErr($"[CRAFTING] ✅ Recipe complete! Executing...");
            ExecuteRecipe(recipe);
            return;
        }

        progress += 5;
        fuel_left -= 1;
        GD.PrintErr($"[CRAFTING] Progress: {progress}/100, Fuel: {fuel_left}");

        OnProcessingTick(recipe);

        // Update UI wenn dieser building gerade angezeigt wird
        var ui_updater = GetUIUpdater();
        if (ui_updater != null && ui_updater.process_building == this)
        {
            ui_updater.SetMachineProgressbar(progress);
            ui_updater.UpdateFuelProgressbar((int)((double)fuel_left / max_fuel_count * 100));
        }

        if (hover_menu.instance.current_object == this)
            hover_menu.InitHoverMenu(this);
    }

    protected virtual void ExecuteRecipe(ProcessingRecipe recipe)
    {
        int output_idx = GetSlotIndexByPurpose(SlotPurpose.OUTPUT);
        int input_idx = GetSlotIndexByPurpose(SlotPurpose.INPUT);

        ItemInfo output_info = recipe.GetOutputItem();
        if (output_info != null)
        {
            if (item_array[output_idx] != null)
            {
                GD.PrintErr(
                    $"[EXECUTE] Adding to existing output: {output_info.name} x{recipe.GetAmountToProduce()}"
                );
                item_array[output_idx].amount += recipe.GetAmountToProduce();
                item_array[output_idx].state = recipe.GetItemState();
            }
            else
            {
                GD.PrintErr(
                    $"[EXECUTE] Creating new output: {output_info.name} x{recipe.GetAmountToProduce()}"
                );
                item_array[output_idx] = new ItemSave(
                    (int)output_info.id,
                    recipe.GetAmountToProduce(),
                    -1,
                    recipe.GetItemState()
                );
            }
        }
        else
        {
            GD.PrintErr($"[EXECUTE] ❌ Output item is null!");
        }

        GD.PrintErr($"[EXECUTE] Consuming input: {recipe.GetAmountToProcess()} items");
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
        GD.PrintErr($"[{this.Name}] CRAFTING STARTED!");
        if (ui_updater != null && ui_updater.process_building == this)
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
        catch { }
    }

    protected virtual void CollectAvailableRecipes() { }

    public Godot.Collections.Array GetAvailableRecipes()
    {
        return recipes;
    }

    public void SelectRecipe(ProcessingRecipe recipe)
    {
        selected_recipe = recipe;
        NotifyItemsChanged();
    }

    public bool CanItemFitInSlot(Inventory.ITEM_ID item_id, int slot_index)
    {
        if (selected_recipe == null)
            return true;

        ItemInfo item_info = Inventory.ITEM_TYPES[item_id];
        if (item_info == null)
            return false;

        int input_idx = GetSlotIndexByPurpose(SlotPurpose.INPUT);
        int aux_idx = GetSlotIndexByPurpose(SlotPurpose.AUXILIARY);

        if (slot_index == input_idx)
        {
            return CanItemFitInInputSlot(item_id);
        }

        if (slot_index == aux_idx && aux_idx != -1)
            return CanItemFitInAuxiliarySlot(item_id);

        return true;
    }

    protected virtual bool CanItemFitInInputSlot(Inventory.ITEM_ID item_id)
    {
        return true;
    }

    protected virtual bool CanItemFitInAuxiliarySlot(Inventory.ITEM_ID item_id)
    {
        return true;
    }

    public bool TryAddItemToSlot(ItemSave item_save, int slot_index)
    {
        if (item_save == null)
            return false;

        if (!CanItemFitInSlot((Inventory.ITEM_ID)item_save.item_id, slot_index))
            return false;

        if (item_array[slot_index] == null)
        {
            item_array[slot_index] = item_save;
        }
        else if (item_array[slot_index].item_id == item_save.item_id)
        {
            item_array[slot_index].amount += item_save.amount;
        }
        else
        {
            return false; // Unterschiedliche Items können nicht kombiniert werden
        }

        NotifyItemsChanged();
        return true;
    }

    public bool CanBeltItemFitInSlots(ItemInfo item_info)
    {
        if (item_info == null)
            return false;

        if (selected_recipe == null)
            return true;

        int input_idx = GetSlotIndexByPurpose(SlotPurpose.INPUT);
        return CanItemFitInInputSlot((Inventory.ITEM_ID)item_info.id);
    }
}
