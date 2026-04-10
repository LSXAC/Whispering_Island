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
    public bool requires_recipe = true;

    [Export]
    public Timer state_timer;
    public Godot.Collections.Array recipes = new Godot.Collections.Array();

    public bool is_crafting = false;
    public int ui_progress = 0;
    public int progress = 0;

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

    private void UpdateRecipeAndMaterialPanels(bool has_no_materials, bool has_no_recipe)
    {
        if (!Logger.NodeIsNotNull(building_information_panel_instance))
            return;

        building_information_panel_instance.DeactivatePanel(
            BuildingInformationPanel.PanelType.NO_INPUT
        );
        building_information_panel_instance.DeactivatePanel(
            BuildingInformationPanel.PanelType.NO_RECIPE
        );

        if (has_no_materials)
            building_information_panel_instance.ActivatePanel(
                BuildingInformationPanel.PanelType.NO_INPUT
            );

        if (has_no_recipe && requires_recipe)
            building_information_panel_instance.ActivatePanel(
                BuildingInformationPanel.PanelType.NO_RECIPE
            );
    }

    public void OnCraftingTimerTimeout()
    {
        int input_idx = GetSlotIndexByPurpose(SlotPurpose.INPUT);
        if (item_array[input_idx] == null)
        {
            StopCrafting();
            return;
        }

        ProcessingRecipe recipe = GetRecipeFromInputSlot();
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
                item_array[output_idx].amount += recipe.GetAmountToProduce();
                item_array[output_idx].state = recipe.GetItemState();
            }
            else
            {
                item_array[output_idx] = new ItemSave(
                    (int)output_info.id,
                    recipe.GetAmountToProduce(),
                    -1,
                    recipe.GetItemState()
                );
            }
        }

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

        int input_idx = GetSlotIndexByPurpose(SlotPurpose.INPUT);

        bool has_no_materials = item_array[input_idx] == null;
        bool has_no_recipe = selected_recipe == null;
        bool has_no_fuel = !RefuelIfNeeded();

        UpdateRecipeAndMaterialPanels(has_no_materials, has_no_recipe);

        if (is_crafting)
            return;

        var ui_updater = GetUIUpdater();
        if (ui_updater == null)
            return;

        Label description = ui_updater.description_Label;
        if (description == null)
            return;

        if (has_no_materials)
            description.Text = TranslationServer.Translate("FURNACE_MENU_DESC_NO_RESOURCE");
        else if (has_no_recipe && requires_recipe)
            description.Text = TranslationServer.Translate("FURNACE_MENU_DESC_NO_RECIPE");
        else if (has_no_fuel)
        {
            description.Text = TranslationServer.Translate("FURNACE_MENU_DESC_NO_FUEL");
            return;
        }
        else if (!machine_enabled)
        {
            description.Text = "";
            return;
        }
        else
            description.Text = TranslationServer.Translate("FURNACE_MENU_DESC");

        if (
            !has_no_materials
            && (!has_no_recipe || !requires_recipe)
            && !has_no_fuel
            && machine_enabled
        )
        {
            is_crafting = true;
            if (ui_updater != null && ui_updater.process_building == this)
                ui_updater.UpdateUI();
            process_timer.Start();
        }
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

            if (
                machine_save.selected_recipe_index >= 0
                && machine_save.selected_recipe_index < recipes.Count
            )
            {
                selected_recipe = (ProcessingRecipe)recipes[machine_save.selected_recipe_index];

                // Update slot icons after loading recipe
                ProcessingTab ui_updater = GetUIUpdater() as ProcessingTab;
                if (ui_updater != null)
                {
                    ui_updater.UpdateRecipeSlotIcons(selected_recipe);
                }

                if (Logger.NodeIsNotNull(building_information_panel_instance))
                {
                    building_information_panel_instance.DeactivatePanel(
                        BuildingInformationPanel.PanelType.NO_RECIPE
                    );
                }
            }
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

        machine_save.selected_recipe_index = -1;

        if (selected_recipe != null)
        {
            for (int i = 0; i < recipes.Count; i++)
            {
                try
                {
                    Variant recipeVariant = recipes[i];
                    ProcessingRecipe recipe = recipeVariant.As<ProcessingRecipe>();

                    if (recipe != null && ReferenceEquals(recipe, selected_recipe))
                    {
                        machine_save.selected_recipe_index = i;
                        break;
                    }
                }
                catch { }
            }
        }

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

        if (recipe != null)
        {
            int input_idx = GetSlotIndexByPurpose(SlotPurpose.INPUT);
            int output_idx = GetSlotIndexByPurpose(SlotPurpose.OUTPUT);

            if (item_array[input_idx] != null)
            {
                ItemInfo required_input = recipe.GetInputRequirement();
                ItemInfo current_input = GetItemResource(input_idx);

                if (required_input != null && current_input != null)
                {
                    if (current_input.id != required_input.id)
                    {
                        Item item_to_transfer = new Item(
                            current_input,
                            item_array[input_idx].amount,
                            (Item.STATE)item_array[input_idx].state
                        );

                        if (PlayerInventoryUI.instance != null)
                            PlayerInventoryUI.instance.AddItem(
                                item_to_transfer,
                                PlayerInventoryUI.instance.inventory_items
                            );

                        item_array[input_idx] = null;
                    }
                }
            }

            if (item_array[output_idx] != null)
            {
                ItemInfo required_output = recipe.GetOutputItem();
                ItemInfo current_output = GetItemResource(output_idx);

                if (required_output != null && current_output != null)
                {
                    if (current_output.id != required_output.id)
                    {
                        Item item_to_transfer = new Item(
                            current_output,
                            item_array[output_idx].amount,
                            (Item.STATE)item_array[output_idx].state
                        );

                        if (PlayerInventoryUI.instance != null)
                            PlayerInventoryUI.instance.AddItem(
                                item_to_transfer,
                                PlayerInventoryUI.instance.inventory_items
                            );

                        item_array[output_idx] = null;
                    }
                }
            }
        }

        NotifyItemsChanged();

        ProcessingTab ui_updater = GetUIUpdater() as ProcessingTab;
        if (ui_updater != null)
        {
            ui_updater.UpdateRecipeSlotIcons(recipe);
        }

        if (recipe != null && Logger.NodeIsNotNull(building_information_panel_instance))
        {
            building_information_panel_instance.DeactivatePanel(
                BuildingInformationPanel.PanelType.NO_RECIPE
            );
        }
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
            item_array[slot_index] = item_save;
        else if (item_array[slot_index].item_id == item_save.item_id)
            item_array[slot_index].amount += item_save.amount;
        else
            return false; // Unterschiedliche Items können nicht kombiniert werden

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
