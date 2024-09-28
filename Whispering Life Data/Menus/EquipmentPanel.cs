using System;
using System.Collections.Generic;
using Godot;

public partial class EquipmentPanel : Control
{
    public static EquipmentPanel INSTANCE = null;

    [Export]
    public ItemSave[] equipped_armor = new ItemSave[4];

    [Export]
    public ItemSave[] equipped_tools = new ItemSave[4];

    [Export]
    public Slot[] slots_armor = new Slot[4];

    [Export]
    public Slot[] slots_tool = new Slot[4];

    [Export]
    public ProgressBar health_bar,
        fatigue_bar;

    public override void _Ready()
    {
        INSTANCE = this;
    }

    public void OnVisiblityChanged()
    {
        UpdateProgressbars();
    }

    private void UpdateProgressbars()
    {
        health_bar.Value = Player.INSTANCE.player_stats.health_value;
        fatigue_bar.Value = Player.INSTANCE.player_stats.fatigue_value;
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
                        Inventory.INSTANCE.item_Types[item_save[i].item_id],
                        item_save[i].amount
                    );
    }

    public void LoadToolFromSave(ItemSave[] item_save)
    {
        for (int i = 0; i < slots_armor.Length; i++)
            slots_tool[i].ClearItem();

        equipped_tools = item_save;

        for (int i = 0; i < slots_tool.Length; i++)
            if (item_save[i] != null)
                slots_tool[i]
                    .SetItem(
                        Inventory.INSTANCE.item_Types[item_save[i].item_id],
                        item_save[i].amount
                    );
    }
}
