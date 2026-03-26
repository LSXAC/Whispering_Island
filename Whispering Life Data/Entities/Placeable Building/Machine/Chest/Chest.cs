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
        ChestTab.instance.chest_inventory.OpenChest(chest_items);
    }
}
