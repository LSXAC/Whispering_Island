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

    public bool can_receive_item(BeltItem ii = null)
    {
        if (building is ChestBase chest_base)
        {
            return item_holder_In.GetChildCount() == 0
                && (
                    ChestInventoryUI.instance.HasItemInInventory(chest_base.chest_items, ii)
                    || ChestInventoryUI.instance.HasEmptySlotInInventory(chest_base.chest_items)
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
            ChestInventoryUI.instance.AddItem(belt_item.item, chest_base.chest_items);
            ChestInventoryUI.instance.UpdateInventoryUI();
        }
        FurnaceTab.instance.UpdateFurnaceUI();
        belt_item.QueueFree();
    }
}
