using System;
using System.Collections.Generic;
using Godot;

public partial class EquipmentPanel : Control
{
    public static EquipmentPanel instance = null;

    [Export]
    public ItemSave[] equipped_armor = new ItemSave[4];

    [Export]
    public ItemSave[] equipped_tools = new ItemSave[4];

    [Export]
    public Slot[] slots_armor = new Slot[4];

    [Export]
    public Slot[] slots_tool = new Slot[4];

    [Export]
    public StatsPanel stats_panel;

    [Export]
    public Label health_bar_label,
        fatigue_bar_label;

    [Export]
    public ProgressBar health_bar,
        fatigue_bar;

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
        health_bar.Value = Player.instance.player_stats.health_value;
        health_bar_label.Text = TranslationServer.Translate("EQUIPMENT_PANEL_HEALTH_BAR") + ":";
        fatigue_bar.Value = Player.instance.player_stats.fatigue_value;
        fatigue_bar_label.Text = TranslationServer.Translate("EQUIPMENT_PANEL_FATIGUE_BAR") + ":";

        for (int i = 0; i < Enum.GetNames(typeof(StatsPanel.stat_types)).Length; i++)
        {
            stats_panel.stats_container.GetChild(i).GetNode<Label>("Type").Text =
                TranslationServer.Translate(
                    "EQUIPMENT_PANEL_" + ((StatsPanel.stat_types)i).ToString()
                ) + ":";
            stats_panel.stats_container.GetChild(i).GetNode<Label>("Number").Text = Player
                .instance.player_stats.stat_amounts[i]
                .ToString("N1");
        }
    }

    public static void UpdateSlotDurability(int index)
    {
        if (
            instance
                .slots_tool[EquipmentSelectBar.current_selected_slot]
                .GetSlotItemUI()
                .current_durability > 0
        )
        {
            PlayerUI
                .instance.equipmentSelectBar.GetSelectedSlotItemUI()
                .SetDurability(
                    instance
                        .slots_tool[EquipmentSelectBar.current_selected_slot]
                        .GetSlotItemUI()
                        .current_durability
                );

            instance
                .slots_tool[EquipmentSelectBar.current_selected_slot]
                .GetSlotItemUI()
                .SetDurability(
                    instance
                        .slots_tool[EquipmentSelectBar.current_selected_slot]
                        .GetSlotItemUI()
                        .current_durability
                );
            instance.equipped_tools[EquipmentSelectBar.current_selected_slot].current_durability =
                instance
                    .slots_tool[EquipmentSelectBar.current_selected_slot]
                    .GetSlotItemUI()
                    .current_durability;
        }
        else
        {
            PlayerUI
                .instance.equipmentSelectBar.select_slots[EquipmentSelectBar.current_selected_slot]
                .ClearItem();
            instance.slots_tool[EquipmentSelectBar.current_selected_slot].ClearItem();
            instance.equipped_tools[EquipmentSelectBar.current_selected_slot] = null;
            PlayerUI.instance.equipmentSelectBar.current_selected_slot_item_ui = null;
        }
        instance.CalculateStatsFromEquipment();
    }

    public void CalculateStatsFromEquipment()
    {
        PlayerStats player_stats = Player.instance.player_stats;
        for (int i = 0; i < player_stats.stat_amounts.Length; i++)
            player_stats.stat_amounts[i] = 1f;

        foreach (ItemSave s in equipped_armor)
        {
            if (s != null)
                if (Inventory.ITEM_TYPES[(Inventory.ITEM_ID)s.item_id].item_stats != null)
                    foreach (
                        ItemStats x in Inventory.ITEM_TYPES[(Inventory.ITEM_ID)s.item_id].item_stats
                    )
                        player_stats.stat_amounts[(int)x.type] += x.bonus;
        }

        if (PlayerUI.instance.equipmentSelectBar.GetSelectedSlotItemUI() != null)
        {
            SlotItemUI slot_item = PlayerUI.instance.equipmentSelectBar.GetSelectedSlotItemUI();
            if (slot_item.item.resource.item_stats != null)
                foreach (ItemStats x in slot_item.item.resource.item_stats)
                    player_stats.stat_amounts[(int)x.type] += x.bonus;
        }
        UpdateProgressbars();
    }

    public void LoadArmorFromSave(ItemSave[] item_save)
    {
        for (int i = 0; i < slots_armor.Length; i++)
            slots_armor[i].ClearItem();

        equipped_armor = item_save;

        for (int i = 0; i < slots_armor.Length; i++)
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
        for (int i = 0; i < slots_armor.Length; i++)
            slots_tool[i].ClearItem();

        equipped_tools = item_save;

        for (int i = 0; i < slots_tool.Length; i++)
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
    }
}
