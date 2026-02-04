using System;
using System.Diagnostics;
using Godot;

public partial class RailStationTab : ColorRect
{
    [Export]
    public ChestInventory chest_inventory_input,
        chest_inventory_output;

    [Export]
    public TabContainer container;

    [Export]
    public CheckBox import_box,
        export_box;

    [Export]
    public Label is_connected_label;
    public static RailStation last_rail_station = null;
    public static RailStationTab instance = null;

    public override void _Ready()
    {
        instance = this;
    }

    public void OnClickCheckImport()
    {
        export_box.ButtonPressed = false;
        last_rail_station.current_station = RailstationArea.STATION.IMPORT;
        last_rail_station.DisconnectMinecart();
    }

    public void UpdateConnectedLabel(bool state)
    {
        is_connected_label.Text = "Connected: " + state.ToString();
    }

    public void OnClickCheckExport()
    {
        import_box.ButtonPressed = false;

        last_rail_station.current_station = RailstationArea.STATION.EXPORT;
        last_rail_station.DisconnectMinecart();
    }

    public void OnTabSelected(int tab)
    {
        container.GetChild<Control>(tab).Visible = true;
        if (Logger.NodeIsNull(last_rail_station))
            return;

        if (last_rail_station.chest_in == null || last_rail_station.chest_out == null)
            return;

        if (tab == 0)
        {
            ChestInventory.current_chest = last_rail_station.chest_in;
            ChestInventory.instance = (
                (RailStationTab)GameMenu.instance.rail_station_tab
            ).chest_inventory_input;
        }
        else
        {
            ChestInventory.current_chest = last_rail_station.chest_out;
            ChestInventory.instance = (
                (RailStationTab)GameMenu.instance.rail_station_tab
            ).chest_inventory_output;
        }
        ChestInventory.instance.OpenChest();
    }
}
