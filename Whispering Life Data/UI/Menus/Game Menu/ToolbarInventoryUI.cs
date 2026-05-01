using System;
using Godot;

public partial class ToolbarInventoryUI : Inventory
{
    public static ToolbarInventoryUI instance = null;

    public override void _Ready()
    {
        instance = this;
        slot_amount = 8;
        base._Ready();
        SetSlots();
    }

    public override void UpdateSlot(int index, SlotItemUI pref_ref = null)
    {
        if (inventory_items[index] == null)
        {
            ClearSlot(index);
            PlayerUI.instance?.equipmentSelectBar.ClearSelectSlot(index);
            return;
        }
        GetNode<Slot>($"GridContainer/SlotBackground{index}/Slot0")
            .UpdateItem(
                new Item(
                    GetItemInfo(index),
                    inventory_items[index].amount,
                    (Item.STATE)inventory_items[index].state
                ),
                GetDurability(index)
            );

        PlayerUI
            .instance?.equipmentSelectBar.select_slots[index]
            .UpdateItem(
                new Item(
                    GetItemInfo(index),
                    inventory_items[index].amount,
                    (Item.STATE)inventory_items[index].state
                ),
                GetDurability(index)
            );

        if (pref_ref != null)
            GetNode<Slot>($"GridContainer/SlotBackground{index}/Slot0")
                .GetSlotItemUI()
                .SelfModulate = pref_ref.SelfModulate;

        if (QuestManager.current_selected_quest != null)
            QuestMiniPanel.instance.UpdateQuestMiniPanel(QuestManager.current_selected_quest);
    }

    public override void ClearSlot(int index)
    {
        GetNode<Slot>($"GridContainer/SlotBackground{index}/Slot0").ClearSlotItem();
        PlayerUI.instance?.equipmentSelectBar.ClearSelectSlot(index);
    }
}
