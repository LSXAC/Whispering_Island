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
                        ii?.item
                    )
                    || ChestTab.instance.chest_inventory.HasEmptySlotInInventory(
                        chest_base.chest_items
                    )
                );
        }

        // For ProcessBuilding, just check if there's space - actual validation happens in receive_item
        return item_holder_In.GetChildCount() == 0 && building.machine_enabled;
    }

    public void receive_item(Node2D item)
    {
        BeltItem belt_item = item as BeltItem;
        if (belt_item != null)
        {
            Debug.Print(
                $"[TAKER] 📥 Receiving item: {belt_item.item.info.name} (Amount: {belt_item.item.amount})"
            );

            if (
                building is ProcessBuilding process_building
                && process_building.selected_recipe != null
            )
            {
                ItemInfo required_input = process_building.selected_recipe.GetInputRequirement();
                if (required_input != null && belt_item.item.info != required_input)
                {
                    Debug.Print(
                        $"[TAKER] ❌ Item REJECTED! Recipe requires: {required_input.name}, but got: {belt_item.item.info.name}"
                    );
                    Debug.Print($"[TAKER] Item will NOT be accepted into the machine!");
                    return;
                }
                Debug.Print(
                    $"[TAKER] ✅ Item ACCEPTED! Recipe requires: {required_input.name}, got: {belt_item.item.info.name}"
                );
            }
        }
        item_holder_In.receive_item(item);
    }

    public void OnItemHolderItemHeld()
    {
        BeltItem belt_item = (BeltItem)item_holder_In.offload_item();

        Debug.Print($"[TAKER] ✅ Item successfully moved into machine: {belt_item.item.info.name}");

        if (building is ChestBase chest_base)
        {
            ChestTab.instance.chest_inventory.AddItem(belt_item.item, chest_base.chest_items);
            ChestTab.instance.chest_inventory.UpdateInventoryUI();
        }

        if (building is ProcessBuilding pb)
        {
            int input_idx = pb.GetSlotIndexByPurpose(SlotPurpose.INPUT);

            // Add item to the input slot
            if (
                pb.item_array[input_idx] != null
                && pb.item_array[input_idx].item_id == (int)belt_item.item.info.id
            )
            {
                pb.item_array[input_idx].amount += belt_item.item.amount;
                Debug.Print($"[TAKER] Added {belt_item.item.amount} to existing stack");
            }
            else if (pb.item_array[input_idx] == null)
            {
                pb.item_array[input_idx] = new ItemSave(
                    (int)belt_item.item.info.id,
                    belt_item.item.amount,
                    -1,
                    (int)belt_item.item.state
                );
                Debug.Print($"[TAKER] Created new stack");
            }

            pb.NotifyItemsChanged();
            if (pb.selected_recipe != null)
            {
                Debug.Print(
                    $"[TAKER] 🔄 ProcessBuilding processing recipe with input: {belt_item.item.info.name}"
                );
            }
        }

        belt_item.QueueFree();
    }
}
