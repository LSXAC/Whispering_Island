using System.Diagnostics;
using Godot;

public partial class Belt : TransportBase
{
    private Area2D path_connect_area;

    public override void _Ready()
    {
        base._Ready();
        path_connect_area = GetNode<Area2D>("PathConnectArea");
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
                        ItemInfo info = item_holder.GetBeltItem().GetItemInfo();
                        if (process_building.GetItemResource(FurnaceTab.SlotType.IMPORT) != null)
                            if (
                                info != process_building.GetItemResource(FurnaceTab.SlotType.IMPORT)
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
                            (int)info.id,
                            1
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

    public void SetRotationAndDisableMonitoring(int rotation)
    {
        Set_Rotation(rotation);
        if (Logger.NodeIsNotNull(path_connect_area))
            path_connect_area.Monitorable = false;
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
}
