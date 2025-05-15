using System;
using System.Diagnostics;
using Godot;

public partial class RailStation : MachineBase
{
    [Export]
    public ChestBase chest_in,
        chest_out;

    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        Debug.Print("RailStation");
        GameMenu.INSTANCE.OnOpenRailStationTab();
        RailStationTab.last_rail_station = this;
        ChestInventory.INSTANCE = (
            (RailStationTab)GameMenu.INSTANCE.rail_station_tab
        ).chest_inventory_in;
        ChestInventory.current_chest = chest_in;
        ChestInventory.INSTANCE.OpenChest();
    }
}
