using System;
using System.Diagnostics;
using Godot;

public partial class RailStation : MachineBase
{
    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        Debug.Print("RailStation");
        //GameMenu.INSTANCE.OnOpenChestTab();
        //ChestInventory.INSTANCE.OpenChest(this);
    }
}
