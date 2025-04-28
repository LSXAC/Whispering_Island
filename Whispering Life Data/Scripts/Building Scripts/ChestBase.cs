using System;
using Godot;
using Godot.Collections;

public partial class ChestBase : MachineBase
{
    [Export]
    public ItemSave[] chest_items = new ItemSave[20];

    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        GameMenu.INSTANCE.OnOpenChestTab();
        ChestInventory.INSTANCE = ((ChestTab)GameMenu.INSTANCE.chest_tab).chest_inventory;
        ChestInventory.INSTANCE.OpenChest(this);
    }

    public int GetAmountOfFreeSlots()
    {
        int amount = 0;
        foreach (ItemSave i_s in chest_items)
            if (i_s == null)
                amount++;

        return amount;
    }
}
