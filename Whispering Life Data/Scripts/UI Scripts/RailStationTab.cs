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

    [Export]
    public CheckBox import_box,
        export_box;

    [Export]
    public Label is_connected_label;
    public static RailStation last_rail_station = null;
    public static RailStationTab INSTANCE = null;

    public override void _Ready()
    {
        INSTANCE = this;
    }

    public void OnClickCheckImport()
    {
        export_box.ButtonPressed = false;
        last_rail_station.export = false;
        last_rail_station.import = true;
    }

    public void UpdateConnectedLabel(bool state)
    {
        is_connected_label.Text = "Connected: " + state.ToString();
    }

    public void OnClickCheckExport()
    {
        import_box.ButtonPressed = false;
        last_rail_station.import = false;
        last_rail_station.export = true;
    }

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
