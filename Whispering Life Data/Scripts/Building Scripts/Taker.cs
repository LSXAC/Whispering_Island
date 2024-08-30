using Godot;

public partial class Taker : StaticBody2D
{
    [Export]
    public MachineBase building;

    [Export]
    public ItemHolder item_holder_In;

    public bool can_receive_item()
    {
        return item_holder_In.GetChildCount() == 0
            && building.import_count < 50
            && building.machine_enabled;
    }

    public void receive_item(Node2D item)
    {
        item_holder_In.receive_item(item);
    }

    public void OnItemHolderItemHeld()
    {
        var item = item_holder_In.offload_item();
        item.QueueFree();
        building.import_count++;
        InventoryItem ii = new InventoryItem();
        ii.init(building.import_item_info);
        FurnaceTab.INSTANCE.import_slot.UpdateFurnaceItem(ii, 1);
    }
}
