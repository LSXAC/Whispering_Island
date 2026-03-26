using System;
using Godot;

public partial class ChestInventory : Inventory
{
    public override void _Ready()
    {
        slot_amount = 20;
        inventory_items = new ItemSave[slot_amount];
        SetSlots();
    }

    public void OpenChest(ItemSave[] chest_items)
    {
        LoadInventoryFromSave(chest_items);
    }
}
