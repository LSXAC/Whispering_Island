using System;
using Godot;

public partial class ChestInventory : InventoryBase
{
    public static ChestInventory INSTANCE = null;
    public ChestBase current_chest;

    public override void _Ready()
    {
        INSTANCE = this;
        SetSlots();
    }

    public void OpenChest(ChestBase chest)
    {
        current_chest = chest;
        LoadInventoryFromSave(chest.chest_items);
    }
}
