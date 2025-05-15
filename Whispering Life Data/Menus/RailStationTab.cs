using System;
using System.Diagnostics;
using Godot;

public partial class RailStationTab : ColorRect
{
    [Export]
    public ChestInventory chest_inventory_in,
        chest_inventory_out;

    [Export]
    public TabContainer container;
    public static RailStation last_rail_station = null;

    public void OnTabSelected(int tab)
    {
        container.GetChild<Control>(tab).Visible = true;
        if (tab == 0)
        {
            ChestInventory.current_chest = last_rail_station.chest_in;
            ChestInventory.INSTANCE = (
                (RailStationTab)GameMenu.INSTANCE.rail_station_tab
            ).chest_inventory_in;
        }
        else
        {
            ChestInventory.current_chest = last_rail_station.chest_out;
            ChestInventory.INSTANCE = (
                (RailStationTab)GameMenu.INSTANCE.rail_station_tab
            ).chest_inventory_out;
        }
        ChestInventory.INSTANCE.OpenChest();
    }
}
