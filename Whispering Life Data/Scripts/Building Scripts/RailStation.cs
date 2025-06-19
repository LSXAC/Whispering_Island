using System;
using System.Diagnostics;
using Godot;

public partial class RailStation : MachineBase
{
    [Export]
    public ChestBase chest_in,
        chest_out;

    [Export]
    public Timer transfer_timer;
    public Minecart minecart = null;
    public bool export = true;
    public bool import = false;
    public bool minecart_connected = false;

    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        GameMenu.instance.OnOpenRailStationTab();
        RailStationTab.last_rail_station = this;
        ChestInventory.instance = (
            (RailStationTab)GameMenu.instance.rail_station_tab
        ).chest_inventory_in;
        ChestInventory.current_chest = chest_in;
        ChestInventory.instance.OpenChest();
    }

    public void OnAreaEntered(Area2D area)
    {
        Debug.Print("Object: " + area.GetParent().Name);
        if (area.GetParent() is Minecart cart)
            ConnectMinecart(cart);
    }

    public void OnTransferTimerTimeout()
    {
        Debug.Print("Connected! & Disconnected!");
        DisconnectMinecart();
    }

    public void ConnectMinecart(Minecart minecart)
    {
        minecart_connected = true;
        this.minecart = minecart;
        minecart.is_running = false;
        transfer_timer.Start();
    }

    public void DisconnectMinecart()
    {
        minecart_connected = false;
        this.minecart.is_running = true;
        this.minecart = null;
        transfer_timer.Stop();
    }
}
