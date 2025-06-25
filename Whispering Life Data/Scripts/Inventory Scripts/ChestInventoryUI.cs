using System;
using Godot;

public partial class ChestInventoryUI : Inventory
{
    public static ChestInventoryUI instance = null;
    public static ChestBase current_chest;

    public override void _Ready()
    {
        instance = this;
        slot_amount = 20;
        inventory_items = new ItemSave[slot_amount];
        SetSlots();
    }

    public void OpenChest()
    {
        LoadInventoryFromSave(current_chest.chest_items);
    }
}
