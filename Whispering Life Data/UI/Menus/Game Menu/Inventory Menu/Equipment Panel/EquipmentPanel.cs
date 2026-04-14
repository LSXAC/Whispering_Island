using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class EquipmentPanel : Control
{
    public static EquipmentPanel instance = null;

    [Export]
    public ItemSave[] equipped_armor = new ItemSave[4];

    [Export]
    public ItemSave[] equipped_tools = new ItemSave[4];

    [Export]
    public Array<Slot> slots_armor = new Array<Slot>();

    [Export]
    public Array<Slot> slots_tool = new Array<Slot>();

    [Export]
    public PlayerStatsUI player_stats_ui;

    public override void _Ready()
    {
        instance = this;
    }

    public void OnVisiblityChanged()
    {
        if (Player.instance != null)
            UpdateProgressbars();
    }

    private void UpdateProgressbars()
    {
        if (IsInstanceValid(player_stats_ui.stats_container))
            for (int i = 0; i < Enum.GetNames(typeof(PlayerStats.TOOLTYPE)).Length; i++)
            {
                player_stats_ui.stats_container.GetChild(i).GetNode<Label>("Type").Text =
                    TranslationServer.Translate(
                        "EQUIPMENT_PANEL_" + ((PlayerStats.TOOLTYPE)i).ToString()
                    ) + ":";
                player_stats_ui.stats_container.GetChild(i).GetNode<Label>("Number").Text = Player
                    .instance.player_stats.GetStatAmount((PlayerStats.TOOLTYPE)i)
                    .ToString("N1");
            }
    }

    public static void UpdateSlots()
    {
        for (int i = 0; i < 4; i++)
        {
            if (instance?.equipped_tools[i] == null)
                continue;

            if (instance.equipped_tools[i].current_durability > 0)
            {
                Debug.Print(i + " | " + instance.equipped_tools[i]);
                Item item = new Item(
                    Inventory.ITEM_TYPES[(Inventory.ITEM_ID)instance.equipped_tools[i].item_id],
                    instance.equipped_tools[i].amount
                );

                Debug.Print("Durability: " + instance.equipped_tools[i].current_durability);

                if (instance?.slots_tool[i]?.GetSlotItemUI() == null)
                    continue;

                instance.slots_tool[i].GetSlotItemUI().current_durability = instance
                    .equipped_tools[i]
                    .current_durability;

                instance
                    .slots_tool[i]
                    .UpdateItem(item, instance.equipped_tools[i].current_durability);

                PlayerUI
                    .instance.equipmentSelectBar.select_slots[i]
                    .GetSlotItemUI()
                    .current_durability = instance.equipped_tools[i].current_durability;

                PlayerUI
                    .instance.equipmentSelectBar.select_slots[i]
                    .UpdateItem(item, instance.equipped_tools[i].current_durability);
            }
            else
            {
                PlayerUI
                    .instance.equipmentSelectBar.select_slots[
                        EquipmentSelectBar.current_selected_slot
                    ]
                    .ClearSlotItem();
                instance.slots_tool[EquipmentSelectBar.current_selected_slot].ClearSlotItem();
                instance.equipped_tools[EquipmentSelectBar.current_selected_slot] = null;
                PlayerUI.instance.equipmentSelectBar.current_selected_slot_item_ui = null;
            }
            instance.CalculateStatsFromEquipment();
        }
    }

    public void SetArmorSlotItem(int index, SlotItemUI slot_item_ui)
    {
        equipped_armor[index] = new ItemSave(
            (int)slot_item_ui.item.info.id,
            slot_item_ui.item.amount,
            slot_item_ui.current_durability,
            (int)slot_item_ui.item.state
        );
        slots_armor[index].SetItem(slot_item_ui.item, slot_item_ui.current_durability);
    }

    public void SetToolSlotItem(int index, SlotItemUI slot_item_ui)
    {
        equipped_tools[index] = new ItemSave(
            (int)slot_item_ui.item.info.id,
            slot_item_ui.item.amount,
            slot_item_ui.current_durability,
            (int)slot_item_ui.item.state
        );
        slots_tool[index].SetItem(slot_item_ui.item, slot_item_ui.current_durability);
    }

    public void ClearArmorSlotItem(int index)
    {
        equipped_armor[index] = null;
        slots_armor[index].ClearSlotItem();
    }

    public void ClearToolSlotItem(int index)
    {
        equipped_tools[index] = null;
        slots_tool[index].ClearSlotItem();
    }

    public void CalculateStatsFromEquipment()
    {
        PlayerStats player_stats = Player.instance.player_stats;
        for (int i = 0; i < player_stats.GetStatsLength(); i++)
            player_stats.SetStat((PlayerStats.TOOLTYPE)i, 1f);

        foreach (ItemSave s in equipped_armor)
        {
            if (s != null)
            {
                WearableAttribute attribute = Inventory
                    .ITEM_TYPES[(Inventory.ITEM_ID)s.item_id]
                    .GetAttributeOrNull<WearableAttribute>();

                if (attribute.stats != null)
                    foreach (PlayerStat x in attribute.stats)
                        player_stats.IncreaseStat(x.tool_type, x.value);
            }
        }

        if (PlayerUI.instance.equipmentSelectBar.GetSelectedSlotItemUI() != null)
        {
            SlotItemUI slot_item_ui = PlayerUI.instance.equipmentSelectBar.GetSelectedSlotItemUI();
            WearableAttribute attribute =
                slot_item_ui.item.info.GetAttributeOrNull<WearableAttribute>();
            if (attribute != null)
                if (attribute.stats != null)
                    foreach (PlayerStat x in attribute.stats)
                        player_stats.IncreaseStat(x.tool_type, x.value);
        }
        UpdateProgressbars();
    }

    public void LoadArmorFromSave(ItemSave[] item_save)
    {
        for (int i = 0; i < slots_armor.Count; i++)
            slots_armor[i].ClearSlotItem();

        equipped_armor = item_save;

        for (int i = 0; i < slots_armor.Count; i++)
            if (item_save[i] != null)
                slots_armor[i]
                    .SetItem(
                        new Item(
                            Inventory.ITEM_TYPES[(Inventory.ITEM_ID)item_save[i].item_id],
                            item_save[i].amount
                        )
                    );
    }

    public void LoadToolFromSave(ItemSave[] item_save)
    {
        for (int i = 0; i < slots_armor.Count; i++)
            slots_tool[i].ClearSlotItem();

        equipped_tools = item_save;

        for (int i = 0; i < slots_tool.Count; i++)
            if (item_save[i] != null)
            {
                slots_tool[i]
                    .SetItem(
                        new Item(
                            Inventory.ITEM_TYPES[(Inventory.ITEM_ID)item_save[i].item_id],
                            item_save[i].amount
                        ),
                        item_save[i].current_durability
                    );
                PlayerUI
                    .instance.equipmentSelectBar.select_slots[i]
                    .SetItem(
                        new Item(
                            Inventory.ITEM_TYPES[(Inventory.ITEM_ID)item_save[i].item_id],
                            item_save[i].amount
                        ),
                        item_save[i].current_durability
                    );
            }
        PlayerUI.instance.equipmentSelectBar.SelectSelectSlot(0);
    }
}
