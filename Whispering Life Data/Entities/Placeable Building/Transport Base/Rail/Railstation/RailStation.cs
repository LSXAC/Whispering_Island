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

        RailStationTab.last_rail_station = this;
        RailStationTab.instance.chest_inventory_input.OpenChest(chest_in.chest_items);
        GameMenu.instance.OnOpenRailStationTab();
    }

    public void OnTransferTimerTimeout()
    {
        if (current_station == RailstationArea.STATION.IMPORT)
        {
            Item item =
                RailStationTab.instance.chest_inventory_input.GetLastItemFromInventoryOrNull(
                    minecart.chestBase.chest_items
                );

            if (item != null)
            {
                if (
                    RailStationTab.instance.chest_inventory_input.HasItemInInventory(
                        chest_in.chest_items,
                        item
                    )
                    || RailStationTab.instance.chest_inventory_input.HasEmptySlotInInventory(
                        chest_in.chest_items
                    )
                )
                {
                    RailStationTab.instance.chest_inventory_input.AddItem(
                        item,
                        chest_in.chest_items
                    );
                    RailStationTab.instance.chest_inventory_input.RemoveItem(
                        item,
                        minecart.chestBase.chest_items
                    );
                    RailStationTab.instance.chest_inventory_input.UpdateInventoryUI();
                    return;
                }
            }
        }

        if (current_station == RailstationArea.STATION.EXPORT)
        {
            Item item =
                RailStationTab.instance.chest_inventory_output.GetLastItemFromInventoryOrNull(
                    chest_out.chest_items
                );

            if (item != null)
            {
                if (
                    RailStationTab.instance.chest_inventory_output.HasItemInInventory(
                        minecart.chestBase.chest_items,
                        item
                    )
                    || RailStationTab.instance.chest_inventory_output.HasEmptySlotInInventory(
                        minecart.chestBase.chest_items
                    )
                )
                {
                    RailStationTab.instance.chest_inventory_output.AddItem(
                        item,
                        minecart.chestBase.chest_items
                    );
                    RailStationTab.instance.chest_inventory_output.RemoveItem(
                        item,
                        chest_out.chest_items
                    );
                    RailStationTab.instance.chest_inventory_output.UpdateInventoryUI();
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
