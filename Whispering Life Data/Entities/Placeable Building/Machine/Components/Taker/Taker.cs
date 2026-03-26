using System.Diagnostics;
using Godot;

public partial class Taker : StaticBody2D
{
    [Export]
    public MachineBase building;

    [Export]
    public ItemHolder item_holder_In;

    public Area2D area;

    public override void _Ready()
    {
        area = GetNode<Area2D>("PathConnectArea");
    }

    public void DisableMonitorable()
    {
        if (Logger.NodeIsNotNull(area))
            area.Monitorable = false;
    }

    public void EnableMonitorable()
    {
        if (Logger.NodeIsNotNull(area))
            area.Monitorable = true;
    }

    public bool can_receive_item(BeltItem ii = null)
    {
        if (building is ChestBase chest_base)
        {
            return item_holder_In.GetChildCount() == 0
                && (
                    ChestTab.instance.chest_inventory.HasItemInInventory(
                        chest_base.chest_items,
                        ii.item
                    )
                    || ChestTab.instance.chest_inventory.HasEmptySlotInInventory(
                        chest_base.chest_items
                    )
                );
        }

        return item_holder_In.GetChildCount() == 0 && building.machine_enabled;
    }

    public void receive_item(Node2D item)
    {
        item_holder_In.receive_item(item);
    }

    public void OnItemHolderItemHeld()
    {
        BeltItem belt_item = (BeltItem)item_holder_In.offload_item();

        if (building is ChestBase chest_base)
        {
            ChestTab.instance.chest_inventory.AddItem(belt_item.item, chest_base.chest_items);
            ChestTab.instance.chest_inventory.UpdateInventoryUI();
        }

        if (building is ProcessBuilding pb)
            pb.NotifyItemsChanged();

        belt_item.QueueFree();
    }
}
