using System;
using Godot;

public partial class Minecart : MoveableBase
{
    [Export]
    public ChestBase chestBase;
    public bool is_running = false;

    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        MinecartTab.current_minecart = this;
        GameMenu.INSTANCE.OnOpenMinecartTab();
        ChestInventory.INSTANCE.OpenChest(chestBase);
    }
}
