using System;
using Godot;

public partial class Chest : ChestBase
{
    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        GameMenu.INSTANCE.OnOpenChestTab();
        ChestInventory.INSTANCE = ((ChestTab)GameMenu.INSTANCE.chest_tab).chest_inventory;
        ChestInventory.current_chest = this;
        ChestInventory.INSTANCE.OpenChest();
    }
}
