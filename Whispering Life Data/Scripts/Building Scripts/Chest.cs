using System;
using Godot;

public partial class Chest : ChestBase
{
    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        GameMenu.instance.OnOpenChestTab();
        ChestInventory.instance = ((ChestTab)GameMenu.instance.chest_tab).chest_inventory;
        ChestInventory.current_chest = this;
        ChestInventory.instance.OpenChest();
    }
}
