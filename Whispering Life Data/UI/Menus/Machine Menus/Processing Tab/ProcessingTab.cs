using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

/// <summary>
/// Generische Basis-Klasse für alle Verarbeitungs-UI-Fenster (Furnace, Combiner, etc.)
/// Ermöglicht dynamische Slot-Konfiguration und generische Rezept-Verarbeitung
/// </summary>
public abstract partial class ProcessingTab : SlotUpdater
{
    [Export]
    public Label fuel_label;

    [Export]
    public ProgressBar machineProgressbar;

    [Export]
    public ProgressBar fuelProgressbar;

    [Export]
    public Label description_Label;

    [Export]
    public RecipeOverviewPanel recipe_overview_panel;

    protected ProcessingSlotConfig[] slot_configs = Array.Empty<ProcessingSlotConfig>();

    protected Dictionary<int, Slot> slot_by_index = new Dictionary<int, Slot>();
    public ProcessBuilding process_building;

    protected void InitializeSlots(ProcessingSlotConfig[] configs)
    {
        slot_configs = configs;
        slot_by_index.Clear();

        for (int i = 0; i < slot_configs.Length; i++)
            if (slot_configs[i].ui_reference != null)
                slot_by_index[i] = slot_configs[i].ui_reference;
    }

    public override void UpdateSlot(int index, SlotItemUI slot_item_ui)
    {
        ClearSlot(index);
        UpdateUI();
    }

    public override void ClearSlot(int index)
    {
        if (index >= 0 && index < slot_by_index.Count)
            slot_by_index[index].ClearSlotItem();
    }

    public override ItemInfo GetItemInfo(int index)
    {
        if (process_building == null)
            throw new Exception("No Process Building set");

        if (index < 0 || index >= process_building.item_array.Length)
            throw new Exception(
                $"Slot index {index} out of bounds. Array length: {process_building.item_array.Length}"
            );

        return Inventory.ITEM_TYPES[(Inventory.ITEM_ID)process_building.item_array[index].item_id];
    }

    public void ClearInfo(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < process_building.item_array.Length)
            process_building.item_array[slotIndex] = null;
    }

    public void UpdateInfoHalf(int slotIndex)
    {
        if (process_building?.item_array == null)
            return;

        if (slotIndex < 0 || slotIndex >= process_building.item_array.Length)
            return;

        if (process_building.item_array[slotIndex] == null)
            return;

        process_building.item_array[slotIndex].amount = (int)(
            process_building.item_array[slotIndex].amount / 2.0
        );
        UpdateUI();
    }

    public void SetMachineProgressbar(int amount)
    {
        machineProgressbar.Value = amount;
    }

    public void UpdateFuelProgressbar(int amount)
    {
        fuelProgressbar.Value = amount;
        fuel_label.Text = process_building.fuel_left + "/" + process_building.max_fuel_count;
    }

    public void SetReferenceBuilding(ProcessBuilding process_building)
    {
        GD.PrintErr(
            $"[ProcessingTab.SetReferenceBuilding] START - building: {(process_building != null ? process_building.GetType().Name : "NULL")}"
        );

        this.process_building = process_building;

        GD.PrintErr(
            $"[ProcessingTab.SetReferenceBuilding] recipe_overview_panel: {(recipe_overview_panel != null ? "✓ FOUND" : "❌ NULL")}"
        );

        if (recipe_overview_panel != null)
        {
            GD.PrintErr(
                "[ProcessingTab.SetReferenceBuilding] Calling SetReferenceBuilding on RecipeOverviewPanel"
            );
            recipe_overview_panel.SetReferenceBuilding(process_building);
        }

        UpdateUI();
        GD.PrintErr("[ProcessingTab.SetReferenceBuilding] END");
    }

    public virtual void UpdateUI()
    {
        if (process_building == null)
            return;

        if (recipe_overview_panel != null)
            recipe_overview_panel.ReloadRecipes();

        foreach (var slot in slot_by_index.Values)
            slot.ClearSlotItem();

        SetMachineProgressbar(process_building.ui_progress);
        UpdateFuelProgressbar(
            (int)((double)process_building.fuel_left / process_building.max_fuel_count * 100)
        );

        for (int i = 0; i < slot_configs.Length; i++)
        {
            if (i >= process_building.item_array.Length)
                break;

            if (process_building.item_array[i] != null && process_building.item_array[i].amount > 0)
            {
                if (slot_by_index.TryGetValue(i, out var slot))
                {
                    slot.SetItem(
                        new Item(
                            GetItemInfo(i),
                            process_building.item_array[i].amount,
                            state: (Item.STATE)process_building.item_array[i].state
                        )
                    );
                }
            }
        }

        if (process_building.selected_recipe != null)
            UpdateRecipeSlotIcons(process_building.selected_recipe);
    }

    protected void OvertakeItems()
    {
        for (int i = 0; i < slot_configs.Length; i++)
        {
            if (i >= process_building.item_array.Length)
                break;

            SetOrClearSlotItem(i);
        }
    }

    protected void SetOrClearSlotItem(int slotIndex)
    {
        if (!slot_by_index.TryGetValue(slotIndex, out var slot))
            return;

        if (slotIndex < 0 || slotIndex >= process_building.item_array.Length)
            return;

        if (slot.GetSlotItemUI() == null)
            ClearInfo(slotIndex);
        else
        {
            process_building.item_array[slotIndex] = new ItemSave(
                (int)slot.GetSlotItemUI().item.info.id,
                slot.GetSlotItemUI().item.amount,
                -1,
                (int)slot.GetSlotItemUI().item.state
            );
        }
    }

    protected int GetSlotIndexByPurpose(SlotPurpose purpose)
    {
        for (int i = 0; i < slot_configs.Length; i++)
            if (slot_configs[i].purpose == purpose)
                return i;

        return -1;
    }

    protected int[] GetSlotIndicesByPurpose(SlotPurpose purpose)
    {
        var indices = new System.Collections.Generic.List<int>();
        for (int i = 0; i < slot_configs.Length; i++)
            if (slot_configs[i].purpose == purpose)
                indices.Add(i);

        return indices.ToArray();
    }

    public void UpdateRecipeSlotIcons(ProcessingRecipe recipe)
    {
        if (recipe == null || process_building == null)
            return;

        foreach (var slot in slot_by_index.Values)
            if (slot != null)
                slot.Icon = null;

        int input_idx = GetSlotIndexByPurpose(SlotPurpose.INPUT);
        if (input_idx >= 0 && slot_by_index.TryGetValue(input_idx, out var input_slot))
        {
            ItemInfo input_requirement = recipe.GetInputRequirement();
            if (input_requirement != null && input_requirement.texture != null)
                input_slot.Icon = input_requirement.texture;
        }

        int output_idx = GetSlotIndexByPurpose(SlotPurpose.OUTPUT);
        if (output_idx >= 0 && slot_by_index.TryGetValue(output_idx, out var output_slot))
        {
            ItemInfo output_requirement = recipe.GetOutputItem();
            if (output_requirement != null && output_requirement.texture != null)
                output_slot.Icon = output_requirement.texture;
        }
    }
}
