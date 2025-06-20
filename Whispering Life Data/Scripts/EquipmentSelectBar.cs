using System;
using System.Diagnostics;
using Godot;

public partial class EquipmentSelectBar : Container
{
    [Export]
    public Color normal_color,
        selected_color;

    [Export]
    public Slot[] select_slots = new Slot[4];

    public SlotItem current_selected_slot_item = null;

    public static int current_selected_slot = 0;

    public override void _Ready()
    {
        SelectSelectSlot(0);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("NUM1"))
            SelectSelectSlot(0);

        if (Input.IsActionJustPressed("NUM2"))
            SelectSelectSlot(1);

        if (Input.IsActionJustPressed("NUM3"))
            SelectSelectSlot(2);

        if (Input.IsActionJustPressed("NUM4"))
            SelectSelectSlot(3);
    }

    public SlotItem GetSelectedSlotItem()
    {
        if (current_selected_slot_item == null)
            Debug.Print("No Item selected in Selectbar");

        return current_selected_slot_item;
    }

    public ItemInfo.Type_Level GetSelectedTypeLevel()
    {
        if (GetSelectedSlotItem() != null)
            return GetSelectedSlotItem().item.item_info.type_level;
        return ItemInfo.Type_Level.Hand;
    }

    public bool HasSameUseType(StatsPanel.stat_types type)
    {
        if (GetSelectedSlotItem() != null)
            if (GetSelectedSlotItem().item.item_info.use_type == type)
                return true;
            else
                return false;

        return true;
    }

    public void SelectSelectSlot(int index)
    {
        select_slots[current_selected_slot].GetParent().GetParent<ColorRect>().Color = normal_color;
        select_slots[index].GetParent().GetParent<ColorRect>().Color = selected_color;
        current_selected_slot_item = select_slots[index].GetSlotItem();
        current_selected_slot = index;

        if (EquipmentPanel.instance != null)
            EquipmentPanel.instance.CalculateStatsFromEquipment();
    }
}
