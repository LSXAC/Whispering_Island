using System;
using System.Diagnostics;
using Godot;

public partial class RailStation : MachineBase
{
    [Export]
    public ChestBase chest_base;

    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        Debug.Print("RailStation");
        GameMenu.INSTANCE.OnOpenRailStationTab();
        ChestInventory.INSTANCE.OpenChest(chest_base);
    }
}
