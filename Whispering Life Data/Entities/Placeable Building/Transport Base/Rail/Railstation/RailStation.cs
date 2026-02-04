using System;
using System.Diagnostics;
using System.Linq;
using Godot;

public partial class RailStation : MachineBase
{
    [Export]
    public ChestBase chest_in,
        chest_out;

    [Export]
    public Timer transfer_timer;

    public Minecart minecart = null;

    [Export]
    public RailstationArea.STATION current_station;

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
        ).chest_inventory_input;
        ChestInventory.current_chest = chest_in;
        ChestInventory.instance.OpenChest();
    }

    public void OnTransferTimerTimeout()
    {
        if (current_station == RailstationArea.STATION.IMPORT)
        {
            Item item = ChestInventory.instance.GetLastItemFromInventoryOrNull(
                minecart.chestBase.chest_items
            );

            if (item != null)
            {
                if (
                    ChestInventory.instance.HasItemInInventory(chest_in.chest_items, item)
                    || ChestInventory.instance.HasEmptySlotInInventory(chest_in.chest_items)
                )
                {
                    ChestInventory.instance.AddItem(item, chest_in.chest_items);
                    ChestInventory.instance.RemoveItem(item, minecart.chestBase.chest_items);
                    ChestInventory.instance.UpdateInventoryUI();
                    return;
                }
            }
        }

        if (current_station == RailstationArea.STATION.EXPORT)
        {
            Item item = ChestInventory.instance.GetLastItemFromInventoryOrNull(
                chest_out.chest_items
            );

            if (item != null)
            {
                if (
                    ChestInventory.instance.HasItemInInventory(
                        minecart.chestBase.chest_items,
                        item
                    )
                    || ChestInventory.instance.HasEmptySlotInInventory(
                        minecart.chestBase.chest_items
                    )
                )
                {
                    ChestInventory.instance.AddItem(item, minecart.chestBase.chest_items);
                    ChestInventory.instance.RemoveItem(item, chest_out.chest_items);
                    ChestInventory.instance.UpdateInventoryUI();
                    return;
                }
            }
        }

        Debug.Print("Transfertimer timeout -> Disconnected!");
        DisconnectMinecart();
    }

    public void ConnectMinecart(Minecart minecart, RailstationArea.STATION station)
    {
        if (current_station != station)
            return;

        minecart_connected = true;
        this.minecart = minecart;
        this.minecart.Position = new Vector2(0, 0);
        minecart.is_running = false;
        Debug.Print("Minecart now connected!");
        transfer_timer.Start();
    }

    public void DisconnectMinecart()
    {
        if (this.minecart == null)
            return;
        minecart_connected = false;
        this.minecart.is_running = true;
        this.minecart = null;
        Debug.Print("Minecard now disconnected!");
        transfer_timer.Stop();
    }

    public override void Load(Resource save)
    {
        if (save is MachineSave machine_save)
        {
            base.Load(machine_save);
            for (int i = 0; i < machine_save.chest_items.Length; i++)
                chest_in.chest_items[i] = machine_save.chest_items[i];
            for (int i = 0; i < machine_save.second_chest_items.Length; i++)
                chest_out.chest_items[i] = machine_save.second_chest_items[i];
        }
        else
            Logger.PrintWrongSaveType();
    }

    public override Resource Save()
    {
        MachineSave ms = (MachineSave)base.Save();
        ms.chest_items = chest_in.chest_items;
        ms.second_chest_items = chest_out.chest_items;
        return ms;
    }
}
