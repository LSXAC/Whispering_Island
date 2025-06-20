using System.Diagnostics;
using Godot;

public partial class Belt : TransportBase
{
    public override void _Ready()
    {
        set_direction();
        building_collider_manager = GetNode<Node2D>("BuildingAreas") as Building_Collider_Manager;
    }

    public void OnDetectorBeltDetected(Area2D area)
    {
        if (ignore_self_detector)
            return;

        if (area is PathConnectArea)
        {
            if (area.GetParent() is Belt && area.GetParent() is not BeltTunnel)
                if (area.GetParent<TransportBase>().can_receive_item())
                {
                    var item = item_holder.offload_item();
                    area.GetParent<Belt>().receive_item(item);
                }

            if (area.GetParent() is BeltTunnel)
                if (area.GetParent<BeltTunnel>().is_tunnel_connected)
                    if (area.GetParent<BeltTunnel>().connected_itemholder != null)
                        if (area.GetParent<BeltTunnel>().item_holder.GetChildCount() == 0)
                        {
                            Debug.Print("Deposit Item");
                            var item = item_holder.offload_item();
                            area.GetParent<BeltTunnel>().from_Belt = true;
                            area.GetParent<BeltTunnel>().GetNode<Timer>("CheckTimer").Start();
                            area.GetParent<BeltTunnel>().item_holder.receive_item(item);
                        }

            if (area.GetParent() is Taker)
            {
                if (area.GetParent().GetParent() is ProcessBuilding)
                {
                    if (area.GetParent<Taker>().can_receive_item())
                    {
                        if (item_holder.hasBeltItem())
                        {
                            ItemResource item_resource = item_holder
                                .GetBeltItem()
                                .GetItemResource();
                            if (
                                ((ProcessBuilding)area.GetParent<Taker>().building).GetItemResource(
                                    FurnaceTab.SlotType.IMPORT
                                ) != null
                            )
                                if (
                                    item_resource
                                    != (
                                        (ProcessBuilding)area.GetParent<Taker>().building
                                    ).GetItemResource(FurnaceTab.SlotType.IMPORT)
                                )
                                    return;
                                else
                                {
                                    var item2 = item_holder.offload_item();
                                    ((ProcessBuilding)area.GetParent<Taker>().building)
                                        .item_array[(int)FurnaceTab.SlotType.IMPORT]
                                        .amount += 1;
                                    area.GetParent<Taker>().receive_item(item2);
                                    return;
                                }
                            var item = item_holder.offload_item();
                            ((ProcessBuilding)area.GetParent<Taker>().building).item_array[
                                (int)FurnaceTab.SlotType.IMPORT
                            ] = new ItemSave((int)item_resource.item_id, 1);
                            area.GetParent<Taker>().receive_item(item);
                        }
                    }
                }
                if (area.GetParent().GetParent() is ChestBase)
                {
                    var item = item_holder.offload_item();
                    if (area.GetParent<Taker>().can_receive_item((BeltItem)item))
                        area.GetParent<Taker>().receive_item(item);
                }
                if (area.GetParent().GetParent() is RailStation)
                {
                    var item = item_holder.offload_item();
                    if (area.GetParent<Taker>().can_receive_item((BeltItem)item))
                        area.GetParent<Taker>().receive_item(item);
                }
            }
        }
    }

    public void OnItemHolderItemHeld()
    {
        detector.Detect();
    }
}
