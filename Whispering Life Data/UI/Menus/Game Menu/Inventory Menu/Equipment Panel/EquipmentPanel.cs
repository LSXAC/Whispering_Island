using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class EquipmentPanel : Control
{
    public static EquipmentPanel instance = null;

    [Export]
    public ArmorInventoryUI armor_inventory_ui;

    [Export]
    public ToolbarInventoryUI toolbar_inventory_ui;

    [Export]
    public PlayerStatsUI player_stats_ui;

    public override void _Ready()
    {
        instance = this;
    }

    public void RemoveDurability(int amount)
    {
        if (toolbar_inventory_ui.inventory_items[EquipmentSelectBar.current_selected_slot] != null)
        {
            toolbar_inventory_ui
                .inventory_items[EquipmentSelectBar.current_selected_slot]
                .current_durability -= amount;

            if (
                instance
                    .toolbar_inventory_ui.slots[EquipmentSelectBar.current_selected_slot]
                    .GetSlotItemUI() != null
            )
            {
                SlotItemUI item = instance
                    .toolbar_inventory_ui.slots[EquipmentSelectBar.current_selected_slot]
                    .GetSlotItemUI();
                item.current_durability -= amount;
                if (item.current_durability < 0)
                {
                    instance
                        .toolbar_inventory_ui.slots[EquipmentSelectBar.current_selected_slot]
                        .ClearSlotItem();
                    PlayerUI.instance.equipmentSelectBar.ClearSelectSlot(
                        EquipmentSelectBar.current_selected_slot
                    );
                    return;
                }
            }
            toolbar_inventory_ui.UpdateSlot(EquipmentSelectBar.current_selected_slot);
        }
    }

    public void AddDurability(int amount)
    {
        if (toolbar_inventory_ui.inventory_items[EquipmentSelectBar.current_selected_slot] != null)
        {
            toolbar_inventory_ui
                .inventory_items[EquipmentSelectBar.current_selected_slot]
                .current_durability += amount;
            toolbar_inventory_ui.UpdateSlot(EquipmentSelectBar.current_selected_slot);
        }
    }
}
