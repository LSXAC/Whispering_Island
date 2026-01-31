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
        ChestInventoryUI.instance = ((ChestTab)GameMenu.instance.chest_tab).chest_inventory_ui;
        ChestInventoryUI.current_chest = this;
        ChestInventoryUI.instance.OpenChest();
    }
}
