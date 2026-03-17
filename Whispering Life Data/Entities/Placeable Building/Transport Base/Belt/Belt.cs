using System.Diagnostics;
using Godot;

public partial class Belt : TransportBase
{
    public override void _Ready()
    {
        base._Ready();
    }

    public void OnDetectorBeltDetected(Area2D area)
    {
        if (ignore_self_detector || area is not PathConnectArea)
            return;

        var item = item_holder.offload_item();

        if (area.GetParent() is Belt belt && area.GetParent() is not BeltTunnel)
            if (belt.can_receive_item())
                belt.receive_item(item);

        if (area.GetParent() is BeltTunnel belt_tunnel)
            if (belt_tunnel.is_tunnel_connected)
                if (belt_tunnel.connected_itemholder != null)
                    if (belt_tunnel.item_holder.GetChildCount() == 0)
                    {
                        belt_tunnel.from_Belt = true;
                        belt_tunnel.GetNode<Timer>("CheckTimer").Start();
                        belt_tunnel.item_holder.receive_item(item);
                    }

        if (area.GetParent() is Taker taker)
        {
            if (taker.GetParent() is ProcessBuilding process_building)
            {
                if (taker.can_receive_item())
                {
                    if (item_holder.hasBeltItem())
                    {
                        BeltItem belt_item = item_holder.GetBeltItem();
                        if (process_building.GetItemResource(FurnaceTab.SlotType.IMPORT) != null)
                            if (
                                belt_item.GetItemInfo()
                                != process_building.GetItemResource(FurnaceTab.SlotType.IMPORT)
                            )
                                return;
                            else
                            {
                                var item2 = item_holder.offload_item();
                                process_building
                                    .item_array[(int)FurnaceTab.SlotType.IMPORT]
                                    .amount += 1;
                                taker.receive_item(item2);
                                return;
                            }
                        process_building.item_array[(int)FurnaceTab.SlotType.IMPORT] = new ItemSave(
                            (int)belt_item.GetItemInfo().id,
                            1,
                            -1,
                            (int)belt_item.item.state
                        );
                        taker.receive_item(item);
                    }
                }
            }
            if (area.GetParent().GetParent() is ChestBase)
                if (taker.can_receive_item((BeltItem)item))
                    taker.receive_item(item);

            if (area.GetParent().GetParent() is RailStation)
                if (taker.can_receive_item((BeltItem)item))
                    taker.receive_item(item);
        }
    }

    public void OnItemHolderItemHeld()
    {
        detector.Detect();
    }

    public override Resource Save()
    {
        TransportBaseSave tb_save = (TransportBaseSave)base.Save();
        BeltSave belt_save = new BeltSave(tb_save, null, current_rotation);
        if (item_holder.hasBeltItem())
        {
            belt_save.belt_holding_item_resource = item_holder.GetBeltItem().item.info;
            belt_save.belt_item_is_moving = item_holder.moving_item;
            belt_save.belt_item_position = item_holder.GetBeltItem().Position;
        }
        return belt_save;
    }

    public override void Load(Resource save)
    {
        if (save is BeltSave belt_save)
        {
            base.Load(belt_save);
            if (belt_save.belt_holding_item_resource != null)
                InitBeltItem(belt_save);
        }
        else
            Logger.PrintWrongSaveType();
    }
}
