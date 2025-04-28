using System;
using System.Diagnostics;
using Godot;

public partial class RailStation : ChestBase
{
    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        Debug.Print("RailStation");
        GameMenu.INSTANCE.OnOpenRailStationTab();
        ChestInventory.INSTANCE = (
            (RailStationTab)GameMenu.INSTANCE.rail_station_tab
        ).chest_inventory;
        ChestInventory.INSTANCE.OpenChest(this);
    }
}
