using System;
using Godot;

public partial class Trashcan : ChestBase
{
    [Export]
    public Timer deletion_timer;

    public void OnDeletionTimerTimeout()
    {
        DeleteItems();
    }

    public void DeleteItems()
    {
        this.chest_items = new ItemSave[20];
    }

    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        GameMenu.instance.OnOpenChestTab();
        ChestInventoryUI.current_chest = GetParent<ChestBase>();
        ChestInventoryUI.instance.OpenChest();
    }
}
