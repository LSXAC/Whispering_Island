using System;
using Godot;

public partial class ChestInventory : InventoryBase
{
    public static ChestInventory INSTANCE = null;
    public static ChestBase current_chest;

    public override void _Ready()
    {
        INSTANCE = this;
        slot_amount = 20;
        inventory_items = new ItemSave[slot_amount];
        SetSlots();
    }

    public void OpenChest()
    {
        LoadInventoryFromSave(current_chest.chest_items);
    }
}
